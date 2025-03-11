using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShipManager : BaseAttack
{
    [SerializeField] GameObject _ghostShipPrefab;
    GhostShipObj _ghostShip;
    PlayerController _playerC;

    float _maxTime = 15f;
    float _time = 0f;

    Transform _target;

    public void InitData(PlayerController playerController)
    {
        _playerC = playerController;
    }
    public void Target(Transform target)
    {
        _target = target;
    }

    private void FixedUpdate()
    {
        if (_playerC.IsDie()) { return; }

        _time += Time.deltaTime;
        if (_time > _maxTime)
        {
            _time = 0f;
            CreateGhostShip();
        }
    }

    void CreateGhostShip()
    {
        if (_ghostShip == null)
        {
            GhostShipObj obj = GameObject.Instantiate(_ghostShipPrefab).GetComponent<GhostShipObj>();
            _ghostShip = obj;
            _ghostShip.InitData();
            Vector3 target = new Vector3(_target.position.x + Random.Range(-20, 20), 0f, _target.position.z + Random.Range(-20, 20));
            _ghostShip.StartAbility(target);
        }
        else
        {
            Vector3 target = new Vector3(_target.position.x + Random.Range(-20, 20), 0f, _target.position.z + Random.Range(-20, 20));
            _ghostShip.StartAbility(target);
        }
    }
}