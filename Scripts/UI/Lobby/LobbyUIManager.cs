using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyData;
using UnityEngine.Rendering;

public enum ETextType
{
    EnergyText = 0,
    QuarterMinText = 1,
    HourMinText = 2,
    DayText = 3,
}

public class LobbyUIManager : SceneStaticObj<LobbyUIManager>
{
    public delegate void AdEventDelegate();
    List<AdEventDelegate> _onTimeEventList = new List<AdEventDelegate>();           // 시간 체크 완료
    List<AdEventDelegate> _notReachedEventList = new List<AdEventDelegate>();       // 시간 체크 도중

    const int MaxDays = 6;
    const int MaxHours = 23;
    const int MaxMinutes = 60;
    const int QuarterMinutes = 14;
    const int MaxSeconds = 60;
    const int MaxEnergy = 30;

    const string ValueTextFormat = "{0}/{1}";
    const string QuaterTextFormat = "{0:D2}M {1:D2}S";
    const string HourMinTextFormat = "{0:D2}H {1:D2}M";
    const string DayTextFormat = "{0:D1}Day";

    UserData _userData;

    [SerializeField] GameObject _loadingObj;
    [SerializeField] UserInterface _userInterface;
    [SerializeField] LobbyParticle _lobbyParticle;

    [SerializeField] SeasonPassPanel _seasonPassPanel;

    [Header("Page")]
    [SerializeField] PageBase[] _lobbyPageArr;
    [SerializeField] GameObject _homeBtnObj;
    [SerializeField] GameObject _shopBtnObj;
    [SerializeField] GameObject _invenBtnObj;
    StageScript _stageS;
    ShopScript _shopS;
    InvenScript _invenS;
    ReadyScript _readyS;
    ItemEditorScript _itemEditorS;
    PageType _currentPageType;

    [Header("Popup")]
    [SerializeField] PopupController _popupController;
    EnergyPopup _energyPopup;
    OneDayRewardPopup _oneDayRewardPopup;
    RoulettePopup _roulettePopup;
    RewardResultPopup _rewardResultPopup;
    ShopBoxPopup _shopBoxPopup;

    [Header("Menu")]
    MenuController _menuController;
    BadgeController _badgeController;

    //[Header("행동력")]
    public const string ActionTime = "ActionTime";
    bool _actionEnergy = false;
    string _actionTimeStr = "";
    TimeSpan _actionTimeDir;
    DateTime _actionStartTime;

    [Header("Ad Check")]
    string[] _stTime;
    TimeSpan[] _timeDir; // 다이아, 노멀상자, 엘리트상자, 골드, 행동력, 일일출석체크(일일나만의상점초기화)
    DateTime[] _startTime;
    DateTime _currentTime;
    bool[] _timeCheck;
    [SerializeField] Transform _targetRotation;
    [SerializeField] GameObject _videoBlocker;

    [Header("Reward")]
    bool _checkRewardBox = false;
    Dictionary<ERewardType, int> _waitBoxDictionary = new Dictionary<ERewardType, int>();

    [Header("TeamSelect")]
    [SerializeField] Transform[] _boatParentArr;

    [Header("Action Schedule")]
    Queue<Action> _actionScheduleQueue = new Queue<Action>();

    bool _isEndOfTutorial = false;

    #region Getter 
    public Transform GetTargetRotation() { return _targetRotation; }
    public void ReadyForStart() => _stageS.ReadyForStart();
    public void ShowRewardParticle(RParticleType pType) => _lobbyParticle.ShowParticle(pType);
    public InvenScript GetInvenPage => _lobbyPageArr[(int)PageType.INVEN] as InvenScript;
    public ShopScript GetShopPage => _lobbyPageArr[(int)PageType.SHOP] as ShopScript;
    public StageScript GetStagePage => _lobbyPageArr[(int)PageType.STAGE] as StageScript;
    public ReadyScript GetReadyPage => _lobbyPageArr[(int)PageType.READY] as ReadyScript;
    public ItemEditorScript GetItemEditorPage => _lobbyPageArr[(int)PageType.ITEMEDITOR] as ItemEditorScript;
    public PopupController GetPopupController => _popupController;
    public UserInterface GetUserInterface => _userInterface;
    public PageType GetCurrentPageType => _currentPageType;
    public SeasonPassPanel GetSeasonPassPanel => _seasonPassPanel;
    public MenuController GetMenuController => _menuController;
    public BadgeController GetBadgeController => _badgeController;
    public GameObject GetVideoBlocker => _videoBlocker;
    public Transform GetPreviewPos(int idx) => _boatParentArr[idx];
    public T GetPopup<T>(PopupType type)
    {
        var popup = _popupController.GetPopup<T>(type);
        if (popup != null)
        {
            return popup;
        }
        else
        {
            return default(T);
        }
    }
    #endregion Getter

