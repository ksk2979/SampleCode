using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using MyData;
using Info;
using MyStructData;
using JetBrains.Annotations;
using System.Linq;
using AirFishLab.ScrollingList.Demo;

/*
유저가 저장될 데이터
재화, 재료
가지고 유닛 현황
유닛 선택 사항
현재 어느 스테이지 인가?
 */

public partial class UserData : Singleton<UserData>
{
    // 유저가 선택한 유닛저장 데이터 (인게임용)
    List<PlayerInfo> _unitInfo = new List<PlayerInfo>();
    int _boatPos;
    List<int> _unitPosition = new List<int>();

    //int _stagePos;

    UnitIcon _boatTemp = null;
    UnitIcon[] _chooseBoatTemp;

    // 업그레이드한 재능 체크
    int _userLevel, _userExp; // utility 세이브 파일

    // version 버전 체크
    int _version;

    // 최고 스테이지
    int _maxStageNumber = 30;

    // 서버&로컬 데이터
    STUserData _stUserData;
    STMaterialData _stMaterialData;
    STUnitData _stUnitData;
    STUnitLevelData _stUnitLevelData;
    STUnitPotentialData _stUnitPotentialData;
    STStageData _stStageData;
    STUnitIDCount _stUnitIDCount;
    STUtilityData _stUtilityData;
    STVersionData _stVersionData;
    STSingleUseProductData _stRewardData;
    STCoalescenceData _stCoalescenceData;
    STCollectionData _stCollectionData;
    STTutorialData _stTutorialData;
    STQuestData _stQuestData;
    STSeasonPassData _stSeasonPassData;

    // 로컬 데이터
    STUnitSelectData _stSelectUnitData;
    STRewardTimeData _stRewardTimeData;
    STOptionData _stOptionData;
    STDailyRewardData _stDailyRewardData;
    STServerSaveData _stServerSaveData;

    bool _server = false;
    BackendGameInfo _serverGameInfo;

    public delegate void UserDataEventDelegate();
    Dictionary<EUserSaveType, UserDataEventDelegate> _saveDataDictionary;       // 세이브용 딜리게이트
    Dictionary<EUserSaveType, UserDataEventDelegate> _initDataDictionary;       // 초기화용 딜리게이트
    Dictionary<EUserSaveType, string> _dataPathDictionary;                      // 데이터 파일경로 사전
    Dictionary<EItemList, Action<int, int>> _unitTypeAddActions;
    Dictionary<EItemList, Action<string>> _unitPotentialAddActions;

    protected override void Awake()
    {
        base.Awake();
        _saveDataDictionary = new Dictionary<EUserSaveType, UserDataEventDelegate>();
        _initDataDictionary = new Dictionary<EUserSaveType, UserDataEventDelegate>();
        _dataPathDictionary = new Dictionary<EUserSaveType, string>();

        for (int i = 0; i < (int)EUserSaveType.NONE; i++)
        {
            InitDataFunc((EUserSaveType)i);
        }
        AllLoadData();
        Init();
    }

    // 서버 세이브용
    private Dictionary<(EUserSaveType, int), Action> _saveFunctions = new Dictionary<(EUserSaveType, int), Action>();

    // 초반에 저장된 데이터를 여기로 넣어주는 작업
    public void Init()
    {
        _unitInfo.Clear();
        _unitPosition.Clear();
        // 유틸리티 불러오기
        _userLevel = _stUtilityData._userLevel;
        _userExp = _stUtilityData._userExp;

        // 스테이지 불러오기
        //_stagePos = _stStageData._stagePos;
        UnitDataAddTypeInit();
        UnitPotentialAddInit();
    }
    public void BackendServerInit()
    {
        if (!_server) { return; }
        _serverGameInfo = GameObject.Find("BackendManager").GetComponent<BackendGameInfo>();
        InitializeServerSaveFunctions();
    }

