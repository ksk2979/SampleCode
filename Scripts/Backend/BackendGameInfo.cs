using BackEnd;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class BackendGameInfo : MonoBehaviour
{
    UserData _userData = null;

    #region 서버 데이터 키
    public static string Server_Key_UserData = "UserData";
    public static string Property_Data = "Property";

    public static string Server_Key_UtilityData = "UtilityData";
    public static string Utility_Data = "Utility";

    public static string Server_Key_VersionData = "VersionData";
    public static string Version_Data = "Version";

    #endregion

    #region SaveData
    public void ServerDataInit()
    {
        PlayerCharacterInit();
        PlayerVersionInit();
        UtilityDataInit();
    }

    /// <summary>
    /// 플레이어 재화 데이터 초기화
    /// </summary>
    void PlayerCharacterInit()
    {
        #region 재화 보유 데이터
        int[] property = new int[(int)EPropertyType.NONE];
        for (int i = 0; i < property.Length; ++i) { property[i] = 0; }
        property[(int)EPropertyType.MONEY] = 500;
        property[(int)EPropertyType.ACTIONENERGY] = 30;

        string propertyStr = string.Join(",", property);
        #endregion

        #region 재료 보유 데이터
        int[] material = new int[(int)EMaterialType.NONE];
        for (int i = 0; i < material.Length; ++i) { material[i] = 5; }

        string materialStr = string.Join(",", material);
        #endregion

        #region 유닛 데이터
        #endregion
        List<int> boat = new List<int>();
        List<int> weapon = new List<int>();
        List<int> defense = new List<int>();
        List<int> captain = new List<int>();
        List<int> sailor = new List<int>();
        List<int> engine = new List<int>();

        boat.Add(1);
        weapon.Add(12); // 기관총으로 지급을 위해
        weapon.Add(13); // 바꿀 기관총

        string boatStr = string.Join(",", boat);
        string weaponStr = string.Join(",", weapon);
        string defenseStr = string.Join(",", defense);
        string captainStr = string.Join(",", captain);
        string sailorStr = string.Join(",", sailor);
        string engineStr = string.Join(",", engine);

        #region 유닛 레벨 데이터
        List<int> boatLevel = new List<int>();
        List<int> weaponLevel = new List<int>();
        List<int> defenseLevel = new List<int>();
        List<int> captainLevel = new List<int>();
        List<int> sailorLevel = new List<int>();
        List<int> engineLevel = new List<int>();

        boatLevel.Add(1);
        weaponLevel.Add(1);
        weaponLevel.Add(1);

        string boatLevelStr = string.Join(",", boatLevel);
        string weaponLevelStr = string.Join(",", weaponLevel);
        string defenseLevelStr = string.Join(",", defenseLevel);
        string captainLevelStr = string.Join(",", captainLevel);
        string sailorLevelStr = string.Join(",", sailorLevel);
        string engineLevelStr = string.Join(",", engineLevel);
        #endregion

        #region 유닛 옵션 데이터
        List<string> capPotentialID = new List<string>();
        List<string> salPotentialID = new List<string>();
        List<string> engPotentialID = new List<string>();

        string capPotentialStr = string.Join("/", capPotentialID); // 0/0/0/ ... A등급 이상 0,0/0,0/0,0/
        string salPotentialStr = string.Join("/", salPotentialID);
        string engPotentialStr = string.Join("/", engPotentialID);
        #endregion

        #region 스테이지 데이터
        string stageID = "0";
        #endregion

        #region 보트 ID 데이터
        List<int> unitIDCount = new List<int>();

        unitIDCount.Add(1); // 기본으로 보트1개 주기때문에
        for (int i = 0; i < 65; ++i)
        {
            unitIDCount.Add(0);
        }

        string boatIDCountStr = string.Join(",", unitIDCount);
        #endregion

        #region 보트 정보 데이터
        List<string> boatNumID = new List<string>();
        List<string> coalescence = new List<string>();
        List<string> potentials = new List<string>();

        boatNumID.Add("110");
        coalescence.Add("1,1,0,0,0,0,0,0,0,0,0,0");
        potentials.Add("0_0_0");

        string boatNumIDStr = string.Join(",", boatNumID);
        string coalescenceStr = string.Join("/", coalescence);
        string potentialsStr = string.Join("/", potentials);
        #endregion

        #region 유틸리티 데이터 (유저 레벨)
        int[] utility = new int[2];
        utility[0] = 1; // userLevel
        utility[1] = 0; // value

        string utilityStr = string.Join(",", utility);
        #endregion

        string combinedStr = string.Join("[",
        propertyStr, materialStr, boatStr, weaponStr, defenseStr, captainStr, sailorStr, engineStr,
        boatLevelStr, weaponLevelStr, defenseLevelStr, captainLevelStr, sailorLevelStr, engineLevelStr,
        capPotentialStr, salPotentialStr, engPotentialStr, stageID, boatIDCountStr, boatNumIDStr, coalescenceStr,
        potentialsStr, utilityStr);

        Param param = new Param();
        param.Add(Property_Data, combinedStr);

        // 생성
        BROSuccessCheck(Server_Key_UserData, param);
    }

    void PlayerVersionInit()
    {
        int version = 0;

        // Param은 뒤끝 서버와 통신을 할 때 넘겨주는 파라미터 클래스이다
        Param param = new Param();

        param.Add(Version_Data, version);

        // 생성
        BROSuccessCheck(Server_Key_VersionData, param);
    }
    void UtilityDataInit()
    {
        #region 일일 보상 데이터
        int[] shopItemOneKey = new int[6];
        int[] shopItemTwoKey = new int[shopItemOneKey.Length];
        int[] shopReceiveCount = new int[shopItemOneKey.Length];
        int loginRewardCount = 0;
        int rouletteCount = 0;

        for (int i = 0; i < shopItemOneKey.Length; ++i)
        {
            shopItemOneKey[i] = 0;
            shopItemTwoKey[i] = 0;
            shopReceiveCount[i] = 0;
        }

        string shopItemOneStr = string.Join(",", shopItemOneKey);
        string shopItemTwoStr = string.Join(",", shopItemTwoKey);
        string shopItemReceiveStr = string.Join(",", shopReceiveCount);

        #endregion

        #region 타임 보상 데이터
        int[] reward = new int[Enum.GetValues(typeof(ETimeCheckType)).Length];

        for (int i = 0; i < reward.Length; ++i) { reward[i] = 0; }
        /*
         Shop_Package0
         Shop_Package1
         Shop_Package2
        Reward_OneDay_Count
         */
        string rewardStr = string.Join(",", reward);
        #endregion

        #region 도감 데이터
        List<int> collection = new List<int>();
        for (int i = 0; i < 21; ++i)
        {
            collection.Add(0);
        }

        string collectionStr = string.Join(",", collection);
        #endregion

        #region 퀘스트 데이터
        List<int> dailyQuestList = new List<int>();
        List<int> mainQuestList = new List<int>();
        List<int> specialQuestList = new List<int>();
        List<bool> dailyReceivedList = new List<bool>();
        List<bool> mainReceivedList = new List<bool>();

        for (int i = 0; i < (int)EDailyQuest.NONE; i++)
        {
            dailyQuestList.Add(0);
            dailyReceivedList.Add(false);
        }
        dailyQuestList[0] = 1;

        mainQuestList.Add(0);
        for (int j = 0; j < 3; ++j)
        {
            mainReceivedList.Add(false);
        }

        string dailyQuestStr = string.Join(",", dailyQuestList);
        string dailytReceivedListStr = string.Join(",", dailyReceivedList);

        string mainQuestStr = string.Join(",", mainQuestList);
        string mainReceivedListStr = string.Join(",", mainReceivedList);

        string specialQuestListStr = string.Join(",", specialQuestList);
        #endregion

        #region 튜토리얼
        string tutorial = "0";
        #endregion

        #region 시즌 패스 데이터
        int[] seasonIndexArr = new int[3]; // currentSeason, level, progressCount
        bool[] seasonBoolenArr = new bool[2]; // isVipActivated, isActivated

        List<bool> normalRewardStateList;
        List<bool> vipRewardStateList;

        for (int i = 0; i < seasonIndexArr.Length; ++i) { seasonIndexArr[i] = 0; }
        for (int i = 0; i < seasonBoolenArr.Length; ++i) { seasonBoolenArr[i] = false; }

        normalRewardStateList = new bool[15].ToList();
        vipRewardStateList = new bool[15].ToList();

        string seaonIndexStr = string.Join(",", seasonIndexArr);
        string seaonboolenStr = string.Join(",", seasonBoolenArr);
        string normalSeasonPassStr = string.Join(",", normalRewardStateList);
        string vipSeasonPassStr = string.Join(",", vipRewardStateList);
        #endregion

        string combinedStr = string.Join("[",
        shopItemOneStr, shopItemTwoStr, shopItemReceiveStr, loginRewardCount, rouletteCount
        , rewardStr, collectionStr,
        dailyQuestStr, dailytReceivedListStr, mainQuestStr, mainReceivedListStr, specialQuestListStr,
        tutorial,
        seaonIndexStr, seaonboolenStr, normalSeasonPassStr, vipSeasonPassStr);

        Param param = new Param();
        param.Add(Utility_Data, combinedStr);

        // 생성
        BROSuccessCheck(Server_Key_UtilityData, param);
    }

    /// <summary>
    /// 게임 정보 저장
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="param"></param>
    void BROSuccessCheck(string tableName, Param param)
    {
        BackendReturnObject BRO = Backend.GameData.Insert(tableName, param);
    
        if (BRO.IsSuccess())
        {
            // 생성된 테이블 정보의 id값
            //Debug.Log(tableName + ": " + BRO.GetInDate());
        }
        else { SaveError(BRO); }
    }
    #endregion

    #region VersionUpdate
    public void VersionUpdate(int version)
    {
        
    }
    #endregion

    #region LoadData
    Dictionary<string, string> _privateData = new Dictionary<string, string>
    {
        { Server_Key_UserData, "" },
        { Server_Key_VersionData, "" },
        { Server_Key_UtilityData, "" },
    };
    void IndateValueUpdate()
    {
        foreach (var key in _privateData.Keys.ToList())
        {
            //if (!string.IsNullOrEmpty(_privateData[key])) continue;
            string serverKey = key;
            BackendReturnObject inDataBRO = Backend.GameData.Get(serverKey, new Where(), 1);

            if (inDataBRO.IsSuccess())
            {
                _privateData[key] = inDataBRO.GetReturnValuetoJSON()["rows"][0]["inDate"][0].ToString();
                Debug.Log("IndataValueUpdate: " + _privateData[key]);
            }
        }
    }
    private string PrivateInDateStringReturn(string key)
    {
        return _privateData.ContainsKey(key) ? _privateData[key] : "";
    }

    public void PlayerVersionUpdateTableLoad()
    {
        if (_userData == null) { _userData = UserData.GetInstance; }
        IndateValueUpdate();
    }
    public void PlayerVersionLoad()
    {
        if (_userData == null) { _userData = UserData.GetInstance; }

        string key = Server_Key_VersionData;

        BackendReturnObject inDataBRO = Backend.GameData.Get(key, new Where(), 1);
        if (inDataBRO.IsSuccess())
        {
            _privateData[key] = inDataBRO.GetReturnValuetoJSON()["rows"][0]["inDate"][0].ToString();
        }

        ServerDataTableLoad(key);
    }
    public void PlayerInfoLoad()
    {
        foreach (var key in _privateData.Keys.ToList())
        {
            if (string.IsNullOrEmpty(_privateData[key])) { Debug.Log($"{key} Null Empty"); continue; }
            ServerDataTableLoad(key);
        }
    }

    void ServerDataTableLoad(string key)
    {
        string serverKey = key;
        BackendReturnObject BRO = Backend.GameData.Get(serverKey, new Where(), 1);

        if (BRO.IsSuccess()) { GetUserDataGameInfo(BRO.GetReturnValuetoJSON(), serverKey); }
        else { CheckError(BRO); }
    }

    void GetUserDataGameInfo(LitJson.JsonData returnData, string tableName)
    {
        if (returnData != null)
        {
            // rows 로 전달 받은 경우
            if (returnData.Keys.Contains("rows"))
            {
                LitJson.JsonData rows = returnData["rows"];

                for (int i = 0; i < rows.Count; ++i)
                {
                    GetLoadData(rows[i], tableName);
                }
                //GetLoadData(rows[0], tableName);
            }
            // row로 전달 받은 경우
            else if (returnData.Keys.Contains("row"))
            {
                //Debug.Log("row로 전달");
                LitJson.JsonData row = returnData["row"];
                GetLoadData(row[0], tableName);
            }
        }
    }

    // 값을 불러와서 게임 내 변수에 저장
    // json parsing
    void GetLoadData(LitJson.JsonData data, string tableName)
    {
        if (tableName == Server_Key_UserData)
        {
            MyStructData.STUserData stUserData = new MyStructData.STUserData();
            MyStructData.STMaterialData stMaterialData = new MyStructData.STMaterialData();
            MyStructData.STUnitData unitData = new MyStructData.STUnitData();
            MyStructData.STUnitLevelData unitLevelData = new MyStructData.STUnitLevelData();
            MyStructData.STUnitPotentialData unitPotentialData = new MyStructData.STUnitPotentialData();
            MyStructData.STStageData stage = new MyStructData.STStageData();
            MyStructData.STUnitIDCount unitIDCountData = new MyStructData.STUnitIDCount();
            MyStructData.STCoalescenceData coalescenceData = new MyStructData.STCoalescenceData();
            MyStructData.STUtilityData utilityData = new MyStructData.STUtilityData();

            stUserData._property = new int[(int)EPropertyType.NONE];
            stMaterialData._material = new int[(int)EMaterialType.NONE];
            unitData._boat = new List<int>();
            unitData._weapon = new List<int>();
            unitData._defense = new List<int>();
            unitData._captain = new List<int>();
            unitData._sailor = new List<int>();
            unitData._engine = new List<int>();

            unitLevelData._boatLevel = new List<int>();
            unitLevelData._weaponLevel = new List<int>();
            unitLevelData._defenseLevel = new List<int>();
            unitLevelData._captainLevel = new List<int>();
            unitLevelData._sailorLevel = new List<int>();
            unitLevelData._engineLevel = new List<int>();

            unitPotentialData._capPotentialID = new List<string>();
            unitPotentialData._salPotentialID = new List<string>();
            unitPotentialData._engPotentialID = new List<string>();

            unitIDCountData._id = new List<int>();
            coalescenceData._boatNumID = new List<string>();
            coalescenceData._coalescence = new List<string>();
            coalescenceData._potentials = new List<string>();

            // 유틸리티 데이터
            utilityData._userLevel = 0;
            utilityData._userExp = 0;

            // "combinedData" 필드에서 하나의 문자열로 결합된 데이터를 가져옴
            if (data.Keys.Contains(Property_Data))
            {
                var value = data[Property_Data][0].ToString();

                string[] splitData = value.Split('[');

                int index = 0;

                // property 데이터
                if (!string.IsNullOrEmpty(splitData[index]))
                {
                    int[] propertyData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    for (int i = 0; i < propertyData.Length; ++i)
                    {
                        stUserData._property[i] = propertyData[i];
                    }
                }
                index++;
                // material 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] materialData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    for (int i = 0; i < materialData.Length; ++i)
                    {
                        stMaterialData._material[i] = materialData[i];
                    }
                }
                index++;
                // boat 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] boatData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    unitData._boat.AddRange(boatData);
                }
                index++;
                // weapon 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] weaponData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    unitData._weapon.AddRange(weaponData);
                }
                index++;
                // defense 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] defenseData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    unitData._defense.AddRange(defenseData);
                }
                index++;
                // captain 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] captainData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    unitData._captain.AddRange(captainData);
                }
                index++;
                // sailor 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] sailorData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    unitData._sailor.AddRange(sailorData);
                }
                index++;
                // engine 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] engineData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    unitData._engine.AddRange(engineData);
                }
                index++;
                // boatLevel 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] boatLevelData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    unitLevelData._boatLevel.AddRange(boatLevelData);
                }
                index++;
                // weaponLevel 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] weaponLevelData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    unitLevelData._weaponLevel.AddRange(weaponLevelData);
                }
                index++;
                // defenseLevel 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] defenseLevelData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    unitLevelData._defenseLevel.AddRange(defenseLevelData);
                }
                index++;
                // captainLevel 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] captainLevelData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    unitLevelData._captainLevel.AddRange(captainLevelData);
                }
                index++;
                // sailorLevel 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] sailorLevelData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    unitLevelData._sailorLevel.AddRange(sailorLevelData);
                }
                index++;
                // engineLevel 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] engineLevelData = splitData[index].Split(',').Select(int.Parse).ToArray();
                    unitLevelData._engineLevel.AddRange(engineLevelData);
                }
                index++;
                // capPotentialID 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    string[] capPotentialData = splitData[index].Split('/');
                    unitPotentialData._capPotentialID.AddRange(capPotentialData);
                }
                index++;
                // salPotentialID 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    string[] salPotentialData = splitData[index].Split('/');
                    unitPotentialData._salPotentialID.AddRange(salPotentialData);
                }
                index++;
                // engPotentialID 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    string[] engPotentialData = splitData[index].Split('/');
                    unitPotentialData._engPotentialID.AddRange(engPotentialData);
                }
                index++;
                // stage 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    var stageData = int.Parse(splitData[index].ToString());
                    stage._stagePos = stageData;
                }
                index++;
                // unitIDCount 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] unitIDCount = splitData[index].Split(',').Select(int.Parse).ToArray();
                    unitIDCountData._id.AddRange(unitIDCount);
                }
                index++;
                // boatNumID 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    string[] boatNumIDData = splitData[index].Split(',');
                    coalescenceData._boatNumID.AddRange(boatNumIDData);
                }
                index++;
                // coalescence 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    string[] coalescenceDataArr = splitData[index].Split('/');
                    coalescenceData._coalescence.AddRange(coalescenceDataArr);
                }
                index++;
                // potentials 데이터
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    string[] potentialsData = splitData[index].Split('/');
                    coalescenceData._potentials.AddRange(potentialsData);
                }
                index++;
                // utility 데이터 (userLevel, userExp)
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] utilityDataArr = splitData[index].Split(',').Select(int.Parse).ToArray();
                    utilityData._userLevel = utilityDataArr[0];
                    utilityData._userExp = utilityDataArr[1];
                }
                index++;
            }

            // 각 데이터 초기화
            _userData.InitServerData(stUserData);
            _userData.InitServerData(stMaterialData);
            _userData.InitServerData(unitData);
            _userData.InitServerData(unitLevelData);
            _userData.InitServerData(unitPotentialData);
            _userData.InitServerData(stage);
            _userData.InitServerData(unitIDCountData);
            _userData.InitServerData(coalescenceData);
            _userData.InitServerData(utilityData);
        }
        else if (tableName == Server_Key_VersionData)
        {
            MyStructData.STVersionData version = new MyStructData.STVersionData();

            if (data.Keys.Contains(Version_Data))
            {
                var value = int.Parse(data[Version_Data][0].ToString());
                version._version = value;
            }

            _userData.InitServerData(version);
        }
        else if (tableName == Server_Key_UtilityData)
        {
            MyStructData.STDailyRewardData daily = new MyStructData.STDailyRewardData();
            MyStructData.STRewardTimeData reward = new MyStructData.STRewardTimeData();
            MyStructData.STCollectionData collection = new MyStructData.STCollectionData();
            MyStructData.STQuestData quest = new MyStructData.STQuestData();
            MyStructData.STTutorialData tutorial = new MyStructData.STTutorialData();
            MyStructData.STSeasonPassData seasonPass = new MyStructData.STSeasonPassData();

            daily._shopItemOneKey = new int[6];
            daily._shopItemTwoKey = new int[daily._shopItemOneKey.Length];
            daily._shopReceiveCount = new int[daily._shopItemOneKey.Length];
            reward._stTimeArr = new string[Enum.GetValues(typeof(ETimeCheckType)).Length];
            collection._collectionList = new List<int>();
            quest.dailyQuestList = new List<int>();
            quest.dailyReceivedList = new List<bool>();
            quest.mainQuestList = new List<int>();
            quest.mainReceivedList = new List<bool>();
            quest.specialQuestList = new List<int>();
            seasonPass._normalRewardStateList = new List<bool>();
            seasonPass._vipRewardStateList = new List<bool>();

            // "combinedData" 필드에서 하나의 문자열로 결합된 데이터를 가져옴
            if (data.Keys.Contains(Utility_Data))
            {
                var value = data[Utility_Data][0].ToString();

                string[] splitData = value.Split('[');

                int index = 0;

                // shopItemOneKey
                if (!string.IsNullOrEmpty(splitData[index]))
                {
                    daily._shopItemOneKey = splitData[index].Split(',').Select(int.Parse).ToArray();
                }
                index++;
                // shopItemTwoKey
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    daily._shopItemTwoKey = splitData[index].Split(',').Select(int.Parse).ToArray();
                }
                index++;
                // shopReceiveCount
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    daily._shopReceiveCount = splitData[index].Split(',').Select(int.Parse).ToArray();
                }
                index++;
                // loginRewardCount and rouletteCount
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    daily._loginRewardCount = int.Parse(splitData[index]);
                }
                index++;
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    daily._rouletteCount = int.Parse(splitData[index]);
                }
                index++;

                // rewardTimeData
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    reward._stTimeArr = splitData[index].Split(',');
                }
                index++;

                // collectionList
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    collection._collectionList = splitData[index].Split(',').Select(int.Parse).ToList();
                }
                index++;

                // dailyQuestList
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    quest.dailyQuestList = splitData[index].Split(',').Select(int.Parse).ToList();
                }
                index++;
                // dailyReceivedList
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    quest.dailyReceivedList = splitData[index].Split(',').Select(bool.Parse).ToList();
                }
                index++;
                // mainQuestList
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    quest.mainQuestList = splitData[index].Split(',').Select(int.Parse).ToList();
                }
                index++;
                // mainReceivedList
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    quest.mainReceivedList = splitData[index].Split(',').Select(bool.Parse).ToList();
                }
                index++;
                // specialQuestList
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    quest.specialQuestList = splitData[index].Split(',').Select(int.Parse).ToList();
                }
                index++;

                // tutorial data
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    tutorial._tutorial = int.Parse(splitData[index]);
                }
                index++;

                // seasonIndexArr
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    int[] newIndex = splitData[index].Split(',').Select(int.Parse).ToArray();
                    seasonPass._currentSeason = newIndex[0];
                    seasonPass._level = newIndex[1];
                    seasonPass._progressCount = newIndex[2];
                }
                index++;

                // seasonBoolenArr
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    bool[] newBoolen = splitData[index].Split(',').Select(bool.Parse).ToArray();
                    seasonPass._isVipActivated = newBoolen[0];
                    seasonPass._isActivated = newBoolen[1];
                }
                index++;

                // normalRewardStateList
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    seasonPass._normalRewardStateList = splitData[index].Split(',').Select(bool.Parse).ToList();
                }
                index++;

                // vipRewardStateList
                if (splitData.Length > index && !string.IsNullOrEmpty(splitData[index]))
                {
                    seasonPass._vipRewardStateList = splitData[index].Split(',').Select(bool.Parse).ToList();
                }
                index++;
            }

            // 각 데이터 초기화
            _userData.InitServerData(daily);
            _userData.InitServerData(reward);
            _userData.InitServerData(collection);
            _userData.InitServerData(quest);
            _userData.InitServerData(tutorial);
            _userData.InitServerData(seasonPass);
        }

        Debug.Log($"Server {tableName} Update");
    }
    #endregion

    #region DataUpdate
    // ------------------------- 테이블 수정 ---------------------------
    // 업데이트 할 내용 함수
    public void ServerUpdate(string privateContent, string updateName, int setIndex)
    {
        string inDate = "";
        inDate = PrivateInDateStringReturn(privateContent);
    
        Param param = new Param();
        param.Add(updateName, setIndex);
        //Debug.Log($"Content:{privateContent} updateName:{updateName} int/setIndex{setIndex}");
        Backend.GameData.UpdateV2(privateContent, inDate, Backend.UserInDate, param, (update) =>
        {
            //try
            //{
            //    Debug.Log($"{privateContent}/{updateName}의 처리상태: {update.GetStatusCode()}");
            //}
            //catch (Exception ex)
            //{
            //    Debug.Log($"비동기 작업 중 예외 발생: {ex.Message}");
            //    UpdateErrorMessage(update);
            //}
        });
    }
    public void ServerUpdate(string privateContent, string updateName, string setIndex)
    {
        string inDate = "";
        inDate = PrivateInDateStringReturn(privateContent);

        Param param = new Param();
        param.Add(updateName, setIndex);
        //Debug.Log($"Content:{privateContent} updateName:{updateName} string/setIndex{setIndex}");
        Backend.GameData.UpdateV2(privateContent, inDate, Backend.UserInDate, param, (update) =>
        {
            //Debug.Log($"{privateContent}/{updateName}의 처리상태:{update.GetStatusCode()}");
            //if (update.IsSuccess()) { Debug.Log($"{privateContent}{updateName} 비동기 처리 완료"); } //Debug.Log("일반적인 수정 완료");
            //else { UpdateErrorMessage(update); }

            //try
            //{
            //    Debug.Log($"{privateContent}/{updateName}의 처리상태: {update.GetStatusCode()}");
            //}
            //catch (Exception ex)
            //{
            //    Debug.Log($"비동기 작업 중 예외 발생: {ex.Message}");
            //    UpdateErrorMessage(update);
            //}
        });
    }

    #endregion

    #region 게임 정보 관련 에러 처리
    // 게임 정보 저장 관련 에러 처리
    void SaveError(BackendReturnObject BRO)
    {
        switch (BRO.GetStatusCode())
        {
            case "404":
                Debug.Log("존재하지 않는 tableName 입니다");
                break;
            case "412":
                Debug.Log("비활성화 된 tableName 입니다");
                break;
            case "413":
                Debug.Log("하나의 row( column들의 집합 )이 400kb를 넘겼습니다");
                break;
            default:
                Debug.Log("서버 공통 에러 발생" + BRO.GetMessage());
                break;
        }
    }
    // 게임 정보 읽기 관련 에러처리
    void CheckError(BackendReturnObject BRO)
    {
        switch (BRO.GetStatusCode())
        {
            case "200":
                Debug.Log("해당 유저의 데이터가 테이블에 없습니다.");
                break;
            case "404":
                if (BRO.GetMessage().Contains("gamer not found"))
                {
                    Debug.Log("gamerIndata가 존재하지 않; gamer의 indata인 경우");
                }
                else
                {
                    Debug.Log("존재하지 않는 테이블");
                }
                break;

            case "400":
                if (BRO.GetMessage().Contains("bad limit"))
                {
                    Debug.Log("limit 값이 100이상인 경우");
                }
                else if (BRO.GetMessage().Contains("bad table"))
                {
                    // public Table 정보를 얻는 코드로 private Table에 접근했을 때 또는
                    // private Table 정보를 얻는 코드로 public Table에 접근했을 때
                    Debug.Log("요청한 코드와 테이블의 공개 여부가 맞지 않습니다");
                }
                break;
            case "412":
                Debug.Log("비활성화된 테이블입니다.");
                break;
            default:
                Debug.Log("서버 공통 에러 발생: " + BRO.GetMessage());
                break;
        }
    }

    // 업데이트(수정) 할때 에러 체크
    void UpdateErrorMessage(BackendReturnObject BRO)
    {
        switch (BRO.GetStatusCode())
        {
            case "405":
                Debug.Log("param에 partition, gamer_id, inData, updatedAt 네가지 필드가 있는 경우");
                break;
            case "403":
                Debug.Log("publicTable의 타인 정보를 수정하고자 하였을 경우");
                break;
            case "404":
                Debug.Log("존재하지 않는 tableName인 경우");
                break;
            case "412":
                Debug.Log("비활성화 된 tableName인 경우");
                break;
            case "413":
                Debug.Log("하나의 row( column 들의 집합 )이 400KB를 넘는 경우");
                break;
            default:
                Debug.Log("서버 공통 에러 발생" + BRO.GetMessage());
                break;
        }
    }
    #endregion

}
