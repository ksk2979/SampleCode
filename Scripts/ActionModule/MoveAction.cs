using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : RootAction
{
    // 초기에 인스펙터에 저장시킨 좌표로 이동
    [SerializeField] Vector3 _posUp;
    [SerializeField] Vector3 _posDown;
    [SerializeField] RectTransform _rt;

    private void Start()
    {
        _rt = this.gameObject.GetComponent<RectTransform>();
    }

    public IEnumerator RunAction(bool up)
    {
        float delayTime = _frontDelayTime;
        if (!up)
        {
            delayTime = _backDelayTime;
        }

        if (delayTime != 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(delayTime);
        }

        Vector3 posStart;
        Vector3 posEnd;
        if (up)
        {
            posStart = _posDown;
            posEnd = _posUp;
        }
        else
        {
            posStart = _posUp;
            posEnd = _posDown;
        }

        for (float t = 0f; t < 1f; t += Time.deltaTime / _actionTime)
        {
            if (_rt != null)
            {
                _rt.anchoredPosition = Vector3.Lerp(posStart, posEnd, ChangeSmoothTime(t));
            }
            else
            {
                this.transform.position = Vector3.Lerp(posStart, posEnd, ChangeSmoothTime(t));
            }

            yield return null;
        }

        if (_rt != null)
        {
            _rt.anchoredPosition = posEnd;
        }
        else
        {
            this.transform.position = posEnd;
        }
    }

    public IEnumerator RunAction(Vector3 start, Vector3 end)
    {
        if (_frontDelayTime != 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(_frontDelayTime);
        }

        for (float t = 0f; t < 1f; t += Time.deltaTime / _actionTime)
        {
            if (_rt != null)
            {
                _rt.anchoredPosition = Vector3.Lerp(start, end, ChangeSmoothTime(t));
            }
            else
            {
                this.transform.position = Vector3.Lerp(start, end, ChangeSmoothTime(t));
            }

            yield return null;
        }
        
        if (_rt != null)
        {
            _rt.anchoredPosition = end;
        }
        else
        {
            this.transform.position = end;
        }
    }

    // 바로 변경
    public void Resetting(bool up)
    {
        Vector3 posEnd;
        if (up) { posEnd = _posUp; }
        else { posEnd = _posDown; }

        if (_rt != null) { _rt.anchoredPosition = posEnd; }
        else { this.transform.position = posEnd; }
    }

}
