using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Simulate boid movement 
/// </summary>
public class BoidManager : MonoBehaviour
{
    private const int threadGroupSize = 1024;
    private static readonly int Boids = Shader.PropertyToID("boids");
    private static readonly int Num_Boids = Shader.PropertyToID("num_boids");
    private static readonly int View_Radius = Shader.PropertyToID("view_radius");
    private static readonly int Avoid_Radius = Shader.PropertyToID("avoid_radius");
    
    [SerializeField] private ComputeShader _boidCompute;
    [SerializeField] private Transform _boidTarget;
    
    [Header("Spawning")]
    [SerializeField] private int _numBoids;
    [SerializeField] private Boid _boidPrefab;
    [SerializeField] private float _spawnRadius;
    
    [Header("Boid Settings")]
    [SerializeField] private BoidSettings _boidSettings;
    [SerializeField] private float _boidViewRadius = 2.5f;
    [SerializeField] private float _boidAvoidRadius = 1f;
    
    private Boid[] _allBoids;
    
    private void Awake()
    {
        _allBoids = new Boid[_numBoids];
        for (int i = 0; i < _numBoids; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * _spawnRadius;
            Boid boid = Instantiate(_boidPrefab, transform);
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;

            _allBoids[i] = boid;
            _allBoids[i].Initialize(_boidSettings, _boidTarget);
        }
    }

    private void Update()
    {
        BoidData[] boidData = new BoidData[_numBoids];
        
        for (int i = 0; i < _allBoids.Length; i++) 
        {
            boidData[i].position = _allBoids[i].Position;
            boidData[i].direction = _allBoids[i].Direction;
        }
        
        ComputeBuffer boidBuffer = new ComputeBuffer(_numBoids, BoidData.Size);
        boidBuffer.SetData(boidData);
        
        _boidCompute.SetBuffer(0, Boids, boidBuffer);
        _boidCompute.SetInt(Num_Boids, _numBoids);
        _boidCompute.SetFloat(View_Radius, _boidViewRadius);
        _boidCompute.SetFloat(Avoid_Radius, _boidAvoidRadius);
        
        int threadGroups = Mathf.CeilToInt(_numBoids / (float)threadGroupSize);
        _boidCompute.Dispatch(0, threadGroups, 1, 1);
        
        boidBuffer.GetData(boidData);
        
        for (int i = 0; i < _numBoids; i++)
        {
            _allBoids[i].UpdateBoid(boidData[i]);
        }
        
        boidBuffer.Release ();
    }
    
    /// <summary>
    /// Container data for a single boid passed to a compute shader to calculate 
    /// </summary>
    public struct BoidData
    {
        [UsedImplicitly] public Vector3 position;
        [UsedImplicitly] public Vector3 direction;
        
        [UsedImplicitly] public int num_flockmates;
        [UsedImplicitly] public Vector3 flock_direction;
        [UsedImplicitly] public Vector3 flock_center;
        [UsedImplicitly] public Vector3 separation_direction;

        public static int Size => sizeof(float) * 3 * 5 + sizeof(int);
    }

    /// <summary>
    /// Settings that determine boid movement and weights
    /// </summary>
    [System.Serializable]
    public class BoidSettings
    {
        public float minSpeed = 5f;
        public float maxSpeed = 8f;
        
        [Tooltip("align with each other")]
        public float alignmentWeight = 2f;
        
        [Tooltip("move towards the center of the flock")]
        public float cohesionWeight = 1f;
        
        [Tooltip("avoid other boids")]
        public float avoidanceWeight = 2.5f;
        
        [Tooltip("go to the target")]
        public float targetWeight = 2f;

        [Tooltip("ability to change directions")]
        public float maxSteerForce = 8f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
}
