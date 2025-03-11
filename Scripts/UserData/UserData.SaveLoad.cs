using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 세이브 로드 기능 파트(서버 포함)
/// </summary>
public partial class UserData : Singleton<UserData>
{
    #region LoadData
    Dictionary<EUserSaveType, System.Action<string, EUserSaveType>> _loadDelegates;
    private void InitializeDelegates()
    {
        _loadDelegates = new Dictionary<EUserSaveType, System.Action<string, EUserSaveType>>
        {
            { EUserSaveType.USERDATA, LoadUserData },
            { EUserSaveType.MATERIALDATA, LoadMaterialData },
            { EUserSaveType.UNITDATA, LoadUnitData },
            { EUserSaveType.UNITLEVELDATA, LoadUnitLevelData },
            { EUserSaveType.UNITPOTENTIALDATA, LoadUnitPotentialData },
            { EUserSaveType.STAGEDATA, LoadStageData },
            { EUserSaveType.UNITCOUNTID, LoadUnitCountID },
            { EUserSaveType.SELECTDATA, LoadSelectUnitData },
            { EUserSaveType.COALESCENECEDATA, LoadCoalescenceData },
            { EUserSaveType.UTILITYDATA, LoadUtilityData },
            { EUserSaveType.VERSIONDATA, LoadVersionData },
            { EUserSaveType.TUTORIALDATA, LoadTutorialData },
            { EUserSaveType.PACKAGEDATA, LoadRewardData },
            { EUserSaveType.DAILYREWARDDATA, LoadShopOneDayItemData },
            { EUserSaveType.REWARDTIMEDATA, LoadRewardTimeData },
            { EUserSaveType.COLLECTIONDATA, LoadCollectionData },
            { EUserSaveType.QUESTDATA, LoadQuestData },
            { EUserSaveType.OPTIONDATA, LoadOptionData },
            { EUserSaveType.SEASONPASSDATA, LoadSeasonPassData },
            { EUserSaveType.SERVERSAVEDATA, LoadServerSaveData }
        };
    }
    bool _dataInit = false;
    public void AllLoadData()
    {
        InitializeDelegates();
        for (int i = 0; i < (int)EUserSaveType.NONE; ++i) 
        { 
            FilePathCheck(_dataPathDictionary[(EUserSaveType)i], (EUserSaveType)i);
        }

        Debug.Log("로드하였습니다");
    }

    void FilePathCheck(string path, EUserSaveType type)
    {
        if (!ES3.FileExists(path))
        {
            CreateData(type);
            if (!_dataInit) { _dataInit = true; PlayerPrefs.DeleteAll(); }
        }
        else
        {
            _loadDelegates[type](path, type);
            //Debug.Log($"{type} Load Success");
        }
    }

    void LoadUserData(string path, EUserSaveType type) => _stUserData = ES3.Load<MyStructData.STUserData>(type.ToString(), path);
    void LoadMaterialData(string path, EUserSaveType type) => _stMaterialData = ES3.Load<MyStructData.STMaterialData>(type.ToString(), path);
    void LoadUnitData(string path, EUserSaveType type) => _stUnitData = ES3.Load<MyStructData.STUnitData>(type.ToString(), path);
    void LoadUnitLevelData(string path, EUserSaveType type) => _stUnitLevelData = ES3.Load<MyStructData.STUnitLevelData>(type.ToString(), path);
    void LoadUnitPotentialData(string path, EUserSaveType type)
    {
        _stUnitPotentialData = ES3.Load<MyStructData.STUnitPotentialData>(type.ToString(), path);
    }
    void LoadStageData(string path, EUserSaveType type) => _stStageData = ES3.Load<MyStructData.STStageData>(type.ToString(), path);
    void LoadUnitCountID(string path, EUserSaveType type) => _stUnitIDCount = ES3.Load<MyStructData.STUnitIDCount>(type.ToString(), path);
    void LoadSelectUnitData(string path, EUserSaveType type) => _stSelectUnitData = ES3.Load<MyStructData.STUnitSelectData>(type.ToString(), path);
    void LoadCoalescenceData(string path, EUserSaveType type) => _stCoalescenceData = ES3.Load<MyStructData.STCoalescenceData>(type.ToString(), path);
    void LoadUtilityData(string path, EUserSaveType type) => _stUtilityData = ES3.Load<MyStructData.STUtilityData>(type.ToString(), path);
    void LoadVersionData(string path, EUserSaveType type) => _stVersionData = ES3.Load<MyStructData.STVersionData>(type.ToString(), path);
    void LoadTutorialData(string path, EUserSaveType type) => _stTutorialData = ES3.Load<MyStructData.STTutorialData>(type.ToString(), path);
    void LoadRewardData(string path, EUserSaveType type) => _stRewardData = ES3.Load<MyStructData.STSingleUseProductData>(type.ToString(), path);
    void LoadShopOneDayItemData(string path, EUserSaveType type) => _stDailyRewardData = ES3.Load<MyStructData.STDailyRewardData>(type.ToString(), path);
    void LoadRewardTimeData(string path, EUserSaveType type) => _stRewardTimeData = ES3.Load<MyStructData.STRewardTimeData>(type.ToString(), path);
    void LoadCollectionData(string path, EUserSaveType type) => _stCollectionData = ES3.Load<MyStructData.STCollectionData>(type.ToString(), path);
    void LoadQuestData(string path, EUserSaveType type) => _stQuestData = ES3.Load<MyStructData.STQuestData>(type.ToString(), path);
    void LoadOptionData(string path, EUserSaveType type) => _stOptionData = ES3.Load<MyStructData.STOptionData>(type.ToString(), path);
    void LoadSeasonPassData(string path, EUserSaveType type) => _stSeasonPassData = ES3.Load<MyStructData.STSeasonPassData>(type.ToString(), path);
    void LoadServerSaveData(string path, EUserSaveType type) => _stServerSaveData = ES3.Load<MyStructData.STServerSaveData>(type.ToString(), path);
    #endregion LoadData

