﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    void Start()
    {
        Camera camera = this.GetComponent<Camera>();
        Rect rect = camera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / ((float)9 / 16); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        camera.rect = rect;
    }

    void OnPreCull() => GL.Clear(true, true, Color.black);

    //private void Awake()
    //{
    //    Camera camera = GetComponent<Camera>();
    //    Rect rect = camera.rect;
    //
    //    // 스크린의 해상도 / 맞출 비율 (9/16)
    //    float scaleHeight = ((float)Screen.width / Screen.height) / ((float)9 / 16); // (가로 / 세로)
    //    float scaleWidth = 1f / scaleHeight;
    //    if (scaleHeight < 1)
    //    {
    //        rect.height = scaleHeight;
    //        rect.y = (1f - scaleHeight) / 2f;
    //    }
    //    else
    //    {
    //        rect.width = scaleWidth;
    //        rect.x = (1f - scaleWidth) / 2f;
    //    }
    //
    //    camera.rect = rect;
    //}

}
