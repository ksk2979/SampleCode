using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowTargetPoint : MonoBehaviour
{
    GameObject pointerObj;
    List<TargetPointUI> _targets = new List<TargetPointUI>();

    CanvasScaler _canvasScaler;
    Transform _playerTrans;

    float _borderSize = 50f;

    public void Init()
    {
        _canvasScaler = this.transform.parent.transform.parent.GetComponent<CanvasScaler>();
        _playerTrans = StageManager.GetInstance.GetPlayers()[0].GetCharacterCtrl()._trans;
        pointerObj = transform.Find("Pointer").gameObject;
        Hide();
    }

    private void OnEnable()
    {
        for (int i = 0; i < _targets.Count; i++)
            _targets[i]._pointerRectTransform.gameObject.SetActive(false);
        _targets.Clear();
    }

    private void Update()
    {
        if (_targets.Count == 0) { gameObject.SetActive(false); }
        for (int i = 0; i < _targets.Count; i++)
        {
            if (_targets[i]._target == null)
            {
                _targets.RemoveAt(i);
                continue;
            }
            Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(_targets[i]._target.position);
            //var resolution = WordToScenePoint();
            // 카메라 뒤에 있는 타겟 처리 (Z축 값이 음수일 경우)
            if (targetPositionScreenPoint.z < 0)
            {
                // Y 좌표를 스크린 높이에서 빼서 Y 좌표를 반전
                targetPositionScreenPoint.y = Screen.height - targetPositionScreenPoint.y;
                // X 좌표도 반전시킬 필요
                targetPositionScreenPoint.x = Screen.width - targetPositionScreenPoint.x;
            }
            // 화면 밖 타겟의 화면 좌표를 제한
            targetPositionScreenPoint.x = Mathf.Clamp(targetPositionScreenPoint.x, _borderSize, Screen.width - _borderSize);
            targetPositionScreenPoint.y = Mathf.Clamp(targetPositionScreenPoint.y, _borderSize, Screen.height - _borderSize);

            bool isOffScreen = targetPositionScreenPoint.x <= _borderSize || targetPositionScreenPoint.x >= Screen.width - _borderSize || targetPositionScreenPoint.y <= _borderSize || targetPositionScreenPoint.y >= Screen.height - _borderSize;

            if (isOffScreen)
            {
                _targets[i]._pointerRectTransform.gameObject.SetActive(true);
                RotatePointerTowardsTargetPosition(_targets[i]._target, _targets[i]._pointerRectTransform);

                Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
                if (cappedTargetScreenPosition.x <= _borderSize) cappedTargetScreenPosition.x = _borderSize;
                if (cappedTargetScreenPosition.x >= Screen.width - _borderSize) cappedTargetScreenPosition.x = Screen.width - _borderSize;
                if (cappedTargetScreenPosition.y <= _borderSize) cappedTargetScreenPosition.y = _borderSize;
                if (cappedTargetScreenPosition.y >= Screen.height - _borderSize) cappedTargetScreenPosition.y = Screen.height - _borderSize;
                cappedTargetScreenPosition = WordToScenePointAll(cappedTargetScreenPosition);
                _targets[i]._pointerRectTransform.localPosition = new Vector3(cappedTargetScreenPosition.x, cappedTargetScreenPosition.y, 0f);
            }
            else
            {
                _targets[i]._pointerRectTransform.gameObject.SetActive(false);
            }
            if (!_targets[i]._target.gameObject.activeSelf)
            {
                Destroy(_targets[i]._pointerRectTransform.gameObject);
                _targets.RemoveAt(i);
            }
        }
    }

    public Vector2 WordToScenePoint()
    {
        CanvasScaler canvasScaler = _canvasScaler;
        float referenceWidth = canvasScaler.referenceResolution.x;
        float referenceHeight = canvasScaler.referenceResolution.y;
        var offset = referenceWidth / (float)Screen.width;
        return new Vector2(referenceWidth, (float)Screen.height * offset);
    }

    public Vector2 WordToScenePoint(Vector2 v2)
    {
        CanvasScaler canvasScaler = _canvasScaler;
        float referenceWidth = canvasScaler.referenceResolution.x;
        float referenceHeight = canvasScaler.referenceResolution.y;
        var offset = referenceWidth / (float)Screen.width;
        return new Vector2(referenceWidth, (float)Screen.height * offset);
    }

    public Vector2 WordToScenePointAll(Vector2 v2)
    {
        CanvasScaler canvasScaler = _canvasScaler;
        float referenceWidth = canvasScaler.referenceResolution.x;
        float referenceHeight = canvasScaler.referenceResolution.y;
        var offset = referenceWidth / (float)Screen.width;
        return v2 * offset;
    }

    private void RotatePointerTowardsTargetPosition(Transform target, Transform pointer)
    {
        Vector3 toPosition = target.position;
        toPosition.y = toPosition.z;
        toPosition.z = 0f;
        Vector3 fromPosition = _playerTrans.position;
        fromPosition.y = fromPosition.z;
        fromPosition.z = 0f;
        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = GetAngleFromVectorFloat(dir);
        pointer.localEulerAngles = new Vector3(0, 0, angle);
    }
    public float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
    public void Hide()
    {
        for (int i = 0; i < _targets.Count; i++)
            _targets[i]._pointerRectTransform.gameObject.SetActive(false);
        _targets.Clear();
        gameObject.SetActive(false);
    }

    public void ClearObj()
    {
        for (int i = 0; i < _targets.Count; ++i)
        {
            Destroy(_targets[i]._pointerRectTransform.gameObject);
        }
        _targets.Clear();
    }

    internal void Show(Transform target)
    {
        gameObject.SetActive(true);
        var tp = new TargetPointUI();
        tp._target = target;
        tp._pointerRectTransform = Instantiate(pointerObj, Vector3.zero, Quaternion.identity).transform as RectTransform;
        tp._pointerRectTransform.SetParent(transform);
        tp._pointerRectTransform.localPosition = Vector3.zero;
        tp._pointerRectTransform.localScale = new Vector3(1f, 1f, 1f);
        tp._pointerRectTransform.gameObject.SetActive(false);
        _targets.Add(tp);
    }

    public struct TargetPointUI
    {
        public Transform _target;
        public RectTransform _pointerRectTransform;
    }

    public int GetTargetCount { get { return _targets.Count; } }
}
