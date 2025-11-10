using System;
using UnityEngine;

[Serializable]
public struct TimeOfDay
{
    [Range(0, 23)] public int _hour;
    [Range(0, 59)] public int _minute;

    public TimeOfDay(int h, int m) { _hour = Mathf.Clamp(h, 0, 23); _minute = Mathf.Clamp(m, 0, 59); }
    public bool EqualsHM(DateTime t) => t.Hour == _hour && t.Minute == _minute;
    public override string ToString() => $"{_hour:00}:{_minute:00}";
}
