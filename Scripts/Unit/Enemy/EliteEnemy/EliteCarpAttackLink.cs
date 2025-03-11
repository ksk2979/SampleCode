using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �׾ �� ����� ���� ����� ��ũ��Ʈ
public class EliteCarpAttackLink : BaseAttack
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(CommonStaticDatas.TAG_PLAYER))
        {
            Interactable player = other.GetComponent<Interactable>();
            if (player.GetDelayHit) { player.TakeToDamage(tdd); return; }

            if (player.GetController() != null)
            {
                player.GetController().KnockBackActionStart();
                player.GetDelayHit = true;
                player.DelayCheckTime(1f);
                player.TakeToDamage(tdd);
            }
        }
    }
}
