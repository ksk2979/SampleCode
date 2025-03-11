using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 보스5에서 콜라이더의 범위가 점점 증가하는 스킬에 사용
public class BossRadiusUpAttack : BaseAttack
{
    public float _delayTime;
    public float _lastDelayTime;
    float _time = 0f;
    SphereCollider _collider;
    // 이동 못하게 묶기
    float _downTime = 0f;
    bool _downSpeed = false;
    // 크기 점점 늘어나게하기위해
    float _myRadius;
    public float _finishRadius;

    private void OnEnable()
    {
        if (_collider == null) 
        { 
            _collider = this.transform.GetComponent<SphereCollider>();
            _myRadius = _collider.radius;
        }
        StartCoroutine(DoHitDamageCo());
    }

    IEnumerator DoHitDamageCo()
    {
        yield return YieldInstructionCache.WaitForSeconds(_delayTime);
        _collider.enabled = true;

        while (_time < _lastDelayTime)
        {
            yield return YieldInstructionCache.WaitForFixedUpdate;
            _time += Time.deltaTime;

            if (_collider.radius < _finishRadius)
            {
                _collider.radius += 0.5f;
            }

            DoHitDamage();
        }

        //_navMeshAgent.radius = _radiusFirst;
        _time = 0f;
        _collider.enabled = false;
        _collider.radius = _myRadius;
        //_navMeshAgent.enabled = false;
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
                player.GetController().KnockBackActionStart();
                player.GetDelayHit = true;
                player.DelayCheckTime(0.5f);
                player.TakeToDamage(tdd);
                if (_downSpeed)
                {
                    player.GetController().GetMovement.DontMove(_downTime);
                }
                //Debug.Log(player.name + "맞았다");
            }
        }
    }

    public float DownTime { get { return _downTime; } set { _downTime = value; } }
    public bool DownSpeed { get { return _downSpeed; } set { _downSpeed = value; } }
}
