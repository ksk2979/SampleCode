using UnityEngine;

/// <summary>
/// 공격에 필요한 데이터들을 가지고 있다.
/// </summary>
public class BaseAttack : MonoBehaviour
{
    protected CharacterStats stats = null;
    protected UnitDamageData tdd; 

    public virtual void InitData(CharacterStats _characterStats, UnitDamageData tdd)
    {
        stats = _characterStats;
        this.tdd = tdd;
        SetInit();
    }

    public virtual void SetInit() { }

    public virtual void DoAttackTarget(Transform target) { }
    public virtual void DoCriticalHitTarget(Transform target) { } // 크리티컬 대미지
    public virtual void DoInstantKillDamageTarget(Transform target) { } // 즉사 대미지
    public virtual void DoPercentageDamage(Transform target) { } // 퍼센트 대미지

    public virtual void DoRangeAttack() { }

    public virtual void DoRangeRoundAttack() { }

    public virtual void DoFire() { }

    public virtual void DoFire(Transform _trans, CharacterStats state) { }

    public virtual void DoFire(Transform _trans, Transform _target, CharacterStats state) { }
}
