using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Pilot : MonoBehaviour
{
    public enum eState
    {
        Hide, // 기본 숨어있기
        Appear, // 대기일때 회전 가능
        Idle,
        Fire, // 해치 오픈
        Disappear, // 발포
        Die,
        Max
    }

    public NormalDel[] InitDels = new NormalDel[(int)eState.Max];
    public NormalDel[] UpdateDels = new NormalDel[(int)eState.Max];
    public NormalDel[] FinishDels = new NormalDel[(int)eState.Max];

    public eState _curState = eState.Hide;
    public int _curStateToInt { get { return (int)_curState; } }
    private Transform core;
    private PlayerStats playerStats;
    private Transform turretTarget;

    //탱크로 공격을 담당
    //- 적을 겨냥 했을때 발포
    //- 경로에 적이 없을때 다시 적을 찾아 로테이션
    //- 발포 중일때 로테이션못함

    private Animator _anim = null;

    public FirePoint firePoint;
    private AnimEnvetSender envetSender;
    public string fireSound = "DoFire"; //사운드 재생하는건가?

    private readonly string DoFireSTR = "DoFire";

    public void Init(PlayerStats playerStats)
    {
        /*
        this.playerStats = playerStats;
        if(_anim == null)
            _anim = GetComponent<Animator>();
        if (envetSender == null)
            envetSender = GetComponent<AnimEnvetSender>();
        _anim.SetFloat(CommonStaticDatas.ANIMPARAM_FIRERATE, playerStats.pilotfireRate);
        firePoint.InitData(playerStats, playerStats.GetTDD2());
        envetSender.AddEvent(DoFireSTR, DoFire);
        core = transform;
        StateStart();
        SetState(eState.Hide);
        */
    }

    private void DoFire()
    {
#if TEST
        if (playerStats.GetType() == typeof(PlayerStats))
        {
            if (GlobalGameStatus.Instance.testConfig.pilotDoNotAttack == true)
                return;
        }
#endif
        firePoint.DoFire();
    }

    public void AppearAfterIdle()
    {
        if (_curState == eState.Die)
            return;
        SetState(eState.Idle);
    }


    public void SetState(eState statas)
    {
        FinishDels[_curStateToInt]();
        _curState = statas;
        if (InitDels[_curStateToInt] != null)
            InitDels[_curStateToInt]();
    }

    public void StateStart()
    {
        InitDels[(int)eState.Hide] = HideInit;
        InitDels[(int)eState.Idle] = IdelInit;
        InitDels[(int)eState.Appear] = AppearInit;
        InitDels[(int)eState.Fire] = AttackInit;
        InitDels[(int)eState.Disappear] = DisappearInit;
        InitDels[(int)eState.Die] = DieInit;

        UpdateDels[(int)eState.Hide] = HideUpdate;
        UpdateDels[(int)eState.Idle] = IdelUpdate;
        UpdateDels[(int)eState.Appear] = AppearUpdate;
        UpdateDels[(int)eState.Fire] = AttackUpdate;
        UpdateDels[(int)eState.Disappear] = DisappearUpdate;
        UpdateDels[(int)eState.Die] = DieUpdate;

        FinishDels[(int)eState.Hide] = HideFinish;
        FinishDels[(int)eState.Idle] = IdelFinish;
        FinishDels[(int)eState.Appear] = AppearFinish;
        FinishDels[(int)eState.Fire] = AttackFinish;
        FinishDels[(int)eState.Disappear] = DisappearFinish;
        FinishDels[(int)eState.Die] = DieFinish;
    }

    private void HideInit() { }
    private void HideUpdate() { }
    private void HideFinish() { }

    private void AttackInit() { _anim.SetBool(CommonStaticDatas.ANIMPARAM_ATTACK, true); }
    private void AttackUpdate() { /*Attack();*/ }
    private void AttackFinish() { _anim.SetBool(CommonStaticDatas.ANIMPARAM_ATTACK, false); }

    private void IdelInit() { }
    private void IdelUpdate()
    {
        //if (CommonFuncUnit.CheckTraceTarget(core, playerStats.GetTDD1().targetTag, playerStats.pilotShootingRange, //playerStats.GetTDD1().layerMask))
        //    SetState(eState.Fire);
    }
    private void IdelFinish() { }

    private void DisappearInit() { _anim.SetTrigger(CommonStaticDatas.ANIMPARAM_DISAPPEAR); }
    private void DisappearUpdate() { }
    private void DisappearFinish() { }

    public void AppearInit() { _anim.SetTrigger(CommonStaticDatas.ANIMPARAM_APPEAR); }
    public void AppearUpdate() { }
    public void AppearFinish() { }

    public void DieInit()
    {
        _anim.SetTrigger(CommonStaticDatas.ANIMPARAM_DISAPPEAR);
    }
    public void DieUpdate() { }
    public void DieFinish() { }

    // Rotate to face the target
    //protected void Attack()
    //{
    //    Vector3 pos = Vector3.zero;
    //    float rotationBodySpeed = 0;
    //    var target = CommonFuncUnit.CheckTraceTarget(core, playerStats.GetTDD1().targetTag, playerStats.pilotShootingRange, //playerStats.GetTDD1().layerMask, turretTarget);
    //    if (target != null)
    //    {
    //        pos = target.position;
    //        pos.y = core.position.y;
    //        rotationBodySpeed = 30;
    //
    //    }
    //    else
    //    {
    //        SetState(eState.Idle);
    //        rotationBodySpeed = 10;
    //    }
    //    Vector3 direction = (pos - core.position).normalized;
    //    Quaternion lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    //    if (core.rotation == lookRot)
    //        return;
    //    core.rotation = Quaternion.Slerp(core.rotation, lookRot, Time.deltaTime * rotationBodySpeed);
    //}

    internal void CallMyTarget(Transform target)
    {
        turretTarget = target;
    }

    private void Update()
    {
        UpdateDels[_curStateToInt]();
    }

    internal void SetDie()
    {
        SetState(eState.Die);
    }

    internal void AttackSpeedUp()
    {
        //_anim.SetFloat(CommonStaticDatas.ANIMPARAM_FIRERATE, playerStats.pilotfireRate);
    }
}
