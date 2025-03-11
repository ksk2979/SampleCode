using System;
using System.Collections;
using System.Collections.Generic;
using MyData;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    [Header("- 몬스터 전용 -")]
    public float _AttackDist; //공격하기 위해 다가가는 거리
    public float _AttackRange; // 실제 공격 범위
    public float _AttackAngle;
    public float _attackSpeed = 1;
    public float _PatrollRange;

    public float _idleTime = 0.7f;
    public float _TraceRange;
    public float _FindEnemyAngle; //적을 공격하기 위해 찾을때 쓰는 앵글각 

    [Header("공격 할때 빠르게 적을 처다볼떄 회전 속도")]
    public float _rotationSpeed = 5f;
    public EnemyData _enemyData;

    internal bool isDead()
    {
        return hp <= 0;
    }

    /// <summary>
    /// 스테이지 배율 적용
    /// </summary>
    /// <returns></returns>
    private float GetDamageStageRatio(UNIT unitType)
    {
        if(unitType == UNIT.Enemy)
        {
            if(GameManager.GetInstance._nowScene != EScene.NONE)
            {
                //return Mathf.Round(_enemyData.attackDamge * StageManager.GetInstance._stageData.GetATKMultiple(_enemyData.nId));
                return StageManager.GetInstance._stageData.GetATK(_enemyData.nId);
            }
            else
            {
                return (float)_enemyData.attackDamge;
            }
        }
        else if(unitType == UNIT.Boss)
        {
            return Mathf.Round(_enemyData.attackDamge * StageManager.GetInstance._stageData.BossMultiplier);
        }
        else
        {
            return (float)_enemyData.attackDamge;
        }

    }

    /// <summary>
    ///  스테이지 배율 적용
    /// </summary>
    /// <returns></returns>
    private float GetHpStageRatio(UNIT unitType)
    {
        if (unitType == UNIT.Enemy)
        {
            if (GameManager.GetInstance._nowScene != EScene.NONE)
            {
                //return Mathf.Round(_enemyData.hp * StageManager.GetInstance._stageData.GetHPMultiple(_enemyData.nId));
                return StageManager.GetInstance._stageData.GetHP(_enemyData.nId);
            }
            else
            {
                return (float)_enemyData.hp;
            }
        }
        else if(unitType == UNIT.Boss)
        {
            return Mathf.Round(_enemyData.hp * StageManager.GetInstance._stageData.BossMultiplier);
        }
        else
        {
            return (float)_enemyData.hp;
        }
    }

    internal void SetData(EnemyData enemyData, UNIT unitType)
    {
        if(enemyData == null)
            return;

        _enemyData = enemyData;
#if TEST
        if (GlobalGameStatus.Instance.testConfig.apllyMobHp)
            hp = GlobalGameStatus.Instance.testConfig.testMonsterHp;
        if (GetComponent<Interactable>().IsSummonEnemy() == false)
            damage = _enemyData._attackDamge;
        else
            damage = GetApStageRatio();
#else
        if (GetComponent<Interactable>().IsSummonEnemy() || GetComponent<Interactable>().IsBoss())
        {
            damage = _enemyData.attackDamge;
            hp = _enemyData.hp;
        }
        else
        {
            damage = GetDamageStageRatio(unitType);
            hp = GetHpStageRatio(unitType);
        }
#endif
        maxHp = hp;


        _AttackDist = enemyData.attackDist;
        _AttackRange = enemyData.attackRange;
        _AttackAngle = enemyData.attackAngle;
        _attackSpeed = enemyData.attackSpeed;
        _PatrollRange = enemyData.patrollRange;


        _idleTime = enemyData.IdleTime;
        _TraceRange = enemyData.traceRange;
        _FindEnemyAngle = enemyData.findEnemyAngle;

        _rotationSpeed = enemyData.rotationSpeed; 

        moveSpeed = enemyData.normalMoveSpeed;
        if (GameManager.GetInstance.TestEditor) { traceMoveSpeed = enemyData.traceMoveSpeed; }
        else { traceMoveSpeed = enemyData.traceMoveSpeed + ((StageManager.GetInstance._stageNumber / 3) * 0.5f); }
        defensive = enemyData.defensivePower;

        var tdd = new UnitDamageData()
        {
            attacker = transform,
            layerMask = 1 << LayerMask.NameToLayer(CommonStaticDatas.LAYERMASK_PLAYER),
            targetTag = new string[] { CommonStaticDatas.TAG_PLAYER },
            damage = damage,
        };

        InitUnitDamageData1(tdd);
    }

    /// <summary>
    ///// 데미지 처리 하는데 체력이 0보다 크면 살아있는거임 같거나 작으면 죽음
    ///// </summary>
    ///// <param name="damage"></param>
    ///// <returns></returns>
    //public virtual bool TakeDamage(float damage, Vector3 dmgUiPos)
    //{
    //    var totalDp = _defensivePower;
    //    damage = damage - totalDp;
    //    damage = Mathf.Clamp(damage, 0, 9999999999999999);
    //    InGamePage.Current.CreateDamageUi(dmgUiPos, (int)damage);
    //    _Hp -= damage;
    //    return 0 < _Hp;
    //}
}
