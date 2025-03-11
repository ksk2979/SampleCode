using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// �����ϴ� ����� ��ũ��Ʈ
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

    // ������ �ϱ� �����ϸ� �浹 �ݶ��̴��� ���ְ� ������ �ǰ� ������ �Ѵ�
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