using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class CommonStaticDatas
{
    public static string TAG_UNTAGGED = "Untagged";
    public static string TAG_ENEMY = "Enemy"; //
    public static string TAG_TRAP = "Trap";
    public static string TAG_PLAYER = "Player"; //
    public static string TAG_WALL = "Wall"; //
    public static string TAG_OBSTACLE = "Obstacle"; //
    public static string TAG_FLOOR = "Floor"; //
    public static string TAG_EXPBOLL = "ExpBoll"; //
    public static string TAG_BULLET = "Bullet";
    public static string TAG_ABILITY = "Ability";

    public static int ANIMPARAM_VELOCITY_X = Animator.StringToHash("VelocityX");
    public static int ANIMPARAM_VELOCITY_Y = Animator.StringToHash("VelocityY");
    public static int ANIMPARAM_ATTACK = Animator.StringToHash("Attack");
    public static int ANIMPARAM_CASTING = Animator.StringToHash("Casting");
    public static int ANIMPARAM_ATTACKON = Animator.StringToHash("AttackOn");
    public static int ANIMPARAM_COMBO = Animator.StringToHash("Combo");
    public static int ANIMPARAM_ATTACK01 = Animator.StringToHash("Attack01");
    public static int ANIMPARAM_ATTACK02 = Animator.StringToHash("Attack02");
    public static int ANIMPARAM_SUMMON = Animator.StringToHash("Summon");
    public static int ANIMPARAM_ATTACK_SPEED = Animator.StringToHash("AttackSpeed");
    public static int ANIMPARAM_OPEN = Animator.StringToHash("Open");
    public static int ANIMPARAM_CLOSEHATCH = Animator.StringToHash("Close");
    public static int ANIMPARAM_FIRERATE = Animator.StringToHash("FireRate");
    public static int ANIMPARAM_STUN = Animator.StringToHash("Stun");
    public static int ANIMPARAM_SHOCK = Animator.StringToHash("Shock");
    public static int ANIMPARAM_SHOOT = Animator.StringToHash("Shoot");
    public static int ANIMPARAM_DROP = Animator.StringToHash("Drop");
    public static int ANIMPARAM_ATTACKPUSH = Animator.StringToHash("AttackPush");
    public static int ANIMPARAM_GROGGY = Animator.StringToHash("Groggy");

    // 1 보스 스킬
    public static int ANIMPARAM_SKILLSTART = Animator.StringToHash("SkillStart");
    public static int ANIMPARAM_ENDSKILL = Animator.StringToHash("EndSkill");
    public static int ANIMPARAM_SPAWN = Animator.StringToHash("Spawn");

    // 2 보스 스킬
    public static int ANIMPARAM_ATTACKCOMBO0 = Animator.StringToHash("AttackCombo0");
    public static int ANIMPARAM_SKILL1END = Animator.StringToHash("Skill1End");
    public static int ANIMPARAM_ATTACK01END = Animator.StringToHash("Attack01End");

    public static int ANIMPARAM_END = Animator.StringToHash("End");
    public static int ANIMPARAM_HIT = Animator.StringToHash("Hit");
    public static int ANIMPARAM_KNOCKBACK = Animator.StringToHash("KnockBack");
    public static int ANIMPARAM_KNOCKDOWN = Animator.StringToHash("KnockDown");
    public static int ANIMPARAM_DIE_1 = Animator.StringToHash("Dead");
    public static int ANIMPARAM_IS_DIE = Animator.StringToHash("IsDead");
    public static int ANIMPARAM_DIE_BOOL = Animator.StringToHash("Deaded");
    public static int ANIMPARAM_FIRE = Animator.StringToHash("Fire");
    public static int ANIMPARAM_FIRECOUNT = Animator.StringToHash("FireCount");
    public static int ANIMPARAM_FIRE_SPEED = Animator.StringToHash("FireSpeed");
    public static int ANIMPARAM_HATCHOPEN = Animator.StringToHash("Open");
    public static int ANIMPARAM_HATCHCLOSE = Animator.StringToHash("Close");
    public static int ANIMPARAM_PILOT = Animator.StringToHash("Pilot");
    public static int ANIMPARAM_APPEAR = Animator.StringToHash("Appear");
    public static int ANIMPARAM_DISAPPEAR = Animator.StringToHash("Disappear");

    public const string LAYERMASK_UNIT = "Unit";
    public const string LAYERMASK_PLAYER = "Player";
    public const string LAYERMASK_OBSTACLE = "Obstacle";
    public const string LAYERMASK_BUILDING = "Building";
    public const string LAYERMASK_FLOOR = "Floor";

    public const string STR_USERLEVEL = "userlevel";
    public const string RES_MANAGER = "Prefabs/Manager/";
    public const string RES_UI = "Prefabs/UI/";
    public const string RES_UI_ITEMICON = "Prefabs/UI/Unit/";
    public const string RES_UI_BOAT = "Prefabs/UI/Unit/Boat/";
    public const string RES_UI_WEAPON = "Prefabs/UI/Unit/Weapon/";
    public const string RES_UI_DEFENSE = "Prefabs/UI/Unit/Defense/";
    public const string RES_UI_CAPTAIN = "Prefabs/UI/Unit/Captain/";
    public const string RES_UI_SAILOR = "Prefabs/UI/Unit/Sailor/";
    public const string RES_UI_ENGINE = "Prefabs/UI/Unit/Engine/";
    public const string RES_UI_MATERIAL = "Prefabs/UI/Unit/Material/";
    public const string RES_BOAT = "Prefabs/Unit/Boat/";
    public const string RES_WEAPON = "Prefabs/Unit/Weapon/";
    public const string RES_DEFENSE = "Prefabs/Unit/Defense/";
    public const string RES_BOATLOBBY = "Prefabs/Unit/BoatLobby/";
    public const string RES_WEAPONLOBBY = "Prefabs/Unit/WeaponLobby/";

    public const string RES_EX = "Prefabs/Effect/Ex/";
    public const string RES_ENEMY = "Prefabs/Unit/Enemy/";
    public const string RES_UNIQUE = "Prefabs/Unit/Unique/";
    public const string RES_TRAP = "Prefabs/Unit/Trap/";
    public const string RES_GIRL = "Prefabs/Ability/Girl/";
    public const string RES_EFFECT = "Prefabs/Effect/";
    public const string RES_EFFECT_ABILITY = "Prefabs/Effect/Ability/";
    public const string RES_ABILITY = "Prefabs/Ability/";
    public const string RES_PREFAB = "Prefabs/";
    public const string RES_TUTORIAL = "Prefabs/Tutorial/";
    public const string RES_STAGEMATERIAL = "Prefabs/UI/StageMaterial/";
    public const string RES_BOSSEFFECT = "Prefabs/Effect/Boss/";
    public const string RES_PLAYERABILITY = "Prefabs/PlayerAbility/";

    public const string RES_ITEMICON = "ItemIcon";
    public const string RES_SKILLICON = "SkillIcon";

    public const string CONNET_TPYE = "ConnectType";
    public const string GUEST_TOKEN = "GuestToken";

    public const string SOUND_CATEGORY_BGM = "BGM";
    public const string SOUND_CATEGORY_SFX = "SFX";
    public const string SOUND_TankDamage01 = "TankDamage01";
    public const string SOUND_ColleagueDamage01 = "ColleagueDamage01";
    public const string SOUND_Boss05_Skill01 = "Boss05_Skill01";
    public const string SOUND_Boss05_Skill02 = "Boss05_Skill02";

    public const string MUSIC_BOSS_APPEARANCE = "99_Boss appearance";


    public const string TARGETRANGE_CIRCLE = "TargetRangeCircle";
    public const string TARGETRANGE_LINE = "TargetRangeLine";
    public const string TimeHours = "{0}H";
    public const string TimeMinutes = "{0}M";
    public const string TimeSeconds = "{0}S";
    public const string STR_LogTimeHours = "{0:00}:{1:00}m";
    public const string STR_LogTimeMinutes = "{0:00}:{1:00}s";
    public const string STR_LogTimeSeconds = "{0:00}";
    public const string STR_PROGRESS_2INPUT_SLASH = "{0}/{1}";
    public const string STR_PROGRESS_2INPUT_SLASH_ENOUGH = "<color=#00ff00ff>{0}</color>/{1}";
    public const string STR_PROGRESS_2INPUT_SLASH_EMPTY = "<color=#ff0000ff>{0}</color>/{1}";
    public const string STR_PROGRESS_2INPUT_DASH = "{0}-{1}";
    public const string STR_PROGRESS_2INPUT_DASH_NUM = "{0:00}-{1:00}";
    public const string STR_1INPUT1 = "{0}";
    public const string STR_1INPUT2 = "{1}";
    public const string STR_SPACE_ONE = " ";


    public const string STR_KEY_LOCALLANGUAGE = "locallanguage";
    //public const string FILEPATH_UI_SYNTHESIS = "Prefabs/UI/Synthesis/";
    //public const string FILEPATH_UI_BOATTEAMSELECT = "Prefabs/UI/BoatTeamSelect/";
}

[System.Serializable]
public class myEventIntera : UnityEvent<Transform, Interactable> { }
[System.Serializable]
public class myEvent : UnityEvent { }