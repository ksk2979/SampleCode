using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;

public class LaserBullet : BaseAttack
{
    private Transform _trans;
    private float _destoryTime;
    private PlayerStats _playerStats;

    public void SetData(Vector3 startPos, Vector3 endPos, PlayerStats playerStats)
    {
        Reset();
        if (_trans == null) { _trans = this.transform; }
        if (_playerStats == null) { _playerStats = playerStats; }
        _trans.position = startPos;
        //Vector3 direction = (endPos - startPos).normalized;
        //_trans.rotation = Quaternion.LookRotation(direction);
        InitData(_playerStats, _playerStats.GetTDD1());
    }

    private void Update()
    {
        _destoryTime += Time.deltaTime;
        if (_destoryTime > 0.3f)
        {
            _destoryTime = 0f;
            SimplePool.Despawn(this.gameObject);
        }
    }

    private void Reset()
    {
        _destoryTime = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tdd != null)
        {
            if (other.CompareTag(tdd.targetTag[0]))
            {
                if (other.GetComponent<Interactable>() != null) { other.GetComponent<Interactable>().TakeToDamage(tdd); }
                else if (other.GetComponent<TentacleScript>() != null) { other.GetComponent<TentacleScript>().HitDamage(tdd.damage); }
            }
        }
    }
}
