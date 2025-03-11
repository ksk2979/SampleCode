using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementController
{
    NavMeshAgent _agent;
    Transform _trans;
    StateManager _stateManager;
    MonoBehaviour _monoBehaviour;

    CharacterStats _stats;
    StatusEffectsController _statusEffect; // �����̻�Ŭ������ �����;��ϴ°ɷ� �����ؾ���

    AnimationController _animaController;

    internal float defalutAnimatorSpeed;
    bool _isMove = false; // ���� �����̰� �ִ°�?

    public MovementController(NavMeshAgent nav, Transform trans, StateManager stateManager, MonoBehaviour monoBehaviour, StatusEffectsController status, CharacterStats stats, AnimationController anima)
    {
        _agent = nav;
        _trans = trans;
        _stateManager = stateManager;
        _monoBehaviour = monoBehaviour;
        _statusEffect = status;
        _stats = stats;
        _animaController = anima;
    }

    public void AgentEnabled(bool value) { _agent.enabled = value; }

    #region ���� ȸ���� ���õ� ���� ===============================
    public enum eRotateStates
    {
        Idle = 0,
        Forward,
        Right,
        Left
    }


    private float _rotateAccelateTime = 0;
    private float _rotateAccelateSpeed = 2;

    private float _velocityX = 0; //����;
    public Vector3 _oldFwd = Vector3.zero;

    public eRotateStates _curRotateState = eRotateStates.Idle;

    public void SetRotateState(eRotateStates rotateStates)
    {
        if (_curRotateState != rotateStates)
            _rotateAccelateTime = 0;
        _curRotateState = rotateStates;
    }
    private float agentVelocityFiexd = 0;
    public float _curSpeedRate
    {
        get
        {
            agentVelocityFiexd = Mathf.Lerp(agentVelocityFiexd, magnitude, Time.deltaTime * 10.0f);
            return agentVelocityFiexd / 5f;
        }
    }

    // ���� ���ǵ忡 ���� �ִ�Ű ����
    public float AnimeVelocityYSetting
    {
        get 
        {
            //Debug.Log(string.Format("speed: {0}, inverse: {1}", _agent.speed, Mathf.InverseLerp(_stats.moveSpeed, _stats.traceMoveSpeed, _agent.speed)));
            return Mathf.InverseLerp(_stats.moveSpeed, _stats.traceMoveSpeed, _agent.speed);
        }
    }

    /// <summary>
    /// ��Ʈ�� ���� �¿� ��� �ϋ� ��ü �ִ԰� ����
    /// </summary>
    public float _rotateBoatDir
    {
        get
        {
            if (_curSpeedRate <= 0)
                return 0;

            _rotateAccelateTime += Time.deltaTime * _rotateAccelateSpeed;

            if (_stateManager._currentState == eCharacterStates.Idle)
            {
                SetRotateState(eRotateStates.Idle);
                _velocityX = Mathf.Lerp(_velocityX, 0, _rotateAccelateTime);
                return _velocityX;
            }

            var curFwd = _trans.forward;
            var sinAngle = Vector3.Cross(curFwd, _oldFwd).y;
            _oldFwd = curFwd;
            float turnThresholds = 0.001f;
            var turningright = turnThresholds < sinAngle;
            var turningleft = sinAngle < -turnThresholds;


            if (turningright)
            {
                SetRotateState(eRotateStates.Right);
                _velocityX = Mathf.Lerp(_velocityX, 1.0f, _rotateAccelateTime);
            }
            else if (turningleft)
            {
                SetRotateState(eRotateStates.Left);
                _velocityX = Mathf.Lerp(_velocityX, -1.0f, _rotateAccelateTime);
            }
            else
            {
                SetRotateState(eRotateStates.Forward);
                _velocityX = Mathf.Lerp(_velocityX, 0, _rotateAccelateTime);
            }

            return _velocityX;
        }
    }
    #endregion ==================================

    // �����ϰ� �̵� ���߰� �ҷ���
    public void DontMove(float time)
    {
        AgentEnabled(false);
        _monoBehaviour.StartCoroutine(DelayDontMove(time));
    }
    IEnumerator DelayDontMove(float time)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        AgentEnabled(true);
    }

    public Vector3 DirFromAngle(float angleInDegrees)
    {
        if (_trans == null)
            return Vector3.zero;
        //��Ʈ�� �¿� ȸ���� ����
        angleInDegrees += _trans.eulerAngles.y;
        //��� ���Ͱ� ��ȯ
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    /// <summary>
    ///stop = true go = false 
    /// </summary>
    /// <param name="flag"></param>
    public void AgentStop(bool flag)
    {
        if (_agent.isOnNavMesh)
            _agent.isStopped = flag;
    }
    public void ResetPath()
    {
        _agent.ResetPath();
    }

    public float totalSpeedConditioningRatio
    {
        get
        {
            float slR = 0;
            float siR = 0;
            if (_statusEffect._slowOn) { slR = 30f / 100f; }
            float ratio = Mathf.Clamp(1f + (-slR) + siR, 0f, 99999f);
            return ratio;
        }
    }

    public float totalSpeedAnimConditioningRatio
    {
        get
        {
            float slR = 0;
            float siR = 0;
            if (_statusEffect._slowOn) { slR = 30f / 100f; } // 30�� �ִ���� �ױ⼭ 100�� ������ 0.3�� ���� �ִ����?
            float ratio = Mathf.Clamp(1f + (-slR) + siR, 0f, 99999f);
            return ratio;
        }
    }

    public void SetSpeed()
    {
        float ratio = totalSpeedConditioningRatio;
        if (_stateManager._currentState == eCharacterStates.Move)
            _agent.speed = _stats.moveSpeed * ratio;
        else
            _agent.speed = _stats.traceMoveSpeed * ratio;
        //if (_animaController != null) { _animaController.SpeedSetting(defalutAnimatorSpeed * totalSpeedAnimConditioningRatio); }
    }

    public void ResetSpeed()
    {
        if (_stateManager._currentState == eCharacterStates.Move)
            _agent.speed = _stats.moveSpeed;
        else
            _agent.speed = _stats.traceMoveSpeed;
        if (_animaController != null) { _animaController.SpeedSetting(defalutAnimatorSpeed); }
    }

    public void Pause(bool flag)
    {
        float ratio = 0;
        if (flag == false)
            ratio = totalSpeedConditioningRatio;
        if (_stateManager._currentState == eCharacterStates.Move)
            _agent.speed = _stats.moveSpeed * ratio;
        else
            _agent.speed = _stats.traceMoveSpeed * ratio;
        if (_animaController != null) { _animaController.SpeedSetting(defalutAnimatorSpeed * ratio); }
    }

    public void SetDestination(Vector3 pos)
    {
        if (_agent.isOnNavMesh)
            _agent.SetDestination(pos);
    }

    //public void AgentPathStatus()
    //{
    //    if (_agent.pathStatus == NavMeshPathStatus.PathPartial || _agent.pathStatus == NavMeshPathStatus.PathInvalid)
    //    {
    //        Debug.LogWarning("��ȿ�� ��ΰ� �ƴմϴ�.");
    //        return;
    //    }
    //}

    public void ToggleAgentControl(bool active)
    {
        if (_agent == null) return;
        if (active == _agent.updateRotation) { return; }

        _agent.isStopped = !active;
        _agent.updateRotation = active;
    }

    /// <summary>
    /// true: �浹 ����, false: �� �浹
    /// </summary>
    /// <param name="on"></param>
    public void NavAgentAction(bool on, bool prioritySet = false)
    {
        if (on)
        {
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            if (prioritySet) { _agent.avoidancePriority = 45; }
        }
        else
        {
            // ����� �ɷ��� ��Ʈ���� ���� ��� ���̸� �ȴ�
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            if (prioritySet) { _agent.avoidancePriority = 50; }
        }
    }

    public void AgentWarp(Vector3 pos)
    {
        _agent.Warp(pos);
    }

    public Vector3 velocity { get { return _agent.velocity; } set { _agent.velocity = value; } }
    public float angularSpeed { get { return _agent.angularSpeed; } set { _agent.angularSpeed = value; } }
    public bool updateRotation { get { return _agent.updateRotation; } set { _agent.updateRotation = value; } }
    public ObstacleAvoidanceType obstacleAvoidanceType { get { return _agent.obstacleAvoidanceType; } set { _agent.obstacleAvoidanceType = value; } }
    public float speed { get { return _agent.speed; } set { _agent.speed = value; } }
    public bool isStopped { get { return _agent.isStopped; } set { _agent.isStopped = value; } }
    public int avoidancePriority { get { return _agent.avoidancePriority; } set { _agent.avoidancePriority = value; } }
    public float magnitude => _agent.velocity.magnitude;
    public bool enabled => _agent.enabled;
    public bool IsMove { get { return _isMove; } set { _isMove = value; } }
}