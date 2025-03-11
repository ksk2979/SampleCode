using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

// 소량 업과 hp회복은 기본적인 어빌리티 선택창에는 안나온다
public enum AbilityCategory
{
    AttackPoint = 0, // 공격력 30% 증가
    MaxHpPoint, // hp최대치 10% 증가
    ShotSpeedSUp, // 공격 속도 소량 업
    ShotSpeedLUP, // 공격 속도 대량 업
    HelthPoint, // 체력 일시 회복(총 3회까지)
    Normal_Shot, // 전방
    Diagonal_Shot, // 30,60도샷
    Side_Shot, // 사이드샷(90도)
    ContinuouslyShot, // 멀티샷
    WhitePhosphorus, // 백린탄(도트데미지 추가)
    ElectricBullet, // 전기탄(0.1초 마비 같은 것)
    SlowBullet, // 슬로우탄(지금 되어있음)
    GravityBullet, // 중력탄 (탄을 쏘면 지면에 닿은 이후 일정 시간 뒤 주변 적을 끌어당김)
    InstantDeathBullet, // 즉사탄 (확률로 적을 즉사시킴 / 보스는 적용 안됨)
    BerSerKer, // 광전사 스킬
    Invincible, // 무적 스킬
    Blooding, // 피흡 스킬
    Resurrection, // 부활 스킬(자동 사용: 80% 회복)
    RotationProtection, // 회전 보호 
    AbilityMaster, // 어빌리티대출 (패시브 기능으로 웨이브마다 나오는 어빌리티를 미리 땡겨서 받을수 있음)
    NettingCreate, // 그물망생성(자동생성으로 몇초마다 그물망이 메인 보트 뒤에 나온다 / 몬스터의 경로 방해)
    GhostShip, // 유령선(도발선)
    Swirl, // 소용돌이 (플레이어 주변에 생성되며 천천히 랜덤한 방향으로 나아간다 / 블랙홀 효과)
    AirStrike, // 공중 포격
    AirMachineGun, // 공중 기관총
    PoisonField, // 독장판
    FireField, // 불장판
    ElectricField, // 전기장판
    FlourField, // 전분장판
    SentrySummon, // 센트리 소환
    BombardmentRequest, // 포격 요청 (보스 객체에 큰거 한방 맥이고 시작)
    SatelliteRequest, // 위성 요청
    DroneCreate, // 드론 생성
    // 추가 뮤탈효과 같은 어빌리티
    // 추가 포획용 드론 (확률적으로 나오는 유니크몬스터를 자동으로 가서 잡아주는 것, 잡을 시 나온 유니크 몬스터의 등급에 따라 잡는 확률이 다른다)
    // 위 포획용 드론에 의해 유니크 몬스터의 수정이 필요 : 몬스터 나오게 되면 일정 시간 이후 들어가게 수정 (웨이브와 관계 x), 때려서 잡게 될때에는 유니크의 체력을 많이 올려서 어느정도 딜을 하게 해서 잡게한다
    NONE
}

public class CharacterStats : MonoBehaviour
{
    [Header("- 공용 -")]
    [Tooltip("일반공격 데미지")]
    public float damage; //데미지
    
    public float GetCalcDamage(UnitDamageData data, PlayerStats ps = null, PlayerAbility pa = null)
    {
        float damage = data.damage;// * data.dmgTotalRatio; // 데미지 증가인데 일단 나둠
        damage = damage - (damage * defensiveRatio);
        damage = Mathf.Clamp(damage, 0, 9999999999999999);
        if (ps != null && pa != null) { // 광전사 모드 일 경우
            if (pa.Berserker)
            {
                float cent = ps.maxHp * 0.5f; // 전체 체력의 50%
                if (cent > ps.hp)
                {
                    pa.BerserkerMode(true);
                    damage += (data.damage * 0.1f);
                }
                else { pa.BerserkerMode(false); }
            }
        }
        return damage;
    }

    [Tooltip("내구도(체력)")]
    public float hp; //체력
    [HideInInspector]
    public float maxHp;
    [Tooltip("방어력")]
    public float defensive;

    public float defensiveRatio
    {
        get
        {
            return (defensive / 1000f);
        }
    }


