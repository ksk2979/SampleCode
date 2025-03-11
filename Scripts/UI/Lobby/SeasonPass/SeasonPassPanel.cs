using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AirFishLab.ScrollingList;
using TMPro;
using UnityEngine.UI;
using MyData;
using MyStructData;
using UnityEngine.U2D;

public class SeasonPassPanel : MonoBehaviour
{
    [Header("SeasonPass Info")]
    DateTime _limitTime;
    const int _vipPrice = 3000;
    int _currentSeason = 0;

    [Header("Components")]
    LobbyUIManager _uiManager;
    CircularScrollingList _scrollingList;
    UserData _userData;
    PassListBank _passListBank;
    List<SeasonPassData> _dataList;
    List<SeasonPassData> _currentSeasonDataList;

    [Header("UI")]
    [SerializeField] GameObject _progressBarObj;
    [SerializeField] TextMeshProUGUI _seasonTitleTMP;
    [SerializeField] TextMeshProUGUI _currentProgressTMP;
    [SerializeField] Slider _progressSlider;
    [SerializeField] GameObject _panelObj;
    [SerializeField] GameObject _blocker;
    [SerializeField] Button[] _allReceiveButtonArr;

    [Header("VIP")]
    [SerializeField] GameObject[] _lockedBlockerArr;
    bool _isPurchased = false;

    bool _isExpired = false;

    const string titleFormat = "Season\n<size=130%>{0}";
    const string progressFormat = "{0} / {1}";
    const string pastSeasonPassTitle = "���� ���� �н� ����";

    #region Init
    private void Awake()
    {
        _passListBank = GetComponent<PassListBank>();
        _scrollingList = GetComponent<CircularScrollingList>();
    }

    public void Init(LobbyUIManager uiManager)
    {
        _uiManager = uiManager;
        _userData = UserData.GetInstance;

        FindCurrentSeason();
        CheckSeasonInfo();

        InitBoard();
        CheckBadge();

        for (int i = 0; i < _allReceiveButtonArr.Length; i++)
        {
            int idx = i;
            _allReceiveButtonArr[i].onClick.AddListener(() => OnTouchAllReceiveButton(idx));
        }


    }

    /// <summary>
    /// �����н� ���� ���� �ʱ�ȭ
    /// </summary>
    void InitBoard()
    {
        _panelObj.SetActive(false);
        _blocker.SetActive(false);

        if (!_isExpired)
        {
            InitProgressBar();

            // �г� ���� ����
            _passListBank.Init(_currentSeasonDataList.Count);
            _scrollingList.Init();

            // VIP ���� ����
            _isPurchased = _userData.IsVipActivated;
            CheckVipUIs();

            //Debug.Log("userData.GetSeasonLevel : " + _userData.GetSeasonLevel);
            if (_userData.GetSeasonLevel <= _currentSeasonDataList.Count)
            {
                // ��ũ�� �ʱ�ȭ
                _scrollingList.Refresh(_userData.GetSeasonLevel - 1);
            }
            CheckAllReceiveButton();
        }
    }

    /// <summary>
    /// �����н� ����� �ʱ�ȭ
    /// </summary>
    void InitProgressBar()
    {
        _seasonTitleTMP.text = string.Format(titleFormat, _currentSeason);

        // ����� ���� ����
        int nextLevel = _userData.GetNextPassLevel(_currentSeasonDataList.Count);
        if (nextLevel > 0 && nextLevel <= _currentSeasonDataList.Count)
        {
            _currentProgressTMP.text = string.Format(progressFormat, _userData.GetCurrentProgress, _currentSeasonDataList[nextLevel - 1].max);
            _progressSlider.value = _userData.GetCurrentProgress / (float)_currentSeasonDataList[nextLevel - 1].max;
        }
        else
        {
            _currentProgressTMP.text = string.Format(progressFormat, "-", "-");
            _progressSlider.value = 1;
        }

    }
    #endregion Init

    /// <summary>
    /// ���� �н� ���� üũ(�ܺ� ���)
    /// </summary>
    public void CheckSeasonPass()
    {
        StartCoroutine(UpdateExpired(5.0f));
    }

