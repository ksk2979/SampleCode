using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 블롭피쉬 (이속 저하)
public class EnemyBlobfishController : EnemyController
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
            if (Random.Range(0, 100) < 30)
            {
                PlayerController player = _target.GetComponent<PlayerController>();
                if (!player.GetStatusEffects.GetSlowOn) { player.GetStatusEffects.SlowOn(); }
                else { player.GetStatusEffects.SlowReAttack(); }
            }
        }
    }
}