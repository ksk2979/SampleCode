using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardResultPopup : PopupBase
{
    [SerializeField] TextLocalizeSetter _titleText;
    [SerializeField] GameObject _touchBlockerText;
    [SerializeField] RectTransform _boardRect;
    [SerializeField] RewardResultItem[] _rewardItemArr;
    List<RParticleType> _standByParticleList = new List<RParticleType>();

    const string filePathFormat = "ItemIcon/Inven{0}";

    const float _itemUnitSize = 55f;
    const int _defaultBoxCount = 2;

    // Blocker(팝업 뒤 배경)
    const float _waitTime = 1.0f;
    float _tmpTime = 0f;

    Vector2 _boardSize = new Vector2(550f, 500f);
    bool _isInit = false;

    void Init()
    {
        foreach (var item in _rewardItemArr)
        {
            item.gameObject.SetActive(false);
        }
        _boardRect.sizeDelta = _boardSize;
        _titleText.key = string.Empty;

        _standByParticleList.Clear();
        for (int i = 0; i < _rewardItemArr.Length; i++)
        {
            _rewardItemArr[i].ResetItem();
        }
    }

    /// <summary>
    /// 팝업 내용 설정
    /// </summary>
    /// <param name="titleKey"></param>
    /// <param name="rType"></param>
    /// <param name="count"></param>
    public void SetPopup(string titleKey, ERewardType rType, int count)
    {
        bool isRegistered = false;
        if (!_isInit)
        {
            _isInit = true;
            Init();
        }

        if (_titleText.key == string.Empty)
        {
            _titleText.key = titleKey;
        }

        // 기존에 등록되어 있는 재화 타입이면 갯수만 변경
        for (int i = 0; i < _rewardItemArr.Length; i++)
        {
            if (_rewardItemArr[i].RewardType == rType)
            {
                _rewardItemArr[i].ChangeItemValue(_rewardItemArr[i].RewardCount + count);
                isRegistered = true;
                break;
            }
        }

        if (isRegistered)
        {
            return;
        }

        // 기존에 등록되어 있지 않은 재화 타입 이면 새로 등록
        for (int j = 0; j < _rewardItemArr.Length; j++)
        {
            if (!_rewardItemArr[j].gameObject.activeSelf)
            {
                _rewardItemArr[j].SetItem(
                    rType,
                    ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(filePathFormat, rType.ToString())),
                    count);
                _rewardItemArr[j].gameObject.SetActive(true);
                break;
            }
        }
    }

    private void Update()
    {
        if(gameObject.activeSelf)
        {
            if(_tmpTime < _waitTime)
            {
                _tmpTime += Time.deltaTime;
                if(_tmpTime >= _waitTime)
                {
                    _touchBlockerText.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// 뒷 보드 사이즈 확인(아이템 갯수에 따라 유동적)
    /// </summary>
    void CheckBoardSize()
    {
        int count = 0;
        foreach (var item in _rewardItemArr)
        {
            if (item.gameObject.activeSelf)
            {
                count++;
            }
        }

        // 기본 사이즈 보다 길어지는 경우
        if (count > _defaultBoxCount)
        {
            count -= _defaultBoxCount;
            _boardRect.sizeDelta = new Vector2(_boardSize.x, _boardSize.y + (_itemUnitSize * count));
        }
    }

    /// <summary>
    /// 팝업 종료시 재생할 파티클 등록(중복 실행 방지)
    /// </summary>
    public void AddParticle(RParticleType particle)
    {
        if (_standByParticleList.Count == 0)
        {
            _standByParticleList.Add(particle);
        }
        else
        {
            if (!_standByParticleList.Contains(particle))
            {
                _standByParticleList.Add(particle);
            }
        }
    }

    /// <summary>
    /// 팝업 종료 이벤트에 파티클 등록
    /// </summary>
    void RegisterParticle()
    {
        for (int i = 0; i < _standByParticleList.Count; i++)
        {
            RParticleType pType = _standByParticleList[i];
            OnCloseEventListener += () => LobbyUIManager.GetInstance.ShowRewardParticle(pType);
        }
    }

    public void ResetPopup()
    {
        _isInit = false;
    }

    public override void OpenPopup()
    {
        Debug.Log("OpenPopup");
        CheckBoardSize();
        _tmpTime = 0;
        _touchBlockerText.SetActive(false);
        base.OpenPopup();
    }

    public override void ClosePopup()
    {
        _isInit = false;
        RegisterParticle();
        ResetPopup();
        base.ClosePopup();
    }

    public void OnTouchBlocker()
    {
        if(_tmpTime >= _waitTime)
        {
            ClosePopup();
        }
    }
}
