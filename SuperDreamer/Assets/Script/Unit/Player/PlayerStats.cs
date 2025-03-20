using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public void SetData(double hp)
    {
        _damage = 0; // 스킬 데이터로 들어감
        _maxHp = hp;
        _hp = _maxHp;
        _moveSpeed = 1f;
        if (_hpbarS == null && _hpBar != null) { _hpbarS = GameObject.Instantiate(_hpBar).GetComponent<HpBarScript>(); _hpbarS.Init(this.transform); }
        OnHpbar(true);
        HpbarUpdate();
    }
    public void OnHpbar(bool active)
    {
        if (_hpbarS != null) { _hpbarS.gameObject.SetActive(active); if (active) { _hpbarS.TargetUpdate(); } }
    }
    public void HpbarUpdate()
    {
        if (_hpbarS != null) { _hpbarS.UpdateHealthBar(HealthPercent(), _hp.ToString("F0")); }
    }
    public float HealthPercent()
    {
        float healthPercent = (float)_hp / (float)_maxHp;
        return healthPercent;
    }
}
