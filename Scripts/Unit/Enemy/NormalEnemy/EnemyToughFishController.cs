using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� ��ȭ ����� ���ݷ��� ��ü�� %�� ���� ������ �Ϲ����� �������� �ۼ�Ʈ�� �����ؼ� �������� ������
public class EnemyToughFishController : EnemyController
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
        _baseAttack.DoPercentageDamage(_target);
    }
}