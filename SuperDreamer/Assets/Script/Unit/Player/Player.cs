using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Interactable
{
    PlayerController _playerController;
    PlayerStats _playerStats;
    RoadWaveScript _roadWaveS;

    public override void Init()
    {
        if (_playerController == null) { _playerController = GetComponent<PlayerController>(); }
        if (_playerStats == null) { _playerStats = GetComponent<PlayerStats>(); }
        _playerController.OnStart();
        _playerStats.SetData(3000d);
    }
    public override void TakeToDamage(double damage)
    {
        if (_playerController.IsDie) { return; }
        if (_playerStats.TakeDamage(damage)) { _playerStats.HpbarUpdate(); }
        else { _playerController.DoDie(); _roadWaveS.PlayerDie(); }
    }
    public void InRoadWaveScript(RoadWaveScript roadWave)
    {
        _roadWaveS = roadWave;
    }

    public PlayerController GetPlayerController { get { return _playerController; } }
    public PlayerStats GetPlayerStats { get { return _playerStats; } }
    public RoadWaveScript GetRoadWaveScript { get { return _roadWaveS; } }
}
