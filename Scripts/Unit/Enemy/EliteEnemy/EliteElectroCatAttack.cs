using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 전기 고양이 장판 딜만 주면 됨
public class EliteElectroCatAttack : BaseAttack
{
    // 들어오는거 대미지만 주면 됨
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(CommonStaticDatas.TAG_PLAYER))
        {
            Interactable player = other.GetComponent<Interactable>();
            if (player.GetDelayHit) { return; }

            if (player.GetController() != null)
            {
                player.GetController().KnockBackActionStart();
                player.GetDelayHit = true;
                player.DelayCheckTime(2f);
                player.TakeToDamage(tdd);
            }
        }
    }
}
