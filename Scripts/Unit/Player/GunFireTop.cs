using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;

public class GunFireTop : MonoBehaviour
{
    public enum eGuntopState
    {
        None = 0,
        Idle, // 대기일때 회전 가능
        Fire, // 발포
        Die,
        Max,
    }

    public NormalDel[] InitDels = new NormalDel[(int)eGuntopState.Max];
    public NormalDel[] UpdateDels = new NormalDel[(int)eGuntopState.Max];
    public NormalDel[] FinishDels = new NormalDel[(int)eGuntopState.Max];

    //포신으로 공격을 담당
    //- 적을 겨냥 했을때 발포
    //- 경로에 적이 없을때 다시 적을 찾아 로테이션
    //- 발포 중일때 로테이션못함

    protected Animator _anim = null;
    public CharacterController _characterStates = null;
    public eGuntopState _curTankTopState = eGuntopState.Idle;
    public int _curStateToInt { get { return (int)_curTankTopState; } }
    //public Pilot pilot = null; // 이제 파일럿은 없기 때문에

    [HideInInspector]
    public TargetPointer _targetPoint = null;

    protected PlayerStats _playerStats = null;
    protected PlayerAbility _playerAbility = null;
    protected Transform _tankTopTrans = null;
    [SerializeField] protected WeaponData _weaponData = null;

    public FirePoint firePoint; // 원샷 기본 포
    protected AnimEnvetSender envetSender;

    protected readonly string DoFireSTR = "DoFire";

    [Header("- 어빌리티 스킬 -")]
    public FirePoint[] _twoShot;
    public FirePoint[] _threeShot;
    public FirePoint[] _diagonalShot;
    public FirePoint[] _sideShot;

    // 곡사포인 포격 스타일이면?
    public bool _parabolaStyle = false;
    // 레이저 포격 스타일이면?
    public bool _laserStyle = false;
    // 지뢰 스타일이면?
    public bool _mineStyle = false;
    float _idleMine = 0f;
    // 어뢰 스타일이면?
    public bool _torpedoStyle = false;

    public AudioPlayer _audioPlayer;

    IFireStrategy _strategy;

    // 센트리가 쏘는 건가?
    bool _sentry = false;
    bool _sentryAttackOn = false;
    public bool SetSentry { set { _sentry = value; } }
    public bool SetSentryAttackOn { set { _sentryAttackOn = value; } }
    PlayerAbility.ContinuouslyShot _sentryShot = new PlayerAbility.ContinuouslyShot
    {
        _continuouslyShot = false,
        _continuouslyShotCount = 0,
    };

    void Awake()
    {
        if (_targetPoint == null)
            _targetPoint = GameObject.FindObjectOfType<TargetPointer>();
        _anim = GetComponent<Animator>();
        envetSender = GetComponent<AnimEnvetSender>();
        _tankTopTrans = transform;
    }

    public void InitCharacterStates(PlayerController player)
    {
        if (_characterStates == null) { _characterStates = player; }//GameObject.FindWithTag("Player").GetComponent<CharacterController>() as PlayerController; }
        if (_characterStates._characterStats == null)
        {
            _characterStates.Init();
        }
        _playerStats = _characterStates._characterStats as PlayerStats;
        _playerAbility = player.GetComponent<PlayerAbility>();
    }

    public virtual void Init()
    {
        firePoint.InitData(_playerStats, _playerStats.GetTDD1());
        envetSender.AddEvent(DoFireSTR, DoFire);
        _anim.SetFloat(CommonStaticDatas.ANIMPARAM_FIRERATE, _playerStats.turretfireRate);
        StateStart();
        //pilot.Init(_playerStats);
        SetState(eGuntopState.Idle);

        _strategy = _parabolaStyle ? new ParabolaStrategy() :
                             _laserStyle ? new LaserStrategy() :
                             _mineStyle ? new MineStrategy() :
                             _torpedoStyle ? new TorpedoStrategy() :
                             new DefaultStrategy();
    }
    
    private void Fire(IFireStrategy strategy)
    {
        if (_sentry)
        {
            strategy.Fire(firePoint, _playerStats, _weaponData, _targetWorldPos, _sentryShot);
        }
        else
        {
            strategy.Fire(firePoint, _playerStats, _weaponData, _targetWorldPos, _playerAbility.GetContinuouslyShot);
        }
        
    }

