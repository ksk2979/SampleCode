using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryPopup : PopupBase
{
    public event PopupEventDelegate OnRetryEventListener;
    public event PopupEventDelegate OnResetEventListener;

    public void OnTouchRetryButton()
    {
        OnRetryEventListener?.Invoke();
        OnRetryEventListener = null;
        ClosePopup();
    }

    public void OnTouchCancelButton()
    {
        OnResetEventListener?.Invoke();
        OnResetEventListener = null;
        ClosePopup();
    }
}
