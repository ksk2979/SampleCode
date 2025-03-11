using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Linq;

public delegate void NormalDel();

public class CharacterController : AbstractStateHandler
{
    public override void OnStart() { }
    internal virtual void Aggro(Transform trans) { }

    public eCharacterStates _curState => _stateManager._currentState;
    public int _curStateToInt { get { return (int)_curState; } }
    [HideInInspector] public bool _main = false;

    public Vector3 _randomPos
    {
        get
        {
            //var pos = Vector3.zero;
            //NavMeshHit hit;
            //do
            //{
            //    Vector3 randomPoint = _trans.position + UnityEngine.Random.insideUnitSphere * 10;
            //    randomPoint.y = 0.15f;
            //
            //    if (NavMesh.SamplePosition(randomPoint, out hit, 10, NavMesh.AllAreas))
            //    {
            //        pos = hit.position;
            //    }
            //} while (pos == Vector3.zero);
            //return pos;

            Vector3 pos = Vector3.zero;
            NavMeshHit hit;
            int maxAttempts = 30;
            int attempts = 0;

            while (pos == Vector3.zero && attempts < maxAttempts)
            {
                Vector3 randomPoint = _trans.position + UnityEngine.Random.insideUnitSphere * 10;
                randomPoint.y = 0.15f; // Y 좌표를 일정하게 설정

                if (NavMesh.SamplePosition(randomPoint, out hit, 10, NavMesh.AllAreas))
                {
                    pos = hit.position;
                }
                attempts++;
            }
            return pos;
        }
    }


    public Vector3 RandomPos(float range)
    {
        var pos = Vector3.zero;
        NavMeshHit hit;
        do
        {
            Vector3 randomPoint = _trans.position + UnityEngine.Random.insideUnitSphere * range;
            randomPoint.y = 0.15f;

            if (NavMesh.SamplePosition(randomPoint, out hit, 10, NavMesh.AllAreas))
            {
                pos = hit.position;
            }
        } while (pos == Vector3.zero);
        return pos;
    }

    protected void StateStart()
    {
        _stateManager.RegisterStateInit(eCharacterStates.Spawn, SpawnInit, SpawnUpdate, SpawnFinish);
        _stateManager.RegisterStateInit(eCharacterStates.Idle, IdleInit, IdleUpdate, IdleFinish);
        _stateManager.RegisterStateInit(eCharacterStates.Move, MoveInit, MoveUpdate, MoveFinish);
        _stateManager.RegisterStateInit(eCharacterStates.Attack, AttackInit, AttackUpdate, AttackFinish);
        _stateManager.RegisterStateInit(eCharacterStates.FindTarget, FindTargetInit, FindTargetUpdate, FindTargetFinish);
        _stateManager.RegisterStateInit(eCharacterStates.Trace, TraceInit, TraceUpdate, TraceFinish);
        _stateManager.RegisterStateInit(eCharacterStates.Die, DieInit, DieUpdate, DieFinish);
        _stateManager.RegisterStateInit(eCharacterStates.KnockBack, KnockBackInit, KnockBackUpdate, KnockBackFinish);
        _stateManager.RegisterStateInit(eCharacterStates.KnockDown, KnockDownInit, KnockDownUpdate, KnockDownFinish);
        _stateManager.RegisterStateInit(eCharacterStates.Stern, SternInit, SternUpdate, SternFinish);
        _stateManager.RegisterStateInit(eCharacterStates.Shock, ShockInit, ShockUpdate, ShockFinish);
        _stateManager.RegisterStateInit(eCharacterStates.DashJump, DashJumpInit, DashJumpUpdate, DashJumpFinish);
        _stateManager.RegisterStateInit(eCharacterStates.ShortIdle, ShortIdleInit, ShortIdleUpdate, ShortIdleFinish);
        _stateManager.RegisterStateInit(eCharacterStates.Hide, HideInit, HideUpdate, HideFinish);
        _stateManager.RegisterStateInit(eCharacterStates.Attack01, Attack01Init, Attack01Update, Attack01Finish);
        _stateManager.RegisterStateInit(eCharacterStates.Attack02, Attack02Init, Attack02Update, Attack02Finish);
        _stateManager.RegisterStateInit(eCharacterStates.Summon, SummonInit, SummonUpdate, SummonFinish);
        _stateManager.RegisterStateInit(eCharacterStates.Drop, DropInit, DropUpdate, DropFinish);
    }

    #region 컴포넌트 변수들 ==============================================
    //[HideInInspector]
    //public NavMeshAgent _agent;
    protected MovementController _movement;
    public Animator _animator;
    protected AnimationController _animaController;
    protected Collider _collider;
    [HideInInspector]
    public Transform _trans;
    [HideInInspector]
    public CharacterStats _characterStats;
    StateManager _stateManager;
    protected StatusEffectsController _statusEffectsController;
    [SerializeField] bool _nowStateDebugCheck = false;

