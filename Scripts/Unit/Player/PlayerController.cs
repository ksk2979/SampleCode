using MyData;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;

public class PlayerController : CharacterController
{
    protected PlayerController _mainPlayer;
    float _boatDIst = 4.5f;

    public GunFireTop _gunFireTop = null;
    private Transform _dirDummy = null;
    protected PlayerStats _playerStats = null;
    //public Transform _antennaPoint = null;

    private Vector3 _dirPos;
    private Vector3 _dir;
    private float _recordClickTime = 0;
    private float _speedAccelate = 0; //이동할때마다 가속도가 붙는다 이동이 멈추면 리셋 해줘야함

    private readonly string DirDummy = "DirDummy";

    InGameUIManager _inGameUI;
    StageManager _stageM;

    public GameObject _weaponPosPoint; //무기가 부착될 자리
    public DefensePoint _defensePoint; // 방패 부착될 자리

    // 현재 자신이 서포터인가
    bool _supporter;
    bool _isTrance = true;

    [SerializeField] bool _ghostShip = false;

    GameObject _accelEffect;

    public Vector3 _velocity
    {
        get
        {
            if (_speedAccelate < _playerStats.moveSpeed)
                _speedAccelate += Time.deltaTime * _playerStats.speedAccelate;
            if(_inGameUI.IsAccel)
            {
                _speedAccelate = _playerStats.moveSpeed;
            }
            return new Vector3(_inGameUI._floatingJoystick.Direction.x, 0, _inGameUI._floatingJoystick.Direction.y).normalized * (_speedAccelate) * _movement.totalSpeedConditioningRatio;

            //var dir = (new Vector3(_inGameUI._floatingJoystick.Direction.x, 0, _inGameUI._floatingJoystick.Direction.y)).normalized;
            //Vector3 normalizedDir = ((_trans.forward * 2) + dir).normalized;
            //var accelSpeed = GetSpeedAccelate();
            //var roatateAccelSpeed = roateRatio(dir);
            //return (normalizedDir * accelSpeed * roatateAccelSpeed) * totalSpeedConditioningRatio;
        }
    }
    /// <summary>
    /// 선회시 속도를 줄인다.
    /// </summary>
    /// <param name="targetDir"></param>
    /// <returns></returns>
    public float roateRatio(Vector3 targetDir)
    {
        float degree = Mathf.Clamp(MathTools.GetDegree(_trans.forward, targetDir), 10, 70);
        if (float.IsNaN(degree))
            degree = 0;
        var rotateDecreeseAccelateRatio = (180f - degree) / 180f;
        var rotationAccelateSpeedRatio = (1f - rotateDecreeseAccelateRatio) * _playerStats.rotationAccelateSpeedRatio;
        return Mathf.Clamp(rotateDecreeseAccelateRatio + rotationAccelateSpeedRatio, 0.01f, 1.01f);
    }
    /// <summary>
    /// 감가속 
    /// </summary>
    /// <returns></returns>
    protected float GetSpeedAccelate()
    {
        if (_curState == eCharacterStates.Move || _curState == eCharacterStates.Trace) //  || _curState == eCharacterStates.FallowCaptine
        {
            if (_speedAccelate < _playerStats.moveSpeed)
                _speedAccelate += _playerStats.speedAccelate;
        }
        else
        {
            // 제동력 관련
            if (0 < _speedAccelate)
                _speedAccelate -= _playerStats.speedAccelate * 1.5f;
        }
        return Mathf.Clamp(_speedAccelate, 0f, _playerStats.moveSpeed);
    }

    internal void Ready()
    {

    }

    public bool isMove()
    {
        return 0 < _movement._curSpeedRate;
    }

    /// <summary>
    /// 초기화
    /// </summary>
    public override void OnStart()
    {
        if (_animaController != null)
        {
            _movement.defalutAnimatorSpeed = _animaController.Speed;
            _animaController.AnimEnventSender("DieFinishCall", DieFinishCall);
        }

        _gunFireTop.Init();

        if (_dirDummy == null)
        {
            var trans = SimplePool.Spawn(CommonStaticDatas.RES_PREFAB, DirDummy, _trans.position, Quaternion.identity).transform;
            _dirDummy = trans;
        }

        _playerStats = _characterStats as PlayerStats;

        _targetTag = _playerStats.GetTDD1().targetTag;
        _unitLayerMask = _playerStats.GetTDD1().layerMask;

        _collider.enabled = true;
        _movement.AgentEnabled(true);

        SetTargetting();

        _recordClickTime = Time.realtimeSinceStartup;

        StateStart();
        InitColorMain();
        //_otherMeshrender = new MeshRenderer[1];
        //_otherMeshrender[0] = _gunFireTop.transform.Find("Weapon").GetComponent<MeshRenderer>();
        InGameUIManager.GetInstance.CreateHpBarUi(_playerStats);
        if (_supporter) { SetState(eCharacterStates.Trace); }
        else { SetState(eCharacterStates.Idle); }

        if (_inGameUI == null)
        {
            _inGameUI = GameObject.Find("InGameCanvas").GetComponent<InGameUIManager>();
            _movement.speed = _playerStats.moveSpeed;
        }
        if (_stageM == null) { _stageM = GameObject.Find("StageManager").GetComponent<StageManager>(); }
    }

