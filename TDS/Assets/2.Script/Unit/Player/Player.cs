using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Interactable
{
    PlayerController _playerController;
    PlayerStats _playerStats;

    public override void Init()
    {
        if (_playerController == null) { _playerController = GetComponent<PlayerController>(); }
        if (_playerStats == null) { _playerStats = GetComponent<PlayerStats>(); }
        _playerController.OnStart();
        _playerStats.SetData(30000d);
    }
    public override void TakeToDamage(double damage)
    {
        if (_playerController.IsDie) { return; }
        if (_playerStats.TakeDamage(damage)) { _playerStats.HpbarUpdate(); }
        else { _playerController.DoDie(); }
    }

    public PlayerController GetPlayerController { get { return _playerController; } }
    public PlayerStats GetPlayerStats { get { return _playerStats; } }
}
