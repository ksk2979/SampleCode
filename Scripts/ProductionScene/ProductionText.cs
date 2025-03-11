using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionText : MonoBehaviour
{
    [Header("텍스트")]
    [SerializeField] TypingText _typingText; // 실제 내용
    [SerializeField] GameObject _blurObj; // 텍스트 오브젝트

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
                // 죽임
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
            "선장: 오오! 주변에 몬스터가 대거 출현하고 있다!",
            "선장: 모두 전투 준비!",
            "선원들: 네! 선장 엄호하겠습니다!",
        };
    }
    public void SetMessage1()
    {
        _messages = new string[]
        {
            "선원1: 거의다 해치웠나?",
            "선원2: 이봐! 그 말을 하면..",
        };
    }
    public void SetMessage2()
    {
        _messages = new string[]
        {
            "선장: 하느님 맙소사..",
        };
    }
}