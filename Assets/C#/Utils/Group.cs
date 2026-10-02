using System;
using UnityEngine;

/// <summary>
/// A Group is for GameObjects that need to enable and disable together
/// </summary>
public class Group : MonoBehaviour
{
    [SerializeField] private bool _isActive = true;
    [SerializeField] private GameObject[] _groupObjects;
    
    public void SetActive(bool isActive)
    {
        foreach (GameObject g in _groupObjects) g.SetActive(isActive);
    }

    private void OnValidate() { SetActive(_isActive); }
}
