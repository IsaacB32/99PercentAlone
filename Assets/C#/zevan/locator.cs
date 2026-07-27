using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class locator : MonoBehaviour
{
    
    public float horizontal;
    public GameObject yAxis;
    
    public float vertical;
    public GameObject xAxis;

    public GameObject looker;
    
    public GameObject target;
    public float targetX;
    public float targetY;

    public float proximity;
    public float facingTargetY;

    public Vector3 vectorTarget;

    private void FixedUpdate()
    {
        horizontal = yAxis.transform.rotation.eulerAngles.y;
        vertical = xAxis.transform.rotation.eulerAngles.x;

        targetY = looker.transform.rotation.eulerAngles.y;
        targetX = looker.transform.rotation.eulerAngles.x;

        facingTargetY = (Mathf.Abs(horizontal-targetY) + Mathf.Abs(vertical-targetX));








    }
}
