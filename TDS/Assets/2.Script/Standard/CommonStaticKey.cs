using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonStaticKey : MonoBehaviour
{
    public static int ANIMPARAM_VELOCITY_X = Animator.StringToHash("VelocityX");
    public static int ANIMPARAM_VELOCITY_Y = Animator.StringToHash("VelocityY");
    public static int ANIMPARAM_ATTACK = Animator.StringToHash("Attack");
    public static int ANIMPARAM_ISDIE = Animator.StringToHash("IsDie");
    public static int ANIMPARAM_DIE = Animator.StringToHash("Die");

    public const string TAG_PLAYER = "Player";
    public const string TAG_ENEMY = "Enemy";
    public const string TAG_SHINSU = "Shinsu";
    public const string TAG_TILE = "Tile";

    public const string LAYERMASK_UNIT = "Unit";
    public const string LAYERMASK_PLAYER = "Player";
    public const string LAYERMASK_TILE = "Tile";

    public const string RESOURCES_ENEMY = "Prefabs/Enemy/";
    public const string RESOURCES_BULLET = "Prefabs/Bullet/";
    public const string RESOURCES_FLOATING = "Prefabs/Floating/";
}
