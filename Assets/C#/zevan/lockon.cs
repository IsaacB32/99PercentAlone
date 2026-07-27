using UnityEngine;

public class lockon : MonoBehaviour
{

    public GameObject looker;
    public Transform lookAtThis;

    void Update()
    {transform.LookAt(lookAtThis);
        
    }
}