    /// <summary>
    /// �ϰ� �ޱ� ��ư Ȱ��ȭ ���� üũ
    /// </summary>
    public void CheckAllReceiveButton()
    {
        // Free
        bool isAvailableFree = false;
        for (int i = 0; i < _userData.GetSeasonLevel; i++)
        {
            if (!_userData.GetSeasonState(i + 1, false))
            {
                isAvailableFree = true;
            }
        }
        _allReceiveButtonArr[0].gameObject.SetActive(isAvailableFree);

        // VIP
        bool isAvailableVIP = false;
        if (_userData.IsVipActivated)
        {
            for (int j = 0; j < _currentSeasonDataList.Count; j++)
            {
                if (!_userData.GetSeasonState(j + 1, true))
                {
                    isAvailableVIP = true;
                }
            }
        }
        _allReceiveButtonArr[1].gameObject.SetActive(isAvailableVIP);
    }

    IEnumerator UpdateExpired(float checkTime)
    {
        while (!_isExpired)
        {
            CheckSeasonExpired();
            yield return YieldInstructionCache.WaitForSeconds(checkTime);
        }
    }

    /// <summary>
    /// �����н��� ���� �Ǿ����� Ȯ��
    /// </summary>
    void CheckSeasonExpired()
    {
        if (_isExpired || !_progressBarObj.activeSelf)
        {
            return;
        }
        if (ConnectManager.GetInstance.IsConnected)
        {
            if (_limitTime.Year != 1 && _limitTime < ConnectManager.GetInstance.CurrentTime)
            {
                _progressBarObj.SetActive(false);
                _isExpired = true;
            }
        }
    }

    /// <summary>
    /// VIP ���� ���ο� ���� ��Ÿ���� UI ���� ����
    /// </summary>
    public void CheckVipUIs()
    {
        for (int i = 0; i < _lockedBlockerArr.Length; i++)
        {
            _lockedBlockerArr[i].SetActive(!_isPurchased);
        }
    }

    /// <summary>
    /// �����н� ���� ���� ����
    /// </summary>
    /// <param name="state"></param>
    public void SetPurchaseState(bool state)
    {
        _isPurchased = state;
        _allReceiveButtonArr[1].gameObject.SetActive(state);
        CheckVipUIs();

        // VIP ���� ��ư Ȱ��ȭ
        var listBox = _scrollingList.ListBoxes;
        for (int i = 0; i < listBox.Length; i++)
        {
            PassListBox passListBox = listBox[i] as PassListBox;
            passListBox.SetVIPInfo();
        }
        CheckBadge();
    }

    #region Buttons
    /// <summary>
    /// ����� ��ġ�� �ϴܿ� �����н� �г� ����
    /// </summary>
    public void OpenSeasonPassPanel()
    {
        bool state = !_panelObj.activeSelf;
        _panelObj.SetActive(state);
        _blocker.SetActive(state);
    }
    public void CloseSeasonPassPanel()
    {
        _panelObj.SetActive(false);
        _blocker.SetActive(false);
    }

    /// <summary>
    /// VIP ���� ��ư
    /// </summary>
    public void OnTouchPurchaseVIP()
    {
        ShopScript shop = _uiManager.GetShopPage;
        var popup = shop.GetBuyQuestionPopup();
        if (popup != null)
        {
            popup.OnAgreeEventListener += () => shop.BuyShopItem(EShopProduct.SEASONPASS);
            popup.SetContext(shop.GetBuyAskText(EShopProduct.SEASONPASS));
            popup.OpenPopup();
        }
    }

