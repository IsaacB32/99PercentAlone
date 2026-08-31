using System.Linq;
using UnityEngine;

/// <summary>
/// The boundraies of the universe, resets the ship position and updates all IUniverseBody objects when distance exceeded
/// </summary>
public class UniverseBoundaries : MonoBehaviour
{
    public static Vector3 WorldOrigin = Vector3.zero;
    
    [SerializeField] private float _universeRadius = 10f;
    [SerializeField] private ShipMovement _shipMovement;

    private UniverseEngine _universeEngine;
    
    //=!= Expensive =!=
    /// <summary>
    /// Scan through the entire scene and find all IUniverseBodies
    /// </summary>
    public static IUniverseBody[] SweepForUniverseBodies => FindObjectsByType<MonoBehaviour>().OfType<IUniverseBody>().ToArray();

    #region Subscribe

    private void OnEnable()
    {
        _shipMovement.OnShipMove += ShipHasMoved;
    }

    private void OnDisable()
    {
        _shipMovement.OnShipMove -= ShipHasMoved;
    }

    #endregion

    private void Awake()
    {
        _universeEngine = new UniverseEngine(SweepForUniverseBodies);
    }

    private void ShipHasMoved(float distance)
    {
        if (distance > _universeRadius)
        {
            Vector3 currentPos = _shipMovement.ResetToWorldOrigin();
            _universeEngine.UpdateUniverse(currentPos);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellowNice;
        Gizmos.DrawWireSphere(transform.position, _universeRadius);
    }
}
