using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnterManager : MonoBehaviour
{
    public TriggerEnterAttack[] triggerEnterAttacks;
    
    public void OnTriggerAttacks(bool flag)
    {
        for (int i = 0; i < triggerEnterAttacks.Length; i++)
        {
            triggerEnterAttacks[i].enabled = flag;
        }
    }

    internal void InitData(EnemyStats enemyStats)
    {
        for (int i = 0; i < triggerEnterAttacks.Length; i++)
        {
            triggerEnterAttacks[i].InitData(enemyStats, enemyStats.GetTDD1());
        }
        OnTriggerAttacks(false);
    }
}