    /// <summary>
    /// �ϰ� ���� ��ư
    /// </summary>
    /// <param name="idx"></param>
    public void OnTouchAllReceiveButton(int idx)
    {
        Dictionary<ERewardType, int> rewardDic = new Dictionary<ERewardType, int>();
        if (idx == 0)
        {
            // ���� Ȯ��(����)
            for (int i = 1; i <= _currentSeasonDataList.Count; i++)
            {
                bool allowed = _userData.GetSeasonLevel >= i;
                bool notReceived = !_userData.CheckPassItemValidation(i);
                if (allowed && notReceived)
                {
                    string[] rTypeArr = _currentSeasonDataList[i - 1].rewardType01.Split(',');
                    string[] rValueArr = _currentSeasonDataList[i - 1].rewardValue01.Split(',');
                    for (int j = 0; j < rTypeArr.Length; j++)
                    {
                        ERewardType type = (ERewardType)int.Parse(rTypeArr[j]);
                        if (rewardDic.ContainsKey(type))
                        {
                            rewardDic[type] += int.Parse(rValueArr[j]);
                        }
                        else
                        {
                            rewardDic.Add(type, int.Parse(rValueArr[j]));
                        }
                    }
                    // ���� ó��(������)
                    _userData.SetSeasonState(i, true, false);
                }
            }
        }
        else
        {
            // VIP ���� ���� ���� Ȯ�� �� ������ ���
            for (int i = 1; i <= _currentSeasonDataList.Count; i++)
            {
                if (!_userData.GetSeasonState(i, true))
                {
                    string[] rTypeArr = _currentSeasonDataList[i - 1].rewardType02.Split(',');
                    string[] rValueArr = _currentSeasonDataList[i - 1].rewardValue02.Split(',');
                    for (int j = 0; j < rTypeArr.Length; j++)
                    {
                        ERewardType type = (ERewardType)int.Parse(rTypeArr[j]);
                        if (rewardDic.ContainsKey(type))
                        {
                            rewardDic[type] += int.Parse(rValueArr[j]);
                        }
                        else
                        {
                            rewardDic.Add(type, int.Parse(rValueArr[j]));
                        }
                    }
                    // ���� ó��(������)
                    _userData.SetSeasonState(i, true, true);
                }
            }
        }
        // ���� ����
        GetPassReward(rewardDic);

        // ����ó��(UI)
        var listBox = _scrollingList.ListBoxes;
        for (int i = 0; i < listBox.Length; i++)
        {
            PassListBox passListBox = listBox[i] as PassListBox;
            if(idx == 0)
            {
                passListBox.SetNormalInfo();
            }
            else
            {
                passListBox.SetVIPInfo();
            }
        }
        CheckAllReceiveButton();
    }
    #endregion Buttons

    public void CheckBadge()
    {
        bool onBadge = false;
        for (int i = 1; i <= _userData.GetSeasonLevel; i++)
        {
            if (!_userData.GetSeasonState(i, false))
            {
                onBadge = true;
                break;
            }
        }
        if (!onBadge && _userData.IsVipActivated)
        {
            for (int j = 1; j <= _currentSeasonDataList.Count; j++)
            {
                if (!_userData.GetSeasonState(j, true))
                {
                    onBadge = true;
                    break;
                }
            }
        }
        _uiManager.SetBadge(BadgeType.SeasonPass, onBadge);
    }

    /// <summary>
    /// ���� �н� ���� ����/���� Ȯ��
    /// </summary>
    void CheckSeasonInfo()
    {
        int registeredSeason = _userData.GetCurrentSeason;
        bool isSameSeason = registeredSeason == _currentSeason;
        // ���� ���� ���� ��� �ִٸ�
        if (registeredSeason != 0 && !isSameSeason)
        {
            // VIP ���� ��� �ε�(���� �н� ������, VIP)
            var seasonDataList = GetLastSeasonData(registeredSeason);
            if (seasonDataList.Count != 0)
            {
                // VIP ���� ���� ���� Ȯ�� �� ������ ���
                Dictionary<ERewardType, int> rewardDic = new Dictionary<ERewardType, int>();
                for (int i = 1; i <= seasonDataList.Count; i++)
                {
                    if (!_userData.GetSeasonState(i, true))
                    {
                        string[] rTypeArr = seasonDataList[i - 1].rewardType02.Split(',');
                        string[] rValueArr = seasonDataList[i - 1].rewardValue02.Split(',');
                        for (int j = 0; j < rTypeArr.Length; j++)
                        {
                            ERewardType type = (ERewardType)int.Parse(rTypeArr[j]);
                            if (rewardDic.ContainsKey(type))
                            {
                                rewardDic[type] += int.Parse(rValueArr[j]);
                            }
                            else
                            {
                                rewardDic.Add(type, int.Parse(rValueArr[j]));
                            }
                        }
                    }
                }
                GetPassReward(rewardDic);
            }
            // �� ���� ������ ������
            if (_currentSeason != 0)
            {
                _userData.SetSeasonInfo(_currentSeason);
                _limitTime = Convert.ToDateTime(_currentSeasonDataList[0].expirationDate).AddDays(1);
            }
            else
            {
                // ������ ����
                _userData.ResetSeasonPass();
                _limitTime = ConnectManager.GetInstance.CurrentTime.AddDays(-1);
            }
        }
        else
        {
            // ���� ���� ���� ��� ���� ����
            // ���� ���� ���� ����
            if (_currentSeasonDataList.Count > 0)
            {
                // ���� �н� �÷��� ������ ���� ���(��������) ���
                if (_userData.GetCurrentSeason <= 0)
                {
                    _userData.SetSeasonInfo(_currentSeason);
                }
                _limitTime = Convert.ToDateTime(_currentSeasonDataList[0].expirationDate).AddDays(1);
            }
            else
            {
                // ���� ���� ���� ����
                _userData.ResetSeasonPass();
                _limitTime = ConnectManager.GetInstance.CurrentTime.AddDays(-1);
            }
        }
        _userData.SaveSeasonPassData();
    }

