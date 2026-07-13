using CustomAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityOrbiter : MonoBehaviour
{
    [SerializeField] private Vector3 _initialVelocity;
    [SerializeField] private SourceOrbit[] _sources;
    
    [Space]
    [SerializeField] private bool _drawOrbit;
    [ReadOnly(nameof(_drawOrbit), true)] [SerializeField] private int _numSteps;
    [ReadOnly(nameof(_drawOrbit), true), Min(0.1f)] [SerializeField] private float _orbitDrawTimeStep = 0.1f;

    private Rigidbody _rb;
    private Vector3 _velocity;
    
    private Vector3[] _cachedOrbitPoints;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _velocity = _initialVelocity;
        CacheOrbitPoints();
    }

    private void FixedUpdate()
    {
        _velocity += CalculateAcceleration(_rb.position, Time.fixedDeltaTime);
        _rb.MovePosition (_rb.position + _velocity * Time.fixedDeltaTime);
    }
    
    private Vector3 CalculateAcceleration(Vector3 pos, float timeStep)
    {
        Vector3 velocity = Vector3.zero;
        foreach (SourceOrbit sourceOrbit in _sources)
        {
            if (!sourceOrbit.source) continue;
            Vector3 vectorToCenter = sourceOrbit.source.VectorToCenter(pos);
            float sqrDst = vectorToCenter.sqrMagnitude;
            Vector3 forceDir = vectorToCenter.normalized;

            velocity += (forceDir * sourceOrbit.massOverride / sqrDst) * timeStep;
        }

        return velocity;
    }
    
    //===== Draw Orbits =====
    
    private void OnValidate()
    {
        CacheOrbitPoints();
    }

    private void CacheOrbitPoints()
    {
        _cachedOrbitPoints = CalculateOrbit();
        return;
        
        Vector3[] CalculateOrbit()
        {
            Vector3[] drawPoints = new Vector3[_numSteps = Mathf.Abs(_numSteps)];
            VirtualOrbit virtualOrbits = new VirtualOrbit(this);

            // Simulate
            for (int step = 0; step < _numSteps; step++)
            {
                virtualOrbits.velocity += CalculateAcceleration(virtualOrbits.position, _orbitDrawTimeStep);
            
                Vector3 newPos = virtualOrbits.position + virtualOrbits.velocity * _orbitDrawTimeStep;
                virtualOrbits.position = newPos;
                drawPoints[step] = newPos;
            }

            return drawPoints;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying && transform.hasChanged) CacheOrbitPoints();
        if (_drawOrbit) DrawOrbit();
    }
    
    private void DrawOrbit()
    {
        if (_cachedOrbitPoints.Length <= 0) return;
        
        Gizmos.color = Color.orangeRed;
        for (int i = 0; i < _cachedOrbitPoints.Length - 1; i++)
        {
            Gizmos.DrawLine(_cachedOrbitPoints[i], _cachedOrbitPoints[i + 1]);
        }
    }

    //===== Structs =====
    
    private class VirtualOrbit
    {
        public Vector3 position;
        public Vector3 velocity;

        public VirtualOrbit(GravityOrbiter orbiter)
        {
            Vector3 pos = orbiter.transform.position;
            position = new Vector3(pos.x, pos.y, pos.z);
            velocity = orbiter._initialVelocity;
        }
    }

    [System.Serializable]
    public struct SourceOrbit
    {
        public GravitySource source;
        public float massOverride;
    }
}