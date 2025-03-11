using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 문어 컨트롤러
/// </summary>

// 일정거리에 들어오면 배에 달라붙고 시간이 지나면 대폭발이 일어나며 자폭하는 몬스터
// Hide로 변형하는걸로
public class EliteOctopusController : EnemyController
{
    [SerializeField] string _effectName = string.Empty; // FX_Explosion_01
    bool _explosion = false; // 한번 폭파하면 끝

    PlayerController _playerController;

    float _explosionTime = 0f;
    [SerializeField] float _distancePos = 1f;

    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            _oneOnStartInit = true;
        }
        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_KNOCKBACK);
        _animaController.ResetTriggerAnimation(CommonStaticDatas.ANIMPARAM_STUN);
        _explosion = false;
        _movement.AgentEnabled(true);
        SetState(eCharacterStates.Spawn);
    }

    protected override void AttackUpdate()
    {
        WatchTarget();
        if (_target != null)
        {
            float dir = Vector3.Distance(_trans.position, _target.position);
            if (dir < 3f) { BasicAttack(); }
            else { _trans.position = Vector3.MoveTowards(_trans.position, _target.position, Time.deltaTime * (_enemyStats.traceMoveSpeed * 3f)); }
        }
        else { SetState(eCharacterStates.Idle); return; }
    }

    public override void BasicAttack()
    {
        // 공격시 달라붙는다
        WatchTarget();
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_ATTACKPUSH);
        SetState(eCharacterStates.Hide);
    }

    protected override void HideInit()
    {
        _movement.AgentEnabled(false);
        // 타겟에 달라붙는다
        if (!TargetCheckFuntion()) { Explosion(); }
        else { _playerController = _target.GetComponent<PlayerController>(); }
    }
    protected override void HideUpdate()
    {
        if (!_playerController.IsDie())
        {
            // 적이 플레이어와의 거리를 두도록 위치 설정
            StandardFuncUnit.WatchTarget(_trans, _target);
            Vector3 direction = (_trans.position - _target.position).normalized; // 현재 위치에서 타겟 위치까지의 방향
            _trans.position = _target.position + direction * _distancePos; // 타겟 위치에서 일정 거리만큼 떨어진 위치
        }
        else
        {
            // 만약 적이 붙이 있는데 죽어 버렸으면 (어떤 다른 공격으로 인해서) 그냥 대기 상태로 돌아간다
            _movement.AgentEnabled(true);
            SetState(eCharacterStates.Idle);
            _explosionTime = 0f;
            _explosion = false;
        }

        _explosionTime += Time.deltaTime;
        if (_explosionTime > 5f) { _explosionTime = 0f; Explosion(); }
    }

    void Explosion()
    {
        if (!_explosion) { OnEffectDream(); }

        _enemyStats.hp = 0;
        DoDie();
    }

    public void OnEffectDream()
    {
        _explosion = true;
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _effectName, Vector3.zero, Quaternion.identity);
        obj.transform.position = _trans.position;
        obj.transform.forward = _trans.forward;
        //if (addChild)
        //{
        //    obj.transform.SetParent(perent);
        //}
        obj.GetComponent<Explosion>().InitData(_enemyStats, _enemyStats.GetTDD1());
    }
    public override void AttackFinishCall() { }
}
