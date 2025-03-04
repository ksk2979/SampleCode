using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class JsonManager : Singleton<JsonManager>
{
    string _filePath;
    string _className;

    // Class {}
    SimpleJSON.JSONObject _title;
    // Array []
    SimpleJSON.JSONArray _array;

    protected override void Awake()
    {
        base.Awake();
    }

    public void JsonInit(string filePath, string className)
    {
        // 기본적으로 JsonSaveLoad 파일에 저장을 한다
#if UNITY_EDITOR
        _filePath = "JsonSaveLoad/" + filePath;

        string tempPath = "Assets/Resources/"; // 첫번째 경로
        string currentPath = null;
        char s = '/';
        string[] arrString = filePath.Split(s);

        for (int i = 0; i < arrString.Length - 1; ++i)
        {
            currentPath += arrString[i];
            if (i < arrString.Length - 2)
            {
                currentPath += "/";
            }
        }

        tempPath += "JsonSaveLoad/" + currentPath;
        if (!Directory.Exists(tempPath))
        {
            Directory.CreateDirectory(tempPath);
        }
#elif UNITY_ANDROID
        _filePath = Application.persistentDataPath + "/" + filePath;
#endif
        _className = className;
        _title = new SimpleJSON.JSONObject();
        _array = new SimpleJSON.JSONArray();
    }

    #region JsonInfo
    // 서버 테이블 키
    //public const string Key_User = "User";
    //public const string Key_Material = "Material";
    //public const string Key_Unit = "Unit";
    //public const string Key_UnitLevel = "UnitLevel";
    //public const string Key_StageData = "Stage";
    //public const string Key_UnitIDData = "UnitIDData";
    //public const string Key_Coalescenece = "CoalesceneceData";
    //public const string Key_Utility = "Utility";
    //public const string Key_Version = "Version";
    //public const string Key_Reward = "Reward";
    //public const string Key_Collection = "Collection";
    //public const string Key_Tutorial = "Tutorial";
    //
    //// key
    //public const string Money = "MONEY";
    //public const string Diamond = "DIAMOND";
    //public const string Evolution = "EVOLUTION";
    //public const string ActionEnergy = "ACTIONENERGY";
    //
    //public const string Copper = "COPPER";
    //public const string Zinc = "ZINC";
    //public const string Aluminum = "ALUMINUM";
    //public const string Steel = "STEEL";
    //public const string Gold = "GOLD";
    //public const string Oil = "OIL";
    //
    //public const string Boat = "BOAT";
    //public const string Weapon = "WEAPON";
    //public const string Defense = "DEFENSE";
    //public const string Captain = "CAPTAIN";
    //public const string Sailor = "SAILOR";
    //public const string Engine = "ENGINE";
    //
    //public const string BoatLevel = "BOATLEVEL";
    //public const string WeaponLevel = "WEAPONLEVEL";
    //public const string DefenseLevel = "DEFENSELEVEL";
    //public const string CaptainLevel = "CAPTAINLEVEL";
    //public const string SailorLevel = "SAILORLEVEL";
    //public const string EngineLevel = "ENGINELEVEL";
    //
    //public const string StageID = "STAGEID";
    //public const string UnitCountID = "UNITCOUNTID";
    //
    //public const string BoatID = "BOATID"; // 보트 메인 자리
    //public const string BoatSPID = "BOATSPID"; // 보트 서포트 자리
    //public const string BoatSPPos = "BOATSPPOS";
    //
    //public const string BoatNumID = "BOATNUMID";
    //public const string Coalescenece = "COALESCENECE";
    //
    //public const string UserLevel = "UserLevel";
    //public const string UserExp = "UserExp";
    //public const string TalentLevel = "TALENTLEVEL";
    //public const string TalentSkillLevel = "TALENTSKILLLEVEL";
    //
    //public const string Version = "VERSION";
    //
    //public const string Shop_Package0 = "PACKAGE0";
    //public const string Shop_Package1 = "PACKAGE1";
    //public const string Shop_Package2 = "PACKAGE2";
    //public const string Reward_OneDay_Count = "ONEDAYCOUNT";
    //
    //public const string TIME_FORMAT = "stTime_{0}";
    //
    //public const string Collection = "COLLECTION";
    //
    //public const string Tutorial = "TUTORIAL";

    public void JsonInfo(MyStructData.STUserData data)
    {
        // 배열안의 내용
        //SimpleJSON.JSONClass enptyKey = new SimpleJSON.JSONClass();
        //SimpleJSON.JSONData value1 = new SimpleJSON.JSONData(data._money.ToString());
        //SimpleJSON.JSONData value2 = new SimpleJSON.JSONData(data._diamond.ToString());
        //SimpleJSON.JSONData value10 = new SimpleJSON.JSONData(data._evolution.ToString());
        //SimpleJSON.JSONData value9 = new SimpleJSON.JSONData(data._actionEnergy.ToString());
        //SimpleJSON.JSONData value3 = new SimpleJSON.JSONData(data._copper.ToString());
        //SimpleJSON.JSONData value4 = new SimpleJSON.JSONData(data._zinc.ToString());
        //SimpleJSON.JSONData value5 = new SimpleJSON.JSONData(data._aluminum.ToString());
        //SimpleJSON.JSONData value6 = new SimpleJSON.JSONData(data._steel.ToString());
        //SimpleJSON.JSONData value7 = new SimpleJSON.JSONData(data._gold.ToString());
        //SimpleJSON.JSONData value8 = new SimpleJSON.JSONData(data._oil.ToString());
        //
        //
        //enptyKey.Add(Money, value1);
        //enptyKey.Add(Diamond, value2);
        //enptyKey.Add(Evolution, value10);
        //enptyKey.Add(ActionEnergy, value9);
        //enptyKey.Add(Copper, value3);
        //enptyKey.Add(Zinc, value4);
        //enptyKey.Add(Aluminum, value5);
        //enptyKey.Add(Steel, value6);
        //enptyKey.Add(Gold, value7);
        //enptyKey.Add(Oil, value8);

        //_array.Add(enptyKey);
    }
    //public void JsonInfo(MyStructData.STUnitData data)
    //{
    //    // 배열안의 내용
    //    SimpleJSON.JSONClass enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._boat.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value1 = new SimpleJSON.JSONData(data._boat[i].ToString());
    //
    //        enptyKey.Add(Boat + (i + 1), value1);
    //    }
    //    _array.Add(enptyKey);
    //
    //    enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._weapon.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value2 = new SimpleJSON.JSONData(data._weapon[i].ToString());
    //
    //        enptyKey.Add(Weapon + (i + 1), value2);
    //    }
    //    _array.Add(enptyKey);
    //
    //    enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._defense.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value2 = new SimpleJSON.JSONData(data._defense[i].ToString());
    //
    //        enptyKey.Add(Defense + (i + 1), value2);
    //    }
    //    _array.Add(enptyKey);
    //
    //    enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._captain.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value2 = new SimpleJSON.JSONData(data._captain[i].ToString());
    //
    //        enptyKey.Add(Captain + (i + 1), value2);
    //    }
    //    _array.Add(enptyKey);
    //
    //    enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._sailor.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value2 = new SimpleJSON.JSONData(data._sailor[i].ToString());
    //
    //        enptyKey.Add(Sailor + (i + 1), value2);
    //    }
    //    _array.Add(enptyKey);
    //
    //    enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._engine.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value2 = new SimpleJSON.JSONData(data._engine[i].ToString());
    //
    //        enptyKey.Add(Engine + (i + 1), value2);
    //    }
    //    _array.Add(enptyKey);
    //
    //}
    //public void JsonInfo(MyStructData.STUnitLevelData data)
    //{
    //    // 배열안의 내용
    //    SimpleJSON.JSONClass enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._boatLevel.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value1 = new SimpleJSON.JSONData(data._boatLevel[i].ToString());
    //
    //        enptyKey.Add(BoatLevel + (i + 1), value1);
    //    }
    //    _array.Add(enptyKey);
    //
    //    enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._weaponLevel.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value2 = new SimpleJSON.JSONData(data._weaponLevel[i].ToString());
    //
    //        enptyKey.Add(WeaponLevel + (i + 1), value2);
    //    }
    //    _array.Add(enptyKey);
    //
    //    enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._defenseLevel.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value2 = new SimpleJSON.JSONData(data._defenseLevel[i].ToString());
    //
    //        enptyKey.Add(DefenseLevel + (i + 1), value2);
    //    }
    //    _array.Add(enptyKey);
    //
    //    enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._captainLevel.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value2 = new SimpleJSON.JSONData(data._captainLevel[i].ToString());
    //
    //        enptyKey.Add(CaptainLevel + (i + 1), value2);
    //    }
    //    _array.Add(enptyKey);
    //
    //    enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._sailorLevel.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value2 = new SimpleJSON.JSONData(data._sailorLevel[i].ToString());
    //
    //        enptyKey.Add(SailorLevel + (i + 1), value2);
    //    }
    //    _array.Add(enptyKey);
    //
    //    enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._engineLevel.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value2 = new SimpleJSON.JSONData(data._engineLevel[i].ToString());
    //
    //        enptyKey.Add(EngineLevel + (i + 1), value2);
    //    }
    //    _array.Add(enptyKey);
    //}
    //public void JsonInfo(MyStructData.STStageData data)
    //{
    //    // 배열안의 내용
    //    SimpleJSON.JSONClass enptyKey = new SimpleJSON.JSONClass();
    //    SimpleJSON.JSONData value2 = new SimpleJSON.JSONData(data._stagePos.ToString());
    //    enptyKey.Add(StageID, value2);
    //
    //    _array.Add(enptyKey);
    //}
    //public void JsonInfo(MyStructData.STUnitIDCount data)
    //{
    //    // 배열안의 내용
    //    SimpleJSON.JSONClass enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._id.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value1 = new SimpleJSON.JSONData(data._id[i].ToString());
    //
    //        enptyKey.Add(UnitCountID + (i + 1), value1);
    //    }
    //    _array.Add(enptyKey);
    //}
    //
    //public void JsonInfo(MyStructData.STUnitSelectData data)
    //{
    //    // 배열안의 내용
    //    SimpleJSON.JSONClass enptyKey = new SimpleJSON.JSONClass();
    //    SimpleJSON.JSONData value2 = new SimpleJSON.JSONData(data._boatPos.ToString());
    //    enptyKey.Add(BoatID, value2);
    //    _array.Add(enptyKey);
    //
    //    enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._boatSPId.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value = new SimpleJSON.JSONData(data._boatSPId[i].ToString());
    //
    //        enptyKey.Add(BoatSPID + (i + 1), value);
    //    }
    //    _array.Add(enptyKey);
    //
    //    enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._boatSPPos.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value = new SimpleJSON.JSONData(data._boatSPPos[i].ToString());
    //
    //        enptyKey.Add(BoatSPPos + (i + 1), value);
    //    }
    //    _array.Add(enptyKey);
    //}
    //
    //public void JsonInfo(MyStructData.STCoalescenceData data)
    //{
    //    // 배열안의 내용
    //    SimpleJSON.JSONClass enptyKey = new SimpleJSON.JSONClass();
    //
    //    for (int i = 0; i < data._boatNumID.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value = new SimpleJSON.JSONData(data._boatNumID[i]);
    //
    //        enptyKey.Add(BoatNumID + (i + 1), value);
    //    }
    //    _array.Add(enptyKey);
    //
    //    enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._coalescenece.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value = new SimpleJSON.JSONData(data._coalescenece[i]);
    //
    //        enptyKey.Add(Coalescenece + (i + 1), value);
    //    }
    //    _array.Add(enptyKey);
    //}
    //
    //public void JsonInfo(MyStructData.STUtilityData data)
    //{
    //    // 배열안의 내용
    //    SimpleJSON.JSONClass enptyKey = new SimpleJSON.JSONClass();
    //    SimpleJSON.JSONData value0 = new SimpleJSON.JSONData(data._userLevel.ToString());
    //    SimpleJSON.JSONData value1 = new SimpleJSON.JSONData(data._userExp.ToString());
    //
    //    enptyKey.Add(UserLevel, value0);
    //    enptyKey.Add(UserExp, value1);
    //
    //    _array.Add(enptyKey);
    //}
    //public void JsonInfo(MyStructData.STVersionData data)
    //{
    //    // 배열안의 내용
    //    SimpleJSON.JSONClass enptyKey = new SimpleJSON.JSONClass();
    //    SimpleJSON.JSONData value1 = new SimpleJSON.JSONData(data._version.ToString());
    //
    //    enptyKey.Add(Version, value1);
    //
    //    _array.Add(enptyKey);
    //}
    //public void JsonInfo(MyStructData.STRewardTimeData data)
    //{
    //    // 배열안의 내용
    //    SimpleJSON.JSONClass enptyKey = new SimpleJSON.JSONClass();
    //    int max = System.Enum.GetValues(typeof(ETimeCheckType)).Length;
    //    for (int i = 0; i <= max; i++)
    //    {
    //        enptyKey.Add(string.Format(TIME_FORMAT, i), new SimpleJSON.JSONData(data._stTimeArr[i]));
    //    }
    //    _array.Add(enptyKey);
    //}
    //
    //public void JsonInfo(MyStructData.STCollectionData data)
    //{
    //    // 배열안의 내용
    //    SimpleJSON.JSONClass enptyKey = new SimpleJSON.JSONClass();
    //    for (int i = 0; i < data._collectionList.Count; ++i)
    //    {
    //        SimpleJSON.JSONData value1 = new SimpleJSON.JSONData(data._collectionList[i].ToString());
    //
    //        enptyKey.Add(Collection + (i + 1), value1);
    //    }
    //    _array.Add(enptyKey);
    //}
    #endregion


    public void JsonSave()
    {
#if UNITY_EDITOR
        string path = "Assets/Resources/" + _filePath + ".json";
#elif UNITY_ANDROID
        string path = _filePath + ".json";
#endif
        _title.Add(_className, _array);
        //string en = Encrypt(_title.ToString(""), "key");
        //System.IO.File.WriteAllText(path, en); // 보안
        //System.IO.File.WriteAllText(path, _title.ToString("")); // 일반

        //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(path);
        //string format = System.Convert.ToBase64String(bytes);
        //System.IO.File.WriteAllText(path, format);
    }

    #region JsonLoad
    public void JsonLoad(ref MyStructData.STUserData data, string filePath, string className)
    {
        //var json = JsonLoadPath(filePath);
        //var list = json[className];
        //
        //for (int i = 0; i < list.Count; ++i)
        //{
        //    var info = list[i];
        //
        //    MyStructData.STUserData temp = new MyStructData.STUserData()
        //    {
        //        _money = int.Parse(info[Money]),
        //        _diamond = int.Parse(info[Diamond]),
        //        _evolution = int.Parse(info[Evolution]),
        //        _actionEnergy = int.Parse(info[ActionEnergy]),
        //        _copper = int.Parse(info[Copper]),
        //        _zinc = int.Parse(info[Zinc]),
        //        _aluminum = int.Parse(info[Aluminum]),
        //        _steel = int.Parse(info[Steel]),
        //        _gold = int.Parse(info[Gold]),
        //        _oil = int.Parse(info[Oil]),
        //    };
        //
        //    data = temp;
        //}
        //Debug.Log("UserData Load Success");
    }
    //public void JsonLoad(ref MyStructData.STUnitData data, string filePath, string className)
    //{
    //    var json = JsonLoadPath(filePath);
    //    var list = json[className];
    //
    //    MyStructData.STUnitData temp = new MyStructData.STUnitData();
    //    var info = list[0];
    //    temp._boat = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._boat.Add(int.Parse(info[Boat + (i + 1)]));
    //    }
    //    info = list[1];
    //    temp._weapon = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._weapon.Add(int.Parse(info[Weapon + (i + 1)]));
    //    }
    //    info = list[2];
    //    temp._defense = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._defense.Add(int.Parse(info[Defense + (i + 1)]));
    //    }
    //    info = list[3];
    //    temp._captain = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._captain.Add(int.Parse(info[Captain + (i + 1)]));
    //    }
    //    info = list[4];
    //    temp._sailor = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._sailor.Add(int.Parse(info[Sailor + (i + 1)]));
    //    }
    //    info = list[5];
    //    temp._engine = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._engine.Add(int.Parse(info[Engine + (i + 1)]));
    //    }
    //
    //    data = temp;
    //    Debug.Log("UnitData Load Success");
    //}
    //public void JsonLoad(ref MyStructData.STUnitLevelData data, string filePath, string className)
    //{
    //    var json = JsonLoadPath(filePath);
    //    var list = json[className];
    //
    //    MyStructData.STUnitLevelData temp = new MyStructData.STUnitLevelData();
    //
    //    var info = list[0];
    //    temp._boatLevel = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._boatLevel.Add(int.Parse(info[BoatLevel + (i + 1)]));
    //    }
    //    info = list[1];
    //    temp._weaponLevel = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._weaponLevel.Add(int.Parse(info[WeaponLevel + (i + 1)]));
    //    }
    //    info = list[2];
    //    temp._defenseLevel = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._defenseLevel.Add(int.Parse(info[DefenseLevel + (i + 1)]));
    //    }
    //    info = list[3];
    //    temp._captainLevel = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._captainLevel.Add(int.Parse(info[CaptainLevel + (i + 1)]));
    //    }
    //    info = list[4];
    //    temp._sailorLevel = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._sailorLevel.Add(int.Parse(info[SailorLevel + (i + 1)]));
    //    }
    //    info = list[5];
    //    temp._engineLevel = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._engineLevel.Add(int.Parse(info[EngineLevel + (i + 1)]));
    //    }
    //
    //    data = temp;
    //    Debug.Log("UnitLevelData Load Success");
    //}
    //
    //public void JsonLoad(ref MyStructData.STStageData data, string filePath, string className)
    //{
    //    var json = JsonLoadPath(filePath);
    //    var list = json[className];
    //
    //    MyStructData.STStageData temp = new MyStructData.STStageData();
    //    var info = list[0];
    //    temp._stagePos = int.Parse(info[StageID]);
    //
    //    data = temp;
    //    Debug.Log("StageData Load Success");
    //}
    //public void JsonLoad(ref MyStructData.STUnitIDCount data, string filePath, string className)
    //{
    //    var json = JsonLoadPath(filePath);
    //    var list = json[className];
    //
    //    MyStructData.STUnitIDCount temp = new MyStructData.STUnitIDCount();
    //    var info = list[0];
    //    temp._id = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._id.Add(int.Parse(info[UnitCountID + (i + 1)]));
    //    }
    //
    //    data = temp;
    //    Debug.Log("UnitIDCount Load Success");
    //}
    //
    //public void JsonLoad(ref MyStructData.STUnitSelectData data, string filePath, string className)
    //{
    //    var json = JsonLoadPath(filePath);
    //    var list = json[className];
    //
    //    MyStructData.STUnitSelectData temp = new MyStructData.STUnitSelectData();
    //
    //    var info = list[0];
    //    temp._boatPos = int.Parse(info[BoatID]);
    //
    //    info = list[1];
    //    temp._boatSPId = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._boatSPId.Add(int.Parse(info[BoatSPID + (i + 1)]));
    //    }
    //    info = list[2];
    //    temp._boatSPPos = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._boatSPPos.Add(int.Parse(info[BoatSPPos + (i + 1)]));
    //    }
    //
    //    data = temp;
    //    Debug.Log("UnitSelectData Load Success");
    //}
    //
    //public void JsonLoad(ref MyStructData.STCoalescenceData data, string filePath, string className)
    //{
    //    var json = JsonLoadPath(filePath);
    //    var list = json[className];
    //
    //    MyStructData.STCoalescenceData temp = new MyStructData.STCoalescenceData();
    //
    //    var info = list[0];
    //    temp._boatNumID = new List<string>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._boatNumID.Add(info[BoatNumID + (i + 1)]);
    //    }
    //    info = list[1];
    //    temp._coalescenece = new List<string>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._coalescenece.Add(info[Coalescenece + (i + 1)]);
    //    }
    //    
    //    data = temp;
    //    Debug.Log("CoalescenceData Load Success");
    //}
    //
    //public void JsonLoad(ref MyStructData.STUtilityData data, string filePath, string className)
    //{
    //    var json = JsonLoadPath(filePath);
    //    if (json == null) { return; }
    //    var list = json[className];
    //
    //    MyStructData.STUtilityData temp = new MyStructData.STUtilityData();
    //    var info = list[0];
    //    temp._userLevel = int.Parse(info[UserLevel]);
    //    temp._userExp = int.Parse(info[UserExp]);
    //    data = temp;
    //
    //    Debug.Log("UtilityData Load Success");
    //}
    //
    //public void JsonLoad(ref MyStructData.STVersionData data, string filePath, string className)
    //{
    //    var json = JsonLoadPath(filePath);
    //    if (json == null) { return; }
    //    var list = json[className];
    //
    //    MyStructData.STVersionData temp = new MyStructData.STVersionData();
    //    var info = list[0];
    //    temp._version = int.Parse(info[Version]);
    //    data = temp;
    //
    //    Debug.Log("VersionData Load Success");
    //}
    //
    //public void JsonLoad(ref MyStructData.STRewardTimeData data, string filePath, string className)
    //{
    //    var json = JsonLoadPath(filePath);
    //    var list = json[className];
    //
    //    for (int i = 0; i < list.Count; ++i)
    //    {
    //        var info = list[i];
    //        MyStructData.STRewardTimeData temp = new MyStructData.STRewardTimeData();
    //        temp.Init();
    //
    //        int max = System.Enum.GetValues(typeof(ETimeCheckType)).Length;
    //        for (int j = 0; j < max; j++)
    //        {
    //            temp._stTimeArr[j] = info[string.Format(TIME_FORMAT, j)];
    //        }
    //
    //        data = temp;
    //    }
    //    Debug.Log("RewardTime Load Success");
    //}
    //public void JsonLoad(ref MyStructData.STCollectionData data, string filePath, string className)
    //{
    //    var json = JsonLoadPath(filePath);
    //    var list = json[className];
    //
    //    MyStructData.STCollectionData temp = new MyStructData.STCollectionData();
    //    var info = list[0];
    //    temp._collectionList = new List<int>();
    //    for (int i = 0; i < info.Count; ++i)
    //    {
    //        temp._collectionList.Add(int.Parse(info[Collection + (i + 1)]));
    //    }
    //
    //    data = temp;
    //    Debug.Log("Collection Load Success");
    //}

    SimpleJSON.JSONNode JsonLoadPath(string filePath)
    {
        // 파일의 경로
#if UNITY_EDITOR
        //string path = "JsonSaveLoad/" + filePath;
        //
        //// 파일을 가져온다
        //var textAsset = Resources.Load<TextAsset>(path);
        //string de = Decrypt(textAsset.text, "key");
        //var json = SimpleJSON.JSON.Parse(de); // 보안
        ////var json = SimpleJSON.JSON.Parse(textAsset.text); // 일반
        ///
        string path = "JsonSaveLoad/" + filePath;

        // 파일 있는지 체크
        FileInfo fileInfo = new FileInfo("Assets/Resources/" + path + ".json");
        if (!fileInfo.Exists) { Debug.Log(filePath + "파일이 없습니다."); return null; }

        // 파일을 가져온다
        var textAsset = Resources.Load<TextAsset>(path);
        string de = Decrypt(textAsset.text, "key");
        var json = SimpleJSON.JSON.Parse(de); // 보안
        //var json = SimpleJSON.JSON.Parse(textAsset.text); // 일반
#elif UNITY_ANDROID
        //string path = Application.persistentDataPath + "/" + filePath + ".json";
        //
        //// 파일을 가져온다
        //string jsonPath = File.ReadAllText(path);
        //string de = Decrypt(jsonPath, "key");
        //var json = SimpleJSON.JSON.Parse(de); // var json = SimpleJSON.JSON.Parse(jsonPath);

        string path = Application.persistentDataPath + "/" + filePath + ".json";

        // 파일 있는지 체크
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists) { Debug.Log(filePath + "파일이 없습니다."); return null; }

        // 파일을 가져온다
        string jsonPath = File.ReadAllText(path);
        string de = Decrypt(jsonPath, "key");
        var json = SimpleJSON.JSON.Parse(de); // var json = SimpleJSON.JSON.Parse(jsonPath);
#endif
        return json;
    }
    #endregion

    // 파일 지우는 코드
    public void FileDelete(string filePath)
    {
#if UNITY_EDITOR
        string path = "Assets/Resources/" + "JsonSaveLoad/" + filePath + ".json";

        // 파일을 가져온다
        //var textAsset = Resources.Load<TextAsset>(path);
        //string textAsset = File.ReadAllText(path);
        // 파일을 가져온다
        //var textAsset = Resources.Load<TextAsset>(path);
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists) { Debug.Log(filePath + "파일이 없습니다."); return; }
        string textAsset = File.ReadAllText(path);
