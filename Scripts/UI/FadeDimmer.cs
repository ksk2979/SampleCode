using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeDimmer : MonoBehaviour
{
    public GameObject _objFade = null;
    public Animator _fadeAnim = null;
    public TextMeshProUGUI _text = null;

    /// <summary>
    /// 1회씩 이벤트를 받아 페이드가 끝날때 호출
    /// </summary>
    private Action _compliteAction = null;

    private void Start()
    {
        var sender = _fadeAnim.GetComponent<AnimEnvetSender>();
        sender.AddEvent("OnCompliteFadeIn", OnCompliteFadeIn);
        sender.AddEvent("OnCompliteFadeOut", OnCompliteFadeOut);
    }

    [ContextMenu("fade in")]
    public void FadeIn()
    {
        //_objFade.SetActive(true);
        _fadeAnim.SetTrigger("fadein");
    }

    [ContextMenu("fade out")]
    public void FadeOut()
    {
        //_objFade.SetActive(true);
        _fadeAnim.SetTrigger("fadeout");
    }

    /// <summary>
    /// 페이드 인 아웃이 애님으로 제어되고 끝날때 호출 된다
    /// </summary>
    public void OnCompliteFadeIn()
    {
        if (_compliteAction != null)
        {
            _compliteAction();
            _compliteAction = null;
        }
        //_objFade.SetActive(false);
        //_text.text = string.Empty;
    }

    /// <summary>
    /// 페이드 인 아웃이 애님으로 제어되고 끝날때 호출 된다
    /// </summary>
    public void OnCompliteFadeOut()
    {
        if (_compliteAction != null)
        {
            _compliteAction();
            _compliteAction = null;
        }
    }

    public void FadeOut(Action compliteAction)
    {
        _compliteAction = compliteAction;
        FadeOut();
    }

    public void FadeOut(Action compliteAction, string text)
    {
        _compliteAction = compliteAction;
        _text.text = text;
        FadeOut();
    }
}
