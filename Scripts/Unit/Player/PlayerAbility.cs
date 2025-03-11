using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;
using UnityEngine.XR;
using UnityEngine.Purchasing;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] Player _player;

    /// <summary>
    /// 어빌리티 모음 AbilityCategory의 값 참고
    /// </summary>
    private List<AbilityData> _abilitySelect;
    Dictionary<AbilityCategory, int> _abilityCountDic;

    // 한 챕터의 어빌 사용 횟수 제한을 위해 (이제 어빌리티를 지속 넣어줘야되서 이거 필요없어짐)
    public int _abilityCount;
    public int _abilityCountMax;

    [Tooltip("어빌리티 추가 공격")]
    public ShotAbility[] _shotAbility; // 0 교체되는 전방, 1 중첩되는 사선, 2 측면

    //[Tooltip("주변 배회 공격")]
    AbilitySphereSkill _rotationProtection;
    //UMManager _umManager;
    AirSupportManager _asManager;
    AirMachineGunManager _amManager;
    //TorpedoManager _tpoManager;
    SwirlManager _swirlManager;
    NettingCreateManager _nettingCreateManager;
    GhostShipManager _ghostShipManager;
    DataManager _dataManager;
    GravityManager _gravityManager;
    ArtilleryManager _artilleryManager;
    SentryManager _sentryManager;
    BombardmentRequestManager _bombardmentManager;
    SatelliteLaserCallManager _satelliteLaserManager;
    DroneManager _droneManager;

    // 광전사 스킬
    bool _berserker = false;
    GameObject _berserkerEffect;
    // 무적 스킬
    Invincible _invincible = null;
    bool _invincibleB = false;
    // 피흡 스킬
    bool _blooding = false;
    AbilityData _bloodingData = null;
    GameObject _bloodingEffect;
    // 부활 스킬
    bool _resurrection = false;
    GameObject _resurrectionEffect;
    AbilityData _resurrectionData = null;
    // 멀티샷
    public struct ContinuouslyShot
    {
        public bool _continuouslyShot;
        public int _continuouslyShotCount;
    }
    ContinuouslyShot _continuouslyShot;
    // 아직 사용은 안되었지만 멀티샷 중접의 경우 여러발 쏠수 있게
    // 지원 공중 공격

    bool _sternOn = false;
    bool _knockBackOn = false;
    bool _knockDownOn = false;
    bool _shockOn = false;
    bool _slowOn = false;
    bool _instantDeathOn = false;
    bool _dotDamageOn = false;
    StageSetData _stageData;

    public void Init()
    {
        _abilitySelect = new List<AbilityData>();
        _abilityCountDic = new Dictionary<AbilityCategory, int>();
        _dataManager = DataManager.GetInstance;
        _ingameUIManager = GameObject.Find("InGameCanvas").GetComponent<InGameUIManager>();
    }

    InGameUIManager _ingameUIManager;
    const string filePath = "ItemIcon/Ability{0:D2}";
    const string fileName = "Ability{0:D2}";

    /// <summary>
    /// 어빌리티 효과 추가 메서드
    /// </summary>
    /// <param name="abilityData">어빌리티 데이터</param>
    /// <param name="pc">플레이어 컨트롤러</param>
    internal void ApplyAbility(AbilityData abilityData, PlayerController pc)
    {
        string effectRes = string.Empty;
        AbilityCategory category = (AbilityCategory)(abilityData.nId - 1);
        switch (category)
        {
            case AbilityCategory.AttackPoint:
                {
                    effectRes = "FX_Buff_01";
                    AttackPointUp(abilityData);
                }
                break;
            case AbilityCategory.MaxHpPoint:
                effectRes = "FX_Buff_02";
                _player._playerStats.MaxHpUp(OperatorValue(_player._playerStats.maxHp, abilityData));
                _player._playerStats.Heal(OperatorValue(_player._playerStats.maxHp, abilityData));
                break;
            case AbilityCategory.ShotSpeedSUp:
            case AbilityCategory.ShotSpeedLUP:
                effectRes = "FX_Buff_03";
                _player._playerStats.turretfireRate = OperatorValue(_player._playerStats.turretfireRate, abilityData);
                pc._gunFireTop.AttackSpeedUp();
                break;
            case AbilityCategory.BerSerKer:
                effectRes = "FX_Buff_11";
                _berserker = true;
                break;
            case AbilityCategory.Invincible:
                effectRes = "FX_Buff_12";
                if (!_invincibleB)
                {
                    _invincibleB = true;
                    _invincible = GameObject.Find("InGameCanvas").transform.Find("InGameUI").transform.Find("Invincible").GetComponent<Invincible>();
                    _invincible.gameObject.SetActive(true);
                }
                _invincible.InvincibleCountUp();
                break;
            case AbilityCategory.Blooding:
                effectRes = "FX_Buff_13";
                _blooding = true;
                _bloodingData = abilityData;
                break;
            case AbilityCategory.Resurrection:
                effectRes = "FX_Buff_14";
                _resurrection = true;
                _resurrectionData = abilityData;
                break;
            case AbilityCategory.Normal_Shot:
                effectRes = "FX_Buff_05";
                if (_shotAbility[0] == ShotAbility.NONE) { _shotAbility[0] = ShotAbility.NORMALSHOT_2; }
                else if (_shotAbility[0] == ShotAbility.NORMALSHOT_2) { _shotAbility[0] = ShotAbility.NORMALSHOT_3; }
                break;
            case AbilityCategory.Diagonal_Shot:
                effectRes = "FX_Buff_05";
                if (_shotAbility[1] == ShotAbility.NONE) { _shotAbility[1] = ShotAbility.DIAGONALSHOT_1; }
                else if (_shotAbility[1] == ShotAbility.DIAGONALSHOT_1) { _shotAbility[1] = ShotAbility.DIAGONALSHOT_2; }
                break;
            case AbilityCategory.Side_Shot:
                effectRes = "FX_Buff_05";
                if (_shotAbility[2] == ShotAbility.NONE) { _shotAbility[2] = ShotAbility.SIDESHOT; }
                break;
            case AbilityCategory.ContinuouslyShot:
                effectRes = "FX_Buff_05";
                _continuouslyShot._continuouslyShot = true;
                _continuouslyShot._continuouslyShotCount++;
                break;
            case AbilityCategory.RotationProtection:
                effectRes = "FX_Buff_15";
                // 처음 들어오면
                if (_rotationProtection == null)
                {
                    AbilitySphereSkill obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_ROTATIONPROTECTION]).GetComponent<AbilitySphereSkill>();
                    obj.Init(this, _player.GetStats().GetTDD1(), this.transform);
                    _rotationProtection = obj;
                    _knockBackOn = true;
                }
                else { _rotationProtection.AddSkillCount(); }
                break;
            /*            case AbilityCategory.UnderwaterMines:
                            effectRes = "Striking_Power_UP_tank";
                            if (_umManager == null)
                            {
                                GameObject obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_UNDERWATERMINES]);
                                obj.transform.localPosition = Vector3.zero;
                                _umManager = obj.GetComponent<UMManager>();
                                _umManager.InitData(_player.GetStats().GetTDD1(), _player.GetController() as PlayerController);
                                _umManager.Init();
                                _umManager.Target(_player.transform);
                            }
                            else { }
                            break;*/
            case AbilityCategory.AirStrike:
                //effectRes = "Striking_Power_UP_tank";
                if (_asManager == null)
                {
                    GameObject obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_AIRSUPPORT]);
                    obj.transform.localPosition = Vector3.zero;
                    _asManager = obj.GetComponent<AirSupportManager>();
                    _asManager.InitData(_player.GetStats().GetTDD1(), _player.GetController() as PlayerController);
                    _asManager.Target(_player.transform);
                }
                else { }
                break;
            case AbilityCategory.AirMachineGun:
                if (_amManager == null)
                {
                    GameObject obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_AIRMACHINEGUN]);
                    obj.transform.localPosition = Vector3.zero;
                    _amManager = obj.GetComponent<AirMachineGunManager>();
                    _amManager.InitData(_player.GetStats().GetTDD1(), _player.GetController() as PlayerController);
                    _amManager.Target(_player.transform);
                }
                else { }
                break;
            case AbilityCategory.PoisonField:
                HandleArtilleryField(0);
                break;
            case AbilityCategory.FireField:
                HandleArtilleryField(1);
                break;
            case AbilityCategory.ElectricField:
                HandleArtilleryField(2);
                break;
            case AbilityCategory.FlourField:
                HandleArtilleryField(3);
                break;
            case AbilityCategory.SentrySummon:
                if (_sentryManager == null)
                {
                    GameObject obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_SENTRY]);
                    obj.transform.localPosition = Vector3.zero;
                    _sentryManager = obj.GetComponent<SentryManager>();
                    _sentryManager.InitData(_player.GetStats().GetTDD1(), _player.GetController() as PlayerController);
                    _sentryManager.Target(_player.transform);
                }
                else { }
                break;
            case AbilityCategory.BombardmentRequest:
                if (_bombardmentManager == null)
                {
                    GameObject obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_BOMBARDMENTREQUEST]);
                    obj.transform.localPosition = Vector3.zero;
                    _bombardmentManager = obj.GetComponent<BombardmentRequestManager>();
                    _bombardmentManager.InitData(_player.GetStats().GetTDD1(), _player.GetController() as PlayerController);
                    _bombardmentManager.PlayerTarget(_player.transform);
                    //_bombardmentManager.Target(_player.transform);
                }
                break;
            /*            case AbilityCategory.Torpedo:
                            effectRes = "Striking_Power_UP_tank";
                            if (_tpoManager == null)
                            {
                                GameObject obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_TORPEDO]);
                                obj.transform.localPosition = Vector3.zero;
                                _tpoManager = obj.GetComponent<TorpedoManager>();
                                _tpoManager.InitData(_player.GetStats().GetTDD1(), _player.GetController() as PlayerController);
                                _tpoManager.Init();
                                _tpoManager.Target(_player.transform);
                            }
                            else { }
                            break;*/
            case AbilityCategory.SatelliteRequest:
                if (_satelliteLaserManager == null)
                {
                    GameObject obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_SATELLITEREQUEST]);
                    obj.transform.localPosition = Vector3.zero;
                    _satelliteLaserManager = obj.GetComponent<SatelliteLaserCallManager>();
                    _satelliteLaserManager.InitData(_player.GetStats().GetTDD1(), _player.GetController() as PlayerController);
                }
                else { }
                break;
            case AbilityCategory.DroneCreate:
                if (_droneManager == null)
                {
                    GameObject obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_DRONE]);
                    obj.transform.localPosition = Vector3.zero;
                    _droneManager = obj.GetComponent<DroneManager>();
                    _droneManager.InitData(_player.GetStats().GetTDD1(), _player.GetController() as PlayerController);
                }
                else { }
                break;
            case AbilityCategory.WhitePhosphorus:
                effectRes = "FX_Buff_06";
                if (!_dotDamageOn)
                {
                    _dotDamageOn = true;
                }
                else { }
                break;
            case AbilityCategory.GhostShip:
                //effectRes = "Striking_Power_UP_tank";
                if (_ghostShipManager == null)
                {
                    GameObject obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_GHOSTSHIP]);
                    obj.transform.localPosition = Vector3.zero;
                    _ghostShipManager = obj.GetComponent<GhostShipManager>();
                    _ghostShipManager.InitData(_player.GetController() as PlayerController);
                    _ghostShipManager.Target(_player.transform);
                }
                else { }
                break;
            // 어빌리티 장인
            // 중첩가능한 어빌리티 보유시 상한치까지 중첩 시켜줌
            case AbilityCategory.AbilityMaster:
                effectRes = "FX_Buff_16";
                if (_stageData == null) { _stageData = StageManager.GetInstance._stageData; }
                List<AbilityData> targetList = new List<AbilityData>();
                foreach (var ability in _abilityCountDic)
                {
                    var data = _dataManager.FindData(DataManager.KEY_ABILITY, (int)(ability.Key + 1)) as AbilityData;
                    if (ability.Value < data.max)
                    {
                        targetList.Add(data);
                    }
                }
                // 대상 어빌리티(중첩가능한)중 랜덤으로 하나 뽑아서 어빌리티 추가
                if (targetList.Count > 0)
                {
                    int rand = Random.Range(0, targetList.Count);
                    var targetData = targetList[rand];
                    // 어빌리티 Max치까지 중첩
                    for (int i = _abilityCountDic[(AbilityCategory)(targetData.nId - 1)]; i < targetData.max; i++)
                    {
                        AddCountDictionary((AbilityCategory)(targetList[rand].nId - 1));
                        ApplyAbility(targetList[rand], pc);
                    }
                }
                break;
            case AbilityCategory.Swirl:
                //effectRes = "FX_Buff_03";
                if (_swirlManager == null)
                {
                    GameObject obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_SWIRL]);
                    obj.transform.localPosition = Vector3.zero;
                    _swirlManager = obj.GetComponent<SwirlManager>();
                    _swirlManager.InitData(_player.GetStats().GetTDD1(), _player.GetController() as PlayerController);
                    _swirlManager.Init();
                    _swirlManager.Target(_player.transform);
                }
                else { }
                break;
            case AbilityCategory.ElectricBullet:
                effectRes = "FX_Buff_07";
                if (!_sternOn)
                {
                    _sternOn = true;
                }
                else { }
                break;
            case AbilityCategory.SlowBullet:
                effectRes = "FX_Buff_09";
                if (!_slowOn)
                {
                    _slowOn = true;
                }
                else { }
                break;
            case AbilityCategory.GravityBullet:
                effectRes = "FX_Buff_08";
                if (_gravityManager == null)
                {
                    GameObject obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_GRAVITY]);
                    obj.transform.localPosition = Vector3.zero;
                    _gravityManager = obj.GetComponent<GravityManager>();
                    _gravityManager.InitData(_player.GetStats().GetTDD1(), _player.GetController() as PlayerController);
                    _gravityManager.Init();
                    _gravityManager.Target(_player.transform);
                }
                else { }
                break;
            case AbilityCategory.InstantDeathBullet:
                effectRes = "FX_Buff_10";
                if (!_instantDeathOn) { _instantDeathOn = true; }
                else { }
                break;
            case AbilityCategory.NettingCreate:
                //effectRes = "FX_Buff_17";
                if (_nettingCreateManager == null)
                {
                    GameObject obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_NETTINGCREATE]);
                    obj.transform.localPosition = Vector3.zero;
                    _nettingCreateManager = obj.GetComponent<NettingCreateManager>();
                    _nettingCreateManager.Init(_player);
                    //_nettingCreateManager.Target(_player.transform);
                    _shockOn = true;
                }
                else { }
                break;
            case AbilityCategory.HelthPoint:
                {
                    effectRes = "FX_Buff_04";
                    _player._playerStats.Heal(OperatorValue(_player._playerStats.maxHp, abilityData));
                }
                break;
            default:
                effectRes = "FX_Buff_01";
                break;
        }
        // 1:공격 2:방어 3:속성 4: 회복 5: 포탄종류 6: 백린탄o 7: 전기탄o 8: 중력탄o 9: 슬로우탄o 10: 즉사탄o 11: 불굴 12: 절대방어
        // 13: 피격수리 14: 복구 15: 물대포 16: 어빌리티 장인 17: 그물망 18: 유령선 19: 소용돌이 20: 공중포격

        Sprite iconImage = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(filePath, (int)category + 1));
        PausePopup popup = _ingameUIManager.GetPopup<PausePopup>(PopupType.PAUSE);
        popup.AddAbilityInfo(fileName + (int)category + 1, iconImage);
        if (effectRes != "" || !string.IsNullOrEmpty(effectRes))
        {
            var abilitEff = SimplePool.Spawn(CommonStaticDatas.RES_EX, effectRes, Vector3.zero, Quaternion.identity);
            abilitEff.transform.position = transform.position;
            abilitEff.transform.SetParent(transform);
            abilitEff.transform.localScale = GradeScale(_player.GetComponent<PlayerStats>().GetBoatData.grade);
        }
    }
    Vector3 GradeScale(int grade)
    {
        Vector3 temp;

        if (grade == 1) { temp = new Vector3(0.5f, 0.5f, 0.5f); }
        else if (grade == 2) { temp = new Vector3(0.6f, 0.6f, 0.6f); }
        else if (grade == 3) { temp = new Vector3(0.7f, 0.7f, 0.7f); }
        else if (grade == 4) { temp = new Vector3(0.8f, 0.8f, 0.8f); }
        else if (grade == 5) { temp = new Vector3(0.9f, 0.9f, 0.9f); }
        else { temp = new Vector3(1f, 1f, 1f); }

        return temp;
    }

    private void HandleArtilleryField(int fieldType)
    {
        if (_artilleryManager == null)
        {
            GameObject obj = GameObject.Instantiate(GameManager.GetInstance._prefabs[(int)AbilitySkillPrefabs.E_ARTILLERY]);
            obj.transform.localPosition = Vector3.zero;
            _artilleryManager = obj.GetComponent<ArtilleryManager>();
            _artilleryManager.InitData(_player.GetStats().GetTDD1(), _player.GetController() as PlayerController, fieldType);
            _artilleryManager.Target(_player.transform);
        }
        else
        {
            if (!_artilleryManager.GetArtilleryCheck[fieldType])
            {
                _artilleryManager.InitData(_player.GetStats().GetTDD1(), _player.GetController() as PlayerController, fieldType);
                _artilleryManager.Target(_player.transform);
            }
        }
    }

    public void BloodingHealthUp()
    {
        if (_bloodingEffect == null) { _bloodingEffect = SimplePool.Spawn(CommonStaticDatas.RES_EX, "FX_Hit_13", this.transform, Vector3.zero, Quaternion.identity); }
        if (_bloodingEffect != null) { _bloodingEffect.SetActive(true); }
        _player._playerStats.Heal(OperatorValue(_player._playerStats.maxHp, _bloodingData));
    }
    public void ResurrectionAction()
    {
        _resurrection = false;
        if (_resurrectionEffect == null) { _resurrectionEffect = SimplePool.Spawn(CommonStaticDatas.RES_EX, "FX_Buff_14", this.transform, Vector3.zero, Quaternion.identity); }
        if (_resurrectionEffect != null) { _resurrectionEffect.SetActive(true); }
        _player._playerStats.Heal(OperatorValue(_player._playerStats.maxHp, _resurrectionData));
    }

    /// <summary>
    /// 사용가능 검사
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public bool CheckAvailable(AbilityCategory category)
    {
        int idx = (int)category + 1;
        int level = UserData.GetInstance.GetUserLevel;
        var data = _dataManager.FindData(DataManager.KEY_ABILITY, idx) as AbilityData;
        if (_abilityCountDic.ContainsKey(category))
        {
            if (_abilityCountDic[category] < data.max)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (data.appear <= level)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    /// <summary>
    /// 어빌리티 횟수 등록
    /// </summary>
    /// <param name="category"></param>
    void AddCountDictionary(AbilityCategory category)
    {
        if (_abilityCountDic.ContainsKey(category))
        {
            _abilityCountDic[category]++;
        }
        else
        {
            _abilityCountDic.Add(category, 1);
        }
    }
    public int InvincibleCountCheck()
    {
        if (_invincible == null) { _invincible = GameObject.Find("InGameCanvas").transform.Find("InGameUI").transform.Find("Invincible").GetComponent<Invincible>(); }
        return _invincible._count;
    }

    /// <summary>
    /// persent 기준값의 퍼센테이지 만큼값을 리턴
    /// add 기준값에 해당값을 더한값을 리턴
    /// </summary>
    /// <param name="defualtValue"></param>
    /// <param name="abilityData"></param>
    /// <returns></returns>
    public float OperatorValue(float defualtValue, AbilityData abilityData)
    {
        return StandardFuncUnit.OperatorValue(defualtValue, abilityData.value, (OperatorCategory)abilityData.abOperator);
    }

    internal void AttackPointUp(AbilityData abilityData)
    {
        _player._playerStats.damage += OperatorValue(_player._playerStats.damage, abilityData); //GetBoatData().damage
        _player._playerStats.GetTDD1().damage = _player._playerStats.damage;
    }

    /// <summary>
    /// 랜덤으로 어빌리티 선택해서 UI에 표시
    /// </summary>
    /// <returns></returns>
    public List<AbilityData> RandomAbilityFuntion()
    {
        List<AbilityCategory> temp = new List<AbilityCategory>();
        for (int i = 0; i < (int)AbilityCategory.NONE; i++)
        {
            if (CheckAvailable((AbilityCategory)i))
            {
                temp.Add((AbilityCategory)i);
            }
        }

        while (_abilitySelect.Count < 3)
        {
            int rand = Random.Range(0, temp.Count);
            var data = _dataManager.FindData(DataManager.KEY_ABILITY, (int)(temp[rand] + 1)) as AbilityData;
            _abilitySelect.Add(data);
            temp.RemoveAt(rand);
        }

        _abilityCount++;

        return _abilitySelect;
    }

    /// <summary>
    /// 어빌리티 랜덤하게 하나 반환
    /// </summary>
    /// <returns></returns>
    public AbilityData AddRandomAbility()
    {
        List<AbilityCategory> temp = new List<AbilityCategory>();
        for (int i = 0; i < (int)AbilityCategory.NONE; i++)
        {
            if (CheckAvailable((AbilityCategory)i))
            {
                temp.Add((AbilityCategory)i);
            }
        }
        int rand = Random.Range(0, temp.Count);
        var data = _dataManager.FindData(DataManager.KEY_ABILITY, (int)(temp[rand] + 1)) as AbilityData;
        StageManager stage = StageManager.GetInstance;
        GameObject player = stage.GetPlayers()[0].gameObject;
        player.GetComponent<Player>().AddAbility(data);
        AddCountDictionary((AbilityCategory)(data.nId - 1));
        return data;
    }

    /// <summary>
    /// 어빌리티 선택
    /// </summary>
    /// <param name="number"></param>
    public void AbilityBtn(int number)
    {
        StageManager stage = StageManager.GetInstance;
        GameObject player = stage.GetPlayers()[0].gameObject;
        player.GetComponent<Player>().AddAbility(_dataManager.FindData(DataManager.KEY_ABILITY, _abilitySelect[number].nId) as AbilityData);
        Debug.Log("Add Ability : " + (AbilityCategory)(_abilitySelect[number].nId - 1));
        AddCountDictionary((AbilityCategory)(_abilitySelect[number].nId - 1));
        _abilitySelect.Clear();
    }

    public void TestAbilitySelectAll()
    {
        _abilitySelect = _dataManager.GetList<AbilityData>(DataManager.KEY_ABILITY);
    }
    public void AbilityBtnSelect(int num)
    {
        GetComponent<Player>().AddAbility(_dataManager.FindData(DataManager.KEY_ABILITY, _abilitySelect[num].nId) as AbilityData);
        _abilitySelect.Clear();
    }
    public void AbilityBombardmentSetting(EnemyController controller)
    {
        if (_bombardmentManager != null)
        {
            _bombardmentManager.Target(controller);
        }
    }

    public void BerserkerMode(bool on)
    {
        if (!_berserker) { return; }
        if (_berserkerEffect == null) { _berserkerEffect = SimplePool.Spawn(CommonStaticDatas.RES_EX, "FX_Hit_11", this.transform, Vector3.zero, Quaternion.identity); }
        if (_berserkerEffect != null) { _berserkerEffect.SetActive(on); }
    }

    public bool Berserker { get { return _berserker; } }
    public bool Blooding { get { return _blooding; } }
    public bool Resurrection { get { return _resurrection; } set { _resurrection = value; } }
    public bool GetDotDamage { get { return _dotDamageOn; } }
    public bool GetSlow { get { return _slowOn; } }
    public bool GetInstantDeathOn { get { return _instantDeathOn; } }
    public bool GetStern { get { return _sternOn; } }
    public bool GetShock { get { return _shockOn; } }
    public bool GetKnockBack { get { return _knockBackOn; } }
    public bool GetKnockDown { get { return _knockDownOn; } }
    public ContinuouslyShot GetContinuouslyShot { get { return _continuouslyShot; } set { _continuouslyShot = value; } }
}
