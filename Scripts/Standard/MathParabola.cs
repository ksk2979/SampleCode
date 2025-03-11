using System;
using UnityEngine;

public class MathParabola 
{
    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector2.Lerp(start, end, t);

        return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
    }
}

public static class MathTools
{
    /// <summary>
    /// 어느것의 기준으로 타겟방향의 각도를 구한다
    /// </summary>
    public static float GetDegree(Vector3 standard, Vector3 target)
    {
        float mountDot = Vector3.Dot(standard, target);
        float mountAngle = Mathf.Acos(mountDot);
        float mountDeg = mountAngle * Mathf.Rad2Deg;
        return mountDeg;
    }

}