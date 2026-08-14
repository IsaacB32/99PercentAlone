using System;
using System.Collections;
using ITween;
using UnityEngine;

public class SampleTweener : MonoBehaviour
{
    [SerializeField] private Transform _target;
    private IEnumerator Start()
    {
        Tween t = transform.IT_Move(_target, 5f);
        t.SetEase(EasingType.InOutBounce);
        yield return new WaitForSeconds(1f);
        // t.Kill();
    }
}
