using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 원거리 공격 타입
/// </summary>
public class EnemyLDTypeController : EnemyController
{
    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            _oneOnStartInit = true;
        }
        SetState(eCharacterStates.Spawn);
    }

    public override void BasicAttack()
    {
        GameObject bullet = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Enemy/EnemyLDAttack", Vector3.zero, Quaternion.identity);
        bullet.transform.forward = _trans.forward;
        bullet.GetComponent<EnemyLDAttack>().Init(this.transform.position, _target.position, _enemyStats);
    }
}

