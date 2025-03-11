using AirFishLab.ScrollingList.ContentManagement;
using AirFishLab.ScrollingList;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using MyData;
using System.Globalization;

public class PassListBox : ListBox
{
    DataManager _dataManager;
    UserData _userData;

    [SerializeField] int _contentNum;

    [Header("Scroll")]
    [SerializeField] Image _iconOn;
    [SerializeField] TextMeshProUGUI _iconNumTMP;

    [Header("Content")]
    [SerializeField] PassItem _freeItem;
    [SerializeField] PassItem _vipItem;

    SeasonPassData _data;

    string spritePath = "ItemIcon/Inven{0}";
    string noneReward = string.Format("{0}", 99);
    public int ContentNum => _contentNum;

    protected override void UpdateDisplayContent(IListContent content)
    {
        _contentNum = ((PassListContent)content).Value;
        _iconNumTMP.text = _contentNum.ToString();

        CheckComponents();
        SetBoxInfo();
    }

    void CheckComponents()
    {
        if (_dataManager == null) { _dataManager = DataManager.GetInstance; }
        if (_userData == null) { _userData = UserData.GetInstance; }
    }

    /// <summary>
    /// ���� �н� �׸��� UI ���� ����
    /// </summary>
    void SetBoxInfo()
    {
        _data = _dataManager.FindData(DataManager.KEY_SEASONPASS, _contentNum) as SeasonPassData;

        SetNormalInfo();
        SetVIPInfo();
    }

    /// <summary>
    /// �Ϲ� �׸� UI ����
    /// </summary>
    public void SetNormalInfo()
    {
        // Normal   
        if (_data.rewardType01.Contains(noneReward))
        {
            _freeItem.DisableItem();
        }
        else
        {
            _freeItem.EnableItem();
            string[] rType01Arr = _data.rewardType01.Split(',');
            string[] rValue01Arr = _data.rewardValue01.Split(',');
            _freeItem.SetPassItem(
                ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(spritePath, ((ERewardType)int.Parse(rType01Arr[0])).ToString())),
                int.Parse(rValue01Arr[0]), 
                _userData.GetSeasonLevel >= _contentNum, 
                _userData.GetSeasonState(_contentNum, false));
        }
    }

    /// <summary>
    /// VIP �׸� UI ���� 
    /// </summary>
    public void SetVIPInfo()
    {
        // VIP
        if (_data == null) return;
        _vipItem.EnableItem();
        string[] rType02Arr = _data.rewardType02.Split(',');
        string[] rValue02Arr = _data.rewardValue02.Split(',');
        _vipItem.SetPassItem(
            ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(spritePath, ((ERewardType)int.Parse(rType02Arr[0])).ToString())),
            int.Parse(rValue02Arr[0]), 
            _userData.IsVipActivated, 
            _userData.GetSeasonState(_contentNum, true));
    }

    /// <summary>
    /// ���� ���� ������ ���� �޼���
    /// </summary>
    /// <param name="rTypeArr">Ÿ�� �迭</param>
    /// <param name="rValueArr">���� �迭</param>
    void ProcessGetReward(string[] rTypeArr, string[] rValueArr)
    {
        LobbyUIManager uiManager = LobbyUIManager.GetInstance;
        var popup = uiManager.GetPopup<RewardResultPopup>(PopupType.REWARDRESULT);
        popup.ResetPopup();

        if (rTypeArr.Length == 1)
        {
            ERewardType rType = (ERewardType)int.Parse(rTypeArr[0]);
            int rValue = int.Parse(rValueArr[0]);
            GetPassReward(rType, rValue);
        }
        else
        {
            ERewardType[] typeArr = new ERewardType[rTypeArr.Length];
            int[] valueArr = new int[rValueArr.Length];
            for (int i = 0; i < rTypeArr.Length; i++)
            {
                typeArr[i]= (ERewardType)int.Parse(rTypeArr[i]);
                valueArr[i] = int.Parse(rValueArr[i]);
            }
            GetPassReward(typeArr, valueArr);
        }
        if ((ERewardType)int.Parse(rTypeArr[0]) < ERewardType.NormalBox)
        {
            uiManager.OpenRewardResultPopup(0.2f);
        }
        uiManager.GetSeasonPassPanel.CheckBadge();
        uiManager.GetSeasonPassPanel.CheckAllReceiveButton();
        _userData.SaveSeasonPassData();
    }

    /// <summary>
    /// �н� ���� ����
    /// </summary>
    /// <param name="rType">Ÿ��</param>
    /// <param name="rValue">����</param>
    void GetPassReward(ERewardType rType, int rValue)
    {
        if (rType == ERewardType.None) return;

        LobbyUIManager.GetInstance.GetShopPage.ProvideNotPurchasingReward(EPayType.NONE, rType,rValue, "���� �н� ����", false, true);
    }

    void GetPassReward(ERewardType[] rTypeArr, int[] rValueArr)
    {
        if (rTypeArr[0] == ERewardType.None) return;

        LobbyUIManager.GetInstance.GetShopPage.ProvideNotPurchasingAllReward(EPayType.NONE, rTypeArr, rValueArr, "���� �н� ����", false);
    }

    public void OnTouchFreeItemButton()
    {
        bool allowed = _userData.GetSeasonLevel >= _contentNum;
        bool notReceived = !_userData.CheckPassItemValidation(_contentNum);

        if (allowed && notReceived)
        {
            // ���� ó��(������)
            _userData.SetSeasonState(_contentNum, true, false);
            _freeItem.SetContentState(true);

            // ������ ����
            string[] rTypeArr = _data.rewardType01.Split(',');
            string[] rValueArr = _data.rewardValue01.Split(",");
            ProcessGetReward(rTypeArr, rValueArr);
        }
    }

    public void OnTouchVIPItemButton()
    {
        if (!_userData.IsVipActivated)
        {
            return;
        }
        bool notReceived = !_userData.CheckPassItemValidation(_contentNum, true);
        if (notReceived)
        {
            // ���� ó��(������)
            _userData.SetSeasonState(_contentNum, true, true);
            _vipItem.SetContentState(true);

            // ������ ����
            string[] rTypeArr = _data.rewardType02.Split(',');
            string[] rValueArr = _data.rewardValue02.Split(",");
            ProcessGetReward(rTypeArr, rValueArr);
        }
    }
}
