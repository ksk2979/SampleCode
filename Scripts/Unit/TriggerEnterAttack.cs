using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnterAttack : BaseAttack
{
    public string _hitEffectStr = string.Empty;
    private void OnTriggerEnter(Collider other)
    {
        if (tdd == null)
            return;
        // 데미지 주기
        //if (other.CompareTag(tdd.targetTag))
        if (StandardFuncUnit.IsTargetCompareTag(other.gameObject, tdd.targetTag))
        {
            if (string.IsNullOrEmpty(_hitEffectStr) == false)
            {
                var obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _hitEffectStr);
                obj.transform.position = other.transform.position;
            }
            other.GetComponent<Interactable>().TakeToDamage(tdd);
        }
    }
}
