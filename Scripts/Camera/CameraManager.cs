using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SceneStaticObj<CameraManager>
{
    public float _moveSpeed = 3;
    public float _minFollowDistance = 0.1f;
    public float _cameraSize = 6.87f;
    public bool _onlyVertical = false;
    private GameObject _player = null;
    private Transform _myTrans = null;
    private float _xPos;

    private Camera _mainCam = null;
    public Vector3 _offsetVect3 = Vector3.zero;
    private float _defualtFov = 60f;

    public AnimationCurve animationCurve;

    const string TAG_PLAYER = "Player";

    private Vector3 _playerPos
    {
        get
        {
            if (_player == null)
                return Vector3.zero;
            return _player.transform.position + _offsetVect3;
        }
    }

    void Init()
    {
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag(TAG_PLAYER);
        if (_myTrans == null)
            _myTrans = this.transform;
        if (_mainCam == null)
            _mainCam = Camera.main;
        _defualtFov = _mainCam.fieldOfView;
    }

    // 카메라 위치 포지션을 잡아주는 함수
    public void InitPos(Transform cameraParent)
    {
        Init();

        _myTrans.position = cameraParent.position;
        _myTrans.rotation = cameraParent.rotation;
        _xPos = _myTrans.position.x;
        _offsetVect3 = _myTrans.position - _player.transform.position;
    }

    void LateUpdate()
    {
        var desiredRatio = 720.0f / 1280.0f;
        var curScreenRatio = (float)Screen.width / (float)Screen.height;
        var ratio = desiredRatio / curScreenRatio;

        Init();

        //_mainCam.orthographicSize = _cameraSize * ratio; // 평면카메라일때 사용하는것

        _myTrans.position = Vector3.Lerp(_myTrans.position, _playerPos, Time.deltaTime * _moveSpeed);
        if (_onlyVertical)
            _myTrans.position = new Vector3(_xPos, _myTrans.position.y, _myTrans.position.z);
        //RaycastHit hit;
        //if (Physics.Raycast(_myTrans.position, (_playerController._trans.position - _myTrans.position).normalized, out hit, 100, layerMask))
        //{
        //    if (hit.collider != null)
        //    {
        //        var building = hit.collider.GetComponent<ObstacleBuilding>();
        //        if (building != null)
        //            building.OnCamera();
        //    }
        //}
    }

    public void ZoomIn(float duration, float zoomValue, System.Action callBack)
    {
        StartCoroutine(ZoomInCo(duration, zoomValue, callBack));
    }

    IEnumerator ZoomInCo(float duration, float zoomValue, System.Action callBack)
    {
        Time.timeScale = 0.4f;
        float flowTime = 0;
        while (flowTime < duration)
        {
            flowTime += Time.deltaTime;
            yield return null;
            _mainCam.fieldOfView = Mathf.Lerp(_mainCam.fieldOfView,
                zoomValue * animationCurve.Evaluate(flowTime / duration),
                Time.deltaTime);
        }
        Time.timeScale = 1;
        _mainCam.fieldOfView = _defualtFov;
        if (callBack != null)
            callBack();
    }
    public void ResetPos(Transform cameraParent)
    {
        _mainCam.transform.position = cameraParent.position;
        _mainCam.transform.rotation = cameraParent.rotation;
    }

    // 쉐이크 효과를 시작하는 함수
    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition += new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 기다림
        }
    }

    public void OffSetSetting(Vector3 pos)
    {
        _offsetVect3 = pos;
    }

    public Transform GetTrans { get { return _myTrans; } }
}
