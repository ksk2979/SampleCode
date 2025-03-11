using MyData;
using MyStructData;
using System.Collections;
using System.Collections.Generic;
using UniRx.Async.Triggers;
using UnityEngine;

public class StageScript : PageBase
{
    [SerializeField] NestedScrollManager _stageSelect;
    [SerializeField] GameObject _stageIconPrefab;
    List<StageIcon> _stageList;

    const string StageNameFormat = "Stage_{0}";
    const string FireBaseTryFormat = "Stage_Start_{0}";
    string _stageName;

    public override void Init(LobbyUIManager uiM)
    {
        base.Init(uiM);
        CreateStageIcon();
        InitScrollSetting();
    }

    #region Content Setting
    void CreateStageIcon()
    {
        _stageList = new List<StageIcon>();
        var dataList = DataManager.GetInstance.GetList<StageData>(DataManager.KEY_STAGE);
        for (int i = 0; i < dataList.Count; i++)
        {
            Sprite iconSprite = Resources.Load<Sprite>(string.Format("ItemIcon/Stage{0:D2}", dataList[i].nId));
            string iconText = string.Format("Stage {0:D2}", dataList[i].nId);

            GameObject stageIconObj = Instantiate(_stageIconPrefab, _stageSelect._rtContent);
            StageIcon stage = stageIconObj.GetComponent<StageIcon>();
            stage.SetIcon(iconSprite, iconText);
            stage.SetButtonAction(ReadyForStart);
            stage.SetBlurState(true);
            _stageList.Add(stage);
        }
    }

    void InitScrollSetting()
    {
        _stageSelect.Init();

        int realSize = 0;
        int showStage = 0;
        QuestPopup questPopup = uiManager.GetPopup<QuestPopup>(PopupType.QUEST);

        if (!userData.StagePosMaxCheck())
        {
            realSize = userData.StagePos + 1;
            showStage = userData.StagePos;
            if (GameManager.GetInstance.GetFirstCleared && showStage > 0)
            {
                showStage -= 1;
                _stageSelect.OnStageEventListener += () =>
                {
                    questPopup.OnCloseEventListener += () => { _stageSelect.ArrowBtn(false); };
                };
            }
            else if(GameManager.GetInstance.GetPrevStage != -1)
            {
                showStage = GameManager.GetInstance.GetPrevStage;
                GameManager.GetInstance.GetPrevStage = -1;
            }
        }
        else
        {
            realSize = userData.StagePos;
            showStage = userData.StagePos - 1;
        }

        _stageSelect.ImmediatelyStagePos(showStage);
        for (int i = 0; i < realSize; i++)
        {
            _stageList[i].SetBlurState(false);
        }

        // 첫 클리어가 되는 경우 
        if (GameManager.GetInstance.GetFirstCleared)
        {
            bool checkInterceptA = false;
            bool checkInterceptB = false;
            // 도감 튜토리얼 있는 경우
            if (UserData.GetInstance.TutorialCheck() == 100)
            {
                checkInterceptA = true;
            }
            // 레벨업 보상
            if(GameManager.GetInstance.GetPrevLevel != userData.GetUserLevel)
            {
                checkInterceptB = true;
                if(checkInterceptA)
                {
                    uiManager.RegisterActionSchedule(() =>
                    {
                        uiManager.CheckLevelUpReward();
                    });
                }
                else
                {
                    uiManager.CheckLevelUpReward();
                }
            }
            // 퀘스트 팝업
            if (checkInterceptA || checkInterceptB)
            {
                uiManager.RegisterActionSchedule(() =>
                {
                    questPopup.OnTouchOpenMainQuest();
                    GameManager.GetInstance.GetFirstCleared = false;
                });
            }
            else
            {
                questPopup.OnTouchOpenMainQuest();
                GameManager.GetInstance.GetFirstCleared = false;
            }
        }
        else
        {
            // 첫 클리어(퀘 보상 없는 경우 )
            // 레벨업
            if (GameManager.GetInstance.GetPrevLevel != userData.GetUserLevel)
            {
                uiManager.CheckLevelUpReward();
            }
        }
    }
    #endregion Content Setting

