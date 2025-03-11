using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStayEnemyAttack : BaseAttack
{
    protected List<Collider> targets = new List<Collider>();

    private void OnDisable()
    {
        if (0 < targets.Count)
        {
            targets.Clear();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (stats == null)
            return;
        if (StandardFuncUnit.IsTargetCompareTag(other.gameObject, tdd.targetTag))
        {
            if (targets.Contains(other) == false)
            {
                targets.Add(other);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (stats == null)
            return;
        //if (other.CompareTag(stats.GetTDD1().targetTag))
        if (StandardFuncUnit.IsTargetCompareTag(other.gameObject, tdd.targetTag))
        {
            if (targets.Contains(other))
            {
                targets.Remove(other);
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].CompareTag(CommonStaticDatas.TAG_ENEMY))
            {
                if (targets[i].GetComponent<CharacterController>().IsDie())
                {
                    targets.RemoveAt(i);
                }
            }
            else if (targets[i].CompareTag(CommonStaticDatas.TAG_TRAP))
            {
                if (targets[i].gameObject.activeSelf == false)
                {
                    targets.RemoveAt(i);
                }
            }
        }
    }

    public virtual void DoHitDamage()
    {
        if (stats == null)
            return;
        for (int i = 0; i < targets.Count; i++)
        {
            //데미지 주기
            targets[i].GetComponent<Interactable>().TakeToDamage( tdd);
        }
    }
}
