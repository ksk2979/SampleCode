using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeAction : RootAction
{
    [SerializeField] RectTransform _rtCP;
    bool _runningAction = false;

    private void Start()
    {
        if (this.GetComponent<RectTransform>() != null)
        {
            _rtCP = this.GetComponent<RectTransform>();
        }
    }

    public IEnumerator RunAction(float shakeAmount, float shakeTime)
    {
        if (_runningAction) { yield break; } // 그냥.. 한번만 들어오게 하고 싶어서

        _runningAction = true;

        float delayTime = _frontDelayTime;
        if (delayTime != 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(delayTime);
        }

        Vector3 initialPosition;

        if (_rtCP != null)
        {
            initialPosition = _rtCP.anchoredPosition;
        }
        else
        {
            initialPosition = this.transform.position;
        }

        for (float t = 0f; t < shakeTime; t += Time.deltaTime / _actionTime)
        {
            if (_rtCP != null)
            {
                _rtCP.anchoredPosition = Random.insideUnitSphere * shakeAmount + initialPosition;
                _rtCP.anchoredPosition = new Vector3(_rtCP.anchoredPosition.x, _rtCP.anchoredPosition.y, initialPosition.z); // z값은 안움직이게
            }
            else
            {
                this.transform.position = Random.insideUnitSphere * shakeAmount + initialPosition;
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, initialPosition.z); // z값은 안움직이게
            }
            
            yield return null;
        }

        if (_rtCP != null)
        {
            _rtCP.anchoredPosition = initialPosition;
        }
        else
        {
            this.transform.position = initialPosition;
        }

        _runningAction = false;
    }
}
