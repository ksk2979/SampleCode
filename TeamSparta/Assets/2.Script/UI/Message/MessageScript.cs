using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _messageText;
    [SerializeField] Image _backgroundImage;
    string _prevText = "";

    Color _colorC;
    float _timeF = 0f; // 총 시간
    float _openF = 0f; // 나오는 시간
    float _closeF = 0f; // 들어가는 시간
    bool _openB = true;

    MessageHandler _messageHandler;

    bool _start = true;

    public void Init(MessageHandler messageHandler)
    {
        _messageHandler = messageHandler;
    }

    public void MessageStart(string text, float duration, Color c)
    {
        _start = true;
        _prevText = text;
        _messageText.text = text;
        _openF = duration / 2;
        _closeF = duration / 2;
        _openB = true;
        _colorC = c;
        this.gameObject.SetActive(true);
    }
    public void MessageStart(string text, float duration)
    {
        _start = true;
        _prevText = text;
        _messageText.text = text;
        _openF = duration / 2;
        _closeF = duration / 2;
        _openB = true;
        _colorC = Color.white;
        this.gameObject.SetActive(true);
    }

    public void TimeUpdate(float duration)
    {
        _timeF = 0f;
        _openF = duration / 2;
        _closeF = duration / 2;
        _openB = true;
    }

    private void Update()
    {
        if (_openB)
        {
            _openF -= Time.deltaTime;
            _timeF += Time.unscaledDeltaTime * 3f;

            if (_timeF > 1f) { _timeF = 1f; }
            _messageText.color = new Color(_colorC.r, _colorC.g, _colorC.b, _timeF);
            _backgroundImage.color = new Color(_messageHandler.GetBackgroundColor().r, _messageHandler.GetBackgroundColor().g, _messageHandler.GetBackgroundColor().b, _timeF); // 약간 반투명 : t / 1.20f

            if (_openF <= 0f) { _openB = false; }
        }
        else
        {
            _closeF -= Time.deltaTime;
            _timeF -= Time.unscaledDeltaTime * 3f;

            if (_timeF < 0f) { _timeF = 0f; }
            _messageText.color = new Color(_colorC.r, _colorC.g, _colorC.b, _timeF);
            _backgroundImage.color = new Color(_messageHandler.GetBackgroundColor().r, _messageHandler.GetBackgroundColor().g, _messageHandler.GetBackgroundColor().b, _timeF); // 약간 반투명 : t / 1.20f

            if (_closeF <= 0f) { _prevText = ""; _start = false; this.gameObject.SetActive(false); }
        }
    }

    public void ResetTime()
    {
        _prevText = "";
        _timeF = 0f; // 총 시간
        _openF = 0f; // 나오는 시간
        _closeF = 0f; // 들어가는 시간
        _openB = true;
    }

    public bool GetStart() { return _start; }
    public string GetPrevText() { return _prevText; }
}
