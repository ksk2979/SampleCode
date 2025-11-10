using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonStaticKey : MonoBehaviour
{
    public static int ANIMPARAM_VELOCITY_X = Animator.StringToHash("VelocityX");
    public static int ANIMPARAM_VELOCITY_Y = Animator.StringToHash("VelocityY");
    public static int ANIMPARAM_ATTACK = Animator.StringToHash("Attack");
    public static int ANIMPARAM_COMBO = Animator.StringToHash("Combo");
    public static int ANIMPARAM_DEAD = Animator.StringToHash("Dead");
    public static int ANIMPARAM_ISDEAD = Animator.StringToHash("IsDead");
    public static int ANIMPARAM_OPENHATCH = Animator.StringToHash("FallDown");
    public static int ANIMPARAM_CLOSEHATCH = Animator.StringToHash("ReBirth");
    public static int ANIMPARAM_SKILL = Animator.StringToHash("Skill");
    public static int ANIMPARAM_SKILL1 = Animator.StringToHash("Skill1");
    public static int ANIMPARAM_SKILL2 = Animator.StringToHash("Skill2");
    public static int ANIMPARAM_SKILLCOMBO = Animator.StringToHash("SkillCombo");
    public static int ANIMPARAM_SKILLDOWN = Animator.StringToHash("SkillDown");
    public static int ANIMPARAM_SKILLEND = Animator.StringToHash("SkillEnd");
    public static int ANIMPARAM_ITEM = Animator.StringToHash("Item");
    public static int ANIMPARAM_HIT = Animator.StringToHash("Hit");
    public static int ANIMPARAM_ATTACK01 = Animator.StringToHash("Attack01");
    public static int ANIMPARAM_ATTACK02 = Animator.StringToHash("Attack02");
    public static int ANIMPARAM_ATTACK03 = Animator.StringToHash("Attack03");
    public static int ANIMPARAM_OPEN = Animator.StringToHash("Open");
    public static int ANIMPARAM_MOVE = Animator.StringToHash("Move");
    public static int ANIMPARAM_ATTACKSPEED = Animator.StringToHash("AttackSpeed");
    public static int ANIMPARAM_STERN = Animator.StringToHash("Stern");
    public static int ANIMPARAM_ISSTERN = Animator.StringToHash("IsStern");

    public const string TAG_PLAYER = "Player";
    public const string TAG_ENEMY = "Enemy";
    public const string TAG_BOX = "Box";
    public const string TAG_ITEM = "Item"; // exp
    public const string TAG_MAGNET = "Magnet";
    public const string TAG_BOMB = "Bomb";
    public const string TAG_FOOD = "Food";
    public const string TAG_GOLD = "Gold";

    public const string LAYERMASK_UNIT = "Unit";
    public const string LAYERMASK_PLAYER = "Player";

    public const string FILEPATH_SKILLPROJECTILE = "Prefabs/Skill/";

    public const string FILEPATH_PLAYER = "Prefabs/Player/";
    public const string FILEPATH_ENEMY = "Prefabs/Enemy/";
    public const string FILEPATH_ZOMBIECATALOG = "Prefabs/Catalog/";

    public const string FILEPATH_UI_PLAYER_LOBBY = "Prefabs/UI/LobbyUI/Player/";
    public const string FILEPATH_UI_CHARACTERINFO = "Prfabs/UI/Character/CharacterInfo/";
    public const string FILEPATH_UI_PLAYER_WEAPON = "Prefabs/UI/Player/WeaponUI/";
    public const string FILEPATH_UI_PLAYER_SKIN = "Prefabs/UI/Player/SkinUI/";
    public const string FILEPATH_UI_PLAYER_NECKLACE = "Prefabs/UI/Player/NecklaceUI/";
    public const string FILEPATH_UI_PLAYER_BELT = "Prefabs/UI/Player/BeltUI/";
    public const string FILEPATH_UI_PLAYER_GLOVES = "Prefabs/UI/Player/GlovesUI/";
    public const string FILEPATH_UI_PLAYER_SHOES = "Prefabs/UI/Player/ShoesUI/";
    public const string FILEPATH_UI_PLAYER_COSTUME = "Prefabs/UI/Player/CostumeUI/";
    public const string FILEPATH_UI_FUSION = "Prefabs/UI/Fusion/";
    public const string FILEPATH_UI_EFFECT = "Prefabs/UI/Effect/";

    public const string FILEPATH_FLOATINGTEXT = "Prefabs/UI/Floating/";
    public const string FILEPATH_HPBAR = "Prefabs/HpBar/";

    public const string FILEPATH_BOX = "Prefabs/Box/";

    public const string FILEPATH_EFFECT = "Prefabs/Effect/";
    public const string FILEPATH_BOSS_ATTACK = "Prefabs/Enemy/Attack/";

    public const string STR_KEY_LOCALLANGUAGE = "locallanguage";

    public const string FILEPATH_SHOP_UIPAGE = "Prefabs/UI/Shop/UIPage/";
    public const string FILEPATH_UICANVAS = "Prefabs/UI/UICanvas/";
    public const string FILEPATH_UI_DISPATCH = "Prefabs/UI/Dispatch/";
    public const string FILEPATH_INGAME_MAP = "Prefabs/Map/Stage/";
    public const string FILEPATH_INGAME_MAP_INDON = "Prefabs/Map/Indon/";
    public const string FILEPATH_UI = "Prefabs/UI/";
    public const string FILEPATH_PARTY = "Prefabs/UI/Party/";

    public const string FILEPATH_PLAYERICON = "Sprites/Character/";

    // 번역기
    public const string STR_KEY_WEAR = "착용";
    public const string STR_KEY_WEARCLEAR = "해제";
    public const string STR_KEY_WIN = "승리";
    public const string STR_KEY_LOSE = "패배";
    public const string STR_KEY_WINTEXT = "축하합니다";
    public const string STR_KEY_LOSETEXT = "다시해보세요";
    public const string STR_KEY_STAGE = "스테이지";
    public const string STR_KEY_SKILLCHANGE = "스킬 변경";
    public const string STR_KEY_FREESKILLCHANGE = "무료 스킬 변경";
    public const string STR_KEY_ADSKILLCHANGE = "광고 시청 후 변경";
}
