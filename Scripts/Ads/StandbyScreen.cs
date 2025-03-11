using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StandbyScreen : MonoBehaviour
{
    public delegate void ScreenEventDelegate();
    public event ScreenEventDelegate OnRewardEventListener;

    [SerializeField] VideoController _videoController;
    [SerializeField] Image _iconImage;
    [SerializeField] TextMeshProUGUI _gameNameTMP;
    [SerializeField] TextMeshProUGUI _timeText;
    [SerializeField] GameObject _exitButtonObj;
    
    [SerializeField] bool _startCheck = false;
    const float _waitMaxTime = 5.0f;
    float _tmpTime;

    /// <summary>
    /// 대기 이미지 설정
    /// </summary>
    /// <param name="iconSprite"></param>
    public void InitScreen(Sprite iconSprite, string gameNameText)
    {
        _iconImage.sprite = iconSprite;
        _gameNameTMP.text = gameNameText; 
        _exitButtonObj.SetActive(false);
        _timeText.gameObject.SetActive(true);
    }

    /// <summary>
    /// 대기 이미지 상태 설정
    /// </summary>
    /// <param name="state"></param>
    public void SetScreenState(bool state)
    {
        gameObject.SetActive(state);
        
        if(!state)
        {
            _iconImage.sprite = null;
        }
    }

    IEnumerator CheckWaitTime()
    {
        while(_startCheck)
        {
            _tmpTime += 0.02f;
            _timeText.text = string.Format("{0}", Mathf.RoundToInt(_waitMaxTime - _tmpTime));
            if (_tmpTime >= _waitMaxTime)
            {
                _tmpTime = 0;
                _exitButtonObj.SetActive(true);
                _timeText.gameObject.SetActive(false);
                _startCheck = false;
            }
            yield return YieldInstructionCache.RealTimeWaitForSeconds(0.02f);
        }
    }

    public void OnTouchDownloadButton()
    {
        _videoController.OnTouchLinkStore();
    }

    public void OnTouchCloseButton()
    {
        //Time.timeScale = 1;
        OnRewardEventListener?.Invoke();
        OnRewardEventListener = null;
        if (GameManager.GetInstance._nowScene == EScene.E_LOBBY)
        {
            LobbyUIManager.GetInstance.GetVideoBlocker.SetActive(false);
        }
        else
        {
            InGameUIManager.GetInstance.GetVideoBlocker.SetActive(false);
        }
        SoundManager.GetInstance.BackgroundMute = false;
        SetScreenState(false);
    }

    private void OnEnable()
    {
        if(_iconImage.sprite != null)
        {
            _startCheck = true;
            StartCoroutine(CheckWaitTime());
        }
    }
}
