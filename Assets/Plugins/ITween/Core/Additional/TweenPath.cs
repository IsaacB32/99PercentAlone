using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace ITween.Pathing
{
    public interface ITweenPath
    {
        public Vector3 Pos { get; }
    }

    public struct ITween_TransformPath : ITweenPath
    {
        public Transform transform;
        public Vector3 Pos => transform.position;
    }

    public struct ITween_VectorPath : ITweenPath
    {
        public Vector3 Pos { get; set; }
    }
    
    /// <summary>
    /// Helper Path for Tweening along 
    /// </summary>
    public class TweenPath<T> where T : ITweenPath
    {
        private readonly PathPoint[] _waypoints;
        public float TotalDistance { get; }
        private int _currentTargetPointIndex;

        //===== Constructors =====
        
        public TweenPath(T starting, [NotNull] T[] points)
        {
            if (points.Length <= 1) throw new Exception($"points must be longer than length 1, currently {points.Length}");
            
            _waypoints = new PathPoint[points.Length + 1];
            _waypoints[^1] = new PathPoint(points[^1], null);
            int waypointIndex = _waypoints.Length - 2;
            for (int i = points.Length - 2; i >= 0; i--)
            {
                _waypoints[waypointIndex] = new PathPoint(points[i], _waypoints[waypointIndex + 1]);
                waypointIndex--;
            }
            _waypoints[0] = new PathPoint(starting, _waypoints[1]);
            _currentTargetPointIndex = 0;
            
            TotalDistance = _waypoints.Sum(point => point.Distance);
            CalculateDistanceFromStart();
        }
        
        private void CalculateDistanceFromStart()
        {
            float suffixSum = 0f;
            for (int i = _waypoints.Length - 1; i >= 0; i--)
            {
                suffixSum += _waypoints[i].Distance;
                _waypoints[i].DistanceFromStart = TotalDistance - suffixSum;
            }
        }

        //===== Control =====

        public PathPoint FindTarget(float progress, Action<int> onStepComplete)
        {
            progress = Mathf.Clamp01(progress);
            float distanceTraveled = TotalDistance * progress;
            
            int pathIndex = _waypoints.Length - 2;
            for (int i = 0; i < _waypoints.Length - 1; i++)
            {
                if (_waypoints[i + 1].DistanceFromStart > distanceTraveled)
                {
                    pathIndex = i;
                    break;
                }
            }
            
            _waypoints[pathIndex].CalculateRelativeProgress(distanceTraveled);
            if (pathIndex != _currentTargetPointIndex)
            {
                onStepComplete?.Invoke(pathIndex);
                _currentTargetPointIndex = pathIndex;
            }
            
            return _waypoints[pathIndex];
        }
        
        //===== Helper =====
        
        public class PathPoint
        {
            public bool IsEnd => Next == null;
            
            public ITweenPath Point { get; }
            public PathPoint Next { get; }
            public float Distance { get; } 
            public float DistanceFromStart { get; set; }

            private float _relativeProgress;
            
            public PathPoint(ITweenPath point, PathPoint next)
            {
                Point = point;
                Next = next;
                Distance = next == null ? 0f : Vector3.Distance(next.Point.Pos, point.Pos);
            }

            public void CalculateRelativeProgress(float distanceTraveled)
            {
                float relativeDistance = distanceTraveled - DistanceFromStart;
                _relativeProgress = relativeDistance / Distance;
            }
            
            public float GetRelativeProgress() { return _relativeProgress; }
            
            public override string ToString()
            {
                return Point.ToString();
            }
        }
    }
}
