using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ParabolaBoomBoll : ParabolaBoll
{
    private void OnTriggerEnter(Collider other)
    {
        // 데미지 주기
        if (StandardFuncUnit.IsTargetCompareTag(other.gameObject, tdd.targetTag))
        {
            DestroyThis();
            return;
        }

        if (other.CompareTag(CommonStaticDatas.TAG_FLOOR))
        {
            DestroyThis();
        }
    }

    public override  void DestroyThis()
    {
        if (gameObject.activeSelf == false)
            return;
        var targets = Physics.OverlapSphere(_trans.position, tdd.explosionRange, tdd.layerMask).ToList();
        targets.Remove(_trans.GetComponent<Collider>());
        foreach (var item in targets)
        {
            var interactable = item.GetComponent<Interactable>();
            interactable.TakeToDamage(tdd);
        }
        if (string.IsNullOrEmpty(FX_Enemy_Projectile_01_HIt) == false)
        {
            var pos = _trans.position;
            pos.y = 0;
            SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, FX_Enemy_Projectile_01_HIt, pos, Quaternion.identity);
        }
        //SimplePool.Spawn(_hitEffect, _transform.position, Quaternion.identity);
        SimplePool.Despawn(gameObject);
    }
}
