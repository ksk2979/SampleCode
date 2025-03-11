using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriggerManager : MonoBehaviour
{
    public TriggerStayEnemyAttack leftPunch;
    public TriggerStayEnemyAttack RightPunch;
    public TriggerStayEnemyAttack BodyPunch;

    internal void InitData(CharacterStats stats)
    {
        RightPunch.InitData(stats, stats.GetTDD1());
        leftPunch.InitData(stats, stats.GetTDD1());
        BodyPunch.InitData(stats, stats.GetTDD1());
    }

    public void DoBasicAttack01()
    {
        RightPunch.DoHitDamage();
        BodyPunch.DoHitDamage();
    }
    public void DoBasicAttack02()
    {
        leftPunch.DoHitDamage();
        BodyPunch.DoHitDamage();
    }
    public void DoBasicAttack03()
    {
        leftPunch.DoHitDamage();
        RightPunch.DoHitDamage();
        BodyPunch.DoHitDamage();
    }

    
}