    private void FireMultiple(IFireStrategy strategy, ShotAbility type)
    {
        if (type == ShotAbility.NORMALSHOT_2)
        {
            for (int i = 0; i < _twoShot.Length; ++i) { strategy.Fire(_twoShot[i], _playerStats, _weaponData, _targetWorldPos, _playerAbility.GetContinuouslyShot); }
        }
        else if (type == ShotAbility.NORMALSHOT_3)
        {
            for (int i = 0; i < _threeShot.Length; ++i) { strategy.Fire(_threeShot[i], _playerStats, _weaponData, _targetWorldPos, _playerAbility.GetContinuouslyShot); }
        }
        else if (type == ShotAbility.DIAGONALSHOT_1)
        {
            for (int i = 0; i < 2; ++i) { strategy.Fire(_diagonalShot[i], _playerStats, _weaponData, _targetWorldPos, _playerAbility.GetContinuouslyShot); }
        }
        else if (type == ShotAbility.DIAGONALSHOT_2)
        {
            for (int i = 0; i < _diagonalShot.Length; ++i) { strategy.Fire(_diagonalShot[i], _playerStats, _weaponData, _targetWorldPos, _playerAbility.GetContinuouslyShot); }
        }
        else if (type == ShotAbility.SIDESHOT)
        {
            for (int i = 0; i < _sideShot.Length; ++i) { strategy.Fire(_sideShot[i], _playerStats, _weaponData, _targetWorldPos, _playerAbility.GetContinuouslyShot); }
        }
    }

    public virtual void DoFire()
    {
        if (_characterStates.IsDie()) { return; }

        if (_sentry)
        {
            Fire(_strategy);
        }
        else
        {
            if (_playerAbility._shotAbility[0] == ShotAbility.NONE) { Fire(_strategy); }
            else if (_playerAbility._shotAbility[0] == ShotAbility.NORMALSHOT_2) { FireMultiple(_strategy, ShotAbility.NORMALSHOT_2); }
            else if (_playerAbility._shotAbility[0] == ShotAbility.NORMALSHOT_3) { FireMultiple(_strategy, ShotAbility.NORMALSHOT_3); }

            if (_playerAbility._shotAbility[1] == ShotAbility.DIAGONALSHOT_1) { FireMultiple(_strategy, ShotAbility.DIAGONALSHOT_1); }
            else if (_playerAbility._shotAbility[1] == ShotAbility.DIAGONALSHOT_2) { FireMultiple(_strategy, ShotAbility.DIAGONALSHOT_2); }

            if (_playerAbility._shotAbility[2] == ShotAbility.SIDESHOT) { FireMultiple(_strategy, ShotAbility.SIDESHOT); }
        }
        
        if (_audioPlayer != null) { _audioPlayer.AudioPlayerFuntion(); }
    }

    public void StateStart()
    {
        InitDels[(int)eGuntopState.Idle] = IdelInit;
        InitDels[(int)eGuntopState.Fire] = AttackInit;
        InitDels[(int)eGuntopState.Die] = DieInit;

        UpdateDels[(int)eGuntopState.Idle] = IdelUpdate;
        UpdateDels[(int)eGuntopState.Fire] = AttackUpdate;
        UpdateDels[(int)eGuntopState.Die] = DieUpdate;


        FinishDels[(int)eGuntopState.Idle] = IdelFinish;
        FinishDels[(int)eGuntopState.Fire] = AttackFinish;
        FinishDels[(int)eGuntopState.Die] = DieFinish;
    }

    private void IdelInit() { /*pilot.SetState(Pilot.eState.Idle);*/ _idleMine = 0f; }
    private void IdelUpdate()
    {
        if (_sentry) { if (!_sentryAttackOn) { return; } }

        if (StandardFuncUnit.CheckTraceTarget(_tankTopTrans, _playerStats.GetTDD1().targetTag, _playerStats.turretShootingRange, _playerStats.GetTDD1().layerMask))
        {
            target = StandardFuncUnit.CheckTraceTarget(_tankTopTrans, _playerStats.GetTDD1().targetTag, _playerStats.turretShootingRange, _playerStats.GetTDD1().layerMask);
            SetState(eGuntopState.Fire);
        }
        else
        {
            if (_mineStyle)
            {
                _idleMine += Time.deltaTime;
                if (_idleMine > 3f)
                {
                    _idleMine = 0f;
                    _anim.SetTrigger(CommonStaticDatas.ANIMPARAM_ATTACK01);
                    _targetWorldPos = _playerStats.transform.position + _tankTopTrans.forward * 5f;
                }
            }
        }
    }
    private void IdelFinish() { }

    public void AppearPilot()
    {
        //pilot.SetState(Pilot.eState.Appear);
    }
    public void DisappearPilot()
    {
        //pilot.SetState(Pilot.eState.Disappear);
    }
    protected Transform target;
    protected Vector3 _targetWorldPos;

