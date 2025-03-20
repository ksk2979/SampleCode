using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    // 지금 데이터가 없으니 받아서 쓰는걸로
    public void SetData(double damage, double hp, int wave)
    {
        _damage = damage;// * Mathf.Pow(1.5f, wave);
        _maxHp = hp * Mathf.Pow(1.25f, wave);
        _hp = _maxHp;
        _moveSpeed = 0.5f;
        if (_hpbarS == null && _hpBar != null) { _hpbarS = GameObject.Instantiate(_hpBar).GetComponent<HpBarScript>(); _hpbarS.Init(this.transform); _hpbarS.OnLife(); }
        OnHpbar(false);
    }
    public void BossHpBarLifeOff()
    {
        if (_hpbarS != null) { _hpbarS.BossLifeOff(); HpbarUpdate(); }
    }
    public void OnHpbar(bool active)
    {
        if (_hpbarS != null) { _hpbarS.gameObject.SetActive(active); if (active) { _hpbarS.TargetUpdate(); } }
    }
    public void HpbarUpdate()
    {
        if (_hpbarS != null) { _hpbarS.UpdateHealthBar(HealthPercent(), _hp.ToString("F0")); _hpbarS.LifeTime(); }
    }
    public float HealthPercent()
    {
        float healthPercent = (float)_hp / (float)_maxHp;
        return healthPercent;
    }
}
