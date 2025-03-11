using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLongAttack : BaseAttack
{
    public float _delayTime;
    public float _lastDelayTime;
    public Vector3 _targetPos;
    float _time = 0f;
    SphereCollider _collider;
    CapsuleCollider _capsuleCollider;
    Transform _myTransform;
    // 이동 못하게 묶기
    float _downTime = 0f;
    bool _downSpeed = false;
    float _moveSpeed = 0.06f;

    private void OnEnable()
    {
        _myTransform = this.GetComponent<Transform>();
        if (_collider == null) {  _collider = _myTransform.GetComponent<SphereCollider>(); }
        if (_capsuleCollider == null) { _capsuleCollider = _myTransform.GetComponent<CapsuleCollider>(); }
        StartCoroutine(DoHitDamageCo());
    }

    public void SettingMoveSpeed(float speed)
    {
        _moveSpeed = speed;
    }

    IEnumerator DoHitDamageCo()
    {
        yield return YieldInstructionCache.WaitForSeconds(_delayTime);
        //_navMeshAgent.enabled = true;
        if (_collider != null) { _collider.enabled = true; }
        else if (_capsuleCollider != null) { _capsuleCollider.enabled = true; }
        // 네비게이션 컴포넌트 사용시에 nav와 소환되는 시점에서 포지션이 안맞으면 에러가 터진다 그걸 막기 위해 어느정도 보정하는 작업
        //NavMeshHit closesHit;
        //if (NavMesh.SamplePosition(this.transform.position, out closesHit, 500f, NavMesh.AllAreas)) { this.transform.position = closesHit.position; }
        //else { Debug.LogError("Could not find position on NavMesh!"); }

        while (_time < _lastDelayTime)
        {
            yield return YieldInstructionCache.WaitForFixedUpdate;
            _time += Time.deltaTime;

            //if (_navMeshAgent.enabled)
            //{
            //    if (_navMeshAgent.radius <= _radiusEnd) { _navMeshAgent.radius += 0.5f; }
            //    else { _navMeshAgent.enabled = false; }
            //}
            _myTransform.position = Vector3.Lerp(_myTransform.position, _targetPos, _moveSpeed);
            DoHitDamage();
        }

        //_navMeshAgent.radius = _radiusFirst;
        _time = 0f;
        if (_collider != null) { _collider.enabled = false; }
        if (_capsuleCollider != null) { _capsuleCollider.enabled = false; }
        //_navMeshAgent.enabled = false;
    }

    public void DoHitDamage()
    {
        if (_collider != null)
        {
            var targets = Physics.OverlapSphere(transform.position, _collider.radius, tdd.layerMask); // 포지션, 거리, 레이어마스크
            for (int i = 0; i < targets.Length; i++)
            {
                //Debug.Log("적을 인식했다");
                Interactable player = targets[i].GetComponent<Interactable>();
                if (player.GetDelayHit) { continue; }

                if (StandardFuncUnit.IsTargetCompareTag(targets[i].gameObject, tdd.targetTag))
                {
                    player.GetController().KnockBackActionStart();
                    player.GetDelayHit = true;
                    player.DelayCheckTime(0.5f);
                    player.TakeToDamage(tdd);
                    if (_downSpeed)
                    {
                        player.GetController().GetMovement.DontMove(_downTime);
                        _downSpeed = false;
                        _downTime = 0f;
                    }
                    //Debug.Log(player.name + "맞았다");
                }
            }
        }
        else if (_capsuleCollider != null)
        {
            Vector3 pos1 = new Vector3(_myTransform.position.x, _myTransform.position.y - _capsuleCollider.height, _myTransform.position.z); // 캡슐의 맨 아래 위치
            Vector3 pos2 = new Vector3(_myTransform.position.x, _myTransform.position.y + _capsuleCollider.height, _myTransform.position.z); // 캡슐의 맨 위 위치
            var targetsC = Physics.OverlapCapsule(pos1, pos2, _capsuleCollider.radius, tdd.layerMask);
            for (int i = 0; i < targetsC.Length; i++)
            {
                //Debug.Log("적을 인식했다");
                Interactable player = targetsC[i].GetComponent<Interactable>();
                if (player.GetDelayHit) { continue; }

                if (StandardFuncUnit.IsTargetCompareTag(targetsC[i].gameObject, tdd.targetTag))
                {
                    player.GetController().KnockBackActionStart();
                    player.GetDelayHit = true;
                    player.DelayCheckTime(0.5f);
                    player.TakeToDamage(tdd);
                    if (_downSpeed)
                    {
                        player.GetController().GetMovement.DontMove(_downTime);
                        _downSpeed = false;
                        _downTime = 0f;
                    }
                    //Debug.Log(player.name + "맞았다");
                }
            }
        }
    }

    public float DownTime { get { return _downTime; } set { _downTime = value; } }
    public bool DownSpeed { get { return _downSpeed; } set { _downSpeed = value; } }
}
