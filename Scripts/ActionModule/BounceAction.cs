using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAction : RootAction
{
    [SerializeField] RectTransform _rt;

    float[] _timeSequence = new float[3];
    private void Start()
    {
        if (this.GetComponent<RectTransform>() != null)
        {
            _rt = this.GetComponent<RectTransform>();
        }

        _timeSequence[0] = _actionTime * 0.3f; // timeUpScale
        _timeSequence[1] = _actionTime * 0.3f; // timeDownScale
        _timeSequence[2] = _actionTime * 0.4f; // timeRealScale
    }

    public IEnumerator RunAction(float originScale)
    {
        float delayTime = _frontDelayTime;
        if (delayTime != 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(delayTime);
        }

        float startScale = 0f;
        float endScale = 0f;
        float nowScale = originScale;

        Vector3 s = new Vector3(nowScale, nowScale, nowScale);

        for (int i = 0; i < _timeSequence.Length; ++i)
        {
            switch (i)
            {
                case 0:
                    startScale = originScale * 1.0f;
                    endScale = originScale * 1.1f;
                    break;
                case 1:
                    startScale = originScale * 1.1f;
                    endScale = originScale * 0.9f;
                    break;
                case 2:
                    startScale = originScale * 0.9f;
                    endScale = originScale * 1.0f;
                    break;
                default:
                    startScale = originScale;
                    endScale = originScale;
                    break;
            }

            for (float t = 0f; t < 1f; t += Time.deltaTime / _timeSequence[i])
            {
                nowScale = Mathf.Lerp(startScale, endScale, ChangeSmoothTime(t));
                s.x = s.y = s.z = nowScale;
                if (_rt != null)
                {
                    _rt.localScale = s;
                }
                else
                {
                    this.transform.localScale = s;
                }
                yield return null;
            }
        }

        s.x = s.y = s.z = originScale;
        if (_rt != null)
        {
            _rt.localScale = s;
        }
        else
        {
            this.transform.localScale = s;
        }
    }
}