    #region Start Game
    /// <summary>
    /// 로비의 스타트 버튼을 누를 시 준비 페이지 열림
    /// </summary>
    public void ReadyForStart()
    {
        if (userData.StagePos < _stageSelect.TargetIndex) { MessageHandler.GetInstance.ShowMessage("전 스테이지를 클리어 이후 진입 가능합니다", 1.5f); return; }
        if (!uiManager.ActionCheck()) { return; }

        var readyPage = uiManager.GetReadyPage;
        readyPage.SetTitleTMP(StageNumber);
        readyPage.OpenPage();

        if (_stageSelect.TargetIndex < DataManager.GetInstance.GetList<StageData>(DataManager.KEY_STAGE).Count)
        {
            GameManager.GetInstance.GetNowStageNumber = _stageSelect.TargetIndex;
            _stageName = string.Format(StageNameFormat, StageTargetIndex(_stageSelect.TargetIndex));
        }
        else { _stageName = "Stage_9"; }
    }

    int StageTargetIndex(int index)
    {
        if (index >= 0 && index <= 2) { return 0; }
        else if (index >= 3 && index <= 5) { return 1; }
        else if (index >= 6 && index <= 8) { return 2; }
        else if (index >= 9 && index <= 11) { return 3; }
        else if (index >= 12 && index <= 14) { return 4; }
        else if (index >= 15 && index <= 17) { return 5; }
        else if (index >= 18 && index <= 20) { return 6; }
        else if (index >= 21 && index <= 23) { return 7; }
        else if (index >= 24 && index <= 26) { return 8; }
        else if (index >= 27 && index <= 29) { return 9; }
        return 9;
    }

    /// <summary>
    /// 게임 시작에 예외상황 체크
    /// </summary>
    public bool CheckException()
    {
        if (userData.ChooseBoatTemp[(int)EUnitPosition.MIDDLE] == null) { MessageHandler.GetInstance.ShowMessage("메인보트가 없습니다", 1.5f); return true; }

        if (userData.TutorialCheck() == 1 || userData.TutorialCheck() == 2) { TutorialManager.GetInstance.GameTutorialStartBtn(); return true; }

        return false;
    }


    /// <summary>
    /// 팀 선택 팝업에서 게임 시작을 누르고 게임 시작 될 때
    /// </summary>
    public void GameStart()
    {
        GameManager.GetInstance.GetPrevLevel = userData.GetUserLevel;
        userData.SetAdAbilityCheck(uiManager.GetReadyPage.ADToggleState);
        userData.SetDiaAbilityCheck(uiManager.GetReadyPage.DiaToggleState);

        // 어빌리티 추가
        if (userData.CheckAdAbility)
        {
            ADManager.GetInstance.ShowRewardedVideo(
                () =>
                {
                    FirebaseManager.GetInstance.LogEvent("AbilityBonus_AD");    // 보상 보너스 AD
                    userData.SetAdAbilityReserved(true);
                    GameStartFunc();
                },
                () =>
                {
                    userData.SetAdAbilityReserved(false);
                    uiManager.GetReadyPage.DisableADToggle();
                },
                ADManager.PLACEMENTKEY_ABILITY);
        }
        else if (userData.CheckDiaAbility)
        {
            if (userData.GetCurrency(EPropertyType.DIAMOND) < 50)
            {
                uiManager.GetShopPage.ShowMessageNotEnoughDia();
                userData.SetDiaAbilityReserved(false);
                uiManager.GetReadyPage.DisableDiaToggle();
            }
            else
            {
                FirebaseManager.GetInstance.LogEvent("AbilityBonus_DIA");    // 보상 보너스 DIAMOND
                userData.SubCurrency(EPropertyType.DIAMOND, 50);
                userData.SetDiaAbilityReserved(true);
                GameStartFunc();
            }
        }
        else
        {
            GameStartFunc();
        }

        userData.SaveUnitSelect();
    }

    /// <summary>
    /// 게임 시작 기능 메서드
    /// </summary>
    void GameStartFunc()
    {
        userData.UnitInfo.Clear();
        var data = userData.GetSelectData;
        for (int i = 0; i < data._boatSPId.Count; ++i) { data._boatSPId[i] = 0; } // 초기화
        for (int i = 0; i < userData.ChooseBoatTemp.Length; ++i)
        {
            if (userData.ChooseBoatTemp[i] != null)
            {
                StartPlayerInfoSetting(i);
            }
            else
            {
                userData.UnitInfo.Add(null);
            }
        }

        FirebaseManager.GetInstance.LogEvent(string.Format(FireBaseTryFormat, StageNumber));    // 게임시작 FB Log
        FirebaseManager.GetInstance.LogEvent(_stageName);
        uiManager.UseEnergy(); // 행동력 감소
        if (userData.GetDailyQuest()[(int)EDailyQuest.TRYSTAGE] <= 0)
        {
            userData.SetDailyQuest(EDailyQuest.TRYSTAGE, 1);
            userData.SaveQuestData();
        }
        uiManager.LoadingStage(_stageName);
        //PlayerPrefs.SetString(GameManager.SAVE_STAGENAME, _stageName);
        //PlayerPrefs.SetInt(GameManager.SAVE_START, 1);
        //PlayerPrefs.SetInt(GameManager.SAVE_STAGENUMBER, _stageSelect.TargetIndex);
        //PlayerPrefs.SetInt(GameManager.SAVE_ABILITY_COUNT, 0);
        //PlayerPrefs.SetInt(GameManager.SAVE_STAGE_MAP, -1);
        //PlayerPrefs.SetInt(GameManager.SAVE_RETRY, 0);
        //PlayerPrefs.SetInt(GameManager.SAVE_WAVE, 1);
    }

