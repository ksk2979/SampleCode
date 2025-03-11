using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 현재 해상도를 불러와서 비율을 지정하는 스크립트
public class CanvasControl : MonoBehaviour
{
    CanvasScaler _canvas;
    
    private void Start()
    {
        _canvas = this.GetComponent<CanvasScaler>();
        //Default 해상도 비율
        float fixedAspectRatio = 9f / 16f;

        //현재 해상도의 비율
        float currentAspectRatio = (float)Screen.width / (float)Screen.height;

        //현재 해상도 가로 비율이 더 길 경우
        if (currentAspectRatio > fixedAspectRatio) _canvas.matchWidthOrHeight = 1;
        //현재 해상도의 세로 비율이 더 길 경우
        else if (currentAspectRatio < fixedAspectRatio) _canvas.matchWidthOrHeight = 0;
    }
}