#elif UNITY_ANDROID
        //string path = Application.persistentDataPath + "/" + filePath + ".json";
        //
        //// 파일을 가져온다
        //string textAsset = File.ReadAllText(path);

        string path = Application.persistentDataPath + "/" + filePath + ".json";

        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists) { Debug.Log(filePath + "파일이 없습니다."); return; }

        // 파일을 가져온다
        string textAsset = File.ReadAllText(path);
#endif

        if (textAsset != null)
        {
            File.Delete(path);
        }
    }

    // 암호파일로 만들어 주는 코드 (클래스이름, 암호명)
    public static string Encrypt(string textToEncrypt, string key)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();
        rijndaelCipher.Mode = CipherMode.CBC;
        rijndaelCipher.Padding = PaddingMode.PKCS7;
        rijndaelCipher.KeySize = 128;
        rijndaelCipher.BlockSize = 128;

        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
        byte[] keyBytes = new byte[16];

        int len = pwdBytes.Length;

        if (len > keyBytes.Length)
        {
            len = keyBytes.Length;
        }

        System.Array.Copy(pwdBytes, keyBytes, len);

        rijndaelCipher.Key = keyBytes;
        rijndaelCipher.IV = keyBytes;

        ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

        byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);

        return System.Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
    }

    // 암호파일 풀어주는 코드
    public static string Decrypt(string textToDecrypt, string key)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();
        rijndaelCipher.Mode = CipherMode.CBC;
        rijndaelCipher.Padding = PaddingMode.PKCS7;
        rijndaelCipher.KeySize = 128;
        rijndaelCipher.BlockSize = 128;

        byte[] encryptedData = System.Convert.FromBase64String(textToDecrypt);
        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
        byte[] keyBytes = new byte[16];

        int len = pwdBytes.Length;

        if (len > keyBytes.Length)
        {
            len = keyBytes.Length;
        }

        System.Array.Copy(pwdBytes, keyBytes, len);

        rijndaelCipher.Key = keyBytes;
        rijndaelCipher.IV = keyBytes;

        byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

        return Encoding.UTF8.GetString(plainText);
    }
}
