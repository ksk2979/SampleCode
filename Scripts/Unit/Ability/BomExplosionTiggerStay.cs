using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomExplosionTiggerStay : TriggerStayEnemyAttack
{
    public void DestroyEnter()
    {
        SimplePool.Despawn(gameObject);
    }
}
