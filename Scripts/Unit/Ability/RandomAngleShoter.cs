using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAngleShoter : BaseAttack
{
    public Transform _fireReadyTrans;
    public float _minShotSpeed = 9;
    public float _maxShotSpeed = 11;
    public string FX_Shoot = string.Empty;
    public string FX_Enemy_Projectile_02 = "FX_Enemy_Projectile_02";

    public void DoFire(Transform _trans, CharacterStats state, UnitDamageData tdd)
    {
        //SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, FX_Muzzle_Flash, _fireReadyTrans.position, Quaternion.identity);
        if (string.IsNullOrEmpty(FX_Shoot) == false)
            SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, FX_Shoot, _fireReadyTrans.position, Quaternion.identity);
        var goShot = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, FX_Enemy_Projectile_02, _fireReadyTrans.position, Quaternion.identity);
        var enemyState = state as EnemyStats;

        var dir = (Util.GetPos_CircleV3ToTarget(_trans, 25, enemyState._AttackAngle) - _fireReadyTrans.position).normalized;
        goShot.transform.forward = dir;

        goShot.GetComponent<Skill>().InitData(enemyState, tdd);
    }
}