    #region SaveData
    public void LocalSaveFuntion(EUserSaveType type)
    {
        _saveDataDictionary[type]?.Invoke();
    }
    public void ServerLocalAllDataSave()
    {
        //for (int i = 0; i < (int)EUserServerSaveType.NONE; ++i)
        //
        //   _saveDataDictionary[TypeChange((EUserServerSaveType)i)]?.Invoke();
        //
        for (int i = 0; i < (int)EUserSaveType.NONE; ++i)
        {
            _saveDataDictionary[(EUserSaveType)i]?.Invoke();
        }

        Debug.Log("서버에서 로컬로 저장 완료");
    }
    public void LocalServerSaveFuntion(EUserServerSaveType type)
    {
        if (_stServerSaveData._serverSaveArr[(int)type]) return;
        _stServerSaveData._serverSaveArr[(int)type] = true;
        LocalSaveFuntion(EUserSaveType.SERVERSAVEDATA);
    }
    // 서버 저장
    public void ServerSave()
    {
        for (int i = 0; i < _stServerSaveData._serverSaveArr.Length; ++i)
        {
            if (_stServerSaveData._serverSaveArr[i])
            {
                //StartCoroutine(DelayServerSaveFuntion((EUserServerSaveType)i));
                ServerSaveFuntion((EUserServerSaveType)i);
                _stServerSaveData._serverSaveArr[i] = false;
            }
        }
        LocalSaveFuntion(EUserSaveType.SERVERSAVEDATA);
    }
    void ServerSaveFuntion(EUserServerSaveType type)
    {
        EUserSaveType tempType = TypeChange(type);
        int index = GetIndexCount(type);

        for (int i = 0; i < index; ++i)
        {
            ServerSaveFuntion(tempType, i);
        }
    }
    IEnumerator DelayServerSaveFuntion(EUserServerSaveType type)
    {
        yield return YieldInstructionCache.WaitForSeconds(1f);
        EUserSaveType tempType = TypeChange(type);
        int index = GetIndexCount(type);

        for (int i = 0; i < index; ++i)
        {
            ServerSaveFuntion(tempType, i);
        }
    }
    // 원래 사용했던 서버 저장
    void ServerSaveFuntion(EUserSaveType type, int numberType)
    {
        var key = (type, numberType);

        if (_saveFunctions.ContainsKey(key))
        {
            _saveFunctions[key]();
            Debug.Log(string.Format("SaveFunction: {0} / {1}", key.type, key.numberType));
        }
        else
        {
            Debug.Log(string.Format("ServerSaveFuntion not found for Type: {0}. Number: {1}", type, numberType));
        }
    }
    EUserSaveType TypeChange(EUserServerSaveType type)
    {
        switch (type)
        {
            case EUserServerSaveType.USERDATA: return EUserSaveType.USERDATA;
            case EUserServerSaveType.UNITDATA: return EUserSaveType.UNITDATA;
            case EUserServerSaveType.UNITLEVELDATA: return EUserSaveType.UNITLEVELDATA;
            case EUserServerSaveType.UNITPOTENTIALDATA: return EUserSaveType.UNITPOTENTIALDATA;
            case EUserServerSaveType.STAGEDATA: return EUserSaveType.STAGEDATA;
            case EUserServerSaveType.UNITCOUNTID: return EUserSaveType.UNITCOUNTID;
            case EUserServerSaveType.COALESCENECEDATA: return EUserSaveType.COALESCENECEDATA;
            case EUserServerSaveType.UTILITYDATA: return EUserSaveType.UTILITYDATA;
            case EUserServerSaveType.VERSIONDATA: return EUserSaveType.VERSIONDATA;
            case EUserServerSaveType.DAILYREWARDDATA: return EUserSaveType.DAILYREWARDDATA;
            case EUserServerSaveType.REWARDTIMEDATA: return EUserSaveType.REWARDTIMEDATA;
            case EUserServerSaveType.COLLECTIONDATA: return EUserSaveType.COLLECTIONDATA;
            case EUserServerSaveType.QUESTDATA: return EUserSaveType.QUESTDATA;
            case EUserServerSaveType.TUTORIALDATA: return EUserSaveType.TUTORIALDATA;
            case EUserServerSaveType.SEASONPASSDATA: return EUserSaveType.SEASONPASSDATA;
            default: return EUserSaveType.NONE;
        }
    }
    int GetIndexCount(EUserServerSaveType type)
    {
        switch (type)
        {
            case EUserServerSaveType.USERDATA: return 1; // 2
            case EUserServerSaveType.UNITDATA: return 6;
            case EUserServerSaveType.UNITLEVELDATA: return 6;
            case EUserServerSaveType.UNITPOTENTIALDATA: return 3;
            case EUserServerSaveType.STAGEDATA: return 1;
            case EUserServerSaveType.UNITCOUNTID: return 1;
            case EUserServerSaveType.COALESCENECEDATA: return 1;
            case EUserServerSaveType.UTILITYDATA: return 1;
            case EUserServerSaveType.VERSIONDATA: return 1;
            case EUserServerSaveType.DAILYREWARDDATA: return 1;
            case EUserServerSaveType.REWARDTIMEDATA: return 1;
            case EUserServerSaveType.COLLECTIONDATA: return 1;
            case EUserServerSaveType.QUESTDATA: return 3;
            case EUserServerSaveType.TUTORIALDATA: return 1;
            case EUserServerSaveType.SEASONPASSDATA: return 4;
            default: return 1;
        }
    }
    #endregion SaveData

