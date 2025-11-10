using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESceneIndex
{
    Init = 0,
    InGame = 1,
}

public enum OperatorCategory
{
    persent = 1,
    add,
    sub,
    partial,
    PersentAdd,
    PersentSub,
}

public enum UNIT
{
    Player = 0,
    Enemy,
    Boss
}

public enum EInvenType
{
    MONEY = 0,
    DIAMOND,
    EVOLUTION,
    NONE
}

public enum EUserSaveType
{
    USERDATA = 0,
    REWARDTIMEDATA,
    GAMECURRENCYDATA,
    TICKETDATA,
    CHARACTERCOLLECTIONDATA,
    STAGEDATA,
    PARTYSTATSDATA,
    UNITSELECTDATA,
    SUMMONSDATA,
    MISSIONDATA,
    BOXDATA,
    UTILITYDATA,
    DISPATCHDATA,
    COLLECTIONDATA,
    VERSIONDATA,
    OPTIONDATA,
    SERVERSAVEDATA,
    NONE,
}

public enum ECharacterSaveType
{
    MAIN = 0,
    STATUS,
    ACCS,
    ACCS_SKILL,
    ACCS_SPECIAL,
    NONE,
}

// double형
public enum EDCoinType
{
    GOLD = 0, // 인게임 골드 재화
    GROWTHSTONE,  // 성장석 재화
    UPGRADESTONE, // 강화석 재화
    SKILLBOOK, // 스킬 재화
    NONE,
}
// int형
public enum ECoinType
{
    DIAMOND = 0, // 다이아 재화
    EMBLEM, // 엠블럼
    FAME, // 명성(칭호) 재화
    FREECOIN, // 무료 재화
    MILEAGE, // 마일리지 재화
    NONE,
}
/// <summary>
/// double재화 1 ~ 99 / int 재화 100 ~
/// </summary>
public enum ERewardType
{
    GOLD = 0,
    GROWTHSTONE = 1,
    UPGRADESTONE = 2,
    SKILLBOOK = 3,
    DIAMOND = 100,
    EMBLEM = 101,
    FAME = 102,
    FREECOIN = 103,
    MILEAGE = 104,
    W_NORMALBOX = 200,  // 노멀
    W_RAREBOX,          // 레어
    W_ANCIENTBOX,       // 고대
    W_EPICBOX,          // 서사
    W_LEGENDBOX,        // 전설 (초반엔 전설까지만 오픈) 진행하면서 신화 태초 오픈
    W_MYTHBOX,          // 신화
    W_BEGINNINGBOX,
    None = 999,
}

public enum ETicketType
{
    GOLDTICKET = 0, // 골드 던전
    GROWTHSTONETICKET, // 하트 던전
    UPGRADESTONETICKET, // 강화석 던전
    SKILLBOOKTICKET, // 스킬북 던전
    CHARACTERDAYTICKET, // 캐릭터 요일 던전
    INSIGNIADUNGEONTICKET, // 문양
    PVPTICKET, // PVP
    RAIDTICKET, // 레이드
    WEAPONSUMMONTICKET, // 무기 소환 티켓
    ARMORSUMMONTICKET, // 방어구 소환 티켓
    RELICSUMMONTICKET, // 유물 소환 티켓
    ACCESSORYSUMMONTICKET, // 장신구 소환 티켓
    NONE,
}
public enum ECharacterCollectionType
{
    CLEANER = 0,        // 카트
    BOYSCOUT,           // 보이스카웃
    SKATEGIRL,          // 스케이트걸
    HIGHSCHOOLGIRL,     // 고등학생
    BOMBGIRL,           // 폭탄걸
    WEREWOLF,           // 웨어울프
    WARCORRESPONDENT,   // 종군기자
    FIREFIGHTER,        // 소방관
    NATURALPERSON,      // 자연인
    DELIVERYMAN,        // 배달원
    HEADTEACHER,        // 체육선생
    SALESPERSON,        // 영업사원
    NURSE,              // 간호사
    DRONEPILOT,         // 드론 조종사
    VIOLINIST,          // 바이올리니스트
    CLUBGIRL,           // 스포츠댄서
    GRANDMA,            // 할머니
    NONE,
}

