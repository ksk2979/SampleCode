using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossPushAttack : BaseAttack
{
    public float _delayTime;
    public Vector3 _localStartPos;
    public Vector3 _localEndPos;
    private Collider _collider;
    public NavMeshObstacle _navMeshAgent;

    public float _colliderAnimeTime = 0.1f;
    private void OnEnable()
    {
        this.transform.localPosition = _localStartPos;
        StartCoroutine(DoHitDamageCo());
    }

    IEnumerator DoHitDamageCo()
    {
        yield return YieldInstructionCache.WaitForSeconds(_delayTime);
        if (_collider == null) { _collider = this.transform.GetComponent<Collider>(); }
        _collider.enabled = true;
        if (_navMeshAgent != null) { _navMeshAgent.enabled = true; }

        while(this.transform.localPosition.z <= _localEndPos.z - 2f)
        {
            yield return YieldInstructionCache.WaitForFixedUpdate;
            //DoHitDamage();
            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, _localEndPos, _colliderAnimeTime);
        }
        
        _collider.enabled = false;
        if (_navMeshAgent != null) { _navMeshAgent.enabled = false; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(CommonStaticDatas.TAG_PLAYER))
        {
            Interactable player = other.GetComponent<Interactable>();
            if (player.GetDelayHit) { return; }

            if (player.GetController() != null)
            {
                player.GetController().KnockBackActionStart();
                player.GetDelayHit = true;
                player.DelayCheckTime(2f);
                player.TakeToDamage(tdd);
            }
        }
    }
    public void DoHitDamage()
    {
        //var targets = Physics.OverlapSphere(transform.position, _collider.radius, tdd.layerMask); // 포지션, 거리, 레이어마스크
        //for (int i = 0; i < targets.Length; i++)
        //{
        //    //Debug.Log("적을 인식했다");
        //    Interactable player = targets[i].GetComponent<Interactable>();
        //    if (player.GetDelayHit) { continue; }
        //    
        //    if (StandardFuncUnit.IsTargetCompareTag(targets[i].gameObject, tdd.targetTag))
        //    {
        //        if (player.GetController() != null)
        //        {
        //            player.GetController().KnockBackActionStart();
        //            player.GetDelayHit = true;
        //            player.DelayCheckTime(2f);
        //            player.TakeToDamage(tdd);
        //        }
        //        //Debug.Log(player.name + "맞았다");
        //    }
        //}
    }
}