using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 박스 공격에 대한 것
// 지정된 박스 사이즈에서 데미지 측정만 함

public class BossBoxAttackEffect : BaseAttack
{
    public float _delayTime;
    public float _lastDelayTime;
    float _time = 0f;
    BoxCollider _collider;
    // 이동 못하게 묶기
    float _downTime = 0f;
    bool _downSpeed = false;

    private void OnEnable()
    {
        if (_collider == null) { _collider = this.transform.GetComponent<BoxCollider>(); }
        StartCoroutine(DoHitDamageCo());
    }

    IEnumerator DoHitDamageCo()
    {
        if (_delayTime > 0f) { yield return YieldInstructionCache.WaitForSeconds(_delayTime); }
        _collider.enabled = true;
        while (_time < _lastDelayTime)
        {
            yield return YieldInstructionCache.WaitForFixedUpdate;
            _time += Time.deltaTime;
        }
        _time = 0f;
        _collider.enabled = false;
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
                if (_downSpeed)
                {
                    player.GetController().GetMovement.DontMove(_downTime);
                    _downSpeed = false;
                    _downTime = 0f;
                }
            }
        }
    }


    public float DownTime { get { return _downTime; } set { _downTime = value; } }
    public bool DownSpeed { get { return _downSpeed; } set { _downSpeed = value; } }
}
