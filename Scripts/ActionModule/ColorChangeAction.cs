using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColorChangeAction : RootAction
{
    [SerializeField] Text _ttCP;
    [SerializeField] Image _imCP;
    [SerializeField] SpriteRenderer _spCP;

    void Start()
    {
        if (this.GetComponent<Text>() != null)
        {
            _ttCP = this.GetComponent<Text>();
        }
        if (this.GetComponent<SpriteRenderer>() != null)
        {
            _spCP = this.GetComponent<SpriteRenderer>();
        }
        if (this.GetComponent<Image>() != null)
        {
            _imCP = this.GetComponent<Image>();
        }
    }

    public IEnumerator RunAction(Color color)
    {
        float delayTime = _frontDelayTime;
        if (delayTime != 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(delayTime);
        }

        Color startColor;
        if (_ttCP != null)
        {
            startColor = _ttCP.color; // 원본 컬러 넣어준다
        }
        else if (_spCP != null)
        {
            startColor = _spCP.color;
        }
        else
        {
            startColor = _imCP.color;
        }

        Color endColor = color;

        for (float t = 0f; t < 1f; t += Time.deltaTime / _actionTime)
        {
            float r = Mathf.Lerp(startColor.r, endColor.r, ChangeSmoothTime(t));
            float g = Mathf.Lerp(startColor.g, endColor.g, ChangeSmoothTime(t));
            float b = Mathf.Lerp(startColor.b, endColor.b, ChangeSmoothTime(t));

            if (_ttCP != null)
            {
                _ttCP.color = new Color(r, g, b, _ttCP.color.a);
            }
            else if (_spCP != null)
            {
                _spCP.color = new Color(r, g, b, _spCP.color.a);
            }
            else
            {
                _imCP.color = new Color(r, g, b, _imCP.color.a);
            }

            yield return null;
        }

        if (_ttCP != null)
        {
            _ttCP.color = endColor;
        }
        else if (_spCP != null)
        {
            _spCP.color = endColor;
        }
        else
        {
            _imCP.color = endColor;
        }
    }
}
