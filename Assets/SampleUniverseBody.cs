using System;
using UnityEngine;

public class SampleUniverseBody : MonoBehaviour, IUniverseBody
{
    public void MoveBody(Vector3 movement)
    {
        transform.position += movement;
    }
}
