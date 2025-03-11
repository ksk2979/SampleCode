using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class Util : MonoBehaviour
{
    public static int GetRandomValue(float min, float max, float[] randomFactors)
    {
        var rn = UnityEngine.Random.Range(min, max);
        float current = 0;
        for (int i = 0; i < randomFactors.Length; i++)
        {
            current += randomFactors[i];
            if (rn < current)
            {
                return i;
            }
        }
        return randomFactors.Length;
    }
 

    //===================================================================
    const double _PI_180 = Math.PI / 180;
    const double _180_PI = 180 / Math.PI;
    public static double Degree2Radian(float _fDegree) { return _fDegree * _PI_180; }
    public static double Radian2Degree(float _fRadian) { return _fRadian * _180_PI; }

    //===================================================================
    // 특정 각도에 특정 거리의 좌표 얻기.
    public static Vector2 GetPos_Circle(float _fDegree, float _fRadiusW, float _fRadiusH, float _fCX = 0, float _fCY = 0)
    {
        return new Vector2((float)(_fCX + _fRadiusW * Math.Cos(Degree2Radian(_fDegree))),
                            (float)(_fCY + _fRadiusH * Math.Sin(Degree2Radian(_fDegree))));
    }

    //===================================================================
    // 특정 각도에 특정 거리의 좌표 얻기.
    public static Vector3 GetPos_CircleV3(float _fDegree, float _fRadiusW, float _fRadiusH, float _fCX = 0, float _fCY = 0)
    {
        return new Vector3((float)(_fCY + _fRadiusH * Math.Sin(Degree2Radian(_fDegree))), 
                            0,
                           (float)(_fCX + _fRadiusW * Math.Cos(Degree2Radian(_fDegree))));
    }

    public static Vector3 GetPos_CircleV3ToTarget(Transform tarns, float range, float angle)
    {
        float minDist = 10;
        minDist += UnityEngine.Random.Range(0, range);
        var halfAngle = (angle / 2);
        var degree = UnityEngine.Random.Range(-halfAngle, halfAngle) + tarns.eulerAngles.y;
        var pos = GetPos_CircleV3(degree, minDist, minDist);
        return tarns.position + pos;
    }

    [Conditional("TRACE_ON")]

    public static void DebugLogWrap(string str)
    {
        UnityEngine.Debug.Log(str);
    }
}
