using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMeeleAttack : BaseAttack
{
    protected EnemyStats _enemyStats = null;

    UnitDamageData _tempTdd;
    public override void SetInit()
    {
        base.SetInit();
        _enemyStats = stats as EnemyStats;
        _tempTdd = new UnitDamageData();
    }

    public override void DoRangeAttack()
    {
        if (stats == null)
            return;
        var tdd = stats.GetTDD1();
        var targets = Physics.OverlapSphere(tdd.attacker.position,  _enemyStats._AttackRange, tdd.layerMask);
        for (int i = 0; i < targets.Length; i++)
        {
            //if (targets[i].CompareTag(tdd.targetTag))
            if(StandardFuncUnit.IsTargetCompareTag(targets[i].gameObject, tdd.targetTag))
            {
                var dir = targets[i].transform.position - tdd.attacker.position;
                float direction = Vector3.Dot(dir, tdd.attacker.forward);
                if (direction > Mathf.Cos((_enemyStats._AttackRange / 2) * Mathf.Deg2Rad))
                {
                    //데미지 주기
                    targets[i].GetComponent<Interactable>().TakeToDamage(tdd);
                }

            }
        }
        enabled = false;
    }

    public override void DoRangeRoundAttack()
    {
        if (stats == null)
            return;
        var tdd = stats.GetTDD1();
        var targets = Physics.OverlapSphere(tdd.attacker.position, _enemyStats._AttackRange, tdd.layerMask);
        for (int i = 0; i < targets.Length; i++)
        {
            if (StandardFuncUnit.IsTargetCompareTag(targets[i].gameObject, tdd.targetTag))
            {
                //    //데미지 주기
                    targets[i].GetComponent<Interactable>().TakeToDamage( tdd);
                //}

            }
        }
        enabled = false;
    }

    public override void DoAttackTarget(Transform target)
    {
        if (target == null) return;

        var tdd = stats.GetTDD1();
        if (IsTargetInRange(target, tdd.attacker.position, tdd.attacker.forward, 150))
        {
            // 데미지 주기
            target.GetComponent<Interactable>().TakeToDamage(tdd);
        }
    }
    public override void DoCriticalHitTarget(Transform target)
    {
        if (target == null) return;

        _tempTdd.CopyFrom(stats.GetTDD1());
        // 2배 데미지 주기
        _tempTdd.damage *= 2;

        if (IsTargetInRange(target, _tempTdd.attacker.position, _tempTdd.attacker.forward, 150))
        {
            target.GetComponent<Interactable>().TakeToDamage(_tempTdd);
        }
    }
    public override void DoInstantKillDamageTarget(Transform target)
    {
        if (target == null) return;

        _tempTdd.CopyFrom(stats.GetTDD1());
        // 즉사 데미지 주기
        _tempTdd.damage = float.MaxValue;

        if (IsTargetInRange(target, _tempTdd.attacker.position, _tempTdd.attacker.forward, 150))
        {
            target.GetComponent<Interactable>().TakeToDamage(_tempTdd);
        }
    }
    public override void DoPercentageDamage(Transform target)
    {
        if (target == null) return;

        Interactable player = target.GetComponent<Interactable>();
        _tempTdd.CopyFrom(stats.GetTDD1());
        // 선체 최대 체력의 지정된 퍼센트 데미지 교체
        _tempTdd.damage = StandardFuncUnit.OperatorValue(player.GetStats().maxHp, _tempTdd.damage, OperatorCategory.persent);
        if (IsTargetInRange(target, _tempTdd.attacker.position, _tempTdd.attacker.forward, 150))
        {
            target.GetComponent<Interactable>().TakeToDamage(_tempTdd);
        }
    }

    private bool IsTargetInRange(Transform target, Vector3 attackerPosition, Vector3 attackerForward, float angle)
    {
        var dir = target.position - attackerPosition;
        float direction = Vector3.Dot(dir, attackerForward);
        return direction > Mathf.Cos((angle / 2) * Mathf.Deg2Rad);
    }
}
