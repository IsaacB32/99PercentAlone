using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orientator : MonoBehaviour
{
    public float horizontal;
    public GameObject yAxis;
    public float vertical;
    public GameObject xAxis;

    private void FixedUpdate()
    {
        horizontal = yAxis.transform.rotation.eulerAngles.y;
        vertical = xAxis.transform.rotation.eulerAngles.x;
    }
}