    #region Init
    private void Start()
    {
        Init();

        if (AudioController.IsPlaying("00_Lobby_BGM") == false)
            SoundManager.GetInstance.PlayAudioBackgroundSound("00_Lobby_BGM");

        _isEndOfTutorial = TutorialManager.GetInstance.CheckTutorial();
        if (_isEndOfTutorial)
        {
            CheckRewardTimes((int)ETimeCheckType.ONEDAY);
        }
        else
        {
            CheckRewardTimes((int)ETimeCheckType.DAILY_ROULETTE, Enum.GetValues(typeof(ETimeCheckType)).Length);
        }

        // 행동력 검사
        if (PlayerPrefs.GetString(ActionTime) != "")
        {
            _actionTimeStr = PlayerPrefs.GetString(ActionTime);
            _actionStartTime = Convert.ToDateTime(_actionTimeStr);
            _actionTimeDir = _currentTime - _actionStartTime;
            if (_actionTimeDir.Days > 0 || _actionTimeDir.Hours > 2)
            {
                _userData.AddCurrency(EPropertyType.ACTIONENERGY, 30);
                _userData.SaveUserData();
                _actionEnergy = false;
                _energyPopup.SwitchTextState(EEnergyTextType.TimeText, _actionEnergy);
                PlayerPrefs.SetString(ActionTime, "");
            }
            else if (_actionTimeDir.Minutes > QuarterMinutes || _actionTimeDir.Hours > 0)
            {
                int hour = _actionTimeDir.Hours;
                int min = _actionTimeDir.Minutes;
                while (true)
                {
                    min -= (QuarterMinutes + 1);
                    _userData.AddCurrency(EPropertyType.ACTIONENERGY, 5);
                    _userData.SaveUserData();
                    if (_userData.GetCurrency(EPropertyType.ACTIONENERGY) >= MaxEnergy)
                    {
                        _actionEnergy = false;
                        _energyPopup.SwitchTextState(EEnergyTextType.TimeText, _actionEnergy);
                        PlayerPrefs.SetString(ActionTime, "");
                        break;
                    }
                    else
                    {
                        ActionEnergyTimeUpdate(_actionEnergy);
                    }

                    if (min < QuarterMinutes + 1)
                    {
                        if (hour > 0)
                        {
                            hour -= 1;
                            min += 60;
                        }
                        else { break; }
                    }
                }
            }
        }

        // 행동력이 Max보다 낮은지 체크
        if (_userData.GetCurrency(EPropertyType.ACTIONENERGY) < MaxEnergy)
        {
            ActionEnergyTimeStart();
            if (PlayerPrefs.GetString(ActionTime) == "")
            {
                // 시간체크 O
                ActionEnergyTimeUpdate(true);
            }
            else
            {
                // 시간체크 X
                ActionEnergyTimeUpdate(false);
            }
        }
        _stageS.StageScroll.CheckUIState();
        StartCoroutine(IUpdate());
        _seasonPassPanel.CheckSeasonPass();
    }

    void Init()
    {
        GameManager.GetInstance._nowScene = EScene.E_LOBBY;
        _userData = UserData.GetInstance;
        _currentTime = ConnectManager.GetInstance.CurrentTime;
        int timeCheckMax = Enum.GetValues(typeof(ETimeCheckType)).Length;
        _stTime = new string[timeCheckMax];
        _timeDir = new TimeSpan[timeCheckMax];
        _startTime = new DateTime[timeCheckMax];
        _timeCheck = new bool[timeCheckMax];
        _videoBlocker.SetActive(false);

        TutorialManager.GetInstance.Init();

        InitPage();

        _badgeController = GetComponent<BadgeController>();
        _badgeController.Init();

        InitDelegateList();
        _popupController.Init(InitPopupSet);

        for (int i = 0; i < _lobbyPageArr.Length; i++)
        {
            _lobbyPageArr[i].Init(this);
        }

        UpdateUserInfo();
        _seasonPassPanel.Init(this);

        _lobbyParticle.Init(this);
        InitLobbyButton();
        _currentPageType = PageType.STAGE;
        ChangePage(_currentPageType);
        _checkRewardBox = true;
    }

    /// <summary>
    /// 페이지 초기화
    /// 실질적인 초기화는 Popup 초기화 이후 발생
    /// </summary>
    void InitPage()
    {
        _stageS = _lobbyPageArr[(int)PageType.STAGE] as StageScript;
        _shopS = _lobbyPageArr[(int)PageType.SHOP] as ShopScript;
        _invenS = _lobbyPageArr[(int)PageType.INVEN] as InvenScript;
        _readyS = _lobbyPageArr[(int)PageType.READY] as ReadyScript;
        _itemEditorS = _lobbyPageArr[(int)PageType.ITEMEDITOR] as ItemEditorScript;
        _readyS.OnStartEventListener += () =>
        {
            if (!_stageS.CheckException())
            {
                _stageS.GameStart();
            }
        };
    }