    [Tooltip("평상시 걸을때 속도(정찰시)")]
    public float moveSpeed = 5f; //평상시 속도
    [Tooltip("적을 쫓아갈때 속도 (빨라짐) 맥스 속도로 쓰임")]
    public float traceMoveSpeed = 5f; //쫓아갈때 속도

    private UnitDamageData UnitDamageData1;
    private UnitDamageData UnitDamageData2;
    private UnitDamageData UnitDamageData3;
    private UnitDamageData UnitDamageData4;
    private ParabolaData parabolaData;

    [Tooltip("자기 자신의 HPBar")]
    public GameObject _hpBar;
    

    public float HpRate()
    {
        return hp / maxHp;
    }
    public string HpSRate()
    {
        return string.Format("{0:N0}", hp);
    }
    /// <summary>
    /// 체력이 적으면 참
    /// </summary>
    /// <returns></returns>
    public bool LowHp()
    {
        return hp < maxHp;
    }

    /// <summary>
    /// ratio 보다 비율이 적으면 true
    /// </summary>
    /// <param name="ratio"></param>
    /// <returns></returns>
    public bool LowHp(float ratio)
    {
        return hp / maxHp < ratio;
    }

    internal void Revival()
    {
        hp = maxHp;
    }

    /// <summary>
    /// 어빌리티 중 치료 효과
    /// </summary>
    /// <param name="v"></param>
    public void Heal(float v)
    {
        if (hp < 0) { hp = 0; }
        hp = Mathf.Clamp((hp + v), 0, maxHp);
    }
    /// <summary>
    /// 어빌리티 중 최대치 증가 효과
    /// </summary>
    public void MaxHpUp(float v)
    {
        maxHp += v;
    }

    public virtual void InitUnitDamageData1(UnitDamageData UnitDamageData)
    {
        UnitDamageData1 = UnitDamageData;
    }

    public virtual UnitDamageData GetTDD1()
    {
        return UnitDamageData1;
    }

    public virtual void InitUnitDamageData2(UnitDamageData UnitDamageData)
    {
        UnitDamageData2 = UnitDamageData;
    }

    public virtual UnitDamageData GetTDD2()
    {
        return UnitDamageData2;
    }

    public virtual void InitUnitDamageData3(UnitDamageData UnitDamageData)
    {
        UnitDamageData3 = UnitDamageData;
    }

    public virtual UnitDamageData GetTDD3()
    {
        return UnitDamageData3;
    }
    public virtual void InitUnitDamageData4(UnitDamageData UnitDamageData)
    {
        UnitDamageData4 = UnitDamageData;
    }
    public virtual UnitDamageData GetTDD4()
    {
        return UnitDamageData4;
    }


    public virtual void InitParabolaData(ParabolaData parabolaData)
    {
        this.parabolaData = parabolaData;
    }

    public virtual ParabolaData GetPara()
    {
        return parabolaData;
    }

    private bool isInvincibility = false; //무적 적용
    public void SetIsInvincibility(bool invincibilty)
    {
        isInvincibility = invincibilty;
    }

    public bool GetIsInvincibility()
    {
        return isInvincibility;
    }
    /// <summary>
    /// 데미지 처리 하는데 체력이 0보다 크면 살아있는거임 같거나 작으면 죽음
    /// </summary>
    /// <returns></returns>
    public virtual bool TakeDamage(UnitDamageData data, PlayerStats ps = null, PlayerAbility pa = null)
    {
        //var interactable = GetComponent<Interactable>();
        //if (interactable.IsDamageDecrease())
        //{
        //    //맞는 놈의 데미지 감소율 적용
        //    var dd = interactable.GetCharacterCtrl().GetAffect(Affects.eAffects.DamageDecrease) as DamageDecrease;
        //    data.decreaseDamageRatio = Mathf.Clamp01(data.decreaseDamageRatio + dd.decreaseRatio); //데미지 감소율 셋팅
        //}

        float damage = GetCalcDamage(data, ps, pa);
        hp -= damage;
        return 1 < hp;
    }
    public virtual bool TakeDamage(float damage)
    {
        hp -= damage;
        return 1 < hp;
    }
}
