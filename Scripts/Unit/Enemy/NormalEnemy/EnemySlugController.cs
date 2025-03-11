using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ¹Î´ÞÆØÀÌ (3¿¬¹ß ±¤¿ª °î»çµô)
public class EnemySlugController : EnemyController
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
        if (_target == null) { return; }
        GameObject bullet = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Enemy/EnemyLDAttack_2", Vector3.zero, Quaternion.identity);
        bullet.transform.forward = _trans.forward;
        bullet.GetComponent<EnemyLDAttack>().Init(this.transform.position, _target.position, _enemyStats);
    }
}