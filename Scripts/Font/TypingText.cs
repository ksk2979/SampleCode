using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypingText : MonoBehaviour
{
    public Text _typingText;
    public TextMeshProUGUI _typingTextUGUI;
    string _message;
    float _typingSpeed;
    bool _typingSkip = false;
    bool _nowTyping = false;

    public IEnumerator Typing(Text typingText, string message, float speed)
    {
        _nowTyping = true;
        _typingText = typingText;
        _message = message;
        _typingSpeed = speed;

        for (int i = 0; i < _message.Length; ++i)
        {
            _typingText.text = _message.Substring(0, i + 1);

            if (_typingSkip) { break; }

            yield return YieldInstructionCache.WaitForSeconds(_typingSpeed);
        }
        _typingText.text = _message;

        _typingSkip = false;
        _nowTyping = false;
    }

    // 리얼타임으로 타이핑 되어야한다
    public IEnumerator Typing(string message, float speed)
    {
        if (_typingTextUGUI == null) { yield break; }
        _nowTyping = true;
        _message = message;
        _typingSpeed = speed;

        for (int i = 0; i < _message.Length; ++i)
        {
            _typingTextUGUI.text = _message.Substring(0, i + 1);

            if (_typingSkip) { break; }

            yield return YieldInstructionCache.RealTimeWaitForSeconds(_typingSpeed);
        }
        _typingTextUGUI.text = _message;

        _typingSkip = false;
        _nowTyping = false;
    }

    public bool TypingSkip {
        get { return _typingSkip; }
        set { _typingSkip = value; }
    }
    public bool NowTyping { get { return _nowTyping; } }
}
