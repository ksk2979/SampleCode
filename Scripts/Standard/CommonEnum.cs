using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Boat,
    Colleague,
    Enemy,
    EliteEnemy,
    SummonEnemy,
    Boss
}

public enum EUserSaveType
{
    USERDATA = 0,
    MATERIALDATA,
    UNITDATA,
    UNITLEVELDATA,
    UNITPOTENTIALDATA,
    STAGEDATA,
    UNITCOUNTID,
    SELECTDATA,
    COALESCENECEDATA,
    UTILITYDATA,
    VERSIONDATA,
    TUTORIALDATA,
    PACKAGEDATA,
    DAILYREWARDDATA,
    REWARDTIMEDATA,
    QUESTDATA,
    COLLECTIONDATA,
    OPTIONDATA,
    SEASONPASSDATA,
    SERVERSAVEDATA,
    NONE,
}
public enum EUserServerSaveType
{
    USERDATA = 0,
    UNITDATA,
    UNITLEVELDATA,
    UNITPOTENTIALDATA,
    STAGEDATA,
    UNITCOUNTID,
    COALESCENECEDATA,
    UTILITYDATA,
    VERSIONDATA,
    DAILYREWARDDATA,
    REWARDTIMEDATA,
    COLLECTIONDATA,
    QUESTDATA,
    TUTORIALDATA,
    SEASONPASSDATA,
    NONE,
}

public enum EStageDifficulty
{
    E_EASY = 0,
    E_NORMAL,
    E_HARD,
    E_NONE,
}

public enum EScene
{
    E_INIT = 0,
    E_LOGIN,
    E_LOBBY,
    E_GAME,
    NONE,
}

public enum AbilitySkillPrefabs
{
    E_ROTATIONPROTECTION = 0,
    E_UNDERWATERMINES, // 현재 어뢰 없음
    E_AIRSUPPORT,
    E_TORPEDO, // 현재 어뢰 없음
    E_GHOSTSHIP,
    E_SWIRL,
    E_NETTINGCREATE,
    E_GRAVITY, // 중력자탄
    E_AIRMACHINEGUN,
    E_ARTILLERY,
    E_SENTRY,
    E_BOMBARDMENTREQUEST,
    E_SATELLITEREQUEST,
    E_DRONE,
    NONE,
}

public enum EPropertyType
{
    MONEY = 0, 
    DIAMOND,
    ACTIONENERGY,
    EVOLUTION,
    NONE
}

public enum EMaterialType
{
    COPPER = 0,
    ZINC,
    ALUMINUM, 
    STEEL, 
    GOLD,
    OIL,
    NONE
} // 구리 아연 알루 철 금(주석) 오일(티타늄)

public enum ERewardType
{
    None = -1,
    Money = 0,
    Diamond = 1,
    ActionEnergy = 2,
    Copper = 3,
    Zinc = 4,
    Aluminum = 5,
    Steel = 6,
    Gold = 7,
    Oil = 8,
    NormalBox = 9,
    EliteBox = 10,
}

public enum EInvenType
{
    Money = 0,
    Diamond = 1,
    Copper = 2,
    Zinc = 3,
    Aluminum = 4,
    Steel = 5,
    Gold = 6,
    Oil = 7,
    Evolution = 8,
    None = 9
}

public enum EOptionType
{
    LOGINPAGE = 0,
    SOUNDMUTE,
    VIBRATION,
    JOYSTICK,
    NONE
}

public enum EDefensePoint
{
    FRONT = 0,
    BACK,
    LEFT,
    RIGHT,
    FRONT_BACK,
    LEFT_RIGHT,
    FRONT_LEFT_RIGHT,
    ALL,
    NONE
}

public enum ETimeCheckType
{
    ONEDAY = 0,
    DAILY_ROULETTE,
    AD_DIA,
    AD_MONEY,
    AD_ACTIONENERGY,
    AD_NORMALBOX,
    AD_ELITEBOX,
}

public enum eCharacterStates
{
    Spawn = 0, Idle, Move, Attack, FindTarget,// 원거리 전용
    Trace, Die, KnockBack, KnockDown, Stern, Shock,
    ///점프 타입 전용 =====================
    DashJump, ShortIdle,
    ///두더지 타입 전용 =====================
    Hide,
    ///보스 전용 =====================
    Attack01, // 스킬1
    Attack02, // 스킬2
    Summon, // 소환
    ///end 보스 전용 =====================
    Drop, // 보스1스킬
    Max,
}

public enum ECoalescenceType
{
    BOAT_ID = 0,
    BOAT_LEVEL,
    WEAPON_ID,
    WEAPON_LEVEL,
    DEFENSE_ID,
    DEFENSE_LEVEL,
    CAPTAIN_ID,
    CAPTAIN_LEVEL,
    SAILOR_ID,
    SAILOR_LEVEL,
    ENGINE_ID,
    ENGINE_LEVEL,
}

public class CommonEnum : MonoBehaviour
{

}
