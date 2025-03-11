using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRaser : FirePoint
{
    private int targetLayer = 0;
    public override void DoFire(CharacterStats stat, UnitDamageData tdd, PlayerAbility.ContinuouslyShot continuouslyShot)
    {
        if(targetLayer == 0)
            targetLayer = 1 << LayerMask.NameToLayer(CommonStaticDatas.LAYERMASK_OBSTACLE);
        if (Physics.Raycast(_fireReadyTrans.position, _fireReadyTrans.forward, out RaycastHit hit, 1000999, targetLayer))
        {
            if (string.IsNullOrEmpty(FX_Shoot) == false)
                SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, FX_Shoot, _fireReadyTrans.position, Quaternion.identity);
            var goShot = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _bulletName, Vector3.zero, Quaternion.identity);
            goShot.GetComponent<WeaponLine>().OnFireData(_fireReadyTrans.position, hit.point, stat, tdd);
        }
    }
}
