using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��ġ ��ũ��Ʈ (��ä�� �ణ�� �о�ٰ� �ϴµ� �� �׳� ���ϰɸ� �ɵ�)
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
            // 30% Ȯ��
            if (Random.Range(0, 100) < 30)
            {
                PlayerController player = _target.GetComponent<PlayerController>();
                player.KnockBackActionStart();
                player.GetMovement.DontMove(0.3f);
                //Debug.Log("�˹�");
            }
        }
    }
}