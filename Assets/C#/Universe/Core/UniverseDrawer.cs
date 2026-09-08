using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UniverseDrawer : Modifier<Universe>
{
   [Space] 
   [SerializeField, BoxGroup("Flags")] private bool _drawUniverseRadius = true;
   [SerializeField, BoxGroup("Flags")] private bool _drawGrid = false;

   [InfoBox("if odd the grid won't render correctly")] 
   [SerializeField, ShowIf(nameof(_drawGrid)), BoxGroup("Grid")] private int _gridSize = 28;
   [SerializeField, ShowIf(nameof(_drawGrid)), BoxGroup("Grid")] private int _yLevel = 0;
   [SerializeField, ShowIf(nameof(_drawGrid)), BoxGroup("Grid")] private bool _drawCoordinates = false;
   [SerializeField, ShowIf(nameof(_drawGrid)), BoxGroup("Grid")] private bool _drawBodyLines = false;
   [SerializeField, ShowIf(nameof(_drawGrid)), BoxGroup("Grid"), Indent] private bool _cullOutOfBounds = false;

   private UniverseBody[] _referenceBodies;
   private Rect _gridBounds;

   protected override void Awake()
   {
      base.Awake();
      _referenceBodies = Universe.SweepForUniverseBodies;
   }
   
   public override void Initialize(Universe universe)
   {
      base.Initialize(universe);
      _referenceBodies = Universe.SweepForUniverseBodies;
   }

   [ContextMenu("Refresh Reference Bodies")]
   private void RefreshReferenceBodies()
   {
      _referenceBodies = Universe.SweepForUniverseBodies;
   }

   private void OnDrawGizmos()
   {
      if (!_target) return;

      if (_drawUniverseRadius)
      {
         Gizmos.color = Color.yellowNice;
         Gizmos.DrawWireSphere(transform.position, _target.UniverseRadius);
      }

      if (_drawGrid)
      {
         Gizmos.color = Color.black;
         int cellSize = _target.UniverseGridSize;

         float totalSize = _gridSize * cellSize;
         Vector3 point = transform.position - new Vector3(totalSize / 2f, 0f, totalSize / 2f);
         _gridBounds = new Rect(-_gridSize/2f, -_gridSize/2f, 1 + _gridSize, 1 + _gridSize);

         int yLevel = _yLevel * cellSize;
         
         for (int x = 0; x <= _gridSize; x++)
         {
            Vector3 start = point + new Vector3(x * cellSize, yLevel, 0f);
            Vector3 end = start + new Vector3(0f, 0f, totalSize);
            Gizmos.DrawLine(start, end);
         }

         for (int z = 0; z <= _gridSize; z++)
         {
            Vector3 start = point + new Vector3(0f, yLevel, z * cellSize);
            Vector3 end = start + new Vector3(totalSize, 0f, 0f);
            Gizmos.DrawLine(start, end);
         }

         if (_drawCoordinates)
         {
#if UNITY_EDITOR
            Handles.Label(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), $"{_target.CurrentUniverseCenter}");

            Vector2Int center = new Vector2Int(_target.CurrentUniverseCenter.x, _target.CurrentUniverseCenter.z);
            int x = (_gridSize / 2) + center.x;
            int z = (_gridSize / 2) + center.y;
            
            point += Vector3.one * 2f;
            Handles.Label(new Vector3(point.x - 16f, point.y, point.z), $"({-x}, {yLevel}, {-z})");
            Handles.Label(new Vector3(point.x - 16f, point.y, point.z + totalSize + 6f), $"({-x}, {yLevel}, {z})");
            Handles.Label(new Vector3(point.x + totalSize , point.y, point.z), $"({x}, {yLevel}, {-z})");
            Handles.Label(new Vector3(point.x + totalSize , point.y, point.z + totalSize), $"({x}, {yLevel}, {z})");
#endif   
         }     

         if (_drawBodyLines)
         {
            _referenceBodies ??= Universe.SweepForUniverseBodies;
            
            Gizmos.color = Color.yellowNice;

            foreach (UniverseBody body in _referenceBodies)
            {
               if (_cullOutOfBounds && !_gridBounds.Contains(new Vector2(body.Coords.x, body.Coords.z))) continue;
               
               Vector3 start = _target.UniverseToWorld(body.Coords);
               Vector3 end = new Vector3(start.x, yLevel, start.z);
#if UNITY_EDITOR
               Color old = Handles.color;
               Handles.color = Gizmos.color;
               Handles.DrawDottedLine(start, end, 5);
               Handles.Label(new Vector3(end.x, end.y + 2f, end.z), $"{body.Coords}");
               Handles.color = old;
#endif
               Gizmos.DrawWireSphere(end, 0.5f);
            }
         }
      }
   }
}
