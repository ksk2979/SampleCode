using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAttackEffect : BaseAttack
{
    public float _delayTime;
    public float _lastDelayTime;
    public float _radiusFirst;
    public float _radiusEnd;
    float _time = 0f;
    SphereCollider _collider;
    public NavMeshAgent _navMeshAgent;
    // 이동 못하게 묶기
    float _downTime = 0f;
    bool _downSpeed = false;
    // 암흑 걸리기
    bool _dark = false;

    private void OnEnable()
    {
        if (_collider == null) { _collider = this.transform.GetComponent<SphereCollider>(); }
        StartCoroutine(DoHitDamageCo());
    }

    IEnumerator DoHitDamageCo()
    {
        yield return YieldInstructionCache.WaitForSeconds(_delayTime);
        _navMeshAgent.enabled = true;
        _collider.enabled = true;
        // 네비게이션 컴포넌트 사용시에 nav와 소환되는 시점에서 포지션이 안맞으면 에러가 터진다 그걸 막기 위해 어느정도 보정하는 작업
        NavMeshHit closesHit;
        if (NavMesh.SamplePosition(this.transform.position, out closesHit, 500f, NavMesh.AllAreas)) { this.transform.position = closesHit.position; }
        else { Debug.LogError("Could not find position on NavMesh!"); }

        while (_time < _lastDelayTime)
        {
            yield return YieldInstructionCache.WaitForFixedUpdate;
            _time += Time.deltaTime;

            if (_navMeshAgent.enabled)
            {
                if (_navMeshAgent.radius <= _radiusEnd) { _navMeshAgent.radius += 0.5f; }
                else { _navMeshAgent.enabled = false; }
            }
            DoHitDamage();
        }

        _navMeshAgent.radius = _radiusFirst;
        _time = 0f;
        _collider.enabled = false;
        _navMeshAgent.enabled = false;
    }

    public void DoHitDamage()
    {
        var targets = Physics.OverlapSphere(transform.position, _collider.radius, tdd.layerMask); // 포지션, 거리, 레이어마스크
        for (int i = 0; i < targets.Length; i++)
        {
            //Debug.Log("적을 인식했다");
            Interactable player = targets[i].GetComponent<Interactable>();
            if (player.GetDelayHit) { continue; }

            if (StandardFuncUnit.IsTargetCompareTag(targets[i].gameObject, tdd.targetTag))
            {
                if (player.GetController() != null)
                {
                    player.GetController().KnockBackActionStart();
                    player.GetDelayHit = true;
                    player.DelayCheckTime(2f);
                    player.TakeToDamage(tdd);
                    if (_downSpeed)
                    {
                        player.GetController().GetMovement.DontMove(_downTime);
                        _downSpeed = false;
                        _downTime = 0f;
                    }
                    if (_dark)
                    {
                        if (!player.GetComponent<PlayerController>().GetSupporter)
                        {
                            if (GameManager.GetInstance.TestEditor) { Debug.Log("어둠"); }
                            else { StageManager.GetInstance.GetInGameUIManager.OnDarkUI(); }
                        }
                    }
                }
                //Debug.Log(player.name + "맞았다");
            }
        }
    }

    public float DownTime { get { return _downTime; } set { _downTime = value; } }
    public bool DownSpeed { get { return _downSpeed; } set { _downSpeed = value; } }
    public bool DarkSet { get { return _dark; } set { _dark = value; } }
}
