using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyData;

public enum EQuestType
{
    Daily = 0,
    Main,
    None,
}

public enum EDailyQuest
{
    LOGIN = 0,
    TRYSTAGE,
    KILLENEMY,
    UPGRADE,
    WATCHAD,
    BUYREWARD,
    ROULETTE,
    ALLCLEAR,
    SEVENCLEAR,
    NONE
}

public enum EMainQuest
{
    WAVE3 = 11,
    WAVE5 = 12,
    STAGECLEAR = 13,
    NONE
}

public class QuestPopup : PopupBase
{
    [Header("Components")]
    LobbyUIManager _uiManager;
    StageScript _stagePage;
    UserData _userData;

    [Header("Main")]
    [SerializeField] QuestScript _mainQuestBoard;

    [Header("Daily")]
    [SerializeField] QuestScript _dailyQuestBoard;
    [Space]
    [SerializeField] Button _allReceiveButton;
    bool _isAllReceive = false;

    const string mainIdFormat = "{0:D2}";
    const string mainTitleFormat = "Stage {0}";
    const string mainSubTitleFormat = "Main {0}";

    const string dailyTitleFormat = "Daily Quest";
    const string dailySubTitleFormat = "Daily {0}";

    bool _isWorking = false;

    public void Init(LobbyUIManager uiManager)
    {
        _uiManager = uiManager;
        _stagePage = _uiManager.GetStagePage;
        _userData = UserData.GetInstance;
        CheckDailyQuestSet();
    }

    /// <summary>
    /// 메인 퀘스트 (각 스테이지별 퀘스트 진행도 표기)
    /// </summary>
    /// <param name="stage">스테이지</param>
    public void SetMainQuest(int stage)
    {
        _mainQuestBoard.SetTitleText(string.Format(mainTitleFormat, stage));

        var dataManager = DataManager.GetInstance;
        QuestData[] questDataArr = new QuestData[3];
        List<Action> buttonActionList = new List<Action>();
        List<int> questIdList = new List<int>();
        // 데이터 로드 및 버튼 기능 설정 
        for (int i = 1; i < Enum.GetValues(typeof(EMainQuest)).Length; i++)
        {
            int id = int.Parse(stage.ToString() + string.Format(mainIdFormat, i));
            questIdList.Add(id);
            var data = dataManager.FindData(DataManager.KEY_QUEST, id) as QuestData;

            questDataArr[i - 1] = data;
            buttonActionList.Add(() =>
            {
                OnTouchReceiveButton(id);
                CheckDailyQuestAlram();
            });
        }
        _mainQuestBoard.SetButtonAction(buttonActionList);

        // 퀘스트 정보 설정
        List<string> questNameList = new List<string>();
        List<string> contextList = new List<string>();
        List<int> curCountList = new List<int>();
        List<int> maxCountList = new List<int>();

        for (int j = 0; j < Enum.GetValues(typeof(EMainQuest)).Length - 1; j++)
        {
            questNameList.Add(string.Format(mainSubTitleFormat, j + 1));
            contextList.Add(questDataArr[j].questName);
            maxCountList.Add(questDataArr[j].clearCount);
            if (stage <= _userData.StagePos + 1)
            {
                if(questDataArr[j].clearCount < _userData.GetMainQuest(stage))
                {
                    curCountList.Add(questDataArr[j].clearCount);
                }
                else
                {
                    curCountList.Add(_userData.GetMainQuest(stage));
                }
            }
        }

        // 미개방 지역인 경우 더미 데이터 처리
        if (curCountList.Count == 0)
        {
            List<bool> receiptedList = new List<bool>();
            for (int k = 0; k < 3; k++)
            {
                curCountList.Add(0);
                receiptedList.Add(false);
            }
            _mainQuestBoard.SetQuestInfo(questNameList, contextList, curCountList, maxCountList, receiptedList, questIdList);
        }
        else
        {
            _mainQuestBoard.SetQuestInfo(questNameList, contextList, curCountList, maxCountList, _userData.GetMainQuestState(stage), questIdList);
        }
        _isAllReceive = false;
        CheckMainQuestSet();
        CheckAllReceiveButton(EQuestType.Main);
        SwitchingBoardState(true);
    }

