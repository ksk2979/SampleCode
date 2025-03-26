using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : CharacterController
{
    Enemy _enemy;
    [SerializeField] NormalAttack _normalAttack;
    [SerializeField] LayerMask _enemyLayer;
    [SerializeField] LayerMask _playerLayer;
    float _climbSpeed = 5f;
    float _checkDistance = 0.5f;
    float _checkUpRadius = 0.3f;
    float _checkUpHeight = 1.2f;
    bool _jump = false;
    float _jumpDelayTime = 2f;

    bool _onAttack = false;
    float _attackDelay = 0f;

    public override void OnStart()
    {
        base.OnStart();
        if (_enemy == null)
        {
            _enemy = GetComponent<Enemy>();
            if (_animEventSender != null)
            {
                _animEventSender.AddEvent("DieFunction", DieFunction);
                _animEventSender.AddEvent("OnAttack", OnAttack);
                _animEventSender.AddEvent("OffAttack", OffAttack);
            }
        }
        ResetFunction();
        if (_anim != null) { _anim.SetFloat(CommonStaticKey.ANIMPARAM_VELOCITY_Y, 1f); }
    }

    private void Update()
    {
        if (_isDie) { return; }
        if(_enemy != null) { _enemy.StayUpdate(); }
        JumpUpdate();
        HpBarUpdate();

        Vector3 moveDir = Vector3.left;
        RaycastHit2D frontHit = Physics2D.Raycast(
            _trans.position + new Vector3(-0.4f, 0.1f, 0f),
            Vector2.left,
            _checkDistance,
            _enemyLayer
        );
        RaycastHit2D frontPlayerHit = Physics2D.Raycast(
            _trans.position + new Vector3(-0.4f, 0.75f, 0f),
            Vector2.left,
            _checkDistance,
            _playerLayer
        );
        Vector2 upCheckPos = (Vector2)_trans.position + Vector2.up * _checkUpHeight;
        Collider2D upHit = Physics2D.OverlapCircle(upCheckPos, _checkUpRadius, _enemyLayer);

        if (!_jump && frontHit.collider != null && upHit == null) { _jump = true; _rigidBody2D.AddForce(Vector3.up * _climbSpeed, ForceMode2D.Impulse); }
        if (!_onAttack)
        {
            if (frontPlayerHit.collider != null)
            {
                _onAttack = true;
                _anim.SetTrigger(CommonStaticKey.ANIMPARAM_ATTACK);
            }
        }
        else
        {
            _attackDelay += Time.deltaTime;
            if (_attackDelay > 1.5f)
            {
                _attackDelay = 0f;
                _onAttack = false;
            }
        }
            _trans.Translate(moveDir.normalized * (_enemy.GetEnemyStats.Speed * Time.deltaTime), Space.World);
    }

    void OnAttack()
    {
        _normalAttack.Init(_enemy.GetEnemyStats.Damage);
    }
    void OffAttack()
    {
        _normalAttack.ResetAttack();
    }
    void JumpUpdate()
    {
        if (_jump)
        {
            _jumpDelayTime -= Time.deltaTime;
            if (_jumpDelayTime < 0f)
            {
                _jumpDelayTime = 2f;
                _jump = false;
            }
        }
    }
    void HpBarUpdate()
    {
        if (_enemy != null) { _enemy.GetEnemyStats.HpbarUpdate(); }
    }

    public void DoDie()
    {
        if (_isDie) { return; }
        _isDie = true;
        if (_collider2D != null) { _collider2D.enabled = false; }
        if (_enemy != null)
        { 
            _enemy.GetEnemyStats.OnHpbar(false);
        }
        // 바로 사라지게
        DieFunction();
        //if (_anim != null) { _anim.SetTrigger(CommonStaticKey.ANIMPARAM_DIE); _anim.SetBool(CommonStaticKey.ANIMPARAM_ISDIE, true); }
    }

    public void DieFunction()
    {
        SimplePool.Despawn(this.gameObject);
    }

    void ResetFunction()
    {
        if (_collider2D != null) { _collider2D.enabled = true; }
        _attack = false;
        _isDie = false;
        if (_normalAttack != null) { _normalAttack.ResetAttack(); }
        if (_anim != null) { _anim.SetFloat(CommonStaticKey.ANIMPARAM_VELOCITY_Y, 0f); _anim.ResetTrigger(CommonStaticKey.ANIMPARAM_DIE); _anim.SetBool(CommonStaticKey.ANIMPARAM_ISDIE, false); }
    }

    public bool GetAttack { get { return _attack; } }
}
