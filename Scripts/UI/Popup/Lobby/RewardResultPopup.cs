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

    // Blocker(�˾� �� ���)
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
    /// �˾� ���� ����
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

        // ������ ��ϵǾ� �ִ� ��ȭ Ÿ���̸� ������ ����
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

        // ������ ��ϵǾ� ���� ���� ��ȭ Ÿ�� �̸� ���� ���
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
    /// �� ���� ������ Ȯ��(������ ������ ���� ������)
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

        // �⺻ ������ ���� ������� ���
        if (count > _defaultBoxCount)
        {
            count -= _defaultBoxCount;
            _boardRect.sizeDelta = new Vector2(_boardSize.x, _boardSize.y + (_itemUnitSize * count));
        }
    }

    /// <summary>
    /// �˾� ����� ����� ��ƼŬ ���(�ߺ� ���� ����)
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
    /// �˾� ���� �̺�Ʈ�� ��ƼŬ ���
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
