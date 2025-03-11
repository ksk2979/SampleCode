using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 조개 (도트 데미지)
public class EnemyClamController : EnemyController
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
        base.BasicAttack();
        if (_target != null)
        {
            // 30% 확률
            if (Random.Range(0,100) < 30)
            {
                PlayerController player = _target.GetComponent<PlayerController>();
                if (!player.GetStatusEffects.GetSlowOn) { player.GetStatusEffects.DotOn(_enemyStats.GetTDD1().damage); }
                else { player.GetStatusEffects.DotReAttack(); }
            }
        }
    }
}