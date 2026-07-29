using System;
using NaughtyAttributes;
using Isaac.Extensions;
using UnityEngine;

/// <summary>
/// Elliptical orbit simulator 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GravityOrbiter : MonoBehaviour
{
    public enum OrbitType
    {
        Elliptical,
        Reality
    }
    [SerializeField] private OrbitType _orbitType;
    
    [Header("Center of Gravity")]
    [SerializeField] private GameObject[] _sources = Array.Empty<GameObject>();
    
    [Header("Orbit Settings")]
    [SerializeField] private bool _adjustOrbitSettings = true;
    [ShowIf(nameof(_adjustOrbitSettings))] [SerializeField] private Vector3 _positionOffset;
    [ShowIf(nameof(_adjustOrbitSettings)), Range(0, 2*Mathf.PI)] [SerializeField] private float _startingOrbitPosition = 0f;
    [ShowIf(nameof(_adjustOrbitSettings))] [SerializeField] private float _semiMajorAxis = 25f;
    [ShowIf(nameof(_adjustOrbitSettings)), Range(0.1f, 0.95f)] [SerializeField] private float _eccentricity = 0.5f;
    [ShowIf(nameof(_adjustOrbitSettings)), Min(0.5f)] [SerializeField] private float _orbitalPeriod = 5f;
    [ShowIf(nameof(_adjustOrbitSettings))] [SerializeField] private bool _simulate = false;

    //===== Orbit Values =====
    private float _semiMinorAxis;
    private float _meanMotion;
    private Quaternion _rotation = Quaternion.identity;
    
    //===== Orbit Type =====
    private Func<float, Vector3> CalculateOrbit;
    
    //===== Rotations =====
    public bool RenderOrbit => _adjustOrbitSettings;
    // ReSharper disable once ConvertToAutoProperty
    public Quaternion Rotation
    {
        get => _rotation;
        set => _rotation = value;
    }

    private void Awake()
    {
        _semiMinorAxis = _semiMajorAxis * Mathf.Sqrt(1 - _eccentricity.sqr());
        _meanMotion = 2 * Mathf.PI / _orbitalPeriod;
        
        CalculateOrbit = _orbitType switch
        {
            OrbitType.Elliptical => CalculateParameterizedOrbitAtTime,
            OrbitType.Reality => CalculateOrbitAtTime,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void Update()
    {
        transform.position = CalculateOrbit(Time.time);
    }

    //===== Calculations =====
    
    /// <summary>
    /// Given a time find the orbital position of the object
    /// </summary>
    /// <returns>center adjusted orbital position</returns>
    private Vector3 CalculateOrbitAtTime(float time)
    {
        Vector3 center = FindCenterOfMass();
        
        float meanAnomaly = _startingOrbitPosition + _meanMotion * time;
        meanAnomaly = Mathf.Repeat(meanAnomaly, Mathf.PI * 2f);
    
        float eccentricAnomaly = SolveKeplerEquation();
    
        float x = _semiMajorAxis * Mathf.Cos(eccentricAnomaly);
        float y = _semiMinorAxis * Mathf.Sin(eccentricAnomaly);
    
        return center + new Vector3(x, 0f, y);
    
        float SolveKeplerEquation()
        {
            float E = meanAnomaly;
            for (int i = 0; i < 50; i++)
            {
                float f = E - _eccentricity * Mathf.Sin(E) - meanAnomaly;
                float fPrime = 1 - _eccentricity * Mathf.Cos(E);
    
                float delta = f / fPrime;
                E -= delta;
    
                // stop once the correction is smaller than tolerance
                if (Mathf.Abs(delta) < 1e-6) break;
            }
    
            return E;
        }
    }
    
    /// <summary>
    /// Calculate the parameterized orbit for drawing the orbit 
    /// </summary>
    private Vector3 CalculateParameterizedOrbitAtTime(float time)
    {
        time = Mathf.Repeat(time, 2 * Mathf.PI);
        
        Vector3 pos = Vector3.zero;
        pos.x = _semiMajorAxis * Mathf.Cos(time);
        pos.z = _semiMinorAxis * Mathf.Sin(time);
        pos = Rotation * pos;
        return FindCenterOfMass() + pos;
    }
    
    /// <summary>
    /// Find the center of the orbit
    /// </summary>
    public Vector3 FindCenterOfMass()
    {
        Vector3 center = Vector3.zero;
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (GameObject source in _sources)
        {
            if (!source) continue;
            center += source.transform.position;
        }

        if (center.Equals(Vector3.zero)) return center;
        return (center / _sources.Length) + _positionOffset;
    }

    /// <summary>
    /// Align the object to its first orbital positon
    /// </summary>
    [Button]
    private void AlignToOrbit()
    {
        transform.position = CalculateParameterizedOrbitAtTime(_startingOrbitPosition);
    }

    [Button]
    private void ResetOrbitRotation()
    {
        Rotation = Quaternion.identity;
    }

    //===== Draw Orbits =====
    
#if UNITY_EDITOR
    
    private Vector3[] _cachedOrbitPoints;
    
    public void OnValidate()
    {
        _semiMinorAxis = _semiMajorAxis * Mathf.Sqrt(1 - _eccentricity.sqr());
        _meanMotion = 2 * Mathf.PI / _orbitalPeriod;
        CacheOrbitPoints();

        CalculateOrbit = _orbitType switch
        {
            OrbitType.Elliptical => CalculateParameterizedOrbitAtTime,
            OrbitType.Reality => CalculateOrbitAtTime,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (!_adjustOrbitSettings && _simulate) _simulate = false;
        
        AlignToOrbit();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            if (transform.hasChanged && !_simulate) CacheOrbitPoints();
            else if (_simulate)
            {
                transform.position = CalculateOrbit(Time.time);
            }
        }
        if (_adjustOrbitSettings) DrawOrbit();
    }
    
    /// <summary>
    /// Draw the predicted orbit of the body
    /// </summary>
    private void DrawOrbit()
    {
        if (_cachedOrbitPoints.Length <= 0) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(FindCenterOfMass(), 3f);
        
        Gizmos.color = Color.orangeRed;
        for (int i = 0; i < _cachedOrbitPoints.Length - 1; i++)
        {
            Gizmos.DrawLine(_cachedOrbitPoints[i], _cachedOrbitPoints[i + 1]);
        }
    }
    
    /// <summary>
    /// Save the orbit positions to be drawn easily 
    /// </summary>
    private void CacheOrbitPoints()
    {
        _cachedOrbitPoints = new Vector3[50];
        for (int i = 0; i < _cachedOrbitPoints.Length; i++)
        {
            _cachedOrbitPoints[i] = CalculateParameterizedOrbitAtTime(i / (2 * Mathf.PI));
        }
    }
    
#endif
    
}