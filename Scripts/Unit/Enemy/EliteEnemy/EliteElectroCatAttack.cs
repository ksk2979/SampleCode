using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ����� ���� ���� �ָ� ��
public class EliteElectroCatAttack : BaseAttack
{
    // �����°� ������� �ָ� ��
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
