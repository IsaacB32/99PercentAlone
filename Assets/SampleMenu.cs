using System;
using System.Collections;
using UnityEngine;

public class SampleMenu : MonoBehaviour
{
    private IEnumerator Start()
    {
        MenuInputController.OpenMenuAsRoot(GetComponent<Menu>(), MenuInputController.FinishCloseMenu);
        yield break;
        
        yield return new WaitForSeconds(5f);
        MenuInputController.CloseActiveMenu();
    }
}
