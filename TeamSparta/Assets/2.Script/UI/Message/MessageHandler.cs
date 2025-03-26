using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageHandler : MonoBehaviour
{
    public static MessageHandler Getinstance;
    [SerializeField] MessageScript _messageObj;
    
    [SerializeField] Color[] _colorArr;

    private void Awake()
    {
        Getinstance = this;
        _messageObj.Init(this);
    }

    public void ShowMessage(string text, float duration, Color c)
    {
        if (_messageObj.GetStart())
        {
            if (_messageObj.GetPrevText() == text) { _messageObj.TimeUpdate(duration); }
            else
            {
                _messageObj.ResetTime();
                _messageObj.MessageStart(text, duration, c);
            }
        }
        else
        {
            _messageObj.MessageStart(text, duration, c);
        }
    }

    public void ShowMessage(string text, float duration)
    {
        if (_messageObj.GetStart())
        {
            if (_messageObj.GetPrevText() == text) { _messageObj.TimeUpdate(duration); }
            else
            {
                _messageObj.ResetTime();
                _messageObj.MessageStart(text, duration);
            }
        }
        else
        {
            _messageObj.MessageStart(text, duration);
        }
    }
    public Color GetTextColor() { return _colorArr[0]; }
    public Color GetBackgroundColor() { return _colorArr[1]; }
}
