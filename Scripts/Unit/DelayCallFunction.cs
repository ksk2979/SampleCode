using UnityEngine;
using System.Collections;

public class DelayCallFunction : MonoBehaviour
{
    public myEvent _event;
    public float _delayTime = 0.0f;
    public float _firstDelayTime = 0.0f;
    public bool _isLoop = false;
    public bool _firstCall = false;
    private IEnumerator coroutine = null;
    public void OnEnable()
    {
        coroutine = DelaydCallFunction();
        StartCoroutine(coroutine);
    }
    public void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public IEnumerator DelaydCallFunction()
    {
        yield return YieldInstructionCache.WaitForSeconds(_firstDelayTime);
        if (_firstCall)
            if (_event != null)
                _event.Invoke();
        if (_isLoop)
        {
            while (gameObject.activeSelf)
            {
                yield return YieldInstructionCache.WaitForSeconds(_delayTime);
                if (_event != null)
                    _event.Invoke();
            }
        }
        else
        {
            yield return YieldInstructionCache.WaitForSeconds(_delayTime);
            if (_event != null)
                _event.Invoke();
        }
    }
}