    /// <summary>
    /// 일일 퀘스트
    /// </summary>
    public void SetDailyQuest()
    {
        _dailyQuestBoard.SetTitleText(dailyTitleFormat);

        var dataManager = DataManager.GetInstance;
        var dailyList = _userData.GetDailyQuest();      // 내 퀘스트 기록

        // 데이터 로드 및 버튼 기능 설정 
        QuestData[] questDataArr = new QuestData[9];
        List<Action> buttonActionList = new List<Action>();
        for (int i = 1; i < Enum.GetValues(typeof(EDailyQuest)).Length; i++)
        {
            var data = dataManager.FindData(DataManager.KEY_QUEST, i) as QuestData;
            questDataArr[i - 1] = data;
            int btnNum = i;
            buttonActionList.Add(() =>
            {
                OnTouchReceiveButton(btnNum);
            });
        }
        _dailyQuestBoard.SetButtonAction(buttonActionList);

        // 퀘스트 정보 설정
        List<string> questNameList = new List<string>();
        List<string> contextList = new List<string>();
        List<int> curCountList = new List<int>();
        List<int> maxCountList = new List<int>();
        List<int> questIdList = new List<int>();
        for (int j = 0; j < (int)EDailyQuest.NONE; j++)
        {
            questNameList.Add(string.Format(dailySubTitleFormat, j + 1));
            contextList.Add(questDataArr[j].questName);
            maxCountList.Add(questDataArr[j].clearCount);
            curCountList.Add(dailyList[j]);
            questIdList.Add(j + 1);
        }
        _dailyQuestBoard.SetQuestInfo(questNameList, contextList, curCountList, maxCountList, _userData.GetDailyQuestState(), questIdList);
        _dailyQuestBoard.SortQuestList();

        _isAllReceive = false;
        CheckDailyQuestSet();
        CheckAllReceiveButton(EQuestType.Daily);
        SwitchingBoardState(false);
    }

    /// <summary>
    /// 메인 / 일일 스위칭
    /// </summary>
    /// <param name="isMain"></param>
    void SwitchingBoardState(bool isMain)
    {
        if (isMain)
        {
            _mainQuestBoard.OpenBoard();
            _dailyQuestBoard.CloseBoard();
        }
        else
        {
            _mainQuestBoard.CloseBoard();
            _dailyQuestBoard.OpenBoard();
        }
    }

    #region Check
    /// <summary>
    /// 일괄 수령 버튼 상태 체크
    /// </summary>
    public void CheckAllReceiveButton(EQuestType questType)
    {
        // 수령 가능한 보상이 있는 경우 일괄 버튼 활성화
        _allReceiveButton.interactable = false;
        QuestItem[] itemList = null;
        if (questType == EQuestType.Daily)
        {
            itemList = _dailyQuestBoard.GetQuestItemArr;

        }
        else if (questType == EQuestType.Main)
        {
            itemList = _mainQuestBoard.GetQuestItemArr;
        }

        if (itemList == null) return;

        for (int i = 0; i < itemList.Length; i++)
        {
            if (itemList[i].IsReceivable)
            {
                _allReceiveButton.interactable = true;
                break;
            }
        }
    }

    /// <summary>
    /// 메인 퀘스트 상태 확인 셋
    /// </summary>
    public void CheckMainQuestSet()
    {
        CheckMainQuestAlram(BadgeType.MainQuest, _stagePage.StageNumber);
        CheckMainQuestAlram(BadgeType.MainQuest_L, _stagePage.StageNumber - 1);
        CheckMainQuestAlram(BadgeType.MainQuest_R, _stagePage.StageNumber + 1);
    }

    /// <summary>
    /// 일일 퀘스트 상태 확인 셋
    /// </summary>
    public void CheckDailyQuestSet()
    {
        CheckAllClearQuest();
        CheckDailyQuestAlram();
    }

