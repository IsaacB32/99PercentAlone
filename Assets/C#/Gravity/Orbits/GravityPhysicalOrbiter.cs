using System;
using Isaac.Attributes;
using UnityEngine;

/// <summary>
/// Simulates orbits with the physics the other GravityBodies use
/// </summary>
public class GravityPhysicalOrbiter : MonoBehaviour
{
    [Header("Orbit Settings")] [SerializeField]
    private Vector3 _initialVelocity = Vector3.right;

    [Serializable]
    public struct SourceOrbit
    {
        public GravitySource source;
        public float massOverride;
    }
    [SerializeField] private SourceOrbit[] _sources;

    [Header("Visualize Orbit")] 
    [SerializeField] private bool _drawOrbit = true;
    [ReadOnly(nameof(_drawOrbit))] [SerializeField] private int _numSteps = 1000;
    [ReadOnly(nameof(_drawOrbit)), Min(0.1f)] [SerializeField] private float _orbitDrawTimeStep = 0.1f;

    //===== References =====
    private Rigidbody _rb;
    private Vector3 _velocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _velocity = _initialVelocity;
    }

    //===== Calculations =====

    private void FixedUpdate()
    {
        _velocity += CalculateAcceleration(_rb.position, Time.fixedDeltaTime);
        _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
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

#if UNITY_EDITOR

    private Vector3[] _cachedOrbitPoints;

    private void OnValidate()
    {
        CacheOrbitPoints();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying && transform.hasChanged) CacheOrbitPoints();
        if (_drawOrbit) DrawOrbit();
    }

    /// <summary>
    /// Draw the predicted orbit of the body
    /// </summary>
    private void DrawOrbit()
    {
        if (_cachedOrbitPoints.Length <= 0) return;

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

    private class VirtualOrbit
    {
        public Vector3 position;
        public Vector3 velocity;

        public VirtualOrbit(GravityPhysicalOrbiter orbiter)
        {
            Vector3 pos = orbiter.transform.position;
            position = new Vector3(pos.x, pos.y, pos.z);
            velocity = orbiter._initialVelocity;
        }
    }

#endif
    
}