// 캐릭터 무기에 사용
public enum EItemGradeType
{
    NORMAL = 0,
    RARE = 4, 
    ANCIENT = 8, 
    EPIC = 12, 
    LEGEND = 16, 
    MYTH = 20, 
    BEGINNING = 24,
    NONE = 28,
}
public enum EGradeType
{
    NORMAL = 0, // 노멀
    RARE,       // 레어
    ANCIENT,    // 고대
    EPIC,       // 서사
    LEGEND,     // 전설 (초반엔 전설까지만 오픈) 진행하면서 신화 태초 오픈
    MYTH,       // 신화
    BEGINNING,  // 태초
    NONE,
}

public enum EScene
{
    INIT = 0,
    LOGIN,
    LOBBY,
    GAME,
    NONE
}

public enum EStatsPercent
{
    DAMAGE_PERCENT = 1,
    HP_PERCENT,
    SPEED_PERCENT,
    DEFENSE_PERCENT,
}

public enum EStatsBtnType
{
    PARTYATK = 0,
    PARTYDEF,
    PARTYHP,
    PARTYNORMALCRITICALCHANCE, // 파티 치명타 확률
    PARTYNORMALCRITICALHIT, // 파티 치명타 피해
    PARTYSKILLPOWER, // 파티 스킬대미지 업
    PARTYPENETRATE, // 파티 관통
    PARTYACCURACY, // 파티 명중
    CHARACTERATK,
    CHARACTERDEF,
    CHARACTERHP,
    CHARACTERLEVEL, // 캐릭터 레벨링
    SKILL_0, // 캐릭터 스킬 업
    SKILL_1,
    SKILL_2,
    CHARACTERNORMALCRITICALCHANCE, // 캐릭터 개인 치명타 확률
    CHARACTERNORMALCRITICALHIT, // 캐릭터 개인 치명타 피해량
    CHARACTERSKILLPOWER, // 캐릭터 개인 스킬 피해량
    NONE,
}

// 무기 장비 강화 할때 쓰임 (데이터 키값 들고오는데도 사용)
public enum EWeaponStatsBtnType
{
    WEAPONNORMAL_0 = 0, // 노멀 (int)EStatsBtnType.NONE
    WEAPONNORMAL_1,
    WEAPONNORMAL_2,
    WEAPONNORMAL_3,
    WEAPONRARE_0, // 레어
    WEAPONRARE_1,
    WEAPONRARE_2,
    WEAPONRARE_3,
    WEAPONANCIENT_0, // 고대
    WEAPONANCIENT_1,
    WEAPONANCIENT_2,
    WEAPONANCIENT_3,
    WEAPONEPIC_0, // 서사
    WEAPONEPIC_1,
    WEAPONEPIC_2,
    WEAPONEPIC_3,
    WEAPONLEGENDARY_0, // 전설
    WEAPONLEGENDARY_1,
    WEAPONLEGENDARY_2,
    WEAPONLEGENDARY_3,
    WEAPONMYTHIC_0, // 신화
    WEAPONMYTHIC_1,
    WEAPONMYTHIC_2,
    WEAPONMYTHIC_3,
    WEAPONPRIMORDIAL_0, // 태초
    WEAPONPRIMORDIAL_1,
    WEAPONPRIMORDIAL_2,
    WEAPONPRIMORDIAL_3,
    NONE,
}

public enum ESummonCategory
{
    WEAPON = 0,
    ACCESSARY,
    NONE
}

public enum EAccessoryType
{
    NONE = 0,
    RING,       // 반지
    EARRING,        // 귀걸이
    BRACELET,       // 팔찌(브레이슬릿)
}

public enum ESummonType
{
    NORMAL,
    CLEARNERSUMMONS,
    BOYSUMMONS,
    SKATESUMMONS,
    HIGHSUMMONS,
    BOMBSUMMONS,
    WERESUMMONS,
    WARSUMMONS,
    FIRESUMMONS,
    NATURALPERSONSUMMONS,
    DELIVERYMANSUMMONS,
    HEADTEACHERSUMMONS,
    SALESPERSONSUMMONS,
    NURSESUMMONS,
    DRONEPILOTSUMMONS,
    VIOLINISTSUMMONS,
    CLUBGIRLSUMMONS,
    GRANDMASUMMONS,
    NONE
}