    /// <summary>
    /// 메인 퀘스트 알림 뱃지
    /// </summary>
    void CheckMainQuestAlram(BadgeType alramType, int stage)
    {
        bool alramState = false;
        if(stage <= 0 || stage > DataManager.GetInstance.GetList<StageData>(DataManager.KEY_STAGE).Count)
        {
            _uiManager.SetBadge(alramType, alramState);
            return;
        }
        if (stage <= _userData.StagePos + 1)
        {
            var questArr = GetMainQuestInfoData().ToArray();
            int myQuestData = _userData.GetMainQuest(stage);
            var receivedList = _userData.GetMainQuestState(stage);

            for (int i = 0; i < questArr.Length; i++)
            {
                if (myQuestData >= questArr[i].clearCount && !receivedList[i])
                {
                    alramState = true;
                    break;
                }
            }
        }
        _uiManager.SetBadge(alramType, alramState);
    }

    /// <summary>
    /// 보상 받기 가능한 퀘스트 여부 체크 (알림)
    /// </summary>
    void CheckDailyQuestAlram()
    {
        bool alramState = false;
        
        var questArr = GetDailyQuestInfoData().ToArray();        // 퀘스트 정보 데이터
        var myQuestData = _userData.GetDailyQuest();             // 내 퀘스트 진행도
        for (int i = 0; i < questArr.Length; i++)
        {
            if (myQuestData[i] >= questArr[i].clearCount && !_userData.GetDailyQuestState()[i])
            {
                alramState = true;
                break;
            }
        }
        _uiManager.SetBadge(BadgeType.DailyQuest, alramState);
    }

    /// <summary>
    /// 일일 퀘스트 모두 클리어 했는지 체크(AllClear 체크)
    /// </summary>
    public void CheckAllClearQuest()
    {
        var questArr = GetDailyQuestInfoData().ToArray();
        var myQuestData = _userData.GetDailyQuest();
        var receivedList = _userData.GetDailyQuestState();

        int checkCount = 0;
        for (int i = 0; i < (int)EDailyQuest.ALLCLEAR; i++)
        {
            if (receivedList[i])
            {
                checkCount++;
            }
            else
            {
                if (questArr[i].clearCount <= myQuestData[i])
                {
                    checkCount++;
                }
            }
        }
        _userData.SetDailyQuest(EDailyQuest.ALLCLEAR, checkCount);
        _userData.SaveQuestData();
    }

    /// <summary>
    /// AllClear 7회 카운트 체크
    /// </summary>
    void CheckSevenClearQuest()
    {
        if (_userData.GetDailyQuestState()[(int)EDailyQuest.ALLCLEAR])
        {
            _userData.SetDailyQuest(EDailyQuest.SEVENCLEAR, _userData.GetDailyQuest()[(int)EDailyQuest.SEVENCLEAR] + 1);
            _userData.SaveQuestData();

            SetDailyQuest();
        }
    }
    #endregion Check

    #region Data
    /// <summary>
    /// 퀘스트 정보 데이터 로드 
    /// </summary>
    /// <returns></returns>
    List<QuestData> GetDailyQuestInfoData()
    {
        List<QuestData> questData = new List<QuestData>();
        var dataManager = DataManager.GetInstance;
        for (int i = 1; i < Enum.GetValues(typeof(EDailyQuest)).Length; i++)
        {
            var data = dataManager.FindData(DataManager.KEY_QUEST, i) as QuestData;
            questData.Add(data);
        }
        return questData;
    }

    List<QuestData> GetMainQuestInfoData()
    {
        List<QuestData> questData = new List<QuestData>();
        var dataManager = DataManager.GetInstance;
        for(int i = 1; i <= 3; i++)
        {
            string key01 = _stagePage.StageNumber.ToString();
            string key02 = string.Format(mainIdFormat, i);
            var data = dataManager.FindData(DataManager.KEY_QUEST, int.Parse(key01 + key02)) as QuestData;
            questData.Add(data);
        }
        return questData;
    }
    #endregion Data

    #region Buttons
    /// <summary>
    /// 메인 퀘스트 버튼
    /// </summary>
    public void OnTouchOpenMainQuest()
    {
        SetMainQuest(_stagePage.StageNumber);
        OpenPopup();
    }

    /// <summary>
    /// 일일 퀘스트 버튼
    /// </summary>
    public void OnTouchOpenDailyQuest()
    {
        // 1. 일일 퀘스트 정보 UserData에서 조회
        // 2. 진행도 수령 상황에 조회
        SetDailyQuest();
        OpenPopup();
    }

