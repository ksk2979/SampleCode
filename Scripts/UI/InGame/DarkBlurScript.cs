using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBlurScript : MonoBehaviour
{
    [SerializeField] RectTransform _maskRect;
    float _out = 1500f;
    float _in = 250f;
    float _duration = 0.5f;
    float _time = 0f;
    bool _on = false;

    public void OnDarkUI()
    {
        _maskRect.gameObject.SetActive(true);
        StartCoroutine(ChangeSize(_in, _duration));
        _time = 0f;
        _on = true;
    }
    public void OutDarkUI()
    {
        StartCoroutine(ChangeSize(_out, _duration));
    }

    private void Update()
    {
        if (_on)
        {
            _time += Time.deltaTime;
            if (_time > 3f)
            {
                _on = false;
                OutDarkUI();
            }
        }
    }

    IEnumerator ChangeSize(float targetSize, float time)
    {
        float currentTime = 0f;
        float startWidth = _maskRect.sizeDelta.x;
        float startHeight = _maskRect.sizeDelta.y;
        while (currentTime < time)
        {
            currentTime += Time.deltaTime;
            float newWidth = Mathf.Lerp(startWidth, targetSize, currentTime / time);
            float newHeight = Mathf.Lerp(startHeight, targetSize, currentTime / time);
            _maskRect.sizeDelta = new Vector2(newWidth, newHeight);
            yield return null;
        }
        _maskRect.sizeDelta = new Vector2(targetSize, targetSize);
        if (targetSize == _out) { _maskRect.gameObject.SetActive(false); }
    }
}