    /// <summary>
    /// 플레이어 정보 저장(배 정보)
    /// </summary>
    public void StartPlayerInfoSetting(int i)
    {
        int[] arr = userData.FindCoalescence(userData.ChooseBoatTemp[i].GetIngerenceID);
        var poten = userData.FindPotential(userData.ChooseBoatTemp[i].GetIngerenceID);
        // 여기서 playerinfo를 수정한다
        // 전체적으로 가져온 데이터정보로 업데이트 한다
        userData.ChooseBoatTemp[i]._playerInfo.PlayerInfoUpdate(arr); // 일단 착용된 정보 업데이트 해주기
        if (arr[0] != 0)
        {
            var nowC = MyData.DataManager.GetInstance.GetData(MyData.DataManager.KEY_BOAT, arr[0], 1) as MyData.BoatData;
            userData.ChooseBoatTemp[i]._playerInfo.SetEquipValue(EItemList.BOAT, nowC.addHp);
            userData.ChooseBoatTemp[i]._playerInfo.SetEquipBoatValue(nowC.addDamage);
        }
        if (arr[2] != 0)
        {
            var nowC = MyData.DataManager.GetInstance.GetData(MyData.DataManager.KEY_WEAPON, arr[2], 1) as MyData.WeaponData;
            userData.ChooseBoatTemp[i]._playerInfo.SetEquipValue(EItemList.WEAPON, nowC.addDamage);
        }
        if (arr[4] != 0)
        {
            var nowC = MyData.DataManager.GetInstance.GetData(MyData.DataManager.KEY_DEFENSE, arr[4], 1) as MyData.DefenseData;
            userData.ChooseBoatTemp[i]._playerInfo.SetEquipValue(EItemList.DEFENSE, nowC.addValue);
        }
        if (arr[6] != 0)
        {
            var nowC = MyData.DataManager.GetInstance.GetData(MyData.DataManager.KEY_CAPTAIN, arr[6], 1) as MyData.CaptainData;
            userData.ChooseBoatTemp[i]._playerInfo.SetEquipValue(EItemList.CAPTAIN, nowC.addDamage);
            userData.ChooseBoatTemp[i]._playerInfo.ChangePotential(EItemList.CAPTAIN, poten[(int)EItemList.CAPTAIN - 3]);
        }
        if (arr[8] != 0)
        {
            var nowC = MyData.DataManager.GetInstance.GetData(MyData.DataManager.KEY_SAILOR, arr[8], 1) as MyData.SailorData;
            userData.ChooseBoatTemp[i]._playerInfo.SetEquipValue(EItemList.SAILOR, nowC.addValue);
            userData.ChooseBoatTemp[i]._playerInfo.ChangePotential(EItemList.SAILOR, poten[(int)EItemList.SAILOR - 3]);
        }
        if (arr[10] != 0)
        {
            var nowC = MyData.DataManager.GetInstance.GetData(MyData.DataManager.KEY_ENGINE, arr[10], 1) as MyData.EngineData;
            userData.ChooseBoatTemp[i]._playerInfo.SetEquipValue(EItemList.ENGINE, nowC.addValue);
            userData.ChooseBoatTemp[i]._playerInfo.ChangePotential(EItemList.ENGINE, poten[(int)EItemList.ENGINE - 3]);
        }
        var data = userData.GetSelectData;
        userData.UnitInfo.Add(userData.ChooseBoatTemp[i]._playerInfo);
        data._boatSPId[i] = userData.ChooseBoatTemp[i].GetIngerenceID;
    }
    #endregion Start Game

    public override void ClosePage()
    {
        uiManager.GetSeasonPassPanel.CloseSeasonPassPanel();
        base.ClosePage();
    }

    public NestedScrollManager StageScroll => _stageSelect;
    public int StageNumber => _stageSelect.TargetIndex + 1;
}
