using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHippocampusAttack : NormalMeeleAttack
{
    EnemyHippocampusController _controller;

    public void Init(EnemyHippocampusController controller, EnemyStats enemyStats)
    {
        _controller = controller;
        stats = enemyStats;
        tdd = stats.GetTDD1();
    }

    // 닿이면 다른 보트에 공격을 이어간다
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(CommonStaticDatas.TAG_PLAYER))
        {
            _controller.AttackOn();
            DoAttackTarget(other.transform);
        }
    }
    public void ActiveOn()
    {
        this.gameObject.SetActive(true);
    }
    public void ActiveOff()
    {
        this.gameObject.SetActive(false);
    }
}
