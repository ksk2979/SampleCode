using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomExplosion : TriggerEnterAttack
{
    public void DestroyEnter()
    {
        SimplePool.Despawn(gameObject);
    }
}
