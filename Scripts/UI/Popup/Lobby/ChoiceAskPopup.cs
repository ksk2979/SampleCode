using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceAskPopup : PopupBase
{
    public event PopupEventDelegate OnAgreeEventListener;
    [SerializeField] TextMeshProUGUI _contextTMP;

    public void SetContext(string text)
    {
        _contextTMP.text = text;
    }

    public void OnTouchConfirmButton()
    {
        OnAgreeEventListener?.Invoke();
        ClosePopup();
    }

    public void OnTouchCancelButton()
    {
        ClosePopup();
    }

    public override void ClosePopup()
    {
        OnAgreeEventListener = null;
        base.ClosePopup();
    }
}