    public void OnTouchReceiveButton(int id)
    {
        if (_isWorking) return;
        _isWorking = true;
        if (id < 100)
        {
            // 일일 퀘스트 완료
            EDailyQuest questType = (EDailyQuest)(id - 1);
            _userData.SetDailyQuestState(questType, true);
            if (questType == EDailyQuest.ALLCLEAR)
            {
                CheckSevenClearQuest();
            }
            else
            {
                CheckDailyQuestSet();
                CheckAllReceiveButton(EQuestType.Daily);
            }
            _dailyQuestBoard.SortQuestList();
            if (UserData.GetInstance.GetServer) { GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.UTILITYDATA); } // GameManager.GetInstance.GetServerSaveListAdd(EUserSaveType.QUESTDATA, 0);
        }
        else
        {
            // 메인 퀘스트 완료
            int idx = (id % 100) - 1;
            idx += (int)EMainQuest.WAVE3;
            _userData.SetMainQuestState(_stagePage.StageNumber, (EMainQuest)idx, true);
            CheckMainQuestSet();
            CheckAllReceiveButton(EQuestType.Main);
            _mainQuestBoard.SortQuestList();
            if (UserData.GetInstance.GetServer) { GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.UTILITYDATA); } // GameManager.GetInstance.GetServerSaveListAdd(EUserSaveType.QUESTDATA, 1); 
        }

        var data = DataManager.GetInstance.FindData(DataManager.KEY_QUEST, id) as QuestData;
        ERewardType rType = (ERewardType)data.rewardType;

        int rValue = data.rewardValue;

        var shop = _uiManager.GetShopPage;

        if(rType < ERewardType.NormalBox)
        {
            shop.ProvideNotPurchasingReward(EPayType.NONE, rType, rValue, "퀘스트 보상", false, _isAllReceive);
        }
        else
        {
            _uiManager.AddWaitBox(rType, rValue);
        }
        _isWorking = false;
    }

    /// <summary>
    /// 일괄 받기 버튼
    /// </summary>
    public void OnTouchAllReceiveButton()
    {
        if (_isAllReceive)
        {
            return;
        }
        _isAllReceive = true;
        List<Action> clearActionList = new List<Action>();
        QuestItem[] questArr= null;
        int noneBoxCount = 0;
        if (_dailyQuestBoard.gameObject.activeSelf)
        {
            questArr = _dailyQuestBoard.GetQuestItemArr;
        }
        else
        {
            questArr = _mainQuestBoard.GetQuestItemArr;
        }
        int[] boxCountArr = new int[2];
        for (int i = 0; i < questArr.Length; i++)
        {
            if (questArr[i].IsReceivable)
            {
                int idx = i;
                clearActionList.Add(() =>
                {
                    if(questArr[idx].QuestRewardType < ERewardType.NormalBox)
                    {
                        noneBoxCount++;
                        questArr[idx].ClearQuest();
                    }
                    else
                    {
                        int boxIdx = (int)questArr[idx].QuestRewardType - (int)ERewardType.NormalBox;
                        boxCountArr[boxIdx]++;
                        _uiManager.SetRewardBoxCheck(false);
                        questArr[idx].ClearQuest();
                    }
                });
            }
        }

        for (int j = 0; j < clearActionList.Count; j++)
        {
            clearActionList[j]?.Invoke();
        }

        // 랜덤 박스 팝업에 등록
        RewardResultPopup popup = LobbyUIManager.GetInstance.GetPopup<RewardResultPopup>(PopupType.REWARDRESULT);
        for (int n = 0; n < boxCountArr.Length; n++)
        {
            if (boxCountArr[n] <= 0) continue;
            ERewardType rType = ERewardType.NormalBox + n;
            popup.SetPopup("Reward", rType, boxCountArr[n]);
        }

        _isAllReceive = false;
        if (noneBoxCount > 0)
        {
            _uiManager.OpenRewardResultPopup(0);
        }
        StartCoroutine(DelayRewardBoxCheck(0.2f));
    }

    IEnumerator DelayRewardBoxCheck(float waitTime)
    {
        yield return YieldInstructionCache.WaitForSeconds(waitTime);
        _uiManager.SetRewardBoxCheck(true);
    }
    #endregion Buttons
}