    /// <summary>
    /// 로비에서 사용하는 팝업들 초기화
    /// </summary>
    void InitPopupSet()
    {
        // 행동력
        _energyPopup = GetPopup<EnergyPopup>(PopupType.ENERGY);
        _energyPopup.Init(this);
        // 출석보상
        _oneDayRewardPopup = GetPopup<OneDayRewardPopup>(PopupType.DAILYREWARD);
        // 랜덤 박스 팝업
        _shopBoxPopup = GetPopup<ShopBoxPopup>(PopupType.BOXOPEN);
        _shopBoxPopup.Init();
        // 룰렛
        _roulettePopup = GetPopup<RoulettePopup>(PopupType.ROULETTE);
        _roulettePopup.Init(this);
        // 보상 결과 
        _rewardResultPopup = GetPopup<RewardResultPopup>(PopupType.REWARDRESULT);

        GetPopup<OptionManager>(PopupType.OPTION).Init();                  // 옵션
        GetPopup<SynthesisPopup>(PopupType.SYNTHESIS).Init(this);          // 합성
        GetPopup<QuestPopup>(PopupType.QUEST).Init(this);                  // 퀘스트
        GetPopup<ActiveSkillPopup>(PopupType.ACTIVESKILL).Init(this);      // 스킬 선택 팝업
        GetPopup<CollectionPopup>(PopupType.COLLECTION).Init(this);        // 컬렉션
    }

    /// <summary>
    /// 로비 버튼 초기화
    /// </summary>
    void InitLobbyButton()
    {
        _menuController = GetComponent<MenuController>();
        _menuController.Init();

        var menuArr = _menuController.RegisteredMenuType;
        for (int i = 0; i < menuArr.Length; i++)
        {
            Action buttonAction = null;
            switch (menuArr[i])
            {
                case MenuType.HOME:
                    buttonAction = () => OnTouchChangePageButton(0);
                    break;
                case MenuType.SHOP:
                    buttonAction = () => OnTouchChangePageButton(1);
                    break;
                case MenuType.INVEN:
                    buttonAction = () => OnTouchChangePageButton(2);
                    break;
                case MenuType.PLAY:
                    buttonAction = () => ReadyForStart();
                    break;
                case MenuType.OPTION:
                    buttonAction = () => GetPopup<OptionManager>(PopupType.OPTION).OpenPopup();
                    break;
                case MenuType.DAILY:
                    buttonAction = () => OnTouchOneDayRewardButton();
                    break;
                case MenuType.COLLECTION:
                    buttonAction = () => GetPopup<CollectionPopup>(PopupType.COLLECTION).OpenPopup();
                    break;
                case MenuType.ENERGY:
                    buttonAction = () => OnTouchAddEnergyButton();
                    break;
                case MenuType.DIA:
                    buttonAction = () => OnTouchAddDiaButton();
                    break;
                case MenuType.MONEY:
                    buttonAction = () => OnTouchAddMoneyButton();
                    break;
                case MenuType.INFO:
                    break;
                case MenuType.PREV:
                    buttonAction = () => _stageS.StageScroll.ArrowBtn(true);
                    break;
                case MenuType.NEXT:
                    buttonAction = () => _stageS.StageScroll.ArrowBtn(false);
                    break;
                case MenuType.MAINQUEST:
                    buttonAction = () => GetPopup<QuestPopup>(PopupType.QUEST).OnTouchOpenMainQuest();
                    break;
                case MenuType.DAILYQUEST:
                    buttonAction = () => GetPopup<QuestPopup>(PopupType.QUEST).OnTouchOpenDailyQuest();
                    break;
                case MenuType.ROULETTE:
                    buttonAction = () => _roulettePopup.OpenPopup();
                    break;
                default:
                    break;
            }
            _menuController.AddButton(menuArr[i], buttonAction);
        }
    }

