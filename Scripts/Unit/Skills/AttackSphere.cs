using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackSphere : BaseAttack
{
    PlayerAbility _playerAbility;
    public void InitData(PlayerAbility player, UnitDamageData data)
    {
        tdd = data;
        _playerAbility = player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tdd != null)
        {
            if (other.CompareTag(tdd.targetTag[0]))
            {
                var unit = other.GetComponent<Interactable>();
                unit.TakeToDamage(tdd);
                if (_playerAbility.GetKnockBack)
                {
                    if (unit.GetController()._curState != eCharacterStates.KnockBack) { unit.GetController().SetState(eCharacterStates.KnockBack); }
                    else { unit.GetController().GetStatusEffects.KnockBackAttack(); }
                }
            }
        }
    }

    public void DoHitDamage()
    {
        var targets = Physics.OverlapSphere(transform.position, 2.5f, tdd.layerMask);
        for (int i = 0; i < targets.Length; i++)
        {
            if (StandardFuncUnit.IsTargetCompareTag(targets[i].gameObject, tdd.targetTag))
                targets[i].GetComponent<Interactable>().TakeToDamage(tdd);
        }
    }
}
