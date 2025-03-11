using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOutAction : RootAction
{
    [SerializeField] private CanvasGroup _cgCP;
    [SerializeField] private SpriteRenderer _srCP;
    [SerializeField] private Image _imCP;
    [SerializeField] private Text _ttCP;
    [SerializeField] private Renderer _rrCP;
    [SerializeField] private float _backActionTime;
    [SerializeField] private RectTransform _rtCP;

    private void Start()
    {
        if (this.GetComponent<CanvasGroup>() != null)
        {
            _cgCP = this.GetComponent<CanvasGroup>();
        }
        if (this.GetComponent<SpriteRenderer>() != null)
        {
            _srCP = this.GetComponent<SpriteRenderer>();
        }
        if (this.GetComponent<Image>() != null)
        {
            _imCP = this.GetComponent<Image>();
        }
        if (this.GetComponent<Text>() != null)
        {
            _ttCP = this.GetComponent<Text>();
        }
        if (this.GetComponent<Renderer>() != null)
        {
            _rrCP = this.GetComponent<Renderer>();
        }
        if (this.GetComponent<RectTransform>() != null)
        {
            _rtCP = this.GetComponent<RectTransform>();
        }

        _timeSequence[0] = _actionTime * 0.3f; // timeUpScale
        _timeSequence[1] = _actionTime * 0.3f; // timeDownScale
        _timeSequence[2] = _actionTime * 0.4f; // timeRealScale
    }

    public IEnumerator AlphaFadeAction(bool fade)
    {
        float delayTime = _frontDelayTime;
        if (fade == false)
        {
            delayTime = _backDelayTime;
        }

        if (delayTime != 0f)
        {
            yield return YieldInstructionCache.WaitForSeconds(delayTime);
        }

        float startAlpha = 0f;
        float endAlpha = 0f;

        if (fade)
        {
            startAlpha = 0f;
            endAlpha = 1f;
            this.gameObject.SetActive(true);
        }
        else
        {
            startAlpha = 1f;
            endAlpha = 0f;
        }

        for (float t = 0f; t < 1f; t += Time.deltaTime / _actionTime)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, ChangeSmoothTime(t));
            if (_cgCP != null)
            {
                _cgCP.alpha = alpha;
            }
            else if (_srCP != null)
            {
                _srCP.color = new Color(_srCP.color.r, _srCP.color.g, _srCP.color.b, alpha);
            }
            else if (_imCP != null)
            {
                _imCP.color = new Color(_imCP.color.r, _imCP.color.g, _imCP.color.b, alpha);
            }
            else if (_ttCP != null)
            {
                _ttCP.color = new Color(_ttCP.color.r, _ttCP.color.g, _ttCP.color.b, alpha);
            }
            else if (_rrCP != null)
            {
                Color c = _rrCP.material.color;
                c.a = alpha;
                _rrCP.material.color = c;
            }

            yield return null;
        }

        if (_cgCP != null)
        {
            _cgCP.alpha = endAlpha;
        }
        else if (_srCP != null)
        {
            _srCP.color = new Color(_srCP.color.r, _srCP.color.g, _srCP.color.b, endAlpha);
        }
        else if (_imCP != null)
        {
            _imCP.color = new Color(_imCP.color.r, _imCP.color.g, _imCP.color.b, endAlpha);
        }
        else if (_ttCP != null)
        {
            _ttCP.color = new Color(_ttCP.color.r, _ttCP.color.g, _ttCP.color.b, endAlpha);
        }
        else if (_rrCP != null)
        {
            _rrCP.material.color = new Color(_rrCP.material.color.r, _rrCP.material.color.g, _rrCP.material.color.b, endAlpha);
        }

        if (endAlpha <= 0f)
        {
            if (_cgCP != null)
            {
                _cgCP.interactable = false;
            }

            this.gameObject.SetActive(false);
        }
        else
        {
            if (_cgCP != null)
            {
                _cgCP.interactable = true;
            }
        }
    }

    // 스케일 1고정 함수
    public IEnumerator ScaleFadeAction(bool fade)
    {
        //gameObject.transform.localScale
        float delayTime = _frontDelayTime;
        if (fade == false)
        {
            delayTime = _backDelayTime;
        }

        if (delayTime != 0f)
        {
            yield return YieldInstructionCache.WaitForSeconds(delayTime);
        }

        float startScale = 0f;
        float endScale = 0f;

        if (fade)
        {
            startScale = 0f;
            endScale = 1f;
            this.gameObject.SetActive(true);
        }
        else
        {
            startScale = 1f;
            endScale = 0f;
        }

        for (float t = 0f; t < 1f; t += Time.deltaTime / _actionTime)
        {
            float scale = Mathf.Lerp(startScale, endScale, ChangeSmoothTime(t));

            if (_rtCP != null)
            {
                _rtCP.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                this.gameObject.transform.localScale = new Vector3(scale, scale, scale);
            }
            
            yield return null;
        }

        if (_rtCP != null)
        {
            _rtCP.localScale = new Vector3(endScale, endScale, endScale);
        }
        else
        {
            this.gameObject.transform.localScale = new Vector3(endScale, endScale, endScale);
        }
        
        if (endScale <= 0f)
        {
            this.gameObject.SetActive(false);
        }
    }

    // 스케일 프리 함수, 지정된 수 만큼 커지고 작아지기
    public IEnumerator ScaleFadeAction(bool fade, float scale)
    {
        //gameObject.transform.localScale
        float delayTime = _frontDelayTime;
        if (fade == false)
        {
            delayTime = _backDelayTime;
        }

        if (delayTime != 0f)
        {
            yield return YieldInstructionCache.WaitForSeconds(delayTime);
        }

        float startScale = 0f;
        float endScale = 0f;

        if (fade)
        {
            startScale = 0f;
            endScale = scale;
            this.gameObject.SetActive(true);
        }
        else
        {
            startScale = scale;
            endScale = 0f;
        }

        for (float t = 0f; t < 1f; t += Time.deltaTime / _actionTime)
        {
            float s = Mathf.Lerp(startScale, endScale, ChangeSmoothTime(t));

            if (_rtCP != null)
            {
                _rtCP.localScale = new Vector3(s, s, s);
            }
            else
            {
                this.gameObject.transform.localScale = new Vector3(s, s, s);
            }
            
            yield return null;
        }

        if (_rtCP != null)
        {
            _rtCP.localScale = new Vector3(endScale, endScale, endScale);
        }
        else
        {
            this.gameObject.transform.localScale = new Vector3(endScale, endScale, endScale);
        }
        
        if (endScale <= 0f)
        {
            this.gameObject.SetActive(false);
        }
    }

    // 원하는 만큼 커졌다가 사라지는 작업
    public IEnumerator ScaleDownAndOutActoin(float addScale)
    {
        float delayTime = _frontDelayTime;

        if (delayTime != 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(delayTime);
        }

        float original = this.gameObject.transform.localScale.x;
        float upEnd = original + addScale;
        float downEnd = 0;

        for (float t = 0f; t < 1f; t += Time.deltaTime / _actionTime)
        {
            float s = Mathf.Lerp(original, upEnd, ChangeSmoothTime(t));

            if (_rtCP != null)
            {
                _rtCP.localScale = new Vector3(s, s, s);
            }
            else
            {
                this.gameObject.transform.localScale = new Vector3(s, s, s);
            }
            
            yield return null;
        }

        for (float t = 0f; t < 1f; t += Time.deltaTime / _backActionTime)
        {
            float s = Mathf.Lerp(upEnd, downEnd, ChangeSmoothTime(t));

            if (_rtCP != null)
            {
                _rtCP.localScale = new Vector3(s, s, s);
            }
            else
            {
                this.gameObject.transform.localScale = new Vector3(s, s, s);
            }
            
            yield return null;
        }

        if (_rtCP != null)
        {
            _rtCP.localScale = new Vector3(downEnd, downEnd, downEnd);
        }
        else
        {
            this.gameObject.transform.localScale = new Vector3(downEnd, downEnd, downEnd);
        }
        this.gameObject.SetActive(false);
    }

    float[] _timeSequence = new float[3];
    // 작아진 오브젝트 크게 하면서 바운스
    public IEnumerator ScaleUpAndBounceAction(float originScale)
    {
        float delayTime = _frontDelayTime;

        if (delayTime != 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(delayTime);
        }

        float startScale;
        float endScale;
        float nowScale = originScale;
        Vector3 s = new Vector3(nowScale, nowScale, nowScale);

        // 먼저 커진다
        this.gameObject.SetActive(true);
        startScale = 0f;
        endScale = originScale * 1.1f;

        for (float t = 0f; t < 1f; t += Time.deltaTime / _actionTime)
        {
            float scale = Mathf.Lerp(startScale, endScale, ChangeSmoothTime(t));

            this.gameObject.transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        // 다 커지면 바운스
        for (int i = 0; i < _timeSequence.Length; i++)
        {
            switch(i)
            {
                case 0:
                    startScale = originScale * 1.1f;
                    endScale = originScale * 0.9f;
                    break;
                case 1:
                    startScale = originScale * 0.9f;
                    endScale = originScale * 1.1f;
                    break;
                case 2:
                    startScale = originScale * 1.1f;
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
                if (_rtCP != null)
                {
                    _rtCP.localScale = s;
                }
                else
                {
                    this.transform.localScale = s;
                }
                yield return null;
            }
        }
        s.x = s.y = s.z = originScale;
        if (_rtCP != null)
        {
            _rtCP.localScale = s;
        }
        else
        {
            this.transform.localScale = s;
        }
    }
}
