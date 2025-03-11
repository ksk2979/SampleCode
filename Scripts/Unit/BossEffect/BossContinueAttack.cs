using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossContinueAttack : BaseAttack
{
    public float _hitTime; // 히트되는 시간
    SphereCollider _collider;
    [SerializeField] bool _triggerHit = false;

    private void OnEnable()
    {
        if (_collider == null) { _collider = this.transform.GetComponent<SphereCollider>(); }
        _collider.enabled = true;
    }

    private void OnDisable()
    {
        _collider.enabled = false;
    }

    private void Update()
    {
        if (!_triggerHit)
        {
            DoHitDamage();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (_triggerHit)
        {
            if (other.CompareTag("Player"))
            {
                Interactable player = other.GetComponent<Interactable>();
                if (player.GetDelayHit) { return; }

                if (StandardFuncUnit.IsTargetCompareTag(player.gameObject, tdd.targetTag))
                {
                    player.GetController().KnockBackActionStart();
                    player.GetDelayHit = true;
                    player.DelayCheckTime(_hitTime);
                    player.TakeToDamage(tdd);
                    //Debug.Log(player.name + "맞았다");
                }
            }
        }
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
                player.DelayCheckTime(_hitTime);
                player.TakeToDamage(tdd);
                //Debug.Log(player.name + "맞았다");
            }
        }
    }
}
