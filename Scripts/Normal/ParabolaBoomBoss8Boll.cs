using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ParabolaBoomBoss8Boll : ParabolaBoomBoll
{
    public string hitField = "FX_Boss_08_Gas_Field";
    public override void DestroyThis()
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

        if (string.IsNullOrEmpty(hitField) == false)
        {
            var pos = _trans.position;
            pos.y = 0;
            var fieldObj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, hitField, pos, Quaternion.identity);
            fieldObj.GetComponent<Skill>().InitData(stat, tdd); // 스킬 클래스 없음
        }
        //SimplePool.Spawn(_hitEffect, _transform.position, Quaternion.identity);
        SimplePool.Despawn(gameObject);
    }
}
