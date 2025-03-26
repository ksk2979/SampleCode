using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EStayDamageType
{
    ATTACK_1 = 0,
    NONE
}

public class Enemy : Interactable
{
    EnemyController _enemyController;
    EnemyStats _enemyStats;
    bool[] _stayDamage;
    float[] _timeDelay;

    public override void Init(double damage, double hp, int wave)
    {
        if (_enemyController == null) { _enemyController = GetComponent<EnemyController>(); }
        if (_enemyStats == null) { _enemyStats = GetComponent<EnemyStats>(); }
        if (_stayDamage == null) { _stayDamage = new bool[(int)EStayDamageType.NONE]; }
        if (_timeDelay == null) { _timeDelay = new float[(int)EStayDamageType.NONE]; }
        _enemyController.OnStart();
        _enemyStats.SetData(damage, hp, wave);
        for (int i = 0; i < _stayDamage.Length; ++i)
        {
            _stayDamage[i] = false;
            _timeDelay[i] = 0f;
        }
    }
    public override void TakeToDamage(double damage)
    {
        if (_enemyController.IsDie) { return; }
        CreateFloatingText(damage);
        if (_enemyStats.TakeDamage(damage)) { _enemyStats.HpbarValueUpdate(); }
        else { _enemyController.DoDie(); }
    }
    public override void StayToDamage(double damage, float delayTime, EStayDamageType type)
    {
        if (_enemyController.IsDie) { return; }
        if (_stayDamage[(int)type]) { return; }
        _stayDamage[(int)type] = true;
        CreateFloatingText(damage);
        _timeDelay[(int)type] = delayTime;
        if (_enemyStats.TakeDamage(damage)) { _enemyStats.HpbarValueUpdate(); }
        else { _enemyController.DoDie(); }
    }
    public void StayUpdate()
    {
        for (int i = 0; i < _stayDamage.Length; ++i)
        {
            if (!_stayDamage[i]) { return; }
            _timeDelay[i] -= Time.deltaTime;
            if (_timeDelay[i] <= 0f) { _timeDelay[i] = 0f; _stayDamage[i] = false; }
        }
    }

    const string KEY_FLOATING = "FloatingText";
    void CreateFloatingText(double damage)
    {
        _enemyStats.OnHpbar(true);
        if (_enemyStats.GetCanvas != null)
        {
            FloatingText floating = SimplePool.Spawn(CommonStaticKey.RESOURCES_FLOATING, KEY_FLOATING).GetComponent<FloatingText>();
            floating.Init(damage, _enemyStats.GetCanvas);
        }
    }
    public EnemyController GetEnemyController { get { return _enemyController; } }
    public EnemyStats GetEnemyStats { get { return _enemyStats; } }
}
