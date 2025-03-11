using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 카메라 액션타입
    // IN - 줌인
    // OUT - 줌아웃
    // NOT_ZOOM - 암것도 안함.
    public enum CameraActionType
    {
        ENUM_ZOOM_IN,
        ENUM_ZOOM_OUT,
        ENUM_ZOOM_HOME_IN,
        ENUM_ZOOM_HOME_OUT,
        ENUM_NOT_ZOOM,
    };
    public static CameraController _instance;

    // 카메라 줌 시간
    const float _zoomTime = 0.7f;

    // 줌 인, 아웃 사이즈
    float _zoomInSize;
    float _zoomOutSize;
    float _zoomHomeInSize;
    private void Awake()
    {
        _instance = this;

        // 화면비에 맞게 카메라 사이즈 정규화
        // (아래 값들은 특정 공식이라기보단 귀납적 추론에 도출된 보정값임)
        float Ratio = Screen.height / (float)Screen.width;

        _zoomInSize = 2.25f * Ratio;
        _zoomOutSize = 5.82f * Ratio; //2.82f * Ratio;
        _zoomHomeInSize = 1.72f * Ratio;

        GetComponent<Camera>().orthographicSize = _zoomOutSize;
    }

    // 카메라 액션 public 함수
    // 카메라 줌 코루틴을 호출하고, UI Button을 제어하는 코루틴을 호출함(UI버튼 이동질 중 클릭 안 되게)
    public void CameraAction(CameraActionType type, Vector3 target)
    {
        StartCoroutine(CameraZoom(type, target));
    }
    public void CameraNotControlAction(CameraActionType type)
    {
        StartCoroutine(CameraZoom(type));
    }

    // 카메라 Zoom & Move를 위한 코루틴
    // type - 카메라 줌 액션 타입
    // targetPosition - 카메라 위치
    // 현재 카메라 포지션에서 목적 카메라 포지션까지 카메라 이동
    // 카메라 줌 액션에 따라 카메라 스케일을 Out->In 혹은 In->Out
    IEnumerator CameraZoom(CameraActionType type, Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float targetScale = 0;
        float startScale = 0;
        switch (type)
        {
            case CameraActionType.ENUM_ZOOM_IN:
                {
                    targetScale = _zoomInSize;
                    startScale = _zoomOutSize;
                }
                break;
            case CameraActionType.ENUM_ZOOM_OUT:
                {
                    targetScale = _zoomOutSize;
                    startScale = _zoomInSize;
                }
                break;
            case CameraActionType.ENUM_ZOOM_HOME_IN:
                {
                    targetScale = _zoomHomeInSize;
                    startScale = _zoomInSize;
                }
                break;
            case CameraActionType.ENUM_ZOOM_HOME_OUT:
                {
                    targetScale = _zoomInSize;
                    startScale = _zoomHomeInSize;
                }
                break;
            case CameraActionType.ENUM_NOT_ZOOM:
                {

                }
                break;
        }

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / _zoomTime)
        {
            float st = Mathf.SmoothStep(0.0f, 1.0f, t);
            transform.position = new Vector3(Mathf.Lerp(startPosition.x, targetPosition.x, t),
                Mathf.Lerp(startPosition.y, targetPosition.y, t), -10.0f);
            // 낫 줌이 아니면 줌 액션 ㄱㄱ
            if (type != CameraActionType.ENUM_NOT_ZOOM)
                GetComponent<Camera>().orthographicSize = Mathf.Lerp(startScale, targetScale, st);
            yield return null;
        }
        transform.position = targetPosition;
        if (type != CameraActionType.ENUM_NOT_ZOOM)
            GetComponent<Camera>().orthographicSize = targetScale;
    }
    IEnumerator CameraZoom(CameraActionType type)
    {
        Vector3 startPosition = transform.position;
        float targetScale = 0;
        float startScale = 0;
        switch (type)
        {
            case CameraActionType.ENUM_ZOOM_IN:
                {
                    targetScale = _zoomInSize;
                    startScale = _zoomOutSize;
                }
                break;
            case CameraActionType.ENUM_ZOOM_OUT:
                {
                    targetScale = _zoomOutSize;
                    startScale = _zoomInSize;
                }
                break;
            case CameraActionType.ENUM_NOT_ZOOM:
                {

                }
                break;
        }

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / _zoomTime)
        {
            float st = Mathf.SmoothStep(0.0f, 1.0f, t);
            // 낫 줌이 아니면 줌 액션 ㄱㄱ
            if (type != CameraActionType.ENUM_NOT_ZOOM)
                GetComponent<Camera>().orthographicSize = Mathf.Lerp(startScale, targetScale, st);
            yield return null;
        }
        if (type != CameraActionType.ENUM_NOT_ZOOM)
            GetComponent<Camera>().orthographicSize = targetScale;
    }

    // 액션 없는 카메라 강제 세팅
    public void CameraSetting(bool zoomIn, Vector3 targetPosition)
    {
        transform.position = targetPosition;
        GetComponent<Camera>().orthographicSize = zoomIn == true ? _zoomInSize : _zoomOutSize;
    }

    GameObject _targetObj;
    // 지정된 오브젝트를 따라가는 카메라 셋팅
    public void CameraTargetObjact(GameObject target)
    {
        _targetObj = target;
        this.transform.position = new Vector3(_targetObj.transform.position.x, _targetObj.transform.position.y, -10f);
        StartCoroutine(TargetObjMove());
    }

    IEnumerator TargetObjMove()
    {
        while (_targetObj != null)
        {
            if (_moveSmoothOn)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, _targetObj.transform.position, Mathf.SmoothStep(0.0f, 1.0f, Time.deltaTime * 6f));

                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10f);
            }
            else
            {
                this.transform.position = new Vector3(_targetObj.transform.position.x, _targetObj.transform.position.y, -10f);
            }
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }
    }

    bool _moveSmoothOn = false;
    public void ChangeTargetMoveSetting()
    {
        if (_moveSmoothOn) { _moveSmoothOn = false; }
        else { _moveSmoothOn = true; }
    }

    public void CameraUnTargetObj()
    {
        _targetObj = null;
        //CameraSetting(false, new Vector3(0f, 0f, -10f));
    }
}
