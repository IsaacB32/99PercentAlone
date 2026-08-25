using System;
using System.Collections;
using ITween;
using UnityEngine;

public class SampleTweener : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private TweenSettings_Visibility _settings;

    private IEnumerator Start()
    {
        VisibilityTween t = transform.IT_Move(_target, _settings);
        yield return new WaitForSeconds(1f);
        t.SetVisible(true);
        // yield return new WaitForSeconds(2f);
        // t.SetVisible(true);
        

        yield break;
    }
}