public enum ECharacterStatsBtnType
{
    ATK = 0,
    DEF,
    HP,
    NONE,
}
public enum EStayDamageType
{
    BOMBATTACK02STAY = 0,
    NONE,
}

public enum ECharacterStatsInfoType
{
    ATK = 0,
    DEF,
    HP,
    SKILL_0,
    SKILL_1,
    SKILL_2,
    NORMALCRITICALCHANCE,
    NORMALCRITCALHIT,
    SKILLPOWER,
    NONE
}
public enum EPartyStatsType
{
    ATK = 0,
    DEF,
    HP,
    NORMALCRITICALCHANCE,
    NORMALCRITICALHIT,
    SKILLPOWER,
    PENETRATE,
    ACCURACY,
    NONE
}
public enum ESummonDataType
{
    WEAPONLEVEL = 0, // 무기 소환
    WEAPONEXP,
    ARMORLEVEL, // 방어구 소환
    ARMOREXP,
    ACCESSORIESLEVEL, // 액세서리 소환
    ACCESSORIESEXP,
    RUNESLEVEL, // 룬 소환
    RUNEEXP,
    NONE
}

public enum EBoxDataType
{
    W_NORMALBOX = 0, // 노멀
    W_RAREBOX,       // 레어
    W_ANCIENTBOX,    // 고대
    W_EPICBOX,       // 서사
    W_LEGENDBOX,     // 전설 (초반엔 전설까지만 오픈) 진행하면서 신화 태초 오픈
    W_MYTHBOX,       // 신화
    W_BEGINNINGBOX,  // 태초
    R_NORMALBOX, // 여기서 부터 룬상자 노멀
    R_RAREBOX,       // 레어
    R_ANCIENTBOX,    // 고대
    R_EPICBOX,       // 서사
    R_LEGENDBOX,     // 전설 (초반엔 전설까지만 오픈) 진행하면서 신화 태초 오픈
    R_MYTHBOX,       // 신화
    R_BEGINNINGBOX,  // 태초
    NONE,
}

public enum ECharacterBoxType
{
    N_CLEANER = 0,        // 카트
    R_CLEANER,            // 카트 (레어)
    N_BOYSCOUT,           // 보이스카웃
    R_BOYSCOUT,           // 보이스카웃 (레어)
    N_SKATEGIRL,          // 스케이트걸
    R_SKATEGIRL,          // 스케이트걸 (레어)
    N_HIGHSCHOOLGIRL,     // 고등학생
    R_HIGHSCHOOLGIRL,     // 고등학생 (레어)
    N_BOMBGIRL,           // 폭탄걸
    R_BOMBGIRL,           // 폭탄걸 (레어)
    N_WEREWOLF,           // 웨어울프
    R_WEREWOLF,           // 웨어울프 (레어)
    N_WARCORRESPONDENT,   // 종군기자
    R_WARCORRESPONDENT,   // 종군기자 (레어)
    N_FIREFIGHTER,        // 소방관
    R_FIREFIGHTER,        // 소방관 (레어)
    N_NATURALPERSON,      // 자연인
    R_NATURALPERSON,      // 자연인 (레어)
    N_DELIVERYMAN,        // 배달원
    R_DELIVERYMAN,        // 배달원 (레어)
    N_HEADTEACHER,        // 체육선생
    R_HEADTEACHER,        // 체육선생 (레어)
    N_SALESPERSON,        // 영업사원
    R_SALESPERSON,        // 영업사원 (레어)
    N_NURSE,              // 간호사
    R_NURSE,              // 간호사 (레어)
    N_DRONEPILOT,         // 드론 조종사
    R_DRONEPILOT,         // 드론 조종사 (레어)
    N_VIOLINIST,          // 바이올리니스트
    R_VIOLINIST,          // 바이올리니스트 (레어)
    N_CLUBGIRL,           // 스포츠댄서
    R_CLUBGIRL,           // 스포츠댄서 (레어)
    N_GRANDMA,            // 할머니
    R_GRANDMA,            // 할머니 (레어)
    NONE,
}