    /// <summary>
    /// ���� ������ ã��, ���� ������ �����͸� �����ϴ� �޼���
    /// </summary>
    public void FindCurrentSeason()
    {
        if (_dataList == null)
        {
            _dataList = DataManager.GetInstance.GetList<SeasonPassData>(DataManager.KEY_SEASONPASS);
        }
        // �����н� �����ϰ� �������� �޾Ƽ� ������ ���� ��¥�� ������ ���ԵǴ��� üũ
        for(int i = 0; i < _dataList.Count; i++)
        {
            var currentTime = ConnectManager.GetInstance.CurrentTime;

            var startDate = Convert.ToDateTime(_dataList[i].startDate);
            var expireDate = Convert.ToDateTime(_dataList[i].expirationDate);
            expireDate.AddDays(1);
            if (currentTime >= startDate && currentTime < expireDate)
            {
                _currentSeason = _dataList[i].passNumber;
                break;
            }
        }

        // ���� �����н� �����Ϳ��� ���� �ѹ��� �������� �����͸� ����
        if (_currentSeason != 0)
        {
            _currentSeasonDataList = _dataList.FindAll(x => x.passNumber == _currentSeason);
        }
        else
        {
            _currentSeasonDataList = new List<SeasonPassData>();
        }
    }

    #region Reward
    public void GetPassReward(Dictionary<ERewardType, int> rewardDic)
    {
        // �����ؾ��� ������ �ִٸ� ����
        if (rewardDic.Count > 0)
        {
            var popup = _uiManager.GetPopup<RewardResultPopup>(PopupType.REWARDRESULT);
            popup.ResetPopup();
            foreach (var reward in rewardDic)
            {
                popup.SetPopup(pastSeasonPassTitle, reward.Key, reward.Value);
                if (reward.Key >= ERewardType.Money && reward.Key < ERewardType.NormalBox)
                {
                    if (reward.Key <= ERewardType.ActionEnergy)
                    {
                        popup.AddParticle((RParticleType)reward.Key);
                    }
                    _userData.AddCurrency(reward.Key, reward.Value);
                }
                else if (reward.Key >= ERewardType.NormalBox)
                {
                    _uiManager.AddWaitBox(reward.Key, reward.Value);
                }
            }
            _userData.SaveUserData();
            popup.OnCloseEventListener += () =>
            {
                _uiManager.UpdateUserInfo();
            };
            popup.OpenPopup();
        }
        LobbyUIManager.GetInstance.GetSeasonPassPanel.CheckBadge();
        _userData.SaveSeasonPassData();
    }
    #endregion Reward

    #region Getter
    public bool IsPurchased => _isPurchased;

    /// <summary>
    /// ���� �����н��� ������ ����� �ִٸ�
    /// �ش� ���� ������ ��ȯ (VIP ������ ���)
    /// </summary>
    /// <param name="lastSeason">���� ���� ����</param>
    /// <returns></returns>
    public List<SeasonPassData> GetLastSeasonData(int lastSeason)
    {
        if (_dataList == null)
        {
            _dataList = DataManager.GetInstance.GetList<SeasonPassData>(DataManager.KEY_SEASONPASS);
        }

        List<SeasonPassData> pastSeasonDataList = new List<SeasonPassData>();
        if (lastSeason != 0 && _userData.IsVipActivated)
        {
            for (int i = 0; i < _dataList.Count; i++)
            {
                if (_dataList[i].passNumber == lastSeason)
                {
                    pastSeasonDataList.Add(_dataList[i]);
                }
            }
        }
        return pastSeasonDataList;
    }
    #endregion Getter
}
