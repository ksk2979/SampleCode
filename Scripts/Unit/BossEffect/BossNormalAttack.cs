using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모든 콜라이더 수용한 데미지 체크 스크립트

public class BossNormalAttack : BaseAttack
{
    [SerializeField] GameObject _effectObj;
    public float _delayTime;
    public float _lastDelayTime;
    float _time = 0f;
    Collider _collider;
    // 이동 못하게 묶기
    float _downTime = 0f;
    bool _downSpeed = false;

    private void OnEnable()
    {
        if (_collider == null) { _collider = this.transform.GetComponent<Collider>(); }
        if (_effectObj != null) { _effectObj.SetActive(false); }
        StartCoroutine(DoHitDamageCo());
    }

    IEnumerator DoHitDamageCo()
    {
        if (_delayTime > 0f) { yield return YieldInstructionCache.WaitForSeconds(_delayTime); }
        _collider.enabled = true;
        if (_effectObj != null) { _effectObj.SetActive(true); }
        while (_time < _lastDelayTime)
        {
            yield return YieldInstructionCache.WaitForFixedUpdate;
            _time += Time.deltaTime;
        }
        _time = 0f;
        _collider.enabled = false;
        if (_effectObj != null) { _effectObj.SetActive(false); }
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
