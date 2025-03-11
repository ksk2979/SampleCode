using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;

/// <summary>
/// 발사체를 생성 하는 총구 역활을 한다.
/// </summary>
public class FirePoint : MonoBehaviour
{
    public string FX_Shoot = string.Empty;
    public string _bulletName = string.Empty;
    protected Transform _fireReadyTrans = null;
    protected CharacterStats _stats = null;
    protected UnitDamageData _tdd;//= new UnitDamageData();

    public bool IsWorld = false;
    protected Transform _target;

    private void Start()
    {
        _fireReadyTrans = transform;
    }

    public void InitData(CharacterStats stat, UnitDamageData tdd)
    {
        this._tdd = tdd;
        _stats = stat;
    }

    GameObject FireBullet(Transform fireFrom, string bulletResource, bool isWorld)
    {
        if (!string.IsNullOrEmpty(FX_Shoot)) { SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, FX_Shoot, fireFrom.position, Quaternion.identity); }
        var bullet = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, bulletResource, fireFrom.position, Quaternion.identity);
        bullet.transform.forward = fireFrom.forward;
        if (!isWorld) { bullet.transform.SetParent(fireFrom); }

        return bullet;
    }

    #region 일반포,기관총
    public virtual void DoFire()
    {
        GameObject goShot = FireBullet(_fireReadyTrans, _bulletName, IsWorld);

        goShot.GetComponentInChildren<Shot>().InitData(_stats, _tdd);
    }

    public virtual void DoFire(CharacterStats stat, UnitDamageData tdd, PlayerAbility.ContinuouslyShot continuouslyShot)
    {
        DoFireOverLapFuntion(stat, tdd);

        if (continuouslyShot._continuouslyShot) { 
            StartCoroutine(DelayDoFire(stat, tdd, 0.05f));
            //if (continuouslyShot._continuouslyShotCount == 2) // 현재 멀티샷은 한번만 중첩된다
            //{
            //    StartCoroutine(DelayDoFire(stat, tdd, 0.1f));
            //}
        }
    }
    IEnumerator DelayDoFire(CharacterStats stat, UnitDamageData tdd, float time)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        DoFireOverLapFuntion(stat, tdd);
    }

    void DoFireOverLapFuntion(CharacterStats stat, UnitDamageData tdd)
    {
        GameObject goShot = FireBullet(_fireReadyTrans, _bulletName, IsWorld);

        goShot.transform.localScale = new Vector3(1, 1, 1);
        goShot.GetComponentInChildren<Shot>().InitData(stat, tdd);
    }

    public virtual void DoFireSkill(CharacterStats stat, UnitDamageData tdd)
    {
        GameObject goShot = FireBullet(_fireReadyTrans, _bulletName, IsWorld);

        goShot.GetComponentInChildren<Skill>().InitData(stat, tdd);
    }
    
    public virtual void DoFireSkill(CharacterStats stat, UnitDamageData tdd, Transform target)
    {
        if (string.IsNullOrEmpty(FX_Shoot) == false)
            SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, FX_Shoot, _fireReadyTrans.position, Quaternion.identity);
        var goShot = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _bulletName, _fireReadyTrans.position, Quaternion.identity);
        goShot.transform.forward = (target.position - _fireReadyTrans.position).normalized;
        if (IsWorld == false)
            goShot.transform.SetParent(_fireReadyTrans);
    
        goShot.GetComponentInChildren<Skill>().InitData(stat, tdd);
    }
    public virtual void DoFireSkill(CharacterStats stat, UnitDamageData tdd, Vector3 pos)
    {
        if (string.IsNullOrEmpty(FX_Shoot) == false)
            SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, FX_Shoot, _fireReadyTrans.position, Quaternion.identity);
        var goShot = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _bulletName, _fireReadyTrans.position, Quaternion.identity);
        goShot.transform.forward = (pos - _fireReadyTrans.position).normalized;
        if (IsWorld == false)
            goShot.transform.SetParent(_fireReadyTrans);
    
        goShot.GetComponentInChildren<Skill>().InitData(stat, tdd);
    }
    #endregion

    #region 자주포
    public virtual void DoFireParabola(PlayerStats stat, WeaponData weaponData, Vector3 target, PlayerAbility.ContinuouslyShot continuouslyShot)
    {
        DoFireParabolaFuntion(stat, weaponData, target);

        if (continuouslyShot._continuouslyShot)
        {
            StartCoroutine(DelayDoFireParabola(stat, weaponData, target, 0.05f));
            if (continuouslyShot._continuouslyShotCount == 2)
            {
                StartCoroutine(DelayDoFireParabola(stat, weaponData, target, 0.1f));
            }
        }
    }

    IEnumerator DelayDoFireParabola(PlayerStats stat,  WeaponData weaponData, Vector3 target, float time)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        DoFireParabolaFuntion(stat, weaponData, target);
    }

    void DoFireParabolaFuntion(PlayerStats stat, WeaponData weaponData, Vector3 target)
    {
        GameObject goShot = FireBullet(_fireReadyTrans, _bulletName, IsWorld);

        goShot.GetComponent<ParabolaBullet>().SetData(this.transform.position, target, weaponData, stat);
    }
    #endregion

    internal void SetTarget(Transform target)
    {
        _target = target;
    }

    #region 레이저

    public void DoFireLaser(PlayerStats stat, Vector3 target, PlayerAbility.ContinuouslyShot continuouslyShot)
    {
        DoFireLaserFuntion(stat, target);

        if (continuouslyShot._continuouslyShot)
        {
            StartCoroutine(DelayDoFireLaser(stat, target, 0.05f));
            if (continuouslyShot._continuouslyShotCount == 2)
            {
                StartCoroutine(DelayDoFireLaser(stat, target, 0.1f));
            }
        }
    }
    IEnumerator DelayDoFireLaser(PlayerStats stat, Vector3 target, float time)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        DoFireLaserFuntion(stat, target);
    }

    void DoFireLaserFuntion(PlayerStats stat, Vector3 target)
    {
        GameObject goShot = FireBullet(_fireReadyTrans, _bulletName, IsWorld);

        goShot.GetComponent<LaserBullet>().SetData(this.transform.position, target, stat);
    }
    #endregion

    #region 지뢰

    public void DoFireMine(PlayerStats stat, WeaponData weaponData, Vector3 target, PlayerAbility.ContinuouslyShot continuouslyShot)
    {
        DoFireMineFuntion(stat, weaponData, target);

        if (continuouslyShot._continuouslyShot)
        {
            StartCoroutine(DelayDoFireMine(stat, weaponData, target, 0.05f));
            if (continuouslyShot._continuouslyShotCount == 2)
            {
                StartCoroutine(DelayDoFireMine(stat, weaponData, target, 0.1f));
            }
        }
    }
    IEnumerator DelayDoFireMine(PlayerStats stat, WeaponData weaponData, Vector3 target, float time)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        DoFireMineFuntion(stat, weaponData, target);
    }

    void DoFireMineFuntion(PlayerStats stat, WeaponData weaponData, Vector3 target)
    {
        GameObject goShot = FireBullet(_fireReadyTrans, _bulletName, IsWorld);

        goShot.GetComponent<MineBullet>().SetData(this.transform.position, target, weaponData, stat);
    }
    #endregion

    #region 어뢰
    public virtual void DoFireTorpedo(PlayerStats stat, WeaponData weaponData, Vector3 target, PlayerAbility.ContinuouslyShot continuouslyShot)
    {
        DoFireTorpedoFuntion(stat, weaponData, target);

        if (continuouslyShot._continuouslyShot)
        {
            StartCoroutine(DelayDoFireTorpedo(stat, weaponData, target, 0.05f));
            if (continuouslyShot._continuouslyShotCount == 2)
            {
                StartCoroutine(DelayDoFireTorpedo(stat, weaponData, target, 0.1f));
            }
        }
    }

    IEnumerator DelayDoFireTorpedo(PlayerStats stat, WeaponData weaponData, Vector3 target, float time)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        DoFireTorpedoFuntion(stat, weaponData, target);
    }

    void DoFireTorpedoFuntion(PlayerStats stat, WeaponData weaponData, Vector3 target)
    {
        GameObject goShot = FireBullet(_fireReadyTrans, _bulletName, IsWorld);

        goShot.GetComponent<TorpedoBullet>().SetData(this.transform.position, target, weaponData, stat);
    }
    #endregion
}
