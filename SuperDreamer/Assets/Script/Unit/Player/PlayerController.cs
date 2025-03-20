using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : CharacterController
{
    Player _player;

    [HideInInspector]public bool[] _earthAttack = { false, false, false, false, false, false };
    [HideInInspector]public bool[] _fireAttack = { false, false, false, false, false, false };
    [HideInInspector]public bool[] _waterAttack = { false, false, false, false, false, false };

    int[] _earthAttackCount = { 0, 0, 0, 0, 0, 0 };
    int[] _fireAttackCount = { 0, 0, 0, 0, 0, 0 };
    int[] _waterAttackCount = { 0, 0, 0, 0, 0, 0 };

    float[] _earthCooldown; // 남은 쿨타임
    float[] _fireCooldown;
    float[] _waterCooldown;

    // 임의 데이터
    [HideInInspector]public float[] _earthAttackTime = { 1.3f, 4f, 10f, 2.5f, 4f, 10f };
    [HideInInspector]public float[] _fireAttackTime = { 3.2f, 4f, 8f, 2.7f, 18f, 4f };
    [HideInInspector]public float[] _waterAttackTime = { 0.8f, 3f, 1f, 3f, 1.5f, 3f };
    
    [HideInInspector]public double[] _earthAttackDamage = { 200, 1700, 8500, 5500, 34000, 320000 };
    [HideInInspector]public double[] _fireAttackDamage = { 300, 800, 6000, 12000, 48000, 100000 };
    [HideInInspector]public double[] _waterAttackDamage = { 110, 400, 2500, 2500, 75000, 96000 };

    string earthPath = "Prefabs/Attacks/Earth/";
    string firePath = "Prefabs/Attacks/Fire/";
    string waterPath = "Prefabs/Attacks/Water/";
    string[] _earthAttackNames = { "EarthAttack_1", "EarthAttack_2", "EarthAttack_1", "EarthAttack_2", "EarthAttack_1", "EarthAttack_2" };
    string[] _fireAttackNames = { "FireAttack_1", "FireAttack_2", "FireAttack_1", "FireAttack_2", "FireAttack_1", "FireAttack_2" };
    string[] _waterAttackNames = { "WaterAttack_1", "WaterAttack_2", "WaterAttack_1", "WaterAttack_2", "WaterAttack_1", "WaterAttack_2" };

    UIManager _uiManager;

    public override void OnStart()
    {
        base.OnStart();
        if (_player == null) { _player = GetComponent<Player>(); }
        if (_uiManager == null) { _uiManager = UIManager.GetInstance; }
        _earthCooldown = new float[_earthAttackTime.Length];
        _fireCooldown = new float[_fireAttackTime.Length];
        _waterCooldown = new float[_waterAttackTime.Length];

        for (int i = 0; i < _earthAttackTime.Length; i++) _earthCooldown[i] = _earthAttackTime[i];
        for (int i = 0; i < _fireAttackTime.Length; i++) _fireCooldown[i] = _fireAttackTime[i];
        for (int i = 0; i < _waterAttackTime.Length; i++) _waterCooldown[i] = _waterAttackTime[i];
    }

    private void Update()
    {
        if (IsDie) { return; }
        HandleCooldown(_earthAttack, _earthAttackCount, _earthCooldown, _earthAttackTime, earthPath, _earthAttackNames, _earthAttackDamage, AttackType.EARTH);
        HandleCooldown(_fireAttack, _fireAttackCount, _fireCooldown, _fireAttackTime, firePath, _fireAttackNames, _fireAttackDamage, AttackType.FIRE);
        HandleCooldown(_waterAttack, _waterAttackCount, _waterCooldown, _waterAttackTime, waterPath, _waterAttackNames, _waterAttackDamage, AttackType.WATER);
    }
    void HandleCooldown(bool[] attackState, int[] attackCount, float[] cooldown, float[] maxCooldown, string path, string[] attackNames, double[] attackDamage, AttackType type)
    {
        for (int i = 0; i < attackState.Length; i++)
        {
            if (attackCount[i] > 0)
            {
                attackState[i] = true;

                if (cooldown[i] > 0f)
                {
                    cooldown[i] -= Time.deltaTime;
                    if (!_player.GetRoadWaveScript.GetBot) {
                        if (_uiManager != null) { _uiManager.GetPlayerUIInfo.GetSlotListS.UpdateCoolTime(type, i, cooldown[i], maxCooldown[i]); }
                    }   
                }
                else
                {
                    ExecuteAttack(path, attackNames[i], attackDamage[i]);
                    cooldown[i] = maxCooldown[i];
                }
            }
        }
    }
    void ExecuteAttack(string path, string attackPrefabName, double damage)
    {
        GameObject attack = SimplePool.Spawn(path, attackPrefabName);
        if (attack != null)
        {
            SpellAttack spell = attack.GetComponent<SpellAttack>();
            spell.OnStart();
            if (spell != null) { spell._damage = damage; }
            attack.transform.position = _trans.position;
        }
    }
    public void AddSpell(int index, AttackType type)
    {
        switch (type)
        {
            case AttackType.EARTH:
                SpellUpdate(_earthAttack, _earthAttackCount, index, type);
                break;
            case AttackType.FIRE:
                SpellUpdate(_fireAttack, _fireAttackCount, index, type);
                break;
            case AttackType.WATER:
                SpellUpdate(_waterAttack, _waterAttackCount, index, type);
                break;
        }
    }
    void SpellUpdate(bool[] attackState, int[] attackCount, int index, AttackType type)
    {
        attackCount[index]++;
        if (!attackState[index]) { attackState[index] = true; }
        if (_player.GetRoadWaveScript.GetBot)
        {
            // 다음 티어로 업그레이드
            if (attackCount[index] >= 3)
            {
                attackCount[index] -= 3;
                if (attackCount[index] <= 0) { attackCount[index] = 0; attackState[index] = false; }
                AddSpell(index + 1, _player.GetRoadWaveScript.GetRandomAttackType());
            }
        }
        else
        {
            // UI업데이트
            if (_uiManager != null)
            {
                _uiManager.GetPlayerUIInfo.GetSlotListS.UpdateCount(type, index, attackCount[index]);
            }
        }
    }

    public void DoDie()
    {
        if (_isDie) { return; }
        _isDie = true;
        if (_player != null) { _player.GetPlayerStats.OnHpbar(false); }
        if (_anim != null) { _anim.SetTrigger(CommonStaticKey.ANIMPARAM_DIE); _anim.SetBool(CommonStaticKey.ANIMPARAM_ISDIE, true); }
    }

    public float CoolTimeIndex(AttackType type, int arr)
    {
        if (type == AttackType.EARTH) { return _earthAttackTime[arr]; }
        else if (type == AttackType.FIRE) { return _fireAttackTime[arr]; }
        else { return _waterAttackTime[arr]; }
    }
    public double DamageIndex(AttackType type, int arr)
    {
        if (type == AttackType.EARTH) { return _earthAttackDamage[arr]; }
        else if (type == AttackType.FIRE) { return _fireAttackDamage[arr]; }
        else { return _waterAttackDamage[arr]; }
    }
    public void Upgrade(AttackType type, int arr)
    {
        if (type == AttackType.EARTH) { SpellUpgrade(_earthAttack, _earthAttackCount, arr, type); }
        else if (type == AttackType.FIRE) { SpellUpgrade(_fireAttack, _fireAttackCount, arr, type); }
        else { SpellUpgrade(_waterAttack, _waterAttackCount, arr, type); }
    }
    void SpellUpgrade(bool[] attackState, int[] attackCount, int index, AttackType type)
    {
        if (attackCount[index] >= 3)
        {
            attackCount[index] -= 3;
            if (attackCount[index] == 0)
            {
                attackState[index] = false;
            }
            // UI업데이트
            if (_uiManager != null)
            {
                _uiManager.GetPlayerUIInfo.GetSlotListS.UpdateCount(type, index, attackCount[index]);
            }
            AddSpell(index + 1, _player.GetRoadWaveScript.GetRandomAttackType());
        }
    }
}