    private void AttackInit() { _anim.SetBool(CommonStaticDatas.ANIMPARAM_ATTACK, true); }
    //연사 시간 마다 발포애니메이션 실행  발포애니메이션 DoFIre 호출 한다
    public void AttackUpdate()
    {
        if (_sentry) { if (!_sentryAttackOn) { SetState(eGuntopState.Idle); return; } }

        Vector3 pos = Vector3.zero;
        float rotationBodySpeed = 0;
        target = StandardFuncUnit.CheckTraceTarget(_tankTopTrans, _playerStats.GetTDD1().targetTag, _playerStats.turretShootingRange, _playerStats.GetTDD1().layerMask);
        if (target != null)
        {
            pos = target.position;
            _targetWorldPos = target.position;
            pos.y = _tankTopTrans.position.y;
            rotationBodySpeed = 30;
            //_anim.SetBool(CommonStaticDatas.ANIMPARAM_PILOT, true);
        }
        else
        {
            //_anim.SetBool(CommonStaticDatas.ANIMPARAM_PILOT, false);
            SetState(eGuntopState.Idle);
            rotationBodySpeed = 10;
        }
        //pilot.CallMyTarget(target);


        Vector3 direction = (pos - _tankTopTrans.position).normalized;
        var lookVector = new Vector3(direction.x, 0, direction.z);
        if (lookVector == Vector3.zero)
            lookVector = Vector3.forward;
        Quaternion lookRot = Quaternion.LookRotation(lookVector);
        if (_tankTopTrans.rotation == lookRot)
            return;
        _tankTopTrans.rotation = Quaternion.Slerp(_tankTopTrans.rotation, lookRot, Time.deltaTime * rotationBodySpeed);
    }
    private void AttackFinish() { _anim.SetBool(CommonStaticDatas.ANIMPARAM_ATTACK, false); }

    public void DieInit() { }
    public void DieUpdate() { }
    public void DieFinish() { }
    public Vector3 TargetPos()
    {
        if (target != null)
            return target.position;
        else
            return Vector3.zero;
    }

public void SetState(eGuntopState statas)
    {
        if (_curTankTopState != eGuntopState.None)
            FinishDels[_curStateToInt]();
        _curTankTopState = statas;
        InitDels[_curStateToInt]();
    }

    private void LateUpdate()
    {
        UpdateDels[_curStateToInt]();
        var dist = Vector3.Distance(_playerStats.transform.position, _targetWorldPos);
    }

    public void SetDie()
    {
        SetState(eGuntopState.Die);
        //pilot.SetDie();
    }

    internal void OnTarget()
    {
        //_targetPoint.OnTarget(_targetPoint);
    }

    public void AttackSpeedUp()
    {
        _anim.SetFloat(CommonStaticDatas.ANIMPARAM_FIRERATE, _playerStats.turretfireRate);
    }
    public void pilotAttackSpeedUp()
    {
        //pilot.AttackSpeedUp();
    }

    public WeaponData GetWeaponData { get { return _weaponData; } set { _weaponData = value; } }
}

public interface IFireStrategy
{
    void Fire(FirePoint firePoint, PlayerStats stats, WeaponData data, Vector3 target, PlayerAbility.ContinuouslyShot continuouslyShot);
}

public class DefaultStrategy : IFireStrategy
{
    public void Fire(FirePoint firePoint, PlayerStats stats, WeaponData data, Vector3 target, PlayerAbility.ContinuouslyShot continuouslyShot)
    {
        firePoint.DoFire(stats, stats.GetTDD1(), continuouslyShot);
    }
}

public class ParabolaStrategy : IFireStrategy
{
    public void Fire(FirePoint firePoint, PlayerStats stats, WeaponData data, Vector3 target, PlayerAbility.ContinuouslyShot continuouslyShot)
    {
        firePoint.DoFireParabola(stats, data, target, continuouslyShot);
    }
}

public class LaserStrategy : IFireStrategy
{
    // Laser 발사 구현
    public void Fire(FirePoint firePoint, PlayerStats stats, WeaponData data, Vector3 target, PlayerAbility.ContinuouslyShot continuouslyShot)
    {
        firePoint.DoFireLaser(stats, target, continuouslyShot);
    }
}

public class MineStrategy : IFireStrategy
{
    // Mine 발사 구현
    public void Fire(FirePoint firePoint, PlayerStats stats, WeaponData data, Vector3 target, PlayerAbility.ContinuouslyShot continuouslyShot)
    {
        firePoint.DoFireMine(stats, data, target, continuouslyShot);
    }
}

public class TorpedoStrategy : IFireStrategy
{
    // Torpedo 발사 구현
    public void Fire(FirePoint firepoint, PlayerStats stats, WeaponData data, Vector3 target, PlayerAbility.ContinuouslyShot continuouslyShot)
    {
        firepoint.DoFireTorpedo(stats, data, target, continuouslyShot);
    }
}