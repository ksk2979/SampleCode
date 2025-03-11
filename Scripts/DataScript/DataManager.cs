using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using System;
using System.Linq;

namespace MyData
{
    /// <summary>
    /// 데이터들을 로드하여 딕셔너리에 저정해 가지고 있는다
    /// </summary>
    public class DataManager : Singleton<DataManager>
    {
        //public const int _stageSize = 7; // 총 스테이지 수

        protected override void Awake()
        {
            base.Awake();
            InitLoad();
        }
        ///// <summary>
        ///// 데이터들을 로드한다
        ///// </summary>
        private DataList dataList;
        public void InitLoad()
        {
            dataList = Resources.Load<DataList>("Data/DataList");
            LoadData(KEY_BOAT, dataList.boatDataList);
            LoadData(KEY_CAPTAIN, dataList.captainDataList);
            LoadData(KEY_DEFENSE, dataList.defenseDataList);
            LoadData(KEY_ENEMY, dataList.enemyDataList);
            LoadData(KEY_ENGINE, dataList.engineDataList);
            LoadData(KEY_SAILOR, dataList.sailorDataList);
            LoadData(KEY_WEAPON, dataList.weaponDataList);
            LoadData(KEY_QUEST, dataList.questDataList);
            LoadData(KEY_UPGRADE, dataList.upgradeDataList);
            LoadData(KEY_GRADE, dataList.gradeDataList);
            LoadData(KEY_POTENTIAL, dataList.potentialDataList);
            LoadData(KEY_LEVEL, dataList.levelDataList);
            LoadData(KEY_ONEDAYREWARD, dataList.oneDayrewardDataList);
            LoadData(KEY_SEASONPASS, dataList.seasonPassDataList);
            LoadData(KEY_SHOPPRODUCT, dataList.shopProductDataList);
            LoadData(KEY_COLLECTION, dataList.collectionDataList);
            LoadData(KEY_STAGE, dataList.stageDataList);
            LoadData(KEY_ABILITY, dataList.abilityDataList);
            LoadData(KEY_ACTIVESKILL, dataList.activeSkillDataList);
            LoadData(KEY_BADWORD, dataList.badWordDataList);
            LoadData(KEY_LANGUAGE, dataList.languageDataList);
            LoadData(KEY_TUTORIAL, dataList.tutorialDataList);
        }

        public void LoadData<T>(string key, List<T> qList) where T: BData
        {
            //Debug.Log("load " + key);
            Dictionary<object, BData> dataDic = new Dictionary<object, BData>();
            foreach (BData data in qList)
                dataDic.Add(data.GetKey(), data);
            AddDataDictionary(key, dataDic);
        }

        #region key ===============================================================================================
        public const string KEY_DATAPATH = "Data/";
        public const string KEY_BOAT = "BoatData";
        public const string KEY_CAPTAIN = "CaptainData";
        public const string KEY_DEFENSE = "DefenseData";
        public const string KEY_ENGINE = "EngineData";
        public const string KEY_SAILOR = "SailorData";
        public const string KEY_WEAPON = "WeaponData";
        public const string KEY_COLLECTION = "CollectionData";
        public const string KEY_PLAYERLEVEL = "PlayerLevelData";
        public const string KEY_QUEST = "QuestData";
        public const string KEY_GRADE = "GradeData";
        public const string KEY_UPGRADE = "UpgradeData";
        public const string KEY_ONEDAYREWARD = "OneDayRewardData";
        public const string KEY_SEASONPASS = "SeasonPassData";
        public const string KEY_SHOPPRODUCT = "ShopProductData";
        public const string KEY_STAGE = "StageData";
        public const string KEY_ENEMY = "EnemyData";
        public const string KEY_ABILITY = "AbilityData";
        public const string KEY_POTENTIAL = "ItemPotentialData";
        public const string KEY_ACTIVESKILL = "ActiveSkillData";
        public const string KEY_LEVEL = "LevelData";
        public const string KEY_TUTORIAL = "TutorialData";

