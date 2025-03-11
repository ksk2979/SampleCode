using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StandardFuncWorld
{
    /// <summary>
    ///  카메라 보이는 위치에서 랜덤한 월드좌표를 가져올때 사용한다
    /// </summary>
    /// <returns></returns>
    internal static Vector3 GetRandomMovePos()
    {
        float x = Random.Range(0, Display.displays[0].renderingWidth);
        float y = Random.Range(0, Display.displays[0].renderingHeight);
        return Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0));
    }
    /// <summary>
    ///  뷰 좌료를 기준으로 스크린 좌표를 가져온다
    /// </summary>
    internal static Vector3 GetPixelPos(Vector3 v3WorldPos_)
    {
        Vector3 viewPortPos = Camera.main.WorldToViewportPoint(v3WorldPos_);
        float fX = Display.displays[0].renderingWidth * viewPortPos.x;
        float fY = Display.displays[0].renderingHeight * viewPortPos.y;
        return new Vector3(fX, fY, 0);
    }

    /// <summary>
    ///  뷰 좌료를 기준으로 스크린 좌표를 가져온다
    /// </summary>
    internal static Vector3 GetScreenToViewResolution(Vector3 v3WorldPos_)
    {
        float widthHalf = Display.displays[0].renderingWidth * 0.5f;
        float heightHalf = Display.displays[0].renderingHeight * 0.5f;
        float fX = v3WorldPos_.x - widthHalf;
        float fY = v3WorldPos_.y - heightHalf;
        return new Vector3(fX, fY, 0);
    }

    /// <summary>
    /// 월드 좌료를 기준으로 스크린 좌표를 가져온다
    /// </summary>
    internal static Vector3 GetWorldToScreenPoint(Vector3 v3WorldPos_)
    {
        return Camera.main.WorldToScreenPoint(v3WorldPos_); ;
    }
   
    public static Vector3 GetForwardTargetPos(Transform _trans, Transform _target, float addDist)
    {
        var dist = Vector3.Distance(_trans.position, _target.position) + addDist;
        return _trans.TransformPoint(Vector3.forward * dist);
    }

    public static Vector3 GetForwardTargetPos(Transform _trans, Vector3 to, float addDist)
    {
        var dist = Vector3.Distance(_trans.position, to) + addDist;
        return _trans.TransformPoint(Vector3.forward * dist);
    }

    public static string GetRemainTime(System.TimeSpan timeSpan)
    {
        string reStr = string.Empty;
        if (0 < timeSpan.Hours)
            reStr = string.Format(CommonStaticDatas.STR_LogTimeHours, timeSpan.Hours, timeSpan.Minutes);
        else
            reStr = string.Format(CommonStaticDatas.STR_LogTimeMinutes, timeSpan.Minutes, timeSpan.Seconds);
        return reStr;
    }
}
