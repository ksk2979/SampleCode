using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteSnailAttack : BaseAttack
{
    private Transform _trans;
    private float _destoryTime;
    private EnemyStats _stats;

    public void SetData(Vector3 startPos, EnemyStats playerStats)
    {
        Reset();
        if (_trans == null) { _trans = this.transform; }
        if (_stats == null) { _stats = playerStats; }
        _trans.position = startPos;
        InitData(_stats, _stats.GetTDD1());
    }

    private void Update()
    {
        _destoryTime += Time.deltaTime;
        if (_destoryTime > 0.5f)
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
                other.GetComponent<Interactable>().TakeToDamage(tdd);
            }
        }
    }
}