        public const string KEY_BADWORD = "BadWordData";
        public const string KEY_LANGUAGE = "LanguageData";
        #endregion key ===============================================================================================

        #region 데이터 운영 부분  =========================================================================================
        Dictionary<string, Dictionary<object, BData>> _dataDicDic = new Dictionary<string, Dictionary<object, BData>>();
        

        public void AddDataDictionary(string dataDicKey_, Dictionary<object, BData> dataDic_)
        {
            _dataDicDic.Add(dataDicKey_, dataDic_);
        }

        /// <summary>
        /// 키 하나인 애들 중 키값으로 데이터 하나를 찾음
        /// </summary>
        public BData FindData(string dataDicKey_, object dataKey_)
        {
            Dictionary<object, BData> dataDic = null;
            _dataDicDic.TryGetValue(dataDicKey_, out dataDic);
            if (dataDic == null)
            {
                Debug.LogError("Not found data dictionary. Key : " + dataDicKey_);
                return null;
            }
            BData data = null;
            dataDic.TryGetValue(dataKey_, out data);

            return data;
        }
        /// <summary>
        /// 키 하나인 애들 중 키값으로 데이터 하나를 찾음
        /// </summary>
        public T FindData<T>(string dataDicKey_, object dataKey_)
        {
            Dictionary<object, BData> dataDic = null;
            _dataDicDic.TryGetValue(dataDicKey_, out dataDic);
            if (dataDic == null)
            {
                Debug.LogError("Not found data dictionary. Key : " + dataDicKey_);
                return default(T);
            }
            BData data = null;
            dataDic.TryGetValue(dataKey_, out data);
            return (T)Convert.ChangeType(data, typeof(T));
        }
        /// <summary>
        /// 해당 데이터 모두를 읽음
        /// </summary>
        public List<T> GetList<T>(string dataDicKey_)
        {
            Dictionary<object, BData> dataDic = null;
            _dataDicDic.TryGetValue(dataDicKey_, out dataDic);
            List<T> listData = new List<T>();

            foreach (KeyValuePair<object, BData> data in dataDic)
            {
                listData.Add((T)Convert.ChangeType(data.Value, typeof(T)) );
            }
            return listData;
        }

        /// <summary>
        /// 투키인 해당 하나의 데이터를 찾음
        /// </summary>
        public T GetData<T>(string dataDicKey_, object dataKey1_, object dataKey2_)
        {
            DoubleKey doubleKey = new DoubleKey(dataKey1_, dataKey2_);
            BData data = FindData(dataDicKey_, doubleKey);
            return (T)Convert.ChangeType(data, typeof(T));
        }

        /// <summary>
        /// 투키인 해당 하나의 데이터를 찾음
        /// </summary>
        public BData GetData(string dataDicKey_, object dataKey1_, object dataKey2_)
        {
            DoubleKey doubleKey = new DoubleKey(dataKey1_, dataKey2_);
            BData data = FindData(dataDicKey_, doubleKey);
            return data;
        }

        public void SerializeAllAddPath(string dataname)
        {
            SerializeAll(KEY_DATAPATH + dataname);
        }

        public void SerializeAll(string directoryPath_)
        {
            TextAsset[] textAssets = Resources.LoadAll<TextAsset>(directoryPath_);

            foreach (TextAsset textAsset in textAssets)
            {
                //Debug.Log(textAsset.name);
                SerializeJson(textAsset.text);
            }
        }

        public void SerializeTextAsset(string assetPath_)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(assetPath_);
            SerializeJson(textAsset.text);
        }
        public void SerializeTextAsset(string directoryPath_, string assetName_)
        {
            SerializeTextAsset(string.Format("{0}/{1}", directoryPath_, assetName_));
        }

