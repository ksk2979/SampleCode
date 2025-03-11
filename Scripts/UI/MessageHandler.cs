using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageHandler : Singleton<MessageHandler>
{
    [SerializeField] Canvas _canvas;
    public TextLocalizeSetter messageText;
    public Image backgroundImage;
    public Color[] _colorArr; // 0 �޼��� �ؽ�Ʈ �÷� 1 ��� �̹��� �÷�

    string prevText = "";

    // �κ񿡼� ��� �Ǵ� ������ �޼���
    public TextMeshProUGUI _exitMessageText;
    public Image _exitBackgroundImage;
    string exitPrevText = "";

    float _startRealTime = 0f;
    float _fadeInDuration = 0.5f;
    float _fadeOutDuration = 0.5f;
    float _totalDuration = 0f;
    float _elapsedTime = 0f;
    float _alphaValue = 0f;
    bool _startText;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(_canvas);
    }

    private void Start()
    {
/*        if (_canvas.renderMode != RenderMode.ScreenSpaceCamera)
        {
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        }
        if (_canvas.worldCamera == null)
        {
            Debug.Log("_canvas.worldCamera");
            _canvas.worldCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
        }*/
    }

    // �ܺ� ȣ��� �Լ� 
    // text : ������ �ؽ�Ʈ
    // duration : �޽����� �ӹ��� �ð�
    // c : �޽��� �÷�
    public void ShowMessage(string text, float duration, Color c)
    {
        // �̹� ���� ������ �޽����� ���ִ� ��� ����
        if (prevText == text) return;

        prevText = text;
        messageText.key = text;
        _elapsedTime = 0f;
        _alphaValue = 0f;
        _startRealTime = Time.realtimeSinceStartup;
        if (!_startText) { StartCoroutine(MessageRealCoroutine(duration)); }
    }

    public void ShowMessageNotRealTime(string text, float duration)
    {
        if (prevText == text) return;

        prevText = text;
        messageText.key = text;
        StartCoroutine(MessageCoroutine(text, duration, Color.white));
    }

    public void ShowMessage(string text, float duration)
    {
        if (prevText == text) return;

        prevText = text;
        messageText.key = text;
        _totalDuration = _fadeInDuration + duration + _fadeOutDuration;
        _elapsedTime = 0f;
        _alphaValue = 0f;
        _startRealTime = Time.realtimeSinceStartup;
        if (!_startText) { StartCoroutine(MessageRealCoroutine(duration)); }
    }

    IEnumerator MessageRealCoroutine(float duration)
    {
        _startText = true;
        TextMeshProUGUI colorT = messageText.GetComponent<TextMeshProUGUI>();

        while (_elapsedTime < _totalDuration)
        {
            _elapsedTime = Time.realtimeSinceStartup - _startRealTime;

            // Fade In
            if (_elapsedTime <= _fadeInDuration)
            {
                _alphaValue = _elapsedTime / _fadeInDuration;
            }
            // Visible
            else if (_elapsedTime <= _fadeInDuration + duration)
            {
                _alphaValue = 1f;
            }
            // Fade Out
            else if (_elapsedTime <= _totalDuration)
            {
                _alphaValue = 1f - (_elapsedTime - _fadeInDuration - duration) / _fadeOutDuration;
            }

            colorT.color = new Color(_colorArr[0].r, _colorArr[0].g, _colorArr[0].b, _alphaValue);
            backgroundImage.color = new Color(_colorArr[1].r, _colorArr[1].g, _colorArr[1].b, _alphaValue);

            yield return null;
        }

        //float t = 0;
        //TextMeshProUGUI colorT = messageText.GetComponent<TextMeshProUGUI>();
        //while (t < 1)
        //{
        //    t += Time.unscaledDeltaTime * 3f;
        //    colorT.color = new Color(_colorArr[0].r, _colorArr[0].g, _colorArr[0].b, t);
        //    backgroundImage.color = new Color(_colorArr[1].r, _colorArr[1].g, _colorArr[1].b, t); // �ణ ������ : t / 1.20f
        //
        //    yield return null;
        //}
        //
        //yield return YieldInstructionCache.RealTimeWaitForSeconds(duration); // duration���� �޽��� Ȧ��
        //
        //while (t > 0)
        //{
        //    t -= Time.unscaledDeltaTime * 3f;
        //    colorT.color = new Color(_colorArr[0].r, _colorArr[0].g, _colorArr[0].b, t);
        //    backgroundImage.color = new Color(_colorArr[1].r, _colorArr[1].g, _colorArr[1].b, t); // 0(����)�� ���ŵǰ�
        //
        //    yield return null;
        //}

        colorT.color = new Color(_colorArr[0].r, _colorArr[0].g, _colorArr[0].b, 0);
        backgroundImage.color = new Color(_colorArr[1].r, _colorArr[1].g, _colorArr[1].b, 0);

        prevText = ""; // �޽��� �ʱ�ȭ
        _startText = false;
    }
    IEnumerator MessageCoroutine(string text, float duration, Color c)
    {
        float t = 0;
        TextMeshProUGUI colorT = messageText.GetComponent<TextMeshProUGUI>();
        while (t < 1)
        {
            t += Time.deltaTime * 3f;
            colorT.color = new Color(_colorArr[0].r, _colorArr[0].g, _colorArr[0].b, t);
            backgroundImage.color = new Color(_colorArr[1].r, _colorArr[1].g, _colorArr[1].b, t); // �ణ ������ : t / 1.20f

            yield return null;
        }

        yield return YieldInstructionCache.WaitForSeconds(duration); // duration���� �޽��� Ȧ��

        while (t > 0)
        {
            t -= Time.deltaTime * 3f;
            colorT.color = new Color(_colorArr[0].r, _colorArr[0].g, _colorArr[0].b, t);
            backgroundImage.color = new Color(_colorArr[1].r, _colorArr[1].g, _colorArr[1].b, t); // 0(����)�� ���ŵǰ�

            yield return null;
        }

        colorT.color = new Color(_colorArr[0].r, _colorArr[0].g, _colorArr[0].b, 0);
        backgroundImage.color = new Color(_colorArr[1].r, _colorArr[1].g, _colorArr[1].b, 0);

        prevText = ""; // �޽��� �ʱ�ȭ
    }

    public void ShowExitMessage(float duration)
    {
        StartCoroutine(ExitMessageCoroutine(duration));
    }

    IEnumerator ExitMessageCoroutine(float duration)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime * 3; // �� ���� �߰��Ϸ���...
            _exitMessageText.color = new Color(_colorArr[0].r, _colorArr[0].g, _colorArr[0].b, t);
            _exitBackgroundImage.color = new Color(_colorArr[1].r, _colorArr[1].g, _colorArr[1].b, t); // �ణ ������ : t / 1.20f

            yield return null;
        }

        yield return YieldInstructionCache.RealTimeWaitForSeconds(duration); // duration���� �޽��� Ȧ��

        while (t > 0)
        {
            t -= Time.unscaledDeltaTime * 3; // ���� �������...
            _exitMessageText.color = new Color(_colorArr[0].r, _colorArr[0].g, _colorArr[0].b, t);
            _exitBackgroundImage.color = new Color(_colorArr[1].r, _colorArr[1].g, _colorArr[1].b, t); // 0(����)�� ���ŵǰ�

            yield return null;
        }

        _exitMessageText.color = new Color(_colorArr[0].r, _colorArr[0].g, _colorArr[0].b, 0);
        _exitBackgroundImage.color = new Color(_colorArr[1].r, _colorArr[1].g, _colorArr[1].b, 0);

        GameManager.GetInstance.GetExitCheck = false;
    }
}
