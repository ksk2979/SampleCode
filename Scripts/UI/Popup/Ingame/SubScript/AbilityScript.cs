using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameTMP;
    [SerializeField] TextMeshProUGUI _contextTMP;
    [SerializeField] Image _iconImage;
    Action _buttonCallback;

    public void Init(string name, Sprite icon, string context, Action buttonCallback)
    {
        _nameTMP.text = name;
        _contextTMP.text = context;
        _iconImage.sprite = icon;
        _buttonCallback = buttonCallback;
        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnTouchAbilityButton);
        btn.interactable = true;
    }

    public void OnTouchAbilityButton()
    {
        GetComponent<Button>().interactable = false;
        _buttonCallback?.Invoke();
        _buttonCallback = null;
    }
}
