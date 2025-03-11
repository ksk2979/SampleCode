using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 넙치 스크립트 (선채를 약간씩 밀어낸다고 하는데 음 그냥 스턴걸면 될듯)
public class EnemyHalibutController : EnemyController
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
                player.KnockBackActionStart();
                player.GetMovement.DontMove(0.3f);
                //Debug.Log("넉백");
            }
        }
    }
}