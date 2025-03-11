using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafeAreaResizer : MonoBehaviour
{
    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = gameObject.GetComponent<RectTransform>();
            }

            return _rectTransform;
        }
    }

    RectTransform _rectTransform;
    DeviceOrientation _deviceOrientation = DeviceOrientation.Unknown;
    public DeviceOrientation defaultOrient;
    public Vector2 correction = new Vector2(40f, 0);


    Rect LastSafeArea = new Rect(0, 0, 0, 0);
    public static AndroidJavaClass unityPlayer;

#if UNITY_IOS && !UNITY_ANDROID && !UNITY_EDITOR
	void Start()
	{
        defaultOrient = Input.deviceOrientation;
        SetIOSSafeArea(defaultOrient);
	}
#elif UNITY_ANDROID && !UNITY_IOS && !UNITY_EDITOR
    private void Start()
    {
        SetAndroidSafeArea();
    }
#endif

    #region iOS
#if UNITY_IOS && !UNITY_ANDROID && !UNITY_EDITOR
    public void SetIOSSafeArea(DeviceOrientation orientation)
    {
        if (UnityEngine.iOS.Device.generation >= UnityEngine.iOS.DeviceGeneration.iPhoneX)
        {
            ResizeArea(orientation);
        }
    }
#endif

    private void Update()
    {
#if UNITY_IOS && !UNITY_ANDROID && !UNITY_EDITOR
        SetIOSSafeArea(Input.deviceOrientation);
#elif UNITY_ANDROID && !UNITY_IOS && !UNITY_EDITOR
        SetAndroidSafeArea();
#endif
    }

    void ResizeArea(DeviceOrientation orientation)
    {
        if (_deviceOrientation != orientation)
        {
            _deviceOrientation = orientation;
            Rect safeArea = Screen.safeArea;
            Vector2 newAnchorMax = safeArea.position + safeArea.size + correction;

            newAnchorMax.x /= Screen.width;
            newAnchorMax.y /= Screen.height;

            switch (_deviceOrientation)
            {
                case DeviceOrientation.FaceUp:
                    {
                        rectTransform.anchorMin = Vector2.one - newAnchorMax;
                        rectTransform.anchorMax = Vector2.one;
                    }
                    break;
            }
        }
    }
    #endregion

    #region Android

#if UNITY_ANDROID && !UNITY_IOS && !UNITY_EDITOR
    public void SetAndroidSafeArea()
    {
        if (getSDKInt() >= 28)
        {
            Refresh();
        }
    }
#endif
    private int getSDKInt()
    {
        using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return version.GetStatic<int>("SDK_INT");
        }
    }
    void Refresh()
    {
        Rect safeArea = GetSafeArea();

        if (safeArea != LastSafeArea)
            ApplySafeArea(safeArea);
    }

    Rect GetSafeArea()
    {
        return Screen.safeArea;
    }

    void ApplySafeArea(Rect r)
    {
        LastSafeArea = r;

        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
    #endregion
}