    /// <summary>
    /// 이벤트 리스트 초기화
    /// </summary>
    void InitDelegateList()
    {
        // OneDayCheck : 0
        _onTimeEventList.Add(() =>
        {
            _oneDayRewardPopup.Init(true);
            _userData.ResetDailyQuest();
        });
        _notReachedEventList.Add(() => _oneDayRewardPopup.Init(false));

        // Roulette : 1
        _onTimeEventList.Add(() =>
        {
            _roulettePopup.SetButtonState(true);
            _userData.RewardTimeSetting("0", (int)ETimeCheckType.DAILY_ROULETTE, false);
        });
        _notReachedEventList.Add(() =>
        {
            _roulettePopup.SetButtonState(false);
            _roulettePopup.SetTimeText(GetFormattedText(ETextType.HourMinText,
                _timeDir[(int)ETimeCheckType.DAILY_ROULETTE].Hours,
                _timeDir[(int)ETimeCheckType.DAILY_ROULETTE].Minutes));
        });

        // Dia : 2
        _onTimeEventList.Add(() =>
        {
            _shopS.SetAdRewardSet(2, true);
            _userData.RewardTimeSetting("0", (int)ETimeCheckType.AD_DIA, false);
        });
        _notReachedEventList.Add(() => { });

        // Money : 3
        _onTimeEventList.Add(() =>
        {
            _shopS.SetAdRewardSet(3, true);
            _userData.RewardTimeSetting("0", (int)ETimeCheckType.AD_MONEY, false);
        });
        _notReachedEventList.Add(() => { });

        // Energy(Action) : 4
        _onTimeEventList.Add(() =>
        {
/*            _userData.RewardTimeSetting("0", (int)ETimeCheckType.AD_ACTIONENERGY, false);
            _energyPopup.SetAdState(true);*/
        });
        _notReachedEventList.Add(() =>
        {
/*            _energyPopup.SetAdState(false);
            _energyPopup.SetEnergyText(EEnergyTextType.AdTimeText,
                GetFormattedText(ETextType.HourMinText,
                _timeDir[(int)ETimeCheckType.AD_ACTIONENERGY].Hours,
                _timeDir[(int)ETimeCheckType.AD_ACTIONENERGY].Minutes));*/
        });

        // Normal Box : 5
        _onTimeEventList.Add(() =>
        {
            _shopS.SetAdRewardSet(0, true);
            _shopS.GetChestNormalTimeText().gameObject.SetActive(false);
            _userData.RewardTimeSetting("0", (int)ETimeCheckType.AD_NORMALBOX, false);
        });
        _notReachedEventList.Add(() =>
        {
            _shopS.SetAdRewardSet(0, false);
            _shopS.GetChestNormalTimeText().gameObject.SetActive(true);
            _shopS.GetChestNormalTimeText().text = GetFormattedText(ETextType.HourMinText,
                _timeDir[(int)ETimeCheckType.AD_NORMALBOX].Hours,
                _timeDir[(int)ETimeCheckType.AD_NORMALBOX].Minutes);
        });

        // Elite Box : 6
        _onTimeEventList.Add(() =>
        {
            _shopS.SetAdRewardSet(1, true);
            _shopS.GetChestEliteTimeText().gameObject.SetActive(false);
            _userData.RewardTimeSetting("0", (int)ETimeCheckType.AD_ELITEBOX, false);
        });
        _notReachedEventList.Add(() =>
        {
            _shopS.SetAdRewardSet(1, false);
            _shopS.GetChestEliteTimeText().gameObject.SetActive(true);
            _shopS.GetChestEliteTimeText().text =
            GetFormattedText(ETextType.DayText, _timeDir[(int)ETimeCheckType.AD_ELITEBOX].Days, 0);
        });
    }
    #endregion Init

    #region Checker
    /// <summary>
    /// 광고보상 시간 체크 (1회성 체크, 초기화)
    /// </summary>
    public void CheckRewardTimes(int startNum, int endNum = -1)
    {
        // 미설정시 전체 체크
        if (endNum == -1) { endNum = _timeCheck.Length; }

        for (int i = startNum; i < endNum; i++)
        {
            if (_userData.RewardTimeReturn(i) == "0")
            {
                _timeCheck[i] = false;
                _onTimeEventList[i]?.Invoke();
            }
            else
            {
                _timeCheck[i] = true;
                _stTime[i] = _userData.RewardTimeReturn(i);
                _startTime[i] = Convert.ToDateTime(_stTime[i]);
                _timeDir[i] = _currentTime - _startTime[i];
                _notReachedEventList[i]?.Invoke();
            }

        }
        _userData.SaveRewardTime();
    }

    /// <summary>
    /// 리워드(및 광고) 시간 리셋
    /// </summary>
    /// <param name="type"></param>
    public void ResetCheckTime(ETimeCheckType type, bool executeEvent = true)
    {
        int idx = (int)type;
        DateTime endTime = TimeManager.GetInstance.DayFirstTime();
        _userData.RewardTimeSetting(endTime.ToString(), idx);
        _stTime[idx] = _userData.RewardTimeReturn(idx);
        _startTime[idx] = Convert.ToDateTime(_stTime[idx]);
        _timeCheck[idx] = true;
        switch (type)
        {
            case ETimeCheckType.AD_NORMALBOX:
                _shopS.GetChestNormalTimeText().gameObject.SetActive(true);
                break;
            case ETimeCheckType.AD_ELITEBOX:
                _shopS.GetChestEliteTimeText().gameObject.SetActive(true);
                break;
        }
        if (executeEvent)
        {
            CheckRewardTimes(idx, idx + 1);
        }
    }

