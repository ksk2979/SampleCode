using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 관통하는 물고기 스크립트
public class EnemyPhasingController : EnemyController
{
    [SerializeField] float _speed = 5f;
    float _dirTime = 0f;
    Collider _attackCollider;
    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            _attackCollider = _baseAttack.GetComponent<Collider>();
            _oneOnStartInit = true;
        }
        AttackFinish();
        SetState(eCharacterStates.Spawn);
    }

    // 공격을 하기 시작하면 충돌 콜라이더를 켜주고 관통이 되게 수정을 한다
    protected override void AttackInit()
    {
        _movement.NavAgentAction(false);
        _movement.AgentStop(true);
        _dirTime = 1.5f;
        _attackCollider.enabled = true;
    }
    protected override void AttackUpdate()
    {
        _dirTime -= Time.deltaTime;
        if (_dirTime < 0f) { SetState(eCharacterStates.Trace); return; }
        _trans.Translate(Vector3.forward * _speed * Time.deltaTime);
    }
    protected override void AttackFinish()
    {
        _movement.NavAgentAction(true);
        _movement.AgentStop(false);
        _attackCollider.enabled = false;
    }
    public override void BasicAttack() { }
    public override void AttackFinishCall() { }
}