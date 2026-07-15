using UnityEngine;

/// <summary>
/// Single Boid object to be simulated by a BoidsManager 
/// </summary>
public class Boid : MonoBehaviour
{
    //===== Positional Data =====
    public Vector3 Position { get; private set; }
    public Vector3 Direction { get; private set; }
    private Vector3 _velocity;
    
    //===== Cached Values =====
    private Transform _cachedTransform;
    private Transform _target;
    private BoidManager.BoidSettings _settings;
    
    private void Awake()
    {
        _cachedTransform = transform;
    }

    /// <summary>
    /// Setup a boid object 
    /// </summary>
    public void Initialize(BoidManager.BoidSettings settings, Transform target)
    {
        _settings = settings;
        _target = target;
        
        Position = _cachedTransform.position;
        Direction = _cachedTransform.forward;

        float startSpeed = (_settings.maxSpeed + _settings.minSpeed) / 2f;
        _velocity = transform.forward * startSpeed;
    }
    
    /// <summary>
    /// Calculate boid position using the provided data
    /// </summary>
    /// <param name="data">Data telling how the boid to move</param>
    public void UpdateBoid(BoidManager.BoidData data)
    {
        Vector3 acceleration = Vector3.zero;

        if (_target) 
        {
            Vector3 offsetToTarget = _target.position - Position;
            acceleration = SteerTowards(offsetToTarget) * _settings.targetWeight;
        }
        
        if (data.num_flockmates != 0)
        {
            data.flock_center /= data.num_flockmates;

            Vector3 offsetToFlockmatesCentre = (data.flock_center - Position);

            Vector3 alignmentForce = SteerTowards(data.flock_direction) * _settings.alignmentWeight;
            Vector3 cohesionForce = SteerTowards(offsetToFlockmatesCentre) * _settings.cohesionWeight;
            Vector3 separationForce = SteerTowards(data.separation_direction) * _settings.avoidanceWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += separationForce;
        }

        _velocity += acceleration * Time.deltaTime;
        float speed = _velocity.magnitude;
        speed = Mathf.Clamp(speed, _settings.minSpeed, _settings.maxSpeed);
        Vector3 dir = _velocity / speed;
        _velocity = dir * speed;

        _cachedTransform.position += _velocity * Time.deltaTime;
        _cachedTransform.forward = dir;
        
        Position = _cachedTransform.position;
        Direction = dir;
    }

    /// <summary>
    /// Helper function, returns a vector towards a given target
    /// </summary>
    Vector3 SteerTowards(Vector3 target)
    {
        Vector3 v = target.normalized * _settings.maxSpeed - _velocity;    
        return Vector3.ClampMagnitude(v, _settings.maxSteerForce);
    }
}
