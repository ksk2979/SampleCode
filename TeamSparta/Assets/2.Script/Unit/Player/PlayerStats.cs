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
        if (_hpBar != null && _hpbarS == null) { _hpbarS = _hpBar.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<UnityEngine.UI.Slider>(); }
        HpbarUpdate();
    }
    public void OnHpbar(bool active)
    {
        if (_hpBar != null) { _hpBar.SetActive(active); }
    }
    public void HpbarUpdate()
    {
        if (_hpbarS != null) { _hpbarS.value = HealthPercent(); }
    }
    public float HealthPercent()
    {
        float healthPercent = (float)_hp / (float)_maxHp;
        return healthPercent;
    }
}