public enum EBuffType
{
    Gold,
    GrowthStone,
    UpgradeStone,
    SkillBook,
    Atk,
    Def,
    Hp,
    None,
}

// 서버 저장 관련
//public enum EServerDataUpdate
//{
//    GOODS_DATA = 0, GOODS_INDEXDATA,
//    TICKET_DATA,
//    CHARACTERCOLLECTION_DATA,
//    STAGE_CHAPTER,
//    PARTYSTATS_DATA,
//    SUMMONS_DATA,
//    MISSION_QUESTMISSION, MISSION_NORMALMISSION, MISSION_
//
//    MISSION, MISSION_ONEDAYCOUNT, MISSION_LOOPCOUNT, MISSION_SPECIALMISSION,
//    BOXINVEN_DATA,
//    UNITSELECTDATA,
//    DISPATCHDATA, DISPATCHCHARACTERPOSITIONDATA,
//    ZOMBIECATALOGDATA,
//    NONE
//}
//public enum EServerCharacterDataUpdate
//{
//    CHARACTER_CHARACTERLEVEL, CHARACTER_EQUIPNUMBER, CHARACTER_WEAPONCHECK, CHARACTER_WEAPONLEVEL, CHARACTER_WEAPONCOUNT, CHARACTER_SKILLCHECK,
//    CHARACTER_ACCESSORYEQUIP, CHARACTER_ACCESSORYCHECK, CHARACTER_ACCESSORYCOUNT,
//    CHARACTER_ACCESSORYSKILLEQUIP, CHARACTER_ACCESSORYSKILLCHECK, CHARACTER_ACCESSORYSKILLCOUNT,
//    CHARACTER_ACCESSORYSPECIALEQUIP, CHARACTER_ACCESSORYSPECIALCHECK, CHARACTER_ACCESSORYSPECIALCOUNT,
//    CHARACTER_CHARACTERSTATS,
//    NONE,
//}

public enum EZombieCatalogData
{
    /// <summary>
    /// // 정확히 몇마리 들어갈지 몰라서
    /// </summary>
    ZOMBIECATALOGNUMBER = 30, 
    NONE,
}

public enum EOptionType
{
    LOGINPAGE = 0,
    SOUNDMUTE,
    EFFECTMUTE,
    PUSHMUTE,
    NONE
}

public enum ETimeCheckType
{
    LOGIN = 0,
    ROULETTE,
    AD_BOX,
    AD_MONEY,
    INDUN,
}

public enum EPayType
{
    NONE = 0,
    MONEY,
    DIA,
    AD,
}

public enum EUserServerSaveType
{
    USERDATA = 0,
    UTILITYDATA,
    VERSIONDATA,
    LEADERBOARDDATA, // 리더보드로 사용
    NONE,
}

public static class CommonEnum
{
    public static ERewardType ConvertTicketTypeToRewardType(ETicketType type)
    {
        if (type == ETicketType.GROWTHSTONETICKET) { return ERewardType.GROWTHSTONE; }
        else if (type == ETicketType.UPGRADESTONETICKET) { return ERewardType.UPGRADESTONE; }
        else if (type == ETicketType.SKILLBOOKTICKET) { return ERewardType.SKILLBOOK; }
        else if (type == ETicketType.CHARACTERDAYTICKET) { return ERewardType.W_NORMALBOX; }
        else { return ERewardType.GOLD; }
    }
    public static string CharacterNameReturn(ECharacterCollectionType type)
    {
        if (type == ECharacterCollectionType.BOYSCOUT) { return "보이스카웃"; }
        else if (type == ECharacterCollectionType.SKATEGIRL) { return "스케이트걸"; }
        else if (type == ECharacterCollectionType.HIGHSCHOOLGIRL) { return "고등학생"; }
        else if (type == ECharacterCollectionType.BOMBGIRL) { return "폭탄걸"; }
        else if (type == ECharacterCollectionType.WEREWOLF) { return "웨어울프"; }
        else if (type == ECharacterCollectionType.WARCORRESPONDENT) { return "리포터"; }
        else { return "카트"; }
    }
}
