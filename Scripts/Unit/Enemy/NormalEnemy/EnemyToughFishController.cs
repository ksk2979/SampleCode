using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 턱 강화 물고기 공격력이 선체의 %로 들어가기 때문에 일반적인 데미지는 퍼센트로 변형해서 데미지가 들어가야함
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