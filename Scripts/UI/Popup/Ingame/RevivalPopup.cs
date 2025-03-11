using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivalPopup : PopupBase
{
    public event PopupEventDelegate OnADEventListener;
    public event PopupEventDelegate OnDiaEventListener;
    public event PopupEventDelegate OnCancelEventListener;

    bool _checkAllowButton = false;
    const float _allowTime = 0.5f;
    float _tmptime = 0;
    public void GetSuccessRevival()
    {
        OnADEventListener = null;
        OnDiaEventListener = null;
        ClosePopup();
    }

    bool CheckTutorial()
    {
        if (UserData.GetInstance.TutorialCheck() == 7)
        {
            OnDiaEventListener?.Invoke();
            OnDiaEventListener = null;
            ClosePopup();
            return false;
        }
        else
        {
            return true;
        }
    }

    public void OnTouchADButton()
    {
        if (_checkAllowButton)
        {
            return;
        }
        else
        {
            StartButtonCheck();
        }

        OnADEventListener?.Invoke();
    }

    public void OnTouchDiaButton()
    {
        if (_checkAllowButton)
        {
            return;
        }
        else
        {
            StartButtonCheck();
        }

        if (CheckTutorial())
        {
            OnDiaEventListener?.Invoke();
        }
    }

    public void OnTouchCancelButton()
    {
        if (_checkAllowButton)
        {
            return;
        }
        else
        {
            StartButtonCheck();
        }
        OnCancelEventListener?.Invoke();
        OnCancelEventListener = null;
        ClosePopup();
    }

    public override void ClosePopup()
    {
        StopAllCoroutines();
        base.ClosePopup();
    }
    public void StartButtonCheck()
    {
        _checkAllowButton = true;
        StartCoroutine(CheckAllowButton());
    }

    IEnumerator CheckAllowButton()
    {
        while (_checkAllowButton)
        {
            _tmptime += 0.02f;
            if (_tmptime >= _allowTime)
            {
                _checkAllowButton = false;
                _tmptime = 0;
            }
            yield return YieldInstructionCache.RealTimeWaitForSeconds(0.02f);
        }
    }
}