    private bool _oneInit = false;
    protected bool _oneOnStartInit = false; // 한번만 들어가면 되는데 소환될때마다 초기화 되는게 있어서
    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        if (_oneInit) { return; }
        _oneInit = true;
        _stateManager = new StateManager(this);
        _trans = transform;
        _characterStats = GetComponent<CharacterStats>();
        _collider = GetComponent<Collider>();
        if (_animator != null) { _animaController = new AnimationController(_animator); }
        _statusEffectsController = new StatusEffectsController(this, _characterStats);
        _movement = new MovementController(GetComponent<NavMeshAgent>(), _trans, _stateManager, this, _statusEffectsController, _characterStats, _animaController);
    }

    public MovementController GetMovement => _movement;
    public StatusEffectsController GetStatusEffects => _statusEffectsController;
    #endregion =================================================================
    
    [HideInInspector]
    public string[] _targetTag = null;
    [HideInInspector]
    public int _unitLayerMask;

    #region 이동 관련
    public virtual void SetSpeed() { _movement.SetSpeed(); }
    public virtual void ResetSpeed() { _movement.ResetSpeed(); }
    internal void Pause(bool flag) { _movement.Pause(flag); }
    #endregion

    private void FixedUpdate()
    {
        _stateManager.UpdateState();
        _statusEffectsController.CustomAffectUpdate();
        _animaController.AnimVelocityYUpdate();
        if (_nowStateDebugCheck)
        {
            Debug.Log(string.Format("{0} CurrentState: {1}", this.name, _stateManager._currentState));
            _nowStateDebugCheck = false;
        }
    }

    public void SetState(eCharacterStates statas)
    {
        _stateManager.SetStates(statas);
    }

    internal bool IsDie()
    {
        return _curState == eCharacterStates.Die;
    }

    public void SetNoneTargetting()
    {
        _collider.enabled = false;
        _movement.NavAgentAction(false);
    }

    public void SetTargetting()
    {
        _collider.enabled = true;
        _movement.NavAgentAction(true);
    }

    public virtual void SetMoveState() { }

    public virtual void DoDie()
    {
        if (_characterStats._hpBar != null) { _characterStats._hpBar.SetActive(false); }
        ClearFxRes();
        _statusEffectsController.ResetCustomAffect();
    }

    [ContextMenu("Rerevival")]
    public virtual void Revival()
    {
        StartCoroutine(_statusEffectsController.IPhysiceDamageImmune(this, 3f));
        StartCoroutine(INonTargeting(this, 3f));
        _characterStats.Revival();
        if (_characterStats._hpBar != null) { _characterStats._hpBar.SetActive(true); }
        _movement.AgentEnabled(true);
        if (_animaController != null) { _animaController.Rebind(); }
        //if (_animator != null)
        //    _animator.Rebind();
        SetState(eCharacterStates.Idle);
    }
    
    IEnumerator INonTargeting(CharacterController c, float duration)
    {
        c.SetNoneTargetting();
        Transform physiceDamageImunne = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Reflection").transform;
        physiceDamageImunne.SetParent(c._trans);
        physiceDamageImunne.localPosition = Vector3.up;

        yield return YieldInstructionCache.WaitForSeconds(duration);

        c.SetTargetting();
        physiceDamageImunne.SetParent(null);
        SimplePool.Despawn(physiceDamageImunne.gameObject);
    }

    private List<Transform> _listFxRes = new List<Transform>();
    internal void AddFxRes(Transform fxRes)
    {
        _listFxRes.Add(fxRes);
    }

    public void ClearFxRes()
    {
        for (int i = _listFxRes.Count -1; 0 <= i; i--)
        {
            SimplePool.Despawn(_listFxRes[i].gameObject);
        }
        _listFxRes.Clear();
    }

    public virtual void DoHit(UnitDamageData tdd)
    {

    }

    #region 맞았을때 색변경
    //맞았을때 색변경 

    public SkinnedMeshRenderer _meshrender;
    public MeshRenderer _mainMeshrender;
    public MeshRenderer[] _otherMeshrender;
    public Color _hitColor = Color.white;                   //맞앗을때 색
    public Color _baseColor = Color.white;                   //맞앗을때 색
    public float _hitChangeColorDuration = 0.05f;
    internal Func<bool> IsBossOrEliteZombie;
    protected readonly string HIT_COLOR = "_Hit";            // 맞았을떄 색변경 쉐이더 변수
    private readonly string HIT_COLORPOWER = "_ColorPower"; // 맞았을떄 색변경 쉐이더 변수
    private readonly string hit_changeColor_fuc_name = "InitColor";
    private readonly string hit_changeColor_fuc_name_Main = "InitColorMain";

    internal void SetBaseColor(Color color)
    {
        _baseColor = color;
        InitColor();
    }

    public virtual void HitDrama()
    {
        if (_meshrender == null)
            return;
        _meshrender.material.SetColor(HIT_COLOR, _hitColor);
        //_meshrender.material.SetFloat(HIT_COLORPOWER, 5f);
        for (int i = 0; i < _otherMeshrender.Length; i++)
        {
            _otherMeshrender[i].material.SetColor(HIT_COLOR, _hitColor);
            _otherMeshrender[i].material.SetFloat(HIT_COLORPOWER, 5f);
        }

        Invoke(hit_changeColor_fuc_name, _hitChangeColorDuration);
    }

    public void InitColor()
    {
        if (_meshrender == null)
            return;
        _meshrender.material.SetColor(HIT_COLOR, _baseColor);
        //_meshrender.material.SetFloat(HIT_COLORPOWER, 0.5f);
        for (int i = 0; i < _otherMeshrender.Length; i++)
        {
            _otherMeshrender[i].material.SetColor(HIT_COLOR, _baseColor);
            _otherMeshrender[i].material.SetFloat(HIT_COLORPOWER, 0.5f);
        }
    }

    public void HitDramaMain()
    {
        if (_mainMeshrender == null)
            return;
        _mainMeshrender.material.SetColor(HIT_COLOR, _hitColor);
        //_meshrender.material.SetFloat(HIT_COLORPOWER, 5f);
        for (int i = 0; i < _otherMeshrender.Length; i++)
        {
            _otherMeshrender[i].material.SetColor(HIT_COLOR, _hitColor);
        }

        Invoke(hit_changeColor_fuc_name_Main, _hitChangeColorDuration);
    }
    public void InitColorMain()
    {
        if (_mainMeshrender == null)
            return;
        _mainMeshrender.material.SetColor(HIT_COLOR, _baseColor);
        //_meshrender.material.SetFloat(HIT_COLORPOWER, 0.5f);
        for (int i = 0; i < _otherMeshrender.Length; i++)
        {
            _otherMeshrender[i].material.SetColor(HIT_COLOR, _baseColor);
        }
    }
    #endregion

    // 맵 이동할때 잠시 꺼줄 함수
    public void ColliderEnalbed(bool on)
    {
        _movement.AgentEnabled(on);
        _collider.enabled = on;
        if (!on) { SetNoneTargetting(); }
        else { StartCoroutine(DelayAgentEnabled()); }
    }
    IEnumerator DelayAgentEnabled()
    {
        yield return YieldInstructionCache.WaitForSeconds(1f); //new WaitForSeconds(1f);

        if (!IsDie()) { SetTargetting(); }
    }

    // 보스1의 스킬을 썻을때 애니메이션 줄 것
    public void AbsorptionAnime(UnitDamageData tdd)
    {
        SetState(eCharacterStates.Drop);
        StartCoroutine(DelayTriggerStart(tdd));
    }

    IEnumerator DelayTriggerStart(UnitDamageData tdd)
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        _movement.AgentEnabled(false);

        yield return YieldInstructionCache.WaitForSeconds(1f);
        if (_main)
        {
            GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type1/Boss_Whale_FOUNTAION", Vector3.zero, Quaternion.identity);
            obj.transform.position = this.transform.position; // 뻥 터지는 효과는 메인 보트에 나온다
        }

        yield return YieldInstructionCache.WaitForSeconds(1f);
        SetNoneTargetting();
        float rand = UnityEngine.Random.Range(0.2f, 1f);
        yield return YieldInstructionCache.WaitForSeconds(rand);
        this.transform.GetChild(0).gameObject.SetActive(true);
        if (_animaController != null) { _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_DROP); }
        yield return YieldInstructionCache.WaitForSeconds(0.15f);
        // 플레이어는 여기에 데미지를 입어야함
        Player player = this.GetComponent<Player>();
        if (player != null) { player.TakeToDamage(tdd); }
        yield return YieldInstructionCache.WaitForSeconds(0.3f);
        if (IsDie()) { yield break; }
        _movement.AgentEnabled(true);
        SetTargetting();
        SetState(eCharacterStates.Idle);
    }

    // 보스 스킬에 의해 흡수된 오브젝트들 태크 셋팅
    public void TagSetting(string tag)
    {
        _delayTagTime = 0f;
        if (IsDie()) { return; }
        if (!this.gameObject.activeSelf) { return; }
        StartCoroutine(DelayTagSetting(tag));
    }
    float _delayTagTime = 0f;
    IEnumerator DelayTagSetting(string tag)
    {
        if (IsDie()) { yield break; }
        while (_delayTagTime < 5.5f)
        {
            if (IsDie()) { yield break; }
            if (!this.gameObject.activeSelf) { yield break; }
            _delayTagTime += Time.deltaTime;
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }
        _delayTagTime = 0f;
        this.tag = tag;
    }

    public void KnockBackActionStart()
    {
        if (_animaController != null) { _animaController.PlayTriggerAnimation(CommonStaticDatas.ANIMPARAM_KNOCKBACK); }
    }

    public virtual void TargetNotting() { }
    public Collider GetCollider() => _collider;
}