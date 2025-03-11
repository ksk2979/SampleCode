using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 엘리트 꽃게 스크립트
/// </summary>

// 3번 때리면 크리티컬이 터지면서 (기본공격 2배) 집게가 조금씩 성장한다 (성장하면 딜 증가)
public class EliteCrabController : EnemyController
{
    // 성장이 눈에 보이는 오브젝트
    [SerializeField] Transform _clawTrans;
    float _clawScale = 1f;
    int _attackStack = 0;

    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();

            _oneOnStartInit = true;
        }
        
        SetState(eCharacterStates.Spawn);
        _clawScale = 1f;
        if (_clawTrans != null) { ClawSclaeSetting(_clawScale); }
        _attackStack = 0;
    }

    public override void BasicAttack()
    {
        // 기본 데미지를 주는 건데 여기서 꽃게는 데미지도 주면서 성장도 해야한다
        _attackStack++;
        if (_attackStack == 3)
        {
            //Debug.Log("BasicAttack: 치명타");
            _baseAttack.DoCriticalHitTarget(_target);
            // 이펙트도 다르게 줄건지는 의문
            if (_clawTrans != null) { ClawSclaeSetting(_clawScale + 0.5f); }
            _attackStack = 0;
            // 성장
            _enemyStats.damage = _enemyStats.damage * 1.25f;
            _enemyStats.GetTDD1().damage = _enemyStats.damage; // 실제 대미지 주는 함수에 적용
        }
        else
        {
            _baseAttack.DoAttackTarget(_target);
        }
    }

    void ClawSclaeSetting(float value)
    {
        _clawScale = value;
        _clawTrans.localScale = new Vector3(_clawScale, _clawScale, _clawScale);
    }
}