    void Update()
    {
        if (_ghostShip) { return; }
        if (_supporter) { return; }

        if (_curState == eCharacterStates.Die)
            return;
        if (0 < _inGameUI._floatingJoystick.Direction.magnitude && _curState != eCharacterStates.Move)
            SetState(eCharacterStates.Move);
    }

    protected override void IdleInit()
    {
        if (_supporter) { return; }
        base.IdleInit();
        _gunFireTop.SetState(GunFireTop.eGuntopState.Idle);
    }

    protected void SetStopAnim()
    {
        _animaController.PlayFloatAnimation(CommonStaticDatas.ANIMPARAM_VELOCITY_X, _movement._rotateBoatDir);
        _animaController.VelocityY = 0f;
    }

    protected override void IdleUpdate()
    {
        if (!_supporter)
        {
            base.IdleUpdate();
            SetStopAnim();
        }
        else
        {
            _animaController.VelocityY = _movement._curSpeedRate;
            var dist = Vector3.Distance(_mainPlayer._trans.position, _trans.transform.position);

            if (_mainPlayer.isMove() == true && 2f < dist)
                SetState(eCharacterStates.Trace);
        }
    }

    public void SetPosition(Vector3 pos)
    {
        _movement.SetDestination(pos);
    }
    public void CheckPosition()
    {
        _movement.AgentStop(false);
        _movement.ResetPath();
    }

    protected override void IdleFinish()
    {
        if (_supporter) { return; }
        base.IdleFinish();
    }

    protected override void MoveInit()
    {
        if (_supporter) { return; }
        base.MoveInit();
        _animaController.VelocityY = 0f;
        _movement._oldFwd = _trans.forward;
    }
    
    protected override void MoveUpdate()
    {
        if (_supporter) { return; }
        base.MoveUpdate();

        _animaController.PlayFloatAnimation(CommonStaticDatas.ANIMPARAM_VELOCITY_X, _movement._rotateBoatDir);
        if(_inGameUI.IsAccel)
        {
            _movement.velocity = _velocity * 5.0f;
            if (_accelEffect == null)
            {
                _accelEffect = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "FX_Accelerate", Vector3.zero, Quaternion.identity);
                _accelEffect.transform.SetParent(_trans);
                _accelEffect.transform.localPosition = new Vector3(0f, 0.83f, 1.93f);
                _accelEffect.transform.localRotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
            }
            if (!_accelEffect.activeSelf) { _accelEffect.SetActive(true); }
        }
        else
        {
            _movement.velocity = _velocity;
        }
        //_speedAccelate _playerStats.moveSpeed _playerStats.speedAccelate _movement.totalSpeedConditioningRatio
        //Debug.Log(string.Format("velocity: {0}, speedAccelate: {1}, moveSpeed: {2}, PlayerSpeedAccelate: {3}, totalSpeed: {4}", _velocity, _speedAccelate, _playerStats.moveSpeed, _playerStats.speedAccelate, _movement.totalSpeedConditioningRatio));
        _dir = new Vector3(_inGameUI._floatingJoystick.Direction.x, 0, _inGameUI._floatingJoystick.Direction.y).normalized;
        _dirPos = _trans.position + _dir;
        _dirDummy.position = _dirPos;

