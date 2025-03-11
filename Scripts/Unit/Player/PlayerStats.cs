using MyData;
using UnityEngine;
//using UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerStats : CharacterStats
{
    //[Tooltip("파일럿 공격데미지 ")]
    //public float pilotDamage; //데미지
    //[Tooltip("파일럿 공격시 타겟팅 할수 있는 범위 ")]
    //public float pilotShootingRange; //데미지

    [Tooltip("공격시 타겟팅 할수 있는 범위 포탑이씀")]
    public float turretShootingRange;
    [Tooltip("이동하기 시작할때 가속도 빠르기")]
    public float speedAccelate;
    [Tooltip("공격후 딜레이(0~1)(최고 공격속도는 _fireDelayTime _maxFireAnimSpeed 두개의 값으로 빨라진다)")]
    public float turretfireRate;
    //public float pilotfireRate;
    //[Tooltip("파츠
    //력")]
    //public float partDefensivePoawer = 0;
    [Tooltip("어빌리티 방어 퍼센테이지")]
    public float abilityDefensivePoawerRate = 0;

    [Tooltip("배기본 선회 속도")]
    public float rotationSpeed;
    [Tooltip("선회시 속도가 느려지는 값(0 - 500)")]
    public float rotationAccelateSpeedRatio;
    [Tooltip("제동력")]
    public float braking;

    public int _curLevel = 0;
    public int _nextLevel
    {
        get
        {
            return _curLevel + 1;
        }
    }
    //휘발성 저장 데이터
    public float _exp = 0;

    private BoatData _boatData;
    private WeaponData _weaponData;

    // 현재 어빌리티 상황
    // 전면 포 3개 max, 사선 포 2개 max, 측면 포 1개 max 각각의 어빌 상황 체크해서 포지션 잡기

    int[] _addedStatus = new int[4];
    // 위치
    int _posNumber;
    StageManager _stageManager;

    public void SetNumber(int num) => _posNumber = num;

    internal void SetData(Info.PlayerInfo info, Dictionary<EItemList, int> dataIdDictionary)
    {
        float addDamage = 0;
        float addHp = 0;
        float addSpd = 0;
        float addSpdA = 0;
        moveSpeed = 0;
        traceMoveSpeed = 0;
        speedAccelate = 0;
        
        foreach (var id in dataIdDictionary)
        {
            if(id.Key == EItemList.BOAT)
            {
                _boatData = DataManager.GetInstance.GetData(DataManager.KEY_BOAT, dataIdDictionary[EItemList.BOAT], 1) as BoatData;

                addDamage += _boatData.damage + ((info.GetPlayerValue(ECoalescenceType.BOAT_LEVEL) - 1) * info.GetEquipBoatValue());
                addHp += _boatData.hp + ((info.GetPlayerValue(ECoalescenceType.BOAT_LEVEL) - 1) * info.GetEquipValue(EItemList.BOAT));

                moveSpeed += _boatData.moveSpeed;
                traceMoveSpeed += _boatData.moveSpeed;
                speedAccelate += _boatData.speedAccelate;
                rotationSpeed = _boatData.rotationSpeed;
                rotationAccelateSpeedRatio = _boatData.rotationAccelateSpeed / 500;
                braking = _boatData.braking / 500;
            }
            else if(id.Key == EItemList.WEAPON)
            {
                _weaponData = DataManager.GetInstance.GetData(DataManager.KEY_WEAPON, dataIdDictionary[EItemList.WEAPON], 1) as WeaponData;
                addDamage += _weaponData.damage + ((info.GetPlayerValue(ECoalescenceType.WEAPON_LEVEL) - 1) * info.GetEquipValue(EItemList.WEAPON));

                turretShootingRange = _weaponData.shootingRange;
                turretfireRate = _weaponData.fireRate;
            }
            else if (id.Key == EItemList.DEFENSE)
            {
                addDamage += UnitDataManager.GetInstance.GetFloatValue(EItemList.DEFENSE, dataIdDictionary[EItemList.DEFENSE], StatusType.atkValue);
                addHp += UnitDataManager.GetInstance.GetFloatValue(EItemList.DEFENSE, dataIdDictionary[EItemList.DEFENSE], StatusType.value)
                        + ((info.GetPlayerValue(ECoalescenceType.DEFENSE_LEVEL) - 1) * info.GetEquipValue(EItemList.DEFENSE));
            }
            else if(id.Key == EItemList.CAPTAIN)
            {
                addDamage += UnitDataManager.GetInstance.GetFloatValue(EItemList.CAPTAIN, dataIdDictionary[EItemList.CAPTAIN], StatusType.damage)
                        + ((info.GetPlayerValue(ECoalescenceType.CAPTAIN_LEVEL) - 1) * info.GetEquipValue(EItemList.CAPTAIN));
            }
            else if(id.Key == EItemList.SAILOR)
            {
                 addHp += UnitDataManager.GetInstance.GetFloatValue(EItemList.SAILOR, dataIdDictionary[EItemList.SAILOR], StatusType.value) 
                        + ((info.GetPlayerValue(ECoalescenceType.SAILOR_LEVEL) - 1) * info.GetEquipValue(EItemList.SAILOR)); 
            }
            else if(id.Key == EItemList.ENGINE)
            {
                float addValue = (info.GetPlayerValue(ECoalescenceType.ENGINE_LEVEL) - 1) * info.GetEquipValue(EItemList.ENGINE);
                addSpd += UnitDataManager.GetInstance.GetFloatValue(EItemList.ENGINE, dataIdDictionary[EItemList.ENGINE], StatusType.value) + addValue;
                addSpdA += UnitDataManager.GetInstance.GetFloatValue(EItemList.ENGINE, dataIdDictionary[EItemList.ENGINE], StatusType.speedAccelate) + addValue;

                moveSpeed += addSpd;
                traceMoveSpeed += addSpd;
                speedAccelate += addSpdA;
            }
        }
        damage = addDamage;
        switch (_weaponData.weaponType)
        {
            case 1:
                damage *= 0.5f;
                break;
            case 2:
                damage *= 0.3f;
                break;
            case 4:
                damage *= 0.75f;
                break;
            case 3:
                damage *= 1f;
                break;
            case 5:
            case 6:
                damage *= 2f;
                break;
            default:
                damage *= 1f;
                break;
        }
        maxHp = addHp;
        hp = maxHp;

        CheckEquipPotential(info);
        ApplyStatusPotential();
        var gm = GameManager.GetInstance;
        if (gm.dmgMultiple > 0f)
        {
            damage *= gm.dmgMultiple;
        }
        if(gm.hpMultiple > 0f)
        {
            maxHp *= gm.hpMultiple;
            hp = maxHp;
        }
    }
    
    internal void SetData(BoatData data, WeaponData weaponData, Info.PlayerInfo info, DefenseData defenseData = null, CaptainData captainData = null, SailorData sailorData = null, EngineData engineData = null)
    {
        Debug.Log("SetData Side");
        _boatData = data;
        _weaponData = weaponData;

        // 공격력
        float addDamage = weaponData.damage + ((info.GetPlayerValue(ECoalescenceType.WEAPON_LEVEL) - 1) * info.GetEquipValue(EItemList.WEAPON));
        if (data != null) { addDamage += data.damage + ((info.GetPlayerValue(ECoalescenceType.BOAT_LEVEL) - 1) * info.GetEquipBoatValue()); }
        if (captainData != null) { addDamage += (captainData.damage + ((info.GetPlayerValue(ECoalescenceType.CAPTAIN_LEVEL) - 1) * info.GetEquipValue(EItemList.CAPTAIN))); }
        if (defenseData != null) { addDamage += defenseData.damage; }
        damage = addDamage;

        defensive = 0;

        // 체력
        float addHp = 0f;
        addHp += (data.hp + ((info.GetPlayerValue(ECoalescenceType.BOAT_LEVEL) - 1) * info.GetEquipValue(EItemList.BOAT)));
        if (defenseData != null) { addHp += (defenseData.value + ((info.GetPlayerValue(ECoalescenceType.DEFENSE_LEVEL) - 1) * info.GetEquipValue(EItemList.DEFENSE))); }
        if (sailorData != null) { addHp += (sailorData.value + ((info.GetPlayerValue(ECoalescenceType.SAILOR_LEVEL) - 1) * info.GetEquipValue(EItemList.SAILOR))); }
        maxHp = addHp;
        //maxHp += 90000; // 테스트
        hp = maxHp;

        turretShootingRange = _weaponData.shootingRange;
        turretfireRate = _weaponData.fireRate;
        float addSpd = 0f;
        float addSpdA = 0f;
        if (engineData != null) { addSpd += (engineData.value + ((info.GetPlayerValue(ECoalescenceType.ENGINE_LEVEL) - 1) * info.GetEquipValue(EItemList.ENGINE))); }
        if (engineData != null) { addSpdA += (engineData.speedAccelate + ((info.GetPlayerValue(ECoalescenceType.ENGINE_LEVEL) - 1) * info.GetEquipValue(EItemList.ENGINE))); }
        moveSpeed = _boatData.moveSpeed + addSpd;
        traceMoveSpeed = _boatData.moveSpeed + addSpd;
        speedAccelate = _boatData.speedAccelate + addSpdA;

        rotationSpeed = _boatData.rotationSpeed;
        rotationAccelateSpeedRatio = _boatData.rotationAccelateSpeed / 500;
        braking = _boatData.braking / 500;

        SetUnitDamageData();
    }

    #region Potential
    /// <summary>
    /// 장비 포텐셜 확인
    /// 스텟옵션은 장착된 보트에 적용
    /// 어빌리티는 메인 보트에 적용
    /// </summary>
    /// <param name="info"></param>
    void CheckEquipPotential(Info.PlayerInfo info)
    {
        if (_stageManager == null) _stageManager = StageManager.GetInstance;

        for (int i = (int)EItemList.CAPTAIN; i <= (int)EItemList.ENGINE; i++)
        {
            AddEquipPotential(info.GetPotential((EItemList)i));
        }
    }

    /// <summary>
    /// 장비 포텐셜 등록
    /// </summary>
    /// <param name="data"></param>
    void AddEquipPotential(List<int> data)
    {
        DataManager dm = DataManager.GetInstance;
        for (int i = 0; i < data.Count; i++)
        {
            ItemPotentialData originData = dm.FindData(DataManager.KEY_POTENTIAL, data[i]) as ItemPotentialData;
            if (originData == null || data[i] == 0)
            {
                continue;
            }
            else
            {
                if ((EPotentialType)originData.potenType == EPotentialType.Ability)
                {
                    _stageManager.SetAddedPotential(originData.abilityEnum);
                }
                else
                {
                    _addedStatus[originData.potenType] += originData.value;
                }
            }
        }
    }

    public void ApplyPotenAbility()
    {
        if (_stageManager == null)
        {
            _stageManager = StageManager.GetInstance;
        }
        // 어빌리티 추가
        var dataManager = DataManager.GetInstance;
        var player = _stageManager.GetPlayersController()[0].GetComponent<Player>();
        var abilityList = _stageManager.GetAddedAbility;
        for (int j = 0; j < abilityList.Count; j++)
        {
            Debug.Log(abilityList[j]);
            var abData = dataManager.FindData(DataManager.KEY_ABILITY, abilityList[j]) as AbilityData;
            if (abData == null)
            {
                continue;
            }
            Debug.Log("Add Ability : " + abData.name);
            player._playerAbility.ApplyAbility(abData, player._PlayerController);
        }
    }
    /// <summary>
    /// 포텐셜 적용
    /// </summary>
    void ApplyStatusPotential()
    {
        // 스텟 추가
        int[] addStatusArr = new int[4];
        for (int i = 0; i < _addedStatus.Length; i++)
        {
            if (_addedStatus[i] <= 0)
                continue;

            switch ((EPotentialType)i)
            {
                case EPotentialType.AtkMultiple:
                    {
                        float dmg = damage * (_addedStatus[i] * 0.01f);
                        addStatusArr[0] = Mathf.RoundToInt(dmg);
                    }
                    break;
                case EPotentialType.AtkAdded:
                    {
                        addStatusArr[1] = _addedStatus[1];
                    }
                    break;
                case EPotentialType.HpMultiple:
                    {
                        float hp = maxHp * (_addedStatus[i] * 0.01f);
                        addStatusArr[2] = Mathf.RoundToInt(hp);
                    }
                    break;
                case EPotentialType.HpAdded:
                    {
                        addStatusArr[3] = _addedStatus[3];
                    }
                    break;
            }
        }

        damage += addStatusArr[0] + addStatusArr[1];
        maxHp += addStatusArr[2] + addStatusArr[3];
        hp = maxHp;
        SetUnitDamageData();
    }
    #endregion Potential

    /// <summary>
    /// 유닛 데미지 설정
    /// </summary>
    public void SetUnitDamageData()
    {
        var turretTdd = new UnitDamageData()
        {
            attacker = transform,
            layerMask = 1 << LayerMask.NameToLayer(CommonStaticDatas.LAYERMASK_UNIT),
            targetTag = new string[] { CommonStaticDatas.TAG_ENEMY, CommonStaticDatas.TAG_TRAP },
            damage = damage,
            _ability = this.GetComponent<PlayerAbility>()
        };

        InitUnitDamageData1(turretTdd);
    }

    /// <summary>
    /// 경험치 획득
    /// </summary>
    /// <param name="exp"></param>
    internal void AddExp(float exp)
    {
        _exp += exp;
        //InGamePage.Current.UpdateLevelExp();
    }
    /// <summary>
    /// UI경험치가 다차면 레벨업!
    /// </summary>
    public void LevelUp()
    {
        Debug.Log("level up!");
        _curLevel++;
        Heal(0.3f);
        //UiManager.Instance.Push(EUiPage.LearnAbilityPage);
        //InGamePage.Current.UpdateLevelExp();
    }

    /// <summary>
    /// 참조가 없음 안쓰는듯 아 효과 추가하는 부분을 처리해주는 공용 함수인데 좋은 것같음
    /// </summary>
    /// <param name="UnitDamageData"></param>
    /// <returns></returns>
    public UnitDamageData GetAddAbilityAttackDmgInfo(UnitDamageData UnitDamageData)
    {
        return UnitDamageData;
    }

    /// <summary>
    /// 0브타 다음 레벨업 할 경험치의 다더해 돌려준다.
    /// </summary>
    /// <param name="curLevel"></param>
    /// <returns></returns>
    internal float GetUntilNextLevlNeedExp(int curLevel)
    {
        float addExp = 0;
        //for (int i = 0; i < curLevel; i++)//curLevel+1 다음레벨
        //{
        //    addExp += StageManager.Current.GetCurChapterExp(i);
        //}
        return addExp;
    }
    //ui에 표시할 exp 량
    public float GetCurUIExp(int nextLevel, float exp)
    {
        float curExp = 0;
        var nextUntilExp = GetUntilNextLevlNeedExp(nextLevel);
        curExp = exp - nextUntilExp;

        return curExp;
    }
    //ui에 표시할 exp 비율
    public float GetCurUIExpRate()
    {
        var maxExp = GetUntilNextLevlNeedExp(_nextLevel) - GetUntilNextLevlNeedExp(_curLevel);
        var curExp = GetCurUIExp(_curLevel, _exp);
        return curExp / maxExp;
    }

    public IEnumerator InvincibleStart(GameObject obj)
    {
        float time = 3f;
        SetIsInvincibility(true);
        obj.SetActive(true);

        while (time > 0f)
        {
            time -= Time.deltaTime;
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }
        SetIsInvincibility(false);
        obj.SetActive(false);
    }
   

    public int GetPosNumber { get { return _posNumber; } set { _posNumber = value; } }
    public BoatData GetBoatData => _boatData;
}

public enum ShotAbility
{
    slow = 3,
    // 여기서 부터는 내가 만든 어빌
    NONE = 0,
    NORMALSHOT_2 = 10, NORMALSHOT_3, // 전방(다른 포지션으로 교체)
    DIAGONALSHOT_1, DIAGONALSHOT_2, // 30,60도샷 (중첩)
    SIDESHOT, // 사이드샷(90도) (한개 밖에 없음)
    
}