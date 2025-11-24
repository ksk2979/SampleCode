using UnityEngine;
using System;
using System.Collections;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("애니메이션 넣으면 애니메이션으로 됨")]
    [SerializeField] Animator _anim;

    [Header("애니 없으면 값을 통해서 됨")]
    [SerializeField] Vector3 _closedLocalPos;
    [SerializeField] Vector3 _openedLocalPos;
    [SerializeField] Vector3 _closedLocalRot;
    [SerializeField] Vector3 _openedLocalRot;
    [SerializeField] float _moveSpeed = 5f;

    [SerializeField] bool _isOpen = false;

    Vector3 _targetPos;
    Quaternion _targetRot;

    Coroutine _moveCoroutine;

    public string GetInteractPrompt()
    {
        return _isOpen ? "문을 닫는다" : "문을 연다";
    }

    public void Interact(PlayerController player)
    {
        _isOpen = !_isOpen;

        if (_anim != null)
        {
            _anim.SetBool("IsOpen", _isOpen);
        }
        else
        {
            //if (_isOpen)
            //{
            //    transform.localRotation = Quaternion.Euler(0, 160, 0);
            //}
            //else
            //{
            //    transform.localRotation = Quaternion.identity;
            //}

            _targetPos = _isOpen ? _openedLocalPos : _closedLocalPos;
            _targetRot = Quaternion.Euler(_isOpen ? _openedLocalRot : _closedLocalRot);

            if (_moveCoroutine != null) { StopCoroutine(_moveCoroutine); }
            _moveCoroutine = StartCoroutine(MoveRoutine());
        }
    }

    IEnumerator MoveRoutine()
    {
        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * _moveSpeed;

            transform.localPosition = Vector3.Lerp(startPos, _targetPos, t);
            transform.localRotation = Quaternion.Lerp(startRot, _targetRot, t);

            yield return null;
        }

        transform.localPosition = _targetPos;
        transform.localRotation = _targetRot;
        _moveCoroutine = null;
    }
}