    /// <summary>
    /// 에너지 체크해서 부족하면 
    /// 에너지 팝업 오픈
    /// </summary>
    /// <returns></returns>
    public bool ActionCheck()
    {
        if (_userData.GetCurrency(EPropertyType.ACTIONENERGY) < 5)
        {
            _energyPopup.OpenPopup();
            return false;
        }
        else
        {
            return true;
        }

    }

    /// <summary>
    /// 진행 중인 튜토리얼 확인(게임 강제 종료 시 재시작, 추후 기능 분할 및 변경 필요)
    /// </summary>
    public bool CheckTutorial()
    {
        bool isEndTutorial = false;
        if (_userData.TutorialCheck() >= 101 || _userData.TutorialCheck() >= 8)
        {
            isEndTutorial = true;
        }
        return isEndTutorial;
    }
    #endregion Checker

    #region Update
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameManager.GetInstance.GetExitCheck)
            {
                GameManager.GetInstance.GetExitCheck = true;
                MessageHandler.GetInstance.ShowExitMessage(1f);
            }
            else
            {
                Application.Quit();
            }
        }
        // 팝업 상태 체크하고 다음 대기중인 액션(팝업)을 실행
        if (_isEndOfTutorial && _waitBoxDictionary.Count <= 0 && !_popupController.IsExistOpenedPopup())
        {
            var action = GetRegisteredAction();
            if (action != null)
            {
                action?.Invoke();
            }
        }

        if (_shopBoxPopup.IsOpened || _rewardResultPopup.IsOpened || !_checkRewardBox || _waitBoxDictionary.Count <= 0)
        {
            return;
        }
        foreach (var boxCount in _waitBoxDictionary)
        {
            if (boxCount.Value > 0)
            {
                if (boxCount.Key == ERewardType.NormalBox)
                {
                    _shopS.ProvideNotPurchasingReward(EPayType.NONE, ERewardType.NormalBox, 1, "", false);
                }
                else if (boxCount.Key == ERewardType.EliteBox)
                {
                    _shopS.ProvideNotPurchasingReward(EPayType.NONE, ERewardType.EliteBox, 1, "", false);
                }
                _waitBoxDictionary[boxCount.Key]--;
                if (_waitBoxDictionary[boxCount.Key] <= 0)
                {
                    _waitBoxDictionary.Remove(boxCount.Key);
                }
                break;
            }
        }
    }

    /// <summary>
    /// 광고 보상 시간 체크 활성화 (업데이트)
    /// </summary>
    /// <returns></returns>
    IEnumerator IUpdate()
    {
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(1f);
            _currentTime = ConnectManager.GetInstance.CurrentTime;
            //Debug.Log("Current Time : " + _currentTime);
            // 광고 보상 초기화 시간 체크
            for (int i = 0; i < _timeCheck.Length; i++)
            {
                if (_timeCheck[i])
                {
                    _timeDir[i] = _currentTime - _startTime[i];
                    if (i == (int)ETimeCheckType.AD_ELITEBOX && _timeDir[i].Days > 6)
                    {
                        _timeCheck[i] = false;
                        _onTimeEventList[i]?.Invoke();
                    }
                    else if (_timeDir[i].Days > 0)
                    {
                        _timeCheck[i] = false;
                        _onTimeEventList[i]?.Invoke();
                    }
                    else
                    {
                        if (i != (int)ETimeCheckType.ONEDAY)
                        {
                            _notReachedEventList[i]?.Invoke();
                        }
                    }
                }
            }
            _userData.SaveRewardTime();

            string timeText = string.Empty;
            // 에너지 충전 시간 체크
            if (_actionEnergy)
            {
                _actionTimeDir = _currentTime - _actionStartTime;
                timeText = GetFormattedText(ETextType.QuarterMinText, _actionTimeDir.Minutes, _actionTimeDir.Seconds);
                if (_actionTimeDir.Minutes >= QuarterMinutes + 1)
                {
                    AddEnergy();
                    if (_userData.GetCurrency(EPropertyType.ACTIONENERGY) >= MaxEnergy)
                    {
                        DisableChargeEnergy();
                        timeText = string.Empty;
                    }
                    else
                    {
                        ActionEnergyTimeUpdate(_actionEnergy);
                    }
                }
            }
            _userInterface.UpdateRemainTimeText(timeText);
        }
    }
    #endregion Update

    #region Info
    /// <summary>
    /// 재화 / 레벨 정보 업데이트
    /// </summary>
    public void UpdateUserInfo()
    {
        _userInterface.UpdateAllText();
        _invenS.ShowMyMaterials();
    }

    /// <summary>
    /// 튜토리얼 완료상태 전환
    /// </summary>
    /// <param name="state"></param>
    public void SetTutorialState(bool state) => _isEndOfTutorial = state;
    #endregion Info

    #region Buttons
    // 팝업 버튼
    public void OnTouchOneDayRewardButton() => _oneDayRewardPopup.OpenPopup();
    public void OnTouchAddEnergyButton() => _energyPopup.OpenPopup();

    public void OpenRewardResultPopup(float waitTime)
    {
        StartCoroutine(DelayShowRewardResult(waitTime));
    }
    IEnumerator DelayShowRewardResult(float waitTime)
    {
        yield return YieldInstructionCache.WaitForSeconds(waitTime);
        GetPopup<RewardResultPopup>(PopupType.REWARDRESULT).OpenPopup();
    }

    /// <summary>
    /// 페이지 변경 버튼
    /// </summary>
    /// <param name="idx">페이지 번호</param>
    public void OnTouchChangePageButton(int idx) => ChangePage((PageType)idx);

    void ChangePageButtonState(PageType type)
    {
        _homeBtnObj.SetActive(false);
        _shopBtnObj.SetActive(false);
        _invenBtnObj.SetActive(false);
        switch (type)
        {
            case PageType.STAGE:
                {
                    _homeBtnObj.SetActive(true);
                }
                break;
            case PageType.SHOP:
                {
                    _shopBtnObj.SetActive(true);
                }
                break;
            case PageType.INVEN:
                {
                    _invenBtnObj.SetActive(true);
                }
                break;
        }
    }

    public void OnTouchAddDiaButton() => _shopS.DiamondPageBtn();
    public void OnTouchAddMoneyButton() => _shopS.GoldPageBtn();
    #endregion Buttons

    #region Change View
    /// <summary>
    /// 페이지 변경
    /// </summary>
    /// <param name="type">페이지 타입</param>
    public void ChangePage(PageType type)
    {
        for (int i = 0; i < _lobbyPageArr.Length; i++)
        {
            _lobbyPageArr[i].ClosePage();
        }
        _lobbyPageArr[(int)type].OpenPage();
        _currentPageType = type;
        ChangePageButtonState(type);
    }

    /// <summary>
    /// 씬 전환 메서드
    /// </summary>
    /// <param name="sceneName">씬 이름</param>
    /// <returns></returns>
    IEnumerator LoadingScene(string sceneName)
    {
        _loadingObj.SetActive(true);
        SoundManager.GetInstance.DestroyEffectSoundObj();
        LocalizeText.DeleteChangeLocalize();
        yield return YieldInstructionCache.WaitForSeconds(1f);
        cSceneManager.GetInstance.ChangeScene(sceneName, () => { });
    }

    public void LoadingStage(string stageName) => StartCoroutine(LoadingScene(stageName));
    #endregion Change View

    #region Energy
    public void RefreshEnergyPopup()
    {
        // 에너지 충전 텍스트
        _energyPopup.SwitchTextState(EEnergyTextType.TimeText, _actionEnergy);
        if (_actionEnergy)
        {
            _energyPopup.SetEnergyText(EEnergyTextType.TimeText,
                GetFormattedText(ETextType.QuarterMinText, _actionTimeDir.Minutes, _actionTimeDir.Seconds));
        }
        // 현재 에너지 텍스트
        _energyPopup.SetEnergyText(EEnergyTextType.ValueText, GetFormattedText(ETextType.EnergyText, UserData.GetInstance.GetCurrency(EPropertyType.ACTIONENERGY), MaxEnergy));
    }

    /// <summary>
    /// 에너지 충전
    /// </summary>
    public void AddEnergy()
    {
        _userData.AddCurrency(EPropertyType.ACTIONENERGY, 5);
        _userData.SaveUserData();
        _userInterface.UpdateAllText();
        UpdateUserInfo();
    }
    /// <summary>
    /// 에너지 사용
    /// </summary>
    public void UseEnergy()
    {
        _userData.SubCurrency(EPropertyType.ACTIONENERGY, 5);
        _userData.SaveUserData();
        _userInterface.UpdateAllText();
        // 최대치 보다 낮아졌으면
        if (_userData.GetCurrency(EPropertyType.ACTIONENERGY) < MaxEnergy)
        {
            // 현재 타임안가고 있었으면
            if (PlayerPrefs.GetString(ActionTime) == "")
            {
                ActionEnergyTimeStart();
                ActionEnergyTimeUpdate(_actionEnergy);
            }
        }
    }

    /// <summary>
    /// 15분마다 에너지 충전 메서드
    /// </summary>
    public void ActionEnergyTimeStart()
    {
        _actionEnergy = true;
        _energyPopup.SwitchTextState(EEnergyTextType.TimeText, _actionEnergy);
    }

    /// <summary>
    /// 에너지가 최대치가 아닌 경우
    /// 시간 체크를 지속시켜주는 메서드
    /// </summary>
    public void ActionEnergyTimeUpdate(bool timeReset)
    {
        if (timeReset)
        {
            DateTime startTime = DateTime.Now; // 현재 시간을 저장
            PlayerPrefs.SetString(ActionTime, startTime.ToString()); // 유니티 저장으로 string 저장
        }
        _actionTimeStr = PlayerPrefs.GetString(ActionTime);
        _actionStartTime = Convert.ToDateTime(_actionTimeStr);
        _actionTimeDir = _currentTime - _actionStartTime;
    }

    /// <summary>
    /// 에너지 구매후 갱신
    /// </summary>
    public void ActionEnergyAddUpdate()
    {
        _energyPopup.SetEnergyText(EEnergyTextType.ValueText,
            GetFormattedText(ETextType.EnergyText, _userData.GetCurrency(EPropertyType.ACTIONENERGY), MaxEnergy));
        if (_userData.GetCurrency(EPropertyType.ACTIONENERGY) >= MaxEnergy)
        {
            DisableChargeEnergy();
            _energyPopup.SwitchTextState(EEnergyTextType.TimeText, _actionEnergy);
        }
    }

    /// <summary>
    /// 에너지 충전 OFF 상태로 전환
    /// </summary>
    void DisableChargeEnergy()
    {
        _actionEnergy = false;
        PlayerPrefs.SetString(ActionTime, "");
    }
    #endregion Energy

    #region Reward

    /// <summary>
    /// 보상 이펙트
    /// </summary>
    /// <param name="num"></param>
    public void ShowRewardEffect(RParticleType type, float delayTime)
    {
        if (delayTime <= 0)
        {
            _lobbyParticle.ShowParticle(type);
        }
        else
        {
            StartCoroutine(DelayShowParticle(type, delayTime));
        }
    }

    IEnumerator DelayShowParticle(RParticleType type, float waitTime)
    {
        yield return YieldInstructionCache.WaitForSeconds(waitTime);
        _lobbyParticle.ShowParticle(type);
    }

    /// <summary>
    /// 레벨업 보상
    /// </summary>
    /// <param name="isFirst">첫 클리어 인지</param>
    public void CheckLevelUpReward()
    {
        int prevLevel = GameManager.GetInstance.GetPrevLevel;
        int curLevel = _userData.GetUserLevel;
        if(prevLevel != curLevel)
        {
            var levelDataList = DataManager.GetInstance.GetList<LevelData>(DataManager.KEY_LEVEL);
            // 보상 확인
            Dictionary<ERewardType, int> rewardDic = new Dictionary<ERewardType, int>();
            for (int i = prevLevel + 1; i <= curLevel; i++)
            {
                string[] rTypeArr = levelDataList[i - 1].rewardType.Split(',');
                string[] rValueArr = levelDataList[i - 1].rewardValue.Split(',');
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
            // 지급해야할 보상이 있다면 지급
            if (rewardDic.Count > 0)
            {
                RewardResultPopup popup = GetPopup<RewardResultPopup>(PopupType.REWARDRESULT);
                popup.ResetPopup();
                foreach (var reward in rewardDic)
                {
                    popup.SetPopup("Level Up", reward.Key, reward.Value);
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
                        //AddWaitBox(reward.Key, reward.Value , !isFirst);
                        AddWaitBox(reward.Key, reward.Value);
                    }
                }
                _userData.SaveUserData();
                popup.OnCloseEventListener += () =>
                {
                    UpdateUserInfo();
                };
                popup.OpenPopup();
            }
        }
    }
    #endregion Reward

    #region Account
    public void LogOut()
    {
        if (UserData.GetInstance.GetOptionData._optionType[(int)EOptionType.LOGINPAGE] == 1)
        {
            BackendFederationAuth backend = GameObject.Find("BackendManager").GetComponent<BackendFederationAuth>();
            backend.GoogleSignout();
        }
        UserData.GetInstance.OptionTypeSetting((int)EOptionType.LOGINPAGE, 0);
        SoundManager.GetInstance.DestroyBackgroundObj();
        SoundManager.GetInstance.DestroyEffectSoundObj();
        cSceneManager.GetInstance.ChangeScene("LoginPage", () => { });
    }
    #endregion Account

    #region Text
    /// <summary>
    /// 데이터를 입력받아 타입에 맞는 형식의 포맷으로 반환
    /// </summary>
    /// <param name="type">텍스트 타입</param>
    /// <param name="value01">값 1</param>
    /// <param name="value02">값 2</param>
    /// <returns></returns>
    string GetFormattedText(ETextType type, int value01, int value02)
    {
        string textFormat = "";
        switch (type)
        {
            case ETextType.EnergyText:
                textFormat = string.Format(ValueTextFormat, value01, value02);
                break;
            case ETextType.QuarterMinText:
                if (value02 < 1)
                {
                    value01 -= 1;
                    value02 = 60;
                }
                textFormat = string.Format(QuaterTextFormat,
                QuarterMinutes - value01, MaxSeconds - value02);
                break;
            case ETextType.HourMinText:
                textFormat = string.Format(HourMinTextFormat,
                     MaxHours - value01, MaxMinutes - value02);
                break;
            case ETextType.DayText:
                textFormat = string.Format(DayTextFormat,
                    MaxDays - value01);
                break;
            default:
                break;
        }
        return textFormat;
    }
    #endregion Text

    #region Alram
    /// <summary>
    /// 알람 세팅
    /// </summary>
    public void SetBadge(BadgeType type, bool state)
    {
        _badgeController.SetBadge(type, state);
    }
    #endregion Alram

    #region WaitBox
    /// <summary>
    /// 실시간 수령 랜덤 박스(대기열 추가)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void AddWaitBox(ERewardType type, int value, bool isImmediately = true)
    {
        if(isImmediately)
        {
            if (_waitBoxDictionary.ContainsKey(type))
            {
                _waitBoxDictionary[type] += value;
            }
            else
            {
                _waitBoxDictionary.Add(type, value);
            }
        }
        else
        {
            for(int i = 0; i < value; i++)
            {
                Action boxAction = () => _shopS.ProvideNotPurchasingReward(EPayType.NONE, type, 1, "", false);
                RegisterActionSchedule(boxAction);
            }
        }
    }
    public void SetRewardBoxCheck(bool state) => _checkRewardBox = state;
    #endregion WaitBox

    #region ActionSchedule
    public void RegisterActionSchedule(Action action)
    {
        _actionScheduleQueue.Enqueue(action);
    }

    public Action GetRegisteredAction()
    {
        if(_actionScheduleQueue.Count > 0)
        {
            return _actionScheduleQueue.Dequeue();
        }
        else
        {
            return null;
        }
    }
    #endregion ActionSchedule

    #region Debug
    public void OnTouchAddAllMaterial()
    {
        for (int i = 0; i < (int)EMaterialType.NONE; i++)
        {
            _userData.AddCurrency((EMaterialType)i, 100);
        }
        _userData.SaveUserData();
        UpdateUserInfo();
    }

    public void OnTouchResetQuest()
    {
        _userData.ResetDailyQuest();
        _userData.SaveQuestData(true);
    }

    public void OnTouchResetRoulette()
    {
        _timeCheck[(int)ETimeCheckType.DAILY_ROULETTE] = false;
        _onTimeEventList[(int)ETimeCheckType.DAILY_ROULETTE]?.Invoke();
        _userData.SaveRewardTime();
        _roulettePopup.ResetRoullet();
    }

    public void OnTouchResetShopDailyProduct()
    {
        _shopS.RewardInit(true);
    }

    public void OnTouchReadyAllClear()
    {
        _userData.SetDailyQuest(EDailyQuest.ALLCLEAR, 7);
        _userData.SetDailyQuestState(EDailyQuest.ALLCLEAR, false);
        _userData.SaveQuestData();
    }

    public void OnTouchStageClear()
    {
        _userData.StagePos = 29;
        _userData.GetQuestData.mainQuestList.Clear();
        for (int i = 1; i < _userData.StagePos + 1; ++i)
        {
            _userData.SetMainQuest(i, 3);
        }
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.STAGEDATA, true);
    }
    public void OnTouchItemCreateBtn(int grade)
    {
        CreateItemsForGrade(grade);
    }

    private void CreateItemsForGrade(int grade)
    {
        CreateItemsForType(EItemList.BOAT, grade);
        CreateItemsForType(EItemList.WEAPON, grade);
        CreateItemsForType(EItemList.DEFENSE, grade);
        CreateItemsForType(EItemList.CAPTAIN, grade);
        CreateItemsForType(EItemList.SAILOR, grade);
        CreateItemsForType(EItemList.ENGINE, grade);
    }

    private void CreateItemsForType(EItemList itemType, int grade)
    {
        int id = grade;
        for (int i = 0; i < 6; ++i)
        {
            _shopS.CreateItem(itemType, id);
            id += 11;
        }
        _userData.UnitTypeSave(itemType);
    }

    public void OnTouchLevelUp()
    {
        if(_userData.GetUserLevel < 100)
        {
            _userData.SetUserLevel(_userData.GetUserLevel + 1);
            UpdateUserInfo();
        }
    }
    #endregion Debug
}
