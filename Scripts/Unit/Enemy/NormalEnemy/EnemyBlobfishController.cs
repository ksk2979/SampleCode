using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����ǽ� (�̼� ����)
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
            // 30% Ȯ��
            if (Random.Range(0, 100) < 30)
            {
                PlayerController player = _target.GetComponent<PlayerController>();
                if (!player.GetStatusEffects.GetSlowOn) { player.GetStatusEffects.SlowOn(); }
                else { player.GetStatusEffects.SlowReAttack(); }
            }
        }
    }
}