using JsonFx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyStructData
{
    // 서버 저장
    /// <summary>
    /// 유저 기본데이터
    /// </summary>
    public struct STUserData
    {
        public EUserSaveType SaveType => EUserSaveType.USERDATA;
        public string FilePath => "cache/0a0f22683b13108719fe38d84f20197a";

        public int[] _property;        // 재화 보유

        public void Init()
        {
            _property = new int[(int)EPropertyType.NONE];

            for (int i = 0; i < (int)EPropertyType.NONE; i++)
            {
                _property[i] = 0;
            }
            _property[(int)EPropertyType.MONEY] = 500;
            _property[(int)EPropertyType.DIAMOND] = 9999999; // 테스트 Test
            _property[(int)EPropertyType.ACTIONENERGY] = 30;
        }
    }

    /// <summary>
    /// 시즌패스 데이터
    /// </summary>
    public struct STSeasonPassData
    {
        public EUserSaveType SaveType => EUserSaveType.SEASONPASSDATA;
        public string FilePath => "cache/589i3k7d087ff7754b22a9e8p55tk80p";

        public int _currentSeason;
        public int _level;
        public int _progressCount;
        public bool _isVipActivated;
        public bool _isActivated;

        public List<bool> _normalRewardStateList;
        public List<bool> _vipRewardStateList;
        public void Init()
        {
            _currentSeason = 0;
            _progressCount = 0;
            _level = 0;
            _isVipActivated = false;
            _isActivated = false;

            _normalRewardStateList = new bool[15].ToList();
            _vipRewardStateList = new bool[15].ToList();
        }
    }

    public struct STMaterialData
    {
        public EUserSaveType SaveType => EUserSaveType.MATERIALDATA;
        public string FilePath => "cache/7f92d18f81699eb9991e78798ef2c207";

        public int[] _material;

        public void Init()
        {
            _material = new int[(int)EMaterialType.NONE];

            for (int i = 0; i < (int)EMaterialType.NONE; i++)
            {
                _material[i] = 5;
            }
        }
    }
    /// <summary>
    /// 유저가 들고 있는 전체 아이템들
    /// </summary>
    public struct STUnitData
    {
        public EUserSaveType SaveType => EUserSaveType.UNITDATA;
        public string FilePath => "cache/8ab99fd240be70af95c57f80c4c928f1";

        public List<int> _boat;           // 현재 보트 보유
        public List<int> _weapon;         // 현재 무기 보유
        public List<int> _defense;        // 현재 방어 보유
        public List<int> _captain;        // 현재 함장 보유
        public List<int> _sailor;         // 현재 선원 보유
        public List<int> _engine;         // 현재 엔진 보유

        public void Init()
        {
            _boat = new List<int>();
            _weapon = new List<int>();
            _defense = new List<int>();
            _captain = new List<int>();
            _sailor = new List<int>();
            _engine = new List<int>();

            _boat.Add(1);
            _weapon.Add(12);
            _weapon.Add(13);
        }
    }
    /// <summary>
    /// 유저가 들고 있는 아이템들의 레벨 (위에 UserUnitData와 같아야한다)
    /// </summary>
    public struct STUnitLevelData
    {
        public EUserSaveType SaveType => EUserSaveType.UNITLEVELDATA;
        public string FilePath => "cache/ae3032886919acef861350157bd70cb1";

        public List<int> _boatLevel;
        public List<int> _weaponLevel;
        public List<int> _defenseLevel;
        public List<int> _captainLevel;
        public List<int> _sailorLevel;
        public List<int> _engineLevel;

        public void Init()
        {
            _boatLevel = new List<int>();
            _weaponLevel = new List<int>();
            _defenseLevel = new List<int>();
            _captainLevel = new List<int>();
            _sailorLevel = new List<int>();
            _engineLevel = new List<int>();

            _boatLevel.Add(1);
            _weaponLevel.Add(1);
            _weaponLevel.Add(1);
        }
    }

    public struct STUnitPotentialData
    {
        public EUserSaveType SaveType => EUserSaveType.UNITPOTENTIALDATA;
        public string FilePath => "cache/t775a2d8e65i001d8ow510w37zc66x2t";

        public List<string> _capPotentialID;
        public List<string> _salPotentialID;
        public List<string> _engPotentialID;

        public void Init()
        {
            _capPotentialID = new List<string>();//(1,2,3)
            _salPotentialID = new List<string>();
            _engPotentialID = new List<string>();
        }
    }
    public struct STStageData
    {
        public EUserSaveType SaveType => EUserSaveType.STAGEDATA;
        public string FilePath => "cache/bc692e53e9d157e3a6ec271b613bce82";

        // 현재 몇 스테이지까지 왔는가 체크
        public int _stagePos;

        public void Init()
        {
            _stagePos = 0;
        }
    }
    public struct STUnitIDCount
    {
        public EUserSaveType SaveType => EUserSaveType.UNITCOUNTID;
        public string FilePath => "cache/ffba107378a12a7d334e2dc99078b002";

        public List<int> _id; // ID값의 카운터를 담아두는 리스트 평균적으로 1만이상이 나올수가 있을까?
        public void Init()
        {
            _id = new List<int>();
            _id.Add(1);
            for (int i = 0; i < 65; ++i)
            {
                _id.Add(0);
            }
        }
    }

    /// <summary>
    /// 유저가 맵에 들고 가기 위해 선택하였던 데이터들 (서버에는 미 저장, 로컬 저장) 게임이 삭제되면 이 데이터는 없어짐
    /// </summary>
    public struct STUnitSelectData
    {
        public EUserSaveType SaveType => EUserSaveType.SELECTDATA;
        public string FilePath => "cache/c14d39131a989ff1366546416c8413ab";

        // 선택 했었던 보트
        public int _boatPos;

        // 선택 했었던 유닛 0: 메인, 순서대로 왼,오,위,아
        public List<int> _boatSPId;
        public List<int> _boatSPPos;

        // 광고, 보석 어빌리티 추가
        public bool _adAbilityCheck;     // 체크박스
        public bool _diaAbilityCheck;
        public bool _adAbilityReserved;  // 수령 시 체크옵션
        public bool _diaAbilityReserved;

        // 액티브 스킬 장착 및 해금 상태
        public List<int> _unlockedSkillList;    // 해금 상태
        public int[] _equipedSkillArr;          // 장착 상태
        public void Init()
        {
            // Boat
            _boatPos = 0;
            _boatSPId = new List<int>();
            _boatSPPos = new List<int>();

            // Ability 
            _adAbilityCheck = false;
            _diaAbilityCheck = false;
            _adAbilityReserved = false;
            _diaAbilityReserved = false;

            _equipedSkillArr = new int[2];
            _unlockedSkillList = new List<int>();

            for (int i = 0; i < 5; i++)
            {
                _boatSPId.Add(0);
                _boatSPPos.Add(0);
            }
        }
    }
    /// <summary>
    /// 유저가 인벤토리에서 필요한 아이템들을 선택해서 서버 저장
    /// </summary>
    public struct STCoalescenceData
    {
        public EUserSaveType SaveType => EUserSaveType.COALESCENECEDATA;
        public string FilePath => "cache/a90ff84e230bd45333aac500f06482bf";

        public List<string> _boatNumID;
        public List<string> _coalescence; //검색했던 값의 같은 자리의 착용된 보트데이터 가져오기
        public List<string> _potentials;
        // 예) 데이터는 string으로 "1,1,2,1,3,1,1,1,1,1,0,0"순으로 ,을 기준으로 하나하나 자르면서 12배열로 int값으로 저장후에 불러온다
        // 0은 당연히 미착용

        public void Init()
        {
            _boatNumID = new List<string>();
            _coalescence = new List<string>();
            _potentials = new List<string>();

            // 고유 아이디값 만드는 방법 - id값 + 현재등급 + 앞에 2개를 검사해서 같은 보트 있는거 만큼 카운터
            _boatNumID.Add("110"); // 첫번째기 때문에 110
            _coalescence.Add("1,1,12,1,0,0,0,0,0,0,0,0");
            _potentials.Add("0_0_0");
        }
    }

    // 유틸리티 데이터 (플레이어 레벨 혹은 스킬에 관련된 데이터)
    public struct STUtilityData
    {
        public EUserSaveType SaveType => EUserSaveType.UTILITYDATA;
        public string FilePath => "cache/908ed5fb74c9ac856d175f6f9f061409";

        public int _userLevel, _userExp;

        public void Init()
        {
            _userLevel = 1;
            _userExp = 0;
        }
    }
    // 버전
    public struct STVersionData
    {
        public EUserSaveType SaveType => EUserSaveType.VERSIONDATA;
        public string FilePath => "cache/a5dfb61cd0b3c573d598647f0b5d24d7";

        public int _version;
        public int _serverLogin;

        public void Init()
        {
            _version = 0;
            _serverLogin = 0; // 서버 로그인을 진행하면 1이 된다
        }
    }

    /// <summary>
    /// 일회성 구매 상품 데이터
    /// </summary>
    public struct STSingleUseProductData
    {
        public EUserSaveType SaveType => EUserSaveType.PACKAGEDATA;
        public string FilePath => "cache/f7faa8213c397654f321f48466f6c465";

        // 상품 패키지
        public int _package0;
        public int _package1;
        public int _package2;

        public void Init()
        {
            _package0 = 0;
            _package1 = 0;
            _package2 = 0;
        }
    }

    /// <summary>
    /// 일일 초기화 상품 / 로그인 보상 데이터
    /// </summary>
    public struct STDailyRewardData
    {
        public EUserSaveType SaveType => EUserSaveType.DAILYREWARDDATA;
        public string FilePath => "cache/a67d15f5f31cc846f86acb4845bb4f19";

        // 상점 일일 상품
        public int[] _shopItemOneKey;
        public int[] _shopItemTwoKey;
        public int[] _shopReceiveCount;

        // 일일 보상
        public int _loginRewardCount; // 일일보상 카운터

        // 룰렛 보상(횟수)
        public int _rouletteCount;

        public void Init()
        {
            _shopItemOneKey = new int[6];
            _shopItemTwoKey = new int[6];
            _shopReceiveCount = new int[6];
            _loginRewardCount = 0;
            _rouletteCount = 0;
            for (int i = 0; i < _shopItemOneKey.Length; ++i)
            {
                _shopItemOneKey[i] = 0;
                _shopItemTwoKey[i] = 0;
                _shopReceiveCount[i] = 0;
            }
        }
    }

    // 로컬 저장 광고나 일일보상에 대한 시간 타임 저장
    public struct STRewardTimeData
    {
        public EUserSaveType SaveType => EUserSaveType.REWARDTIMEDATA;
        public string FilePath => "cache/b24531c8556f78ff946511ab6784355c";

        public string[] _stTimeArr;

        public void Init()
        {
            _stTimeArr = new string[Enum.GetValues(typeof(ETimeCheckType)).Length];
            for(int i = 0; i < _stTimeArr.Length; i++)
            {
                _stTimeArr[i] = "0";
            }
        }
    }

    // 도감 컬렉션 데이터
    public struct STCollectionData
    {
        public EUserSaveType SaveType => EUserSaveType.COLLECTIONDATA;
        public string FilePath => "cache/99d2b3487c578ef152e8620ac14b8f06";

        public List<int> _collectionList; // 지금 현재 21마리의 보호종이 있음

        public void Init()
        {
            _collectionList = new List<int>();
            for (int i = 0; i < 21; ++i)
            {
                _collectionList.Add(0);
            }
        }
    }

    public struct STQuestData
    {
        public EUserSaveType SaveType => EUserSaveType.QUESTDATA;
        public string FilePath => "cache/t507xe33275a028s7e684f327a77528e";

        // 퀘스트 기록
        public List<int> dailyQuestList;
        public List<int> mainQuestList;
        public List<int> specialQuestList;

        // 수령 여부
        public List<bool> dailyReceivedList;
        public List<bool> mainReceivedList;
        public void Init()
        {
            dailyQuestList = new List<int>();
            mainQuestList = new List<int>();
            specialQuestList = new List<int>();

            dailyReceivedList = new List<bool>();
            mainReceivedList = new List<bool>();

            // 일일퀘 초기화
            for(int i = 0; i < (int)EDailyQuest.NONE; i++)
            {
                dailyQuestList.Add(0);
                dailyReceivedList.Add(false);
            }

            // 로그인
            dailyQuestList[0] = 1;

            // 메인퀘 초기화 :(1대 3 매칭) -> 웨이브 클리어 기록 1 : 수령 기록 3 
            mainQuestList.Add(0);
            for (int j = 0; j < 3; j++)
            {
                mainReceivedList.Add(false);
            }
        }
    }

    public struct STTutorialData
    {
        public EUserSaveType SaveType => EUserSaveType.TUTORIALDATA;
        public string FilePath => "cache/92c9844f468a6155464b699cd64866aa";

        public int _tutorial;

        public void Init()
        {
            _tutorial = 0;
        }
    }
    public struct STOptionData
    {
        public EUserSaveType SaveType => EUserSaveType.OPTIONDATA;
        public string FilePath => "cache/a67f155c56ab468765dd5413344f4451";

        public int[] _optionType;

        public void Init()
        {
            _optionType = new int[(int)EOptionType.NONE];
            for (int i = 0; i < (int)EOptionType.NONE; i++)
            {
                _optionType[i] = 0;
            }
            _optionType[(int)EOptionType.VIBRATION] = 1;
        }
    }

    public struct STServerSaveData
    {
        public EUserSaveType SaveType => EUserSaveType.SERVERSAVEDATA;
        public string FilePath => "cache/ded07ce50158a4a42eabc900bb4e7081";

        public bool[] _serverSaveArr;

        public void Init()
        {
            _serverSaveArr = new bool[(int)EUserServerSaveType.NONE];
        }
    }
}

public class StandardStructDatas : MonoBehaviour
{

}