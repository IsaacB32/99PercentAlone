using System;
using System.Collections.Generic;
using UnityEngine;

public class RadioManager : MonoBehaviour
{
    private HashSet<RadioSignal> _activeSignals = new HashSet<RadioSignal>();
    public void AddRadioSignal(RadioSignal signal) { _activeSignals.Add(signal); }
    public void RemoveRadioSignal(RadioSignal body) { _activeSignals.Remove(body); }
    public void RemoveRadioSignalWhere(Predicate<RadioSignal> match) { _activeSignals.RemoveWhere(match); }
}
