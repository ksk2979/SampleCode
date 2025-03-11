using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionText : MonoBehaviour
{
    [Header("�ؽ�Ʈ")]
    [SerializeField] TypingText _typingText; // ���� ����
    [SerializeField] GameObject _blurObj; // �ؽ�Ʈ ������Ʈ

    List<LocalizedMessage> _messageTexts;

    int _messageCheckCount = 0;

    private int _typingCount = 0;
    private int _currentMessageIndex = 0;

    ProductionManager _manager;

    int _textCount = 3;

    public void Init(ProductionManager manager)
    {
        _manager = manager;
        _messageTexts = new List<LocalizedMessage>();
        for (int i = 0; i < _textCount; ++i)
        {
            LocalizedMessage message = new LocalizedMessage();
            message.MessageCount(i);
            _messageTexts.Add(message);
        }
    }

    public void TypingAction()
    {
        if (_typingCount == 0)
        {
            StartTyping();
        }
        else if (_typingCount == 1)
        {
            if (_typingText.NowTyping)
            {
                SkipTyping();
            }
            else
            {
                DisplayNextMessage();
            }
        }
        else if (_typingCount == 2)
        {
            DisplayNextMessage();
        }
    }

    private void StartTyping()
    {
        _blurObj.SetActive(true);
        StartCoroutine(_typingText.Typing(LocalizeText.Get(_messageTexts[_messageCheckCount]._messages[_currentMessageIndex]), 0.05f));
        _typingCount++;
    }

    private void SkipTyping()
    {
        _typingText.TypingSkip = true;
        _typingCount++;
    }

    private void DisplayNextMessage()
    {
        _blurObj.SetActive(false);
        _typingCount = 0;
        _currentMessageIndex++;
        ShowNextMessage();
    }

    private void ShowNextMessage()
    {
        if (_currentMessageIndex < _messageTexts[_messageCheckCount]._messages.Length)
        {
            TypingAction();
        }
        if (_currentMessageIndex == _messageTexts[_messageCheckCount]._messages.Length)
        {
            if (_messageCheckCount == 0)
            { 
                _manager.GoIngame(); 
            }
            else if (_messageCheckCount == 1)
            {
                Time.timeScale = 1;
                StartCoroutine(_manager.Scenario2());
            }
            else if (_messageCheckCount == 2)
            {
                Time.timeScale = 1;
                // ����
                StartCoroutine(_manager.Scenario3());
            }
            _currentMessageIndex = 0;
            _messageCheckCount++;
        }
    }
}


[System.Serializable]
public class LocalizedMessage
{
    public string[] _messages;

    public void MessageCount(int i)
    {
        if (i == 0) { SetMessage0(); }
        else if (i == 1) { SetMessage1(); }
        else if (i == 2) { SetMessage2(); }
    }

    public void SetMessage0()
    {
        _messages = new string[]
        {
            "����: ����! �ֺ��� ���Ͱ� ��� �����ϰ� �ִ�!",
            "����: ��� ���� �غ�!",
            "������: ��! ���� ��ȣ�ϰڽ��ϴ�!",
        };
    }
    public void SetMessage1()
    {
        _messages = new string[]
        {
            "����1: ���Ǵ� ��ġ����?",
            "����2: �̺�! �� ���� �ϸ�..",
        };
    }
    public void SetMessage2()
    {
        _messages = new string[]
        {
            "����: �ϴ��� ���һ�..",
        };
    }
}