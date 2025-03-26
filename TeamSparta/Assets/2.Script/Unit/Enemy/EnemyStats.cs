using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class EnemyStats : CharacterStats
{
    float _hpbarDelayTime = 0f;
    bool _hpbarOn = false;

    // 지금 데이터가 없으니 받아서 쓰는걸로
    public void SetData(double damage, double hp, int wave)
    {
        _damage = damage;
        _moveSpeed = PickWeightedSpeed();
        float speedMultiplier = (5 - _moveSpeed) * 0.5f + 1f;
        float waveBonus = (wave <= 1) ? 1f : Mathf.Pow(1.25f, wave - 1); // 웨이브에 따른 체력 증가치
        _maxHp = hp * speedMultiplier * waveBonus; // 현재 기본 체력 30 속도 배율에 따른 체력 증가
        _hp = _maxHp;
        if (_hpBar != null && _hpbarS == null) { _hpbarS = _hpBar.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<UnityEngine.UI.Slider>(); }
        OnHpbar(false);
    }
    // 가중치를 줘서 속도가 빠르면 체력이 낮게 구성
    int PickWeightedSpeed()
    {
        int[] speeds = { 1, 2, 3, 4 };
        int[] weights = { 40, 30, 20, 10 };

        int total = 0;
        foreach (int w in weights) total += w;

        int rand = Random.Range(0, total);
        int cumulative = 0;

        for (int i = 0; i < speeds.Length; i++)
        {
            cumulative += weights[i];
            if (rand < cumulative)
                return speeds[i];
        }

        return 1;
    }
    public void OnHpbar(bool active)
    {
        if (_hpbarS != null)
        { 
            if (active)
            { 
                _hpbarOn = true;
            }
            _hpbarDelayTime = 0f;
            _hpBar.SetActive(active);
        }
    }
    public void HpbarUpdate()
    {
        if (_hpbarOn)
        {
            _hpbarDelayTime += Time.deltaTime;
            if (_hpbarDelayTime > 2f)
            {
                _hpbarDelayTime = 0f;
                _hpbarOn = false;
                OnHpbar(false);
            }
        }
    }
    public void HpbarValueUpdate()
    {
        if (_hpbarS != null) { _hpbarS.value = HealthPercent(); }
    }
    public float HealthPercent()
    {
        float healthPercent = (float)_hp / (float)_maxHp;
        return healthPercent;
    }

    public Transform GetCanvas { get { if (_hpbarS != null) { return _hpbarS.transform.parent.transform; } else { return null; } } }
}