        if (_inGameUI._floatingJoystick.Direction.magnitude <= 0)
            SetState(eCharacterStates.Idle);
    }

    protected override void MoveFinish()
    {
        if (_supporter) { return; }
        base.MoveFinish();
        _speedAccelate = 0;
    }

    protected override void AttackInit()
    {
        base.AttackInit();
        _gunFireTop.OnTarget();
    }

    public override void DoHit(UnitDamageData _data)
    {
        SoundManager.GetInstance.PlayAudioEffectSound(CommonStaticDatas.SOUND_TankDamage01);
    }
    public void DoHit()
    {
        SoundManager.GetInstance.PlayAudioEffectSound(CommonStaticDatas.SOUND_TankDamage01);
    }


    public void HitFinishCall()
    {

    }

    #region Supporting 서포터 무빙
    protected override void TraceInit()
    {
        _movement.AgentStop(false);
        SetSpeed();
    }
    public void SetTraceState(bool state)
    {
        if (_supporter)
        {
            _isTrance = state;
        }
    }

    protected override void TraceUpdate()
    {
        if (!_isTrance) return;

        var pos = TraceTankPos();
        _movement.SetDestination(pos);

        if (_inGameUI.IsAccel)
        {
            _movement.velocity = _velocity * 5.0f;
            if (_accelEffect == null)
            {
                _accelEffect = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "FX_Accelerate", Vector3.zero, Quaternion.identity);
                _accelEffect.transform.SetParent(_trans);
                _accelEffect.transform.localPosition = new Vector3(0f, 0.83f, 1.93f);
                _accelEffect.transform.localRotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
            }
            if (!_accelEffect.activeSelf) { _accelEffect.SetActive(true); }
        }

        _animaController.VelocityY = _movement._curSpeedRate;

        var dist = Vector3.Distance(pos, _trans.transform.position);
        if (_mainPlayer.isMove() == false && dist < 1.3f) { SetState(eCharacterStates.Idle); }

    }

    protected override void TraceFinish()
    {
        _movement.AgentStop(true);
    }

    // 플레이 주변 위치 포착
    public Vector3 TraceTankPos()
    {
        Vector3 tracePos = Vector3.zero;
        var pos = _mainPlayer.transform.TransformPoint(TargetDirPos());
        pos.y = _trans.position.y;
        return pos;
    }
    // 플레이 주변의 용병 위치 확인
    private Vector3 TargetDirPos()
    {
        switch (_playerStats.GetPosNumber)
        {
            case 1: return (Vector3.left + Vector3.forward) * _boatDIst;
            case 2: return (Vector3.right + Vector3.back) * _boatDIst;
            case 3: return (Vector3.forward + Vector3.right) * _boatDIst;
            case 4: return (Vector3.back + Vector3.left) * _boatDIst;
        }
        return Vector3.zero;
    }

    protected Vector3 pos = Vector3.zero;
    #endregion

    public override void DoDie()
    {
        if (_curState == eCharacterStates.Die)
            return;
        base.DoDie();
        SetState(eCharacterStates.Die);
        if(!_supporter)
        {
            _stageM.KillAllBoat();
        }
        var explosion = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "FX_Explosion_03", _trans.position, Quaternion.identity);
        var explosionTdd = new UnitDamageData()
        {
            attacker = transform,
            layerMask = 1 << LayerMask.NameToLayer(CommonStaticDatas.LAYERMASK_UNIT),
            targetTag = new string[] { CommonStaticDatas.TAG_ENEMY, CommonStaticDatas.TAG_TRAP },
            damage = _playerStats.maxHp * 3f,
            _ability = null
        };
        explosion.GetComponent<SphereCollider>().radius = 30f;
        explosion.GetComponent<Explosion>().HitRadius = 30f;
        explosion.GetComponent<Explosion>().InitData(_playerStats, explosionTdd);
    }

    public void DieFinishCall()
    {
        if (_supporter) { return; }
        if (GameManager.GetInstance.TestEditor) { if (!GameManager.GetInstance.Production) { SpawnTestManager.GetInstance.SaveData(true); } }
        else 
        { 
            _stageM.EndStageMaterialShow(true);
        }
    }

    protected override void DieInit()
    {
        base.DieInit();
        _gunFireTop.SetDie();
        _collider.enabled = false;
        _movement.AgentEnabled(false);
        //_animator.SetBool(CommonStaticDatas.ANIMPARAM_IS_DIE, true);
        _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_DIE_1);
    }

    public override void Revival()
    {
        base.Revival();
        this.GetComponent<PlayerAbility>().BerserkerMode(false);
        _gunFireTop.SetState(GunFireTop.eGuntopState.Idle);
    }

    public PlayerController GetMainPlayer { get { return _mainPlayer; } set { _mainPlayer = value; } }
    public bool GetSupporter { get { return _supporter; } set { _supporter = value; } }
    public bool GetGhostShip => _ghostShip;
    public bool GetTrace => _isTrance;
}