    /// <summary>
    /// 데이터 구조체가 수행할 기능들을 초기화
    /// </summary>
    /// <param name="type"></param>
    public void InitDataFunc(EUserSaveType type)
    {
        if (type == EUserSaveType.NONE) return;

        string key = "";
        string path = "";
        switch (type)
        {
            case EUserSaveType.USERDATA:
                {
                    key = _stUserData.SaveType.ToString();
                    path = _stUserData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stUserData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stUserData, path);
                    }));
                }
                break;
            case EUserSaveType.MATERIALDATA:
                {
                    key = _stMaterialData.SaveType.ToString();
                    path = _stMaterialData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stMaterialData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stMaterialData, path);
                    }));
                }
                break;
            case EUserSaveType.UNITDATA:
                {
                    key = _stUnitData.SaveType.ToString();
                    path = _stUnitData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stUnitData.Init();
                        // Test
                        //_stUnitData._weapon.Add(1); // 대포
                        //_stUnitData._weapon.Add(23); // 곡사포
                        //_stUnitData._weapon.Add(34); // 레이저
                        //_stUnitData._weapon.Add(45); // 지뢰
                        //_stUnitData._weapon.Add(56); // 어뢰
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stUnitData, path);
                    }));
                }
                break;
            case EUserSaveType.UNITLEVELDATA:
                {
                    key = _stUnitLevelData.SaveType.ToString();
                    path = _stUnitLevelData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stUnitLevelData.Init();
                        // Test
                        //_stUnitLevelData._weaponLevel.Add(1); // 대포
                        //_stUnitLevelData._weaponLevel.Add(1); // 곡사포
                        //_stUnitLevelData._weaponLevel.Add(1); // 레이저
                        //_stUnitLevelData._weaponLevel.Add(1); // 지뢰
                        //_stUnitLevelData._weaponLevel.Add(1); // 어뢰
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stUnitLevelData, path);
                    }));
                }
                break;
            case EUserSaveType.UNITPOTENTIALDATA:
                {
                    key = _stUnitPotentialData.SaveType.ToString();
                    path = _stUnitPotentialData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stUnitPotentialData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stUnitPotentialData, path);
                    }));
                }
                break;
            case EUserSaveType.STAGEDATA:
                {
                    key = _stStageData.SaveType.ToString();
                    path = _stStageData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stStageData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stStageData, path);
                    }));
                }
                break;
            case EUserSaveType.UNITCOUNTID:
                {
                    key = _stUnitIDCount.SaveType.ToString();
                    path = _stUnitIDCount.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stUnitIDCount.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stUnitIDCount, path);
                    }));
                }
                break;
            case EUserSaveType.SELECTDATA:
                {
                    key = _stSelectUnitData.SaveType.ToString();
                    path = _stSelectUnitData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stSelectUnitData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stSelectUnitData, path);
                    }));
                }
                break;
            case EUserSaveType.COALESCENECEDATA:
                {
                    key = _stCoalescenceData.SaveType.ToString();
                    path = _stCoalescenceData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stCoalescenceData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stCoalescenceData, path);
                    }));
                }
                break;
            case EUserSaveType.UTILITYDATA:
                {
                    key = _stUtilityData.SaveType.ToString();
                    path = _stUtilityData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stUtilityData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stUtilityData, path);
                    }));
                }
                break;
            case EUserSaveType.VERSIONDATA:
                {
                    key = _stVersionData.SaveType.ToString();
                    path = _stVersionData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stVersionData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stVersionData, path);
                    }));
                }
                break;
            case EUserSaveType.TUTORIALDATA:
                {
                    key = _stTutorialData.SaveType.ToString();
                    path = _stTutorialData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stTutorialData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stTutorialData, path);
                    }));
                }
                break;
            case EUserSaveType.PACKAGEDATA:
                {
                    key = _stRewardData.SaveType.ToString();
                    path = _stRewardData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stRewardData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stRewardData, path);
                    }));
                }
                break;
            case EUserSaveType.DAILYREWARDDATA:
                {
                    key = _stDailyRewardData.SaveType.ToString();
                    path = _stDailyRewardData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stDailyRewardData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stDailyRewardData, path);
                    }));
                }
                break;
            case EUserSaveType.REWARDTIMEDATA:
                {
                    key = _stRewardTimeData.SaveType.ToString();
                    path = _stRewardTimeData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stRewardTimeData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stRewardTimeData, path);
                    }));
                }
                break;
            case EUserSaveType.QUESTDATA:
                {
                    key = _stQuestData.SaveType.ToString();
                    path = _stQuestData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stQuestData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stQuestData, path);
                    }));
                }
                break;
            case EUserSaveType.COLLECTIONDATA:
                {
                    key = _stCollectionData.SaveType.ToString();
                    path = _stCollectionData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stCollectionData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stCollectionData, path);
                    }));
                }
                break;
            case EUserSaveType.OPTIONDATA:
                {
                    key = _stOptionData.SaveType.ToString();
                    path = _stOptionData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stOptionData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stOptionData, path);
                    }));
                }
                break;
            case EUserSaveType.SEASONPASSDATA:
                {
                    key = _stSeasonPassData.SaveType.ToString();
                    path = _stSeasonPassData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stSeasonPassData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stSeasonPassData, path);
                    }));
                }
                break;
            case EUserSaveType.SERVERSAVEDATA:
                {
                    key = _stServerSaveData.SaveType.ToString();
                    path = _stServerSaveData.FilePath;
                    _initDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        _stServerSaveData.Init();
                    }));
                    _saveDataDictionary.Add(type, new UserDataEventDelegate(() =>
                    {
                        ES3.Save(key, _stServerSaveData, path);
                    }));
                }
                break;
        }
        _dataPathDictionary.Add(type, path);
    }

    private void InitializeServerSaveFunctions()
    {
        // USERDATA
        _saveFunctions[(EUserSaveType.USERDATA, 0)] = () =>
        {
            string propertyStr = string.Join(",", _stUserData._property);
            string materialStr = string.Join(",", _stMaterialData._material);
            string boatStr = string.Join(",", _stUnitData._boat);
            string weaponStr = string.Join(",", _stUnitData._weapon);
            string defenseStr = string.Join(",", _stUnitData._defense);
            string captainStr = string.Join(",", _stUnitData._captain);
            string sailorStr = string.Join(",", _stUnitData._sailor);
            string engineStr = string.Join(",", _stUnitData._engine);
            string boatLevelStr = string.Join(",", _stUnitLevelData._boatLevel);
            string weaponLevelStr = string.Join(",", _stUnitLevelData._weaponLevel);
            string defenseLevelStr = string.Join(",", _stUnitLevelData._defenseLevel);
            string captainLevelStr = string.Join(",", _stUnitLevelData._captainLevel);
            string sailorLevelStr = string.Join(",", _stUnitLevelData._sailorLevel);
            string engineLevelStr = string.Join(",", _stUnitLevelData._engineLevel);
            string capPotentialStr = string.Join("/", _stUnitPotentialData._capPotentialID);
            string salPotentialStr = string.Join("/", _stUnitPotentialData._salPotentialID);
            string engPotentialStr = string.Join("/", _stUnitPotentialData._engPotentialID);
            string stageStr = _stStageData._stagePos.ToString();
            string boatIDCountStr = string.Join(",", _stUnitIDCount._id);
            string boatNumIDStr = string.Join(",", _stCoalescenceData._boatNumID);
            string coalescenceStr = string.Join("/", _stCoalescenceData._coalescence);
            string potentialsStr = string.Join("/", _stCoalescenceData._potentials);
            int[] utility = { _stUtilityData._userLevel, _stUtilityData._userExp };
            string utilityStr = string.Join(",", utility);

            string combinedStr = string.Join("[",
           propertyStr, materialStr, boatStr, weaponStr, defenseStr, captainStr, sailorStr, engineStr,
           boatLevelStr, weaponLevelStr, defenseLevelStr, captainLevelStr, sailorLevelStr, engineLevelStr,
           capPotentialStr, salPotentialStr, engPotentialStr, stageStr, boatIDCountStr, boatNumIDStr, coalescenceStr,
           potentialsStr, utilityStr);

            _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_UserData, BackendGameInfo.Property_Data, combinedStr);
        };

        // VERSIONDATA
        _saveFunctions[(EUserSaveType.VERSIONDATA, 0)] = () =>
            _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_VersionData, BackendGameInfo.Version_Data, _stVersionData._version);

        _saveFunctions[(EUserSaveType.UTILITYDATA, 0)] = () =>
        {
            // 일일 보상 데이터
            string shopItemOneStr = string.Join(",", _stDailyRewardData._shopItemOneKey);
            string shopItemTwoStr = string.Join(",", _stDailyRewardData._shopItemTwoKey);
            string shopItemReceiveStr = string.Join(",", _stDailyRewardData._shopReceiveCount);
            string loginRewardCountStr = _stDailyRewardData._loginRewardCount.ToString();
            string rouletteCountStr = _stDailyRewardData._rouletteCount.ToString();

            // 타임 보상 데이터
            string rewardStr = string.Join(",", _stRewardTimeData._stTimeArr);

            // 도감 데이터
            string collectionStr = string.Join(",", _stCollectionData._collectionList);

            // 퀘스트 데이터
            string dailyQuestStr = string.Join(",", _stQuestData.dailyQuestList);
            string dailyReceivedStr = string.Join(",", _stQuestData.dailyReceivedList);
            string mainQuestStr = string.Join(",", _stQuestData.mainQuestList);
            string mainReceivedStr = string.Join(",", _stQuestData.mainReceivedList);
            string specialQuestStr = string.Join(",", _stQuestData.specialQuestList);

            // 튜토리얼 데이터
            string tutorialStr = _stTutorialData._tutorial.ToString();

            // 시즌 패스 데이터
            int[] seasonIndexArr = { _stSeasonPassData._currentSeason, _stSeasonPassData._level, _stSeasonPassData._progressCount };
            bool[] seasonBoolenArr = { _stSeasonPassData._isVipActivated, _stSeasonPassData._isActivated };
            string seasonIndexStr = string.Join(",", seasonIndexArr);
            string seasonBoolenStr = string.Join(",", seasonBoolenArr);
            string normalSeasonPassStr = string.Join(",", _stSeasonPassData._normalRewardStateList);
            string vipSeasonPassStr = string.Join(",", _stSeasonPassData._vipRewardStateList);

            // 모든 데이터를 하나로 결합
            string combinedStr = string.Join("[",
                shopItemOneStr, shopItemTwoStr, shopItemReceiveStr, loginRewardCountStr, rouletteCountStr,
                rewardStr, collectionStr,
                dailyQuestStr, dailyReceivedStr, mainQuestStr, mainReceivedStr, specialQuestStr,
                tutorialStr,
                seasonIndexStr, seasonBoolenStr, normalSeasonPassStr, vipSeasonPassStr);

            // 서버에 업데이트
            _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_UtilityData, BackendGameInfo.Utility_Data, combinedStr);
        };

        //// STAGEDATA
        //_saveFunctions[(EUserSaveType.STAGEDATA, 0)] = () =>
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_StageData, BackendGameInfo.Stage_Data, _stStageData._stagePos);
        //
        //// DAILYREWARDDATA
        //_saveFunctions[(EUserSaveType.DAILYREWARDDATA, 0)] = () =>
        //{
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_DailyRewardData, BackendGameInfo.ShopItemOneKey_Data, string.Join(",", _stDailyRewardData._shopItemOneKey));
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_DailyRewardData, BackendGameInfo.ShopItemTwoKey_Data, string.Join(",", _stDailyRewardData._shopItemTwoKey));
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_DailyRewardData, BackendGameInfo.ShopItemReceive_Data, string.Join(",", _stDailyRewardData._shopReceiveCount));
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_DailyRewardData, BackendGameInfo.DailyReward_Data, _stDailyRewardData._loginRewardCount);
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_DailyRewardData, BackendGameInfo.RouletteCount_Data, _stDailyRewardData._rouletteCount);
        //};
        //
        //// REWARDTIMEDATA
        //_saveFunctions[(EUserSaveType.REWARDTIMEDATA, 0)] = () =>
        //{
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_RewardTimeData, BackendGameInfo.RewardTime_Data, string.Join(",", _stRewardTimeData._stTimeArr));
        //};
        //
        //// COLLECTIONDATA
        //_saveFunctions[(EUserSaveType.COLLECTIONDATA, 0)] = () =>
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_CollectionData, BackendGameInfo.Collection_Data, string.Join(",", _stCollectionData._collectionList));
        //
        //// QUESTDATA
        //_saveFunctions[(EUserSaveType.QUESTDATA, 0)] = () =>
        //{
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_key_QuestData, BackendGameInfo.DailyQuest_Data, string.Join(",", _stQuestData.dailyQuestList));
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_key_QuestData, BackendGameInfo.DailyReceived_Data, string.Join(",", _stQuestData.dailyReceivedList));
        //};
        //_saveFunctions[(EUserSaveType.QUESTDATA, 1)] = () =>
        //{
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_key_QuestData, BackendGameInfo.MainQuest_Data, string.Join(",", _stQuestData.mainQuestList));
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_key_QuestData, BackendGameInfo.MainReceived_Data, string.Join(",", _stQuestData.mainReceivedList));
        //};
        //_saveFunctions[(EUserSaveType.QUESTDATA, 2)] = () =>
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_key_QuestData, BackendGameInfo.SpecialQuest_Data, string.Join(",", _stQuestData.specialQuestList));
        //
        //// TUTORIALDATA
        //_saveFunctions[(EUserSaveType.TUTORIALDATA, 0)] = () =>
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_TutorialData, BackendGameInfo.Tutorial_Data, _stTutorialData._tutorial);
        //
        //// SEASONPASSDATA
        //_saveFunctions[(EUserSaveType.SEASONPASSDATA, 0)] = () =>
        //{
        //    int[] index = { _stSeasonPassData._currentSeason, _stSeasonPassData._level, _stSeasonPassData._progressCount };
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_SeasonPassData, BackendGameInfo.SeasonPassIndex_Data, string.Join(",", index));
        //};
        //_saveFunctions[(EUserSaveType.SEASONPASSDATA, 1)] = () =>
        //{
        //    bool[] boolen = { _stSeasonPassData._isVipActivated, _stSeasonPassData._isActivated };
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_SeasonPassData, BackendGameInfo.SeasonPassBoolen_Data, string.Join(",", boolen));
        //};
        //_saveFunctions[(EUserSaveType.SEASONPASSDATA, 2)] = () =>
        //{
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_SeasonPassData, BackendGameInfo.SeasonPass_Data, string.Join(",", _stSeasonPassData._normalRewardStateList));
        //};
        //_saveFunctions[(EUserSaveType.SEASONPASSDATA, 3)] = () =>
        //{
        //    _serverGameInfo.ServerUpdate(BackendGameInfo.Server_Key_SeasonPassData, BackendGameInfo.SeasonPassVIP_Data, string.Join(",", _stSeasonPassData._vipRewardStateList));
        //};
    }

    public void CreateData(EUserSaveType type)
    {
        _initDataDictionary[type]?.Invoke();
        LocalSaveFuntion(type);
    }

    #region Init Server Data
    public void InitServerData(STUserData data) => _stUserData = data;
    public void InitServerData(STUnitData data) => _stUnitData = data;
    public void InitServerData(STMaterialData data) => _stMaterialData = data;
    public void InitServerData(STUnitLevelData data) => _stUnitLevelData = data;
    public void InitServerData(STUnitPotentialData data) => _stUnitPotentialData = data;
    public void InitServerData(STStageData data) => _stStageData = data;
    public void InitServerData(STUnitIDCount data) => _stUnitIDCount = data;
    public void InitServerData(STCoalescenceData data) => _stCoalescenceData = data;
    public void InitServerData(STUtilityData data) => _stUtilityData = data;
    public void InitServerData(STVersionData data) => _stVersionData._version = data._version;
    public void InitServerData(STTutorialData data) => _stTutorialData = data;
    public void InitServerData(STDailyRewardData data) => _stDailyRewardData = data;
    public void InitServerData(STRewardTimeData data) => _stRewardTimeData = data;
    public void InitServerData(STQuestData data) => _stQuestData = data;
    public void InitServerData(STCollectionData data) => _stCollectionData = data;
    public void InitServerData(STSeasonPassData data) => _stSeasonPassData = data;
    #endregion Init Server Data

    public int[] FindCoalescence(string findID)
    {
        for (int i = 0; i < _stCoalescenceData._boatNumID.Count; ++i)
        {
            if (_stCoalescenceData._boatNumID[i] == findID)
            {
                return StaticScript.IntSplit(_stCoalescenceData._coalescence[i], ',');
            }
        }
        return null;
    }

    public List<string> FindPotential(string findID)
    {
        for (int i = 0; i < _stCoalescenceData._boatNumID.Count; ++i)
        {
            if (_stCoalescenceData._boatNumID[i] == findID)
            {
                return _stCoalescenceData._potentials[i].Split('_').ToList();
            }
        }
        return null;
    }
    public int[] FindCoalescence(int findID) => FindCoalescence(findID.ToString());
    public int[] FindCoalescenceArr(int num) => StaticScript.IntSplit(_stCoalescenceData._coalescence[num], ',');
    public string CoalesceneceStr(int[] arr) => string.Join(",", arr);
    public List<string> FindPotential(int findID) => FindPotential(findID.ToString());

    public void ExpUp(int exp, int maxExp)
    {
        if (exp == 0) { return; }
        _userExp += exp;
        while (_userExp >= maxExp)
        {
            _userExp -= maxExp;
            _userLevel++;
        }

        _stUtilityData._userLevel = _userLevel;
        _stUtilityData._userExp = _userExp;

        UtilityDataSave();
    }
    
    /// <summary>
    /// 강제 레벨업
    /// </summary>
    /// <param name="level"></param>
    public void SetUserLevel(int level)
    {
        _userLevel = level;
        _stUtilityData._userLevel = level;
        UtilityDataSave();
    }

    public bool StagePosMaxCheck()
    {
        if (_stStageData._stagePos == GetMaxStageNumber) { return true; }
        else { return false; }
    }

    public string RewardTimeReturn(int arr)
    {
        if (arr > -1 && arr < _stRewardTimeData._stTimeArr.Length)
        {
            return _stRewardTimeData._stTimeArr[arr];
        }
        return "0";
    }
    public void RewardTimeSetting(string time, int arr, bool fastSave = true)
    {
        if (arr > -1 && arr < _stRewardTimeData._stTimeArr.Length)
        {
            _stRewardTimeData._stTimeArr[arr] = time;
        }
        if (fastSave)
        {
            SaveRewardTime();
        }
    }
    public int TutorialCheck() => _stTutorialData._tutorial;

    #region property
    public int FindBoatIndex(string ingerenceID)
    {
        for (int i = 0; i < _stCoalescenceData._boatNumID.Count; i++)
        {
            if (_stCoalescenceData._boatNumID[i] == ingerenceID)
            {
                return i;
            }
        }
        return -1;
    }
    public List<PlayerInfo> UnitInfo { get { return _unitInfo; } set { _unitInfo = value; } }
    public List<int> UnitPosition { get { return _unitPosition; } set { _unitPosition = value; } }
    public int BoatPos { get { return _boatPos; } set { _boatPos = value; } }
    public int StagePos { get { return _stStageData._stagePos; } set { _stStageData._stagePos = value; } }
    public int GetUserLevel => _userLevel;
    public int GetUserExp => _userExp;
    public int GetMaxStageNumber => _maxStageNumber;

    public UnitIcon BoatTemp { get { return _boatTemp; } set { _boatTemp = value; } }
    public UnitIcon[] ChooseBoatTemp { get { if (_chooseBoatTemp == null) { _chooseBoatTemp = new UnitIcon[System.Enum.GetValues(typeof(EUnitPosition)).Length]; } return _chooseBoatTemp; } set { _chooseBoatTemp = value; } }
    public bool GetServer { get { return _server; } set { _server = value; } }
    public STVersionData GetVersionData => _stVersionData;
    public void SetVersionServerLogin(int value) { _stVersionData._serverLogin = value; LocalSaveFuntion(EUserSaveType.VERSIONDATA); }
    public STUnitData GetUnitData => _stUnitData;
    public STUnitLevelData GetLevelData => _stUnitLevelData;
    public STUnitPotentialData GetPotentialData => _stUnitPotentialData;
    public STCoalescenceData GetCoalescenceData => _stCoalescenceData;
    public STUnitSelectData GetSelectData => _stSelectUnitData;
    public STCollectionData GetCollectionData => _stCollectionData;
    public STOptionData GetOptionData => _stOptionData;
    public STSingleUseProductData GetSingleUseProductData => _stRewardData;
    public STQuestData GetQuestData => _stQuestData;
    #endregion

    // 시작 전 광고 / 다이아로 결과 보상 150%, 300% 적용
    public bool CheckAdAbility => _stSelectUnitData._adAbilityCheck;
    public bool CheckDiaAbility => _stSelectUnitData._diaAbilityCheck;
    // 보상 배율 예약 확인
    public bool IsAdAbilityReserved => _stSelectUnitData._adAbilityReserved;
    public bool IsDiaAbilityReserved => _stSelectUnitData._diaAbilityReserved;
    public void SetAdAbilityCheck(bool check) => _stSelectUnitData._adAbilityCheck = check;
    public void SetDiaAbilityCheck(bool check) => _stSelectUnitData._diaAbilityCheck = check;
    public void SetAdAbilityReserved(bool check) => _stSelectUnitData._adAbilityReserved = check;
    public void SetDiaAbilityReserved(bool check) => _stSelectUnitData._diaAbilityReserved = check;

    // ActiveSkill
    public int GetEquipedSkill(int idx) => _stSelectUnitData._equipedSkillArr[idx];
    public void SetEquipedSkill(int idx, int id) => _stSelectUnitData._equipedSkillArr[idx] = id;
    public int[] GetEquipedSkillArr() => _stSelectUnitData._equipedSkillArr;
}