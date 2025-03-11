using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;

public class Enemy : Interactable
{
    public EnemyStats _enemyStats;
    public EnemyController _enemyController;
    public readonly string HeadShot = "HeadShot";
    bool _stayDamage = false;
    GameObject _instantDeathEffect;
    EnemyData _enemyData;
    public override void Init(SingleKeyData_Int data)
    {
        var enemyData = data as EnemyData;
        if (_enemyData == null) { _enemyData = enemyData; }
        _type = typeof(Enemy);
        _enemyStats.SetData(enemyData, _unit);
        _enemyController.OnStart();
        cc = _enemyController;
        cs = _enemyStats;
        this.tag = CommonStaticDatas.TAG_ENEMY;
        _stayDamage = false;
        base.Init(data);
    }

    public override void Ready()
    {
        _enemyController.Ready();
    }

    public override void TakeToDamage(UnitDamageData tdd, bool useAffect = true)
    {
        _enemyController.HitDrama();
        if (_enemyStats.isDead())
        {
            if (!_enemyController.IsDie()) { _enemyController.DoDie(); }
            return;
        }   

        if (useAffect)
            //상위에서 어빌리티 능력을 공통으로 처리한다.
            base.TakeToDamage(tdd);

        if (_enemyStats.GetIsInvincibility() == true)
            return;

        //var tdd = data.GetTDD1();
        //if (CommonFuncUnit.IsBossUnit(_enemyStats._enemyData._nId) == false) //보스 유닛이 아닐때
        if(tdd.attacker!=null && tdd.attacker.GetComponent<Interactable>().IsBossOrElite())
        {
            if (tdd._isHeadShot)
            {
                //data.hitTransPos = _enemyController._trans.position;
                //InGamePage.Current.CreateDamageUi(tdd, HeadShot); // 스트링 임시 InGamePage없어서 잠시 지움
                _enemyController.DoDie();
                return;
            }
        }
          
        if (IsPhysiceDamageImmune())
            return;

        if (_playerStats == null) { 
            _playerStats = GameObject.FindGameObjectWithTag(CommonStaticDatas.TAG_PLAYER).gameObject.GetComponent<PlayerStats>(); 
            _playerAbility = _playerStats.GetComponent<PlayerAbility>();
        }

        // 즉사 활성화
        if (tdd._ability != null && tdd._ability.GetInstantDeathOn)
        {
            // 보스가 아닌것 2% 확률로 즉사
            if (!IsBoss())
            {
                int rand = UnityEngine.Random.Range(0, 100);
                if (rand < 2)
                {
                    _enemyController.DoDie();
                    if (_instantDeathEffect == null) { _instantDeathEffect = SimplePool.Spawn(CommonStaticDatas.RES_EX, "FX_Hit_10", this.transform.position, Quaternion.identity); }
                    if (_instantDeathEffect != null) { _instantDeathEffect.SetActive(true); }
                    return;
                }
            }
        }

        if (_enemyStats.TakeDamage(tdd, _playerStats, _playerAbility))
        {
            if (IsBoss()) { InGameUIManager.GetInstance.SetBossHp(_enemyStats.HpRate()); }

            if (tdd._ability != null)
            {
                // 슬로우 / 스턴 / 도트뎀
                if (!IsBossOrElite())
                {
                    if (tdd._ability.GetStern)
                    {
                        if (_enemyController._curState != eCharacterStates.Stern) { _enemyController.SetState(eCharacterStates.Stern); }
                        else { _enemyController.GetStatusEffects.SternReAttack(); }
                    }
                    if (tdd._ability.GetShock)
                    {
                        if (_enemyController._curState != eCharacterStates.Shock) { _enemyController.SetState(eCharacterStates.Shock); }
                        else { _enemyController.GetStatusEffects.ShockAttack(); }
                    }
                    if (tdd._ability.GetSlow)
                    {
                        if (!_enemyController.GetStatusEffects.GetSlowOn) { _enemyController.GetStatusEffects.SlowOn(); }
                        else { _enemyController.GetStatusEffects.SlowReAttack(); }
                    }
                    if (tdd._ability.GetDotDamage)
                    {
                        if (!_enemyController.GetStatusEffects.GetDotOn) { _enemyController.GetStatusEffects.DotOn(tdd.damage); }
                        else { _enemyController.GetStatusEffects.DotReAttack(); }
                    }
                }
                else { }
            }
            
            _enemyController.DoHit(tdd);
            return;
        }
        if (IsBossOrElite())
            InGameUIManager.GetInstance.SetBossHp(0);
        if (_playerAbility.Blooding) { _playerAbility.BloodingHealthUp(); }
        _enemyController.DoDie();
    }

    public override void StayToDamage(UnitDamageData tdd, bool useAffect = true)
    {
        if (_stayDamage) { return; }
        StartCoroutine(DelayDamage());

        _enemyController.HitDrama();
        if (_enemyStats.isDead())
            return;

        if (_enemyStats.GetIsInvincibility())
            return;

        if (IsPhysiceDamageImmune())
            return;

        if (_playerStats == null)
        {
            _playerStats = GameObject.FindGameObjectWithTag(CommonStaticDatas.TAG_PLAYER).gameObject.GetComponent<PlayerStats>();
            _playerAbility = _playerStats.GetComponent<PlayerAbility>();
        }
        if (_enemyStats.TakeDamage(tdd, _playerStats, _playerAbility))
        {
            if (IsBoss()) { InGameUIManager.GetInstance.SetBossHp(_enemyStats.HpRate()); }
            _enemyController.DoHit(tdd);
            return;
        }
        if (IsBossOrElite())
            InGameUIManager.GetInstance.SetBossHp(0);
        if (_playerAbility.Blooding) { _playerAbility.BloodingHealthUp(); }
        _enemyController.DoDie();
    }

    IEnumerator DelayDamage()
    {
        _stayDamage = true;
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        _stayDamage = false;
    }

    PlayerStats _playerStats = null;
    PlayerAbility _playerAbility = null;
    public EnemyData GetEnemyData => _enemyData;
}
