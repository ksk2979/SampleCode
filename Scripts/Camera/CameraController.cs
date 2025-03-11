using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // ī�޶� �׼�Ÿ��
    // IN - ����
    // OUT - �ܾƿ�
    // NOT_ZOOM - �ϰ͵� ����.
    public enum CameraActionType
    {
        ENUM_ZOOM_IN,
        ENUM_ZOOM_OUT,
        ENUM_ZOOM_HOME_IN,
        ENUM_ZOOM_HOME_OUT,
        ENUM_NOT_ZOOM,
    };
    public static CameraController _instance;

    // ī�޶� �� �ð�
    const float _zoomTime = 0.7f;

    // �� ��, �ƿ� ������
    float _zoomInSize;
    float _zoomOutSize;
    float _zoomHomeInSize;
    private void Awake()
    {
        _instance = this;

        // ȭ��� �°� ī�޶� ������ ����ȭ
        // (�Ʒ� ������ Ư�� �����̶�⺸�� �ͳ��� �߷п� ����� ��������)
        float Ratio = Screen.height / (float)Screen.width;

        _zoomInSize = 2.25f * Ratio;
        _zoomOutSize = 5.82f * Ratio; //2.82f * Ratio;
        _zoomHomeInSize = 1.72f * Ratio;

        GetComponent<Camera>().orthographicSize = _zoomOutSize;
    }

    // ī�޶� �׼� public �Լ�
    // ī�޶� �� �ڷ�ƾ�� ȣ���ϰ�, UI Button�� �����ϴ� �ڷ�ƾ�� ȣ����(UI��ư �̵��� �� Ŭ�� �� �ǰ�)
    public void CameraAction(CameraActionType type, Vector3 target)
    {
        StartCoroutine(CameraZoom(type, target));
    }
    public void CameraNotControlAction(CameraActionType type)
    {
        StartCoroutine(CameraZoom(type));
    }

    // ī�޶� Zoom & Move�� ���� �ڷ�ƾ
    // type - ī�޶� �� �׼� Ÿ��
    // targetPosition - ī�޶� ��ġ
    // ���� ī�޶� �����ǿ��� ���� ī�޶� �����Ǳ��� ī�޶� �̵�
    // ī�޶� �� �׼ǿ� ���� ī�޶� �������� Out->In Ȥ�� In->Out
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
            // �� ���� �ƴϸ� �� �׼� ����
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
            // �� ���� �ƴϸ� �� �׼� ����
            if (type != CameraActionType.ENUM_NOT_ZOOM)
                GetComponent<Camera>().orthographicSize = Mathf.Lerp(startScale, targetScale, st);
            yield return null;
        }
        if (type != CameraActionType.ENUM_NOT_ZOOM)
            GetComponent<Camera>().orthographicSize = targetScale;
    }

    // �׼� ���� ī�޶� ���� ����
    public void CameraSetting(bool zoomIn, Vector3 targetPosition)
    {
        transform.position = targetPosition;
        GetComponent<Camera>().orthographicSize = zoomIn == true ? _zoomInSize : _zoomOutSize;
    }

    GameObject _targetObj;
    // ������ ������Ʈ�� ���󰡴� ī�޶� ����
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
