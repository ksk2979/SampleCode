using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : CharacterController
{
    Enemy _enemy;
    [SerializeField] NormalAttack _normalAttack;
    float _attackTime = 0f;

    public override void OnStart()
    {
        base.OnStart();
        if (_enemy == null) 
        { 
            _enemy = GetComponent<Enemy>();
            if (_animEventSender != null)
            {
                _animEventSender.AddEvent("DieFunction", DieFunction);
            }
        }
        ResetFunction();
        if (_anim != null) { _anim.SetFloat(CommonStaticKey.ANIMPARAM_VELOCITY_Y, 1f); }
    }

    private void Update()
    {
        if (_isDie) { return; }
        if (!_enemy.GetRoadWaveScript.GetStartGame) { DoDie(); return; }
        if(_enemy != null) { _enemy.StayUpdate(); }
        if (_attack)
        {
            _attackTime += Time.deltaTime;
            if (_attackTime > 0.5f)
            {
                if (_normalAttack != null) { _normalAttack.Init(_enemy.GetEnemyStats.Damage); }
                _attackTime = 0f;
            }
            return;
        }
        if (_trans != null)
        {
            _trans.Translate(Vector2.up * _enemy.GetEnemyStats.Speed * Time.deltaTime);
            if (_trans.localPosition.y > 6.7f) { _anim.SetFloat(CommonStaticKey.ANIMPARAM_VELOCITY_Y, 0f); _attack = true; }
        }
    }

    public void DoDie()
    {
        if (_isDie) { return; }
        _isDie = true;
        if (_collider2D != null) { _collider2D.enabled = false; }
        if (_enemy != null)
        { 
            _enemy.GetEnemyStats.OnHpbar(false);
            if (_type == UnitType.UNIT) { _enemy.GetRoadWaveScript.GetEnemyGold(3); }
            else if (_type == UnitType.BOSS) { _enemy.GetRoadWaveScript.GetBossSoul(2); }
            else if (_type == UnitType.BOUNTY)
            {
                if (this.name == "Bounty_1") { _enemy.GetRoadWaveScript.GetBountyGold(200); }
                else { _enemy.GetRoadWaveScript.GetBountySoul(1); }
            }
        }
        if (_anim != null) { _anim.SetTrigger(CommonStaticKey.ANIMPARAM_DIE); _anim.SetBool(CommonStaticKey.ANIMPARAM_ISDIE, true); }
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
        _attackTime = 0f;
        if (_normalAttack != null) { _normalAttack.ResetAttack(); }
        if (_anim != null) { _anim.SetFloat(CommonStaticKey.ANIMPARAM_VELOCITY_Y, 0f); _anim.ResetTrigger(CommonStaticKey.ANIMPARAM_DIE); _anim.SetBool(CommonStaticKey.ANIMPARAM_ISDIE, false); }
    }

    public bool GetAttack { get { return _attack; } }
}