    public void ServerInitLocalDataSave()
    {
        CreateData(EUserSaveType.SELECTDATA);
        CreateData(EUserSaveType.REWARDTIMEDATA);
    }
    public void ServerInitLocalDataDelete()
    {
        PlayerPrefs.DeleteAll();
    }

    // 스테이지
    public void StagePosSave()
    {
        if (_stStageData._stagePos == _maxStageNumber) { return; } // 마지막 스테이지
        //_stagePos++;
        _stStageData._stagePos++;// = _stagePos;
        if (_server) { GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA); } //GameManager.GetInstance.GetServerSaveListAdd(EUserSaveType.STAGEDATA);
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.STAGEDATA, true);
    }
    public bool StagePercentCheck()
    {
        if (_stStageData._stagePos % 3 == 0) { Debug.Log("서버 저장"); return true; }

        return false;
    }

    /// <summary>
    /// 컬렉션 데이터
    /// </summary>
    /// <param name="arr"></param>
    /// <param name="count"></param>
    public void CollectionUnitSave(int arr, int count)
    {
        _stCollectionData._collectionList[arr] = count;

        if (_server) { GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.UTILITYDATA); } // GameManager.GetInstance.GetServerSaveListAdd(EUserSaveType.COLLECTIONDATA);
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.COLLECTIONDATA, true);
    }
    /// <summary>
    /// 출석 데이터
    /// </summary>
    /// <param name="rewardCount"></param>
    public void SaveDailyRewardData()
    {
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.DAILYREWARDDATA, true);
        // 일일보상카운터가 여기 속해있다
        if (_server) { GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.UTILITYDATA); } // DAILYREWARDDATA
    }

    public void UtilityDataSave()
    {
        // 재능&레벨 세이브
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.UTILITYDATA);
        if (_server) { GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA); } // UTILITYDATA
    }
    public void SaveCoalescenece(List<string> coalescenceData, List<string> potentials)
    {
        _stCoalescenceData._coalescence = coalescenceData;
        _stCoalescenceData._potentials = potentials;
        
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.COALESCENECEDATA, true);
        if (_server) { GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA); } // COALESCENECEDATA
    }

    public void SavePotentials(List<string> potentials)
    {
        _stCoalescenceData._potentials = potentials;

        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.COALESCENECEDATA, true);
        if (_server) { GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA); } // COALESCENECEDATA
    }

    public void SaveUnitSelect()
    {
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.SELECTDATA);
    }

    public void SaveRewardTime()
    {
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.REWARDTIMEDATA, true);
        if (_server) { GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.UTILITYDATA); } // REWARDTIMEDATA
    }
    public void OptionTypeSetting(int arr, int value)
    {
        _stOptionData._optionType[arr] = value;
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.OPTIONDATA, true);
    }

    public void SaveTutorial(int save)
    {
        _stTutorialData._tutorial = save;
        if (_server) 
        {
            GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.UTILITYDATA); // TUTORIALDATA
            //if (TutorialCheck() == 9) { GameManager.GetInstance.GetServerSaveListAdd(EUserSaveType.TUTORIALDATA, 0, true); }
            //else { GameManager.GetInstance.GetServerSaveListAdd(EUserSaveType.TUTORIALDATA, 0); }
        }
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.TUTORIALDATA, true);
    }

    public void SaveUserData(bool fastSave = false)
    {
        // 돈이나 다이아 까지는거 세이브
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.USERDATA, fastSave);
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.MATERIALDATA, true);
    }

    public void SaveSeasonPassData()
    {
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.SEASONPASSDATA, true);
        if (_server) { GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.UTILITYDATA); } // SEASONPASSDATA
    }

    public void SaveQuestData(bool fastSave = false)
    {
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.QUESTDATA, fastSave);
        if (_server) { GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.UTILITYDATA); } // QUESTDATA
    }
}
