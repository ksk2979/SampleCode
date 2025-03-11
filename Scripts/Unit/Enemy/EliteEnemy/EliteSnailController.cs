using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 달팽이
/// </summary>

// 3개의 눈에서 레이저가 나간다 (관통)
public class EliteSnailController : EnemyController
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
        GameObject bullet = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Enemy/EnemyLaserAttack", Vector3.zero, Quaternion.identity);
        bullet.transform.forward = _trans.forward;
        bullet.GetComponent<EliteSnailAttack>().SetData(this.transform.position, _enemyStats);
    }
}
