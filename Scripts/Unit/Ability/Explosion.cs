using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : BaseAttack
{
    float _hitRadius = 2.5f;
    private void OnEnable()
    {
        StartCoroutine(DoHitDamageCo());
    }

    IEnumerator DoHitDamageCo()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.1f);
        DoHitDamage();
    }

    public void DoHitDamage()
    {
        var targets = Physics.OverlapSphere(transform.position, _hitRadius, tdd.layerMask);
        for (int i = 0; i < targets.Length; i++)
        {
            if (StandardFuncUnit.IsTargetCompareTag(targets[i].gameObject, tdd.targetTag))
            {
                if (targets[i].GetComponent<Interactable>() != null) { targets[i].GetComponent<Interactable>().TakeToDamage(tdd); }
                else if (targets[i].GetComponent<TentacleScript>() != null) { targets[i].GetComponent<TentacleScript>().HitDamage(tdd.damage); }
            }
        }
    }

    public void DisableEff()
    {
        SimplePool.Despawn(gameObject);
    }

    public float HitRadius { get { return _hitRadius; } set { _hitRadius = value; } }
}