        public void SerializeJson(string json_)
        {
            JsonReaderSettings jsonReaderSettings = new JsonReaderSettings();
            jsonReaderSettings.TypeHintName = "__type";
            JsonReader jsonReader = new JsonReader(json_, jsonReaderSettings);
            Dictionary<string, object> dic = (Dictionary<string, object>)jsonReader.Deserialize();

            foreach (KeyValuePair<string, object> kvp in dic)
            {
                Debug.Log( kvp.Key);
                //Debug.Log(string.Format("{0} {1}", kvp.Key, kvp.Value));

                BData[] datas = kvp.Value as BData[];

                Dictionary<object, BData> dataDic = new Dictionary<object, BData>();

                foreach (BData data in datas)
                {
                    dataDic.Add(data.GetKey(), data);
                    //Debug.Log(data.ToString());
                }

                AddDataDictionary(kvp.Key, dataDic);
            }
        }

        #endregion 데이터 운영 부분  =========================================================================================
    }

    public interface IDataInterface
    {
        int MaxLevel { get; }
    }

    [System.Serializable]
    public class LevelData :SingleKeyData_Int
    {
        public int exp;
        public string rewardType;
        public string rewardValue;
    }
    [System.Serializable]
    public class EnemyData : SingleKeyData_Int
    {
        public string name;
        public string resName;
        public float attackDamge;
        public float hp;
        public float defensivePower;
        public float normalMoveSpeed;
        public float traceMoveSpeed;
        public float attackDist;
        public float attackRange;
        public float attackAngle;
        public float attackSpeed;
        public float patrollRange;
        public float IdleTime;
        public float traceRange;
        public float findEnemyAngle;
        public float rotationSpeed;
        public List<int> skill01;
        public List<int> skill02;
        //public string bullet;
    }
    
    [System.Serializable]
    public class BoatData : DoubleKeyData_Int
    {
        public int equipType;
        public int basicWeaponType; // 기본 착용 무기
        public List<int> weaponType; // 착용 가능한 무기 타입
        public int maxLevel;
        public string name;
        public string resName;
        public int grade;
        public float hp;
        public float addHp;
        public float damage;
        public float addDamage;
        public float speedAccelate;
        public float moveSpeed;
        public float defensive;
        public float rotationSpeed; //배의 기본 회전속도
        public float rotationAccelateSpeed; //배의 선회시 속도 감속 0~ 500
        public float braking; //제동력 관련 멈출때 관성으로 나아가는것을 줄여준다. 0~500
        public List<int> needMatAmount; //재료량
        public string explanation;
    }
    [System.Serializable]
    public class CaptainData : DoubleKeyData_Int
    {
        public int equipType;
        public int maxLevel;
        public string name;
        public string resName;
        public int grade;
        public float damage;
        public float addDamage;
        public List<int> needMatAmount;
        public string explanation;
    }

    [System.Serializable]
    public class DefenseData : DoubleKeyData_Int
    {
        public int equipType;
        public int maxLevel;
        public string name;
        public string resName;
        public int defensePoint;
        public int grade;
        public float value;
        public float addValue;
        public float damage;
        public float addDamage;
        public List<int> needMatAmount;
        public string explanation;
    }

    [System.Serializable]
    public class EngineData : DoubleKeyData_Int
    {
        public int equipType;
        public int maxLevel;
        public string name;
        public string resName;
        public int grade;
        public float value;
        public float addValue;
        public float speedAccelate;
        public List<int> needMatAmount;
        public string explanation;
    }

    [System.Serializable]
    public class SailorData : DoubleKeyData_Int
    {
        public int equipType;
        public int maxLevel;
        public string name;
        public string resName;
        public int grade;
        public float value;
        public float addValue;
        public List<int> needMatAmount;
        public string explanation;
    }

    [System.Serializable]
    public class WeaponData : DoubleKeyData_Int
    {
        public int weaponType;
        public int equipType;
        public int maxLevel;
        public string name;
        public string resName;
        public int grade;
        public float damage;
        public float addDamage;
        public float fireRate;
        public float shootingRange;
        public int ability;
        public List<int> needMatAmount;
        public string explanation;
    }

    [System.Serializable]
    public class AbilityData : SingleKeyData_Int
    {
        public string name;
        public float value;
        public int abOperator; // 퍼센트 1, 더하기 2, 빼기 3, 전체값에서 일부값의 퍼센트 계산 4, 숫자(전체값)를 몇 퍼센트 증가시키기 5, 숫자(전체값)를 몇 퍼센트 감소시키기 6
        public int appear;     // 등장 스테이지(시작 기준)
        public int max;        // 최대 레벨
        public string explanation;
    }
    [System.Serializable]
    public class ActiveSkillData : SingleKeyData_Int
    {
        public string name;
        public float value;
        public int unlock;
        public float coolTime;
        public string explanation;
    }
    [System.Serializable]
    public class GradeData : SingleKeyData_Int
    {
        public string text;
        public string outlineColor;
        public string cornerDecoColor;
        public string lightColor;
        public string glowColor;
        public string gradeColor;
    }
    [System.Serializable]
    public class ItemPotentialData : SingleKeyData_Int
    {
        public int abilityEnum;
        public int value;
        public int potenType;  // 포텐셜 타입
        public string name;
        public int max;        // 최대 레벨
        public string explanation;
    }

    [System.Serializable]
    public class CollectionData : SingleKeyData_Int
    {
        public int objective;
        public int rewardType01;
        public int rewardValue01;
        public int rewardType02;
        public int rewardValue02;
    }

    [System.Serializable]
    public class QuestData : SingleKeyData_Int
    {
        public int questType;
        public string questName;
        public int clearCount;
        public int rewardType;
        public int rewardValue;
    }

    [System.Serializable]
    public class UpgradeData : SingleKeyData_Int
    {
        public int level;
        public int cost;
    }
    [System.Serializable]
    public class OneDayRewardData : SingleKeyData_Int
    {
        public int[] rewardType;
        public int[] rewardValue;
    }
    [System.Serializable]
    public class SeasonPassData : SingleKeyData_Int
    {
        public string rewardType01;
        public string rewardValue01;
        public string rewardType02;
        public string rewardValue02;
        public int level;
        public int max;
        public int passNumber;
        public string startDate;           // 시작일
        public string expirationDate;      // 만료일
    }
    [System.Serializable]
    public class ShopProductData :SingleKeyData_Int
    {
        public string productKey;
        public int price;
        public int rewardType;
        public int rewardValue;
    }

    [System.Serializable]
    public class StageData : SingleKeyData_Int
    {
        public string limitTime; // 제한 시간

        public List<int> wave1; // 1 웨이브 몬스터 ID
        public List<int> wave2; // 2 웨이브 몬스터 ID
        public List<int> wave3; // 3 웨이브 몬스터 ID

        public List<float> enemy1; // 1 웨이브 몬스터 ID, 추가 수량
        public List<float> enemy2; // 2 웨이브 몬스터 ID, 추가 수량
        public List<float> enemy3; // 3 웨이브 몬스터 ID, 추가 수량
        public List<float> enemy4;
        public List<float> enemy5;
        public List<float> enemy6;
        
        public List<int> bossId; // 보스의 ID
        public float bossMutiplier; // 보스의 배율
        public List<int> materialSetting; // 재료 설정
        public float stageExp; // 유저 경험치
    }

    [System.Serializable]
    public class TutorialData : SingleKeyData_Int
    {
        public string messageKey;
        public int nextStepID;
        public int hasMessage;
        public string targetUI;
    }

    [System.Serializable]
    public class BadWordData : SingleKeyData_Int
    {
        public string badWord;
    }

    [System.Serializable]
    public class LanguageData : SingleKeyData_String
    {
        public string English;
        public string Korean;
        public string Chinese;
        public string Thai;
        public string Japanese;
        public string French;
        public string Spanish;
        public string German;
        public string Russian;
    }
}




