using System.Collections.Generic;
using UnityEngine;

internal static class YieldInstructionCache
{
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }
        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

    private static readonly Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(new FloatComparer());
    private static readonly Dictionary<float, WaitForSecondsRealtime> _realTimeInterval = new Dictionary<float, WaitForSecondsRealtime>(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds wfs;
        if (!_timeInterval.TryGetValue(seconds, out wfs))
            _timeInterval.Add(seconds, wfs = new WaitForSeconds(seconds));
        return wfs;
    }
    public static WaitForSecondsRealtime RealTimeWaitForSeconds(float seconds)
    {
        WaitForSecondsRealtime wfs;
        if (!_realTimeInterval.TryGetValue(seconds, out wfs))
            _realTimeInterval.Add(seconds, wfs = new WaitForSecondsRealtime(seconds));
        return wfs;
    }
}
