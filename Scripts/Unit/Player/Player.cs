using System;
using System.Collections;
using System.Collections.Generic;
using MyData;
using UnityEngine;

public class Player : Interactable
{
    public PlayerController _PlayerController;
    public PlayerStats _playerStats;
    public PlayerAbility _playerAbility;
    public override void Init(DoubleKeyData_Int data, DoubleKeyData_Int data2, Info.PlayerInfo info, DoubleKeyData_Int data3 = null, DoubleKeyData_Int data4 = null, DoubleKeyData_Int data5 = null, DoubleKeyData_Int data6 = null)
    {
        var boatData = data as BoatData;
        var weaponData = data2 as WeaponData;
        DefenseData defenseData = null;
        CaptainData captainData = null;
        SailorData sailorData = null;
        EngineData engineData = null;
        if (data3 != null) { defenseData = data3 as DefenseData; }
        if (data4 != null) { captainData = data4 as CaptainData; }
        if (data5 != null) { sailorData = data5 as SailorData; }
        if (data6 != null) { engineData = data6 as EngineData; }
        cc = _PlayerController;
        cs = _playerStats;
        _playerStats.SetData(boatData, weaponData, info, defenseData, captainData, sailorData, engineData);
        _PlayerController.OnStart();
        _playerAbility.Init();
        _option = InGameUIManager.GetInstance.GetPopup<OptionManager>(PopupType.OPTION);
    }

    public override void Init(Info.PlayerInfo info, Dictionary<EItemList, int> dataIdDictionary)
    {
        cc = _PlayerController;
        cs = _playerStats;
        _playerStats.SetData(info, dataIdDictionary);
        _PlayerController.OnStart();
        _playerAbility.Init();
        _option = InGameUIManager.GetInstance.GetPopup<OptionManager>(PopupType.OPTION);
    }

    public override void Ready()
    {
        _PlayerController.Ready();
    }

    public void SetNumber(int num)
    {
        _playerStats.SetNumber(num);
    }

    internal void AddAbility(AbilityData abilityData)
    {
        if (_PlayerController.IsDie())
            return;
        _playerAbility.ApplyAbility(abilityData, _PlayerController);
    }
    OptionManager _option;
    public override void TakeToDamage(UnitDamageData tdd, bool useAffect = true)
    {
        if (_PlayerController.GetGhostShip) { return; }
        _PlayerController.HitDramaMain();
        //상위에서 어빌리티 능력을 공통으로 처리한다.
        base.TakeToDamage(tdd);
        if (IsPhysiceDamageImmune())
            return;
        if (_playerStats.GetIsInvincibility()) { return; }
      
        if (_playerStats.TakeDamage(tdd))
        {
            if (_option != null) { if (_option.GetVibration) { Vibration.Vibrate((long)50); } }
            _PlayerController.DoHit(tdd);
            return;
        }
        if (_playerAbility.Resurrection) { _playerAbility.ResurrectionAction(); return; }

        _PlayerController.DoDie();
    }
    public void TakeToDamage(float damage)
    {
        if (_PlayerController.GetGhostShip) { return; }
        _PlayerController.HitDramaMain();
        //상위에서 어빌리티 능력을 공통으로 처리한다.
        //base.TakeToDamage(tdd);
        if (IsPhysiceDamageImmune())
            return;

        if (_playerStats.GetIsInvincibility()) { return; }

        if (_playerStats.TakeDamage(damage))
        {
            if (_option != null) { if (_option.GetVibration) { Vibration.Vibrate((long)50); } }
            _PlayerController.DoHit();
            return;
        }
        if (_playerAbility.Resurrection) { _playerAbility.ResurrectionAction(); return; }

        _PlayerController.DoDie();
    }

 
}
