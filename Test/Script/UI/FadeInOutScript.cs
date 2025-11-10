using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class FadeInOutScript : MonoBehaviour
{
    [Header("Fade Elements")]
    [SerializeField] Image _fadeImage;
    [SerializeField] TextMeshProUGUI _fadeText;
    [SerializeField] float _fadeDuration = 1f;

    Coroutine _fadeRoutine;

    // 오브젝트 기준 없어기는걸로 가정
    public void FadeOut(float startDelayTime = 0f, float duration = -1f)
    {
        if (!gameObject.activeSelf) 
        { 
            gameObject.SetActive(true);
            FadeReset(1f);
        }
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        float time = (duration > 0f) ? duration : _fadeDuration;
        _fadeRoutine = StartCoroutine(FadeRoutine(1f, 0f, time, startDelayTime));
    }
    // 오브젝트 기준 생기는걸로 가정
    public void FadeIn(float startDelayTime = 0f, float duration = -1f)
    {
        if (!gameObject.activeSelf) 
        { 
            gameObject.SetActive(true);
            FadeReset(0f);
        }
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        float time = (duration > 0f) ? duration : _fadeDuration;
        _fadeRoutine = StartCoroutine(FadeRoutine(0f, 1f, time, startDelayTime));
    }
    void FadeReset(float alpha)
    {
        Color imgColor = _fadeImage != null ? _fadeImage.color : Color.white;
        Color txtColor = _fadeText != null ? _fadeText.color : Color.white;
        if (_fadeImage != null)
        {
            imgColor.a = alpha;
            _fadeImage.color = imgColor;
        }
        if (_fadeText != null)
        {
            txtColor.a = alpha;
            _fadeText.color = txtColor;
        }
    }

    IEnumerator FadeRoutine(float startAlpha, float endAlpha, float duration, float startDelayTime)
    {
        yield return YieldInstructionCache.WaitForSeconds(startDelayTime);

        if (_fadeImage == null && _fadeText == null) { gameObject.SetActive(false); _fadeRoutine = null; yield break; }

        Color imgColor = _fadeImage != null ? _fadeImage.color : Color.white;
        Color txtColor = _fadeText != null ? _fadeText.color : Color.white;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = Mathf.Clamp01(t / duration);

            float alpha = Mathf.Lerp(startAlpha, endAlpha, normalized);

            if (_fadeImage != null)
            {
                imgColor.a = alpha;
                _fadeImage.color = imgColor;
            }
            if (_fadeText != null)
            {
                txtColor.a = alpha;
                _fadeText.color = txtColor;
            }

            yield return null;
        }

        if (_fadeImage != null)
        {
            imgColor.a = endAlpha;
            _fadeImage.color = imgColor;
        }
        if (_fadeText != null)
        {
            txtColor.a = endAlpha;
            _fadeText.color = txtColor;
        }

        if (endAlpha == 0f) { gameObject.SetActive(false); }
        _fadeRoutine = null;
    }
}
