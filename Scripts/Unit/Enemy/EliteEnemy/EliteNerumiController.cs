using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 낼름낼름
/// </summary>

// 커다란 혀로 공격하는 네루미 공격 시 점액으로 느려지며 일정확률로 즉사 된다
public class EliteNerumiController : EnemyController
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
        // 공격을 하면 이속감소를 시키고 일정 확률로 즉사 대미지 주기
        int rand = Random.Range(0, 100);
        if (rand == 1) { Debug.Log("네루미 즉사"); _baseAttack.DoInstantKillDamageTarget(_target); }
        else { _baseAttack.DoAttackTarget(_target); }
        
        if (_target != null)
        {
            PlayerController playerController = _target.GetComponent<PlayerController>();
            if (!playerController.GetStatusEffects.GetSlowOn) { playerController.GetStatusEffects.SlowOn(); }
            else { playerController.GetStatusEffects.SlowReAttack(); }
        }
    }
}
