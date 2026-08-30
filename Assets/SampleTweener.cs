using System;
using System.Collections;
using ITween;
using UnityEngine;

public class SampleTweener : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private TweenSettings_Visibility _settings;

    private VisibilityTween t;

    private IEnumerator Start()
    {
        t = transform.IT_Move(_target, _settings);
        yield break;
    }

    public void GO()
    {
        t.SetVisible(true);
    }
}
