using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] protected double _hp;
    [SerializeField] protected double _maxHp;
    [SerializeField] protected double _damage;
    [SerializeField] protected float _moveSpeed = 5f;

    [Tooltip("자기 자신의 HPBar")]
    [SerializeField] protected GameObject _hpBar;
    protected Slider _hpbarS;

    internal void Revival()
    {
        _hp = _maxHp;
    }
    public void Heal(double v)
    {
        _hp = _hp + v;
        if (_hp >= _maxHp) { _hp = _maxHp; }
    }
    public double GetCalcDamage(double hitDamage)
    {
        double damage = hitDamage;
        if (damage <= 0) { damage = 0; }

        return damage;
    }

    public virtual bool TakeDamage(double hitDamage)
    {
        double damage = GetCalcDamage(hitDamage);
        _hp -= damage;
        return 0 < _hp;
    }
    public double Hp { get { return _hp; } }
    public double Damage { get { return _damage; } }
    public float Speed { get { return _moveSpeed; } }
}
