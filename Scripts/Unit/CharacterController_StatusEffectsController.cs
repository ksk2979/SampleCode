using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectsController
{
    // 스턴 / 슬로우 / 도트뎀 / 도발
    public float _sternTime = 0f;
    public float _slowTime = 0f;
    public float _knockBackTime = 0f;
    public float _shockTime = 0f;
    public float _dotTime = 0f;
    public bool _sternOn = false;
    public bool _knockBackOn = false;
    public bool _shockOn = true;
    public bool _slowOn = false;
    public bool _dotOn = false;
    public float _dotDamage;
    float _dotDamageTime = 0f;
    public bool _provocation = false;
    public float _provocationTime = 0f;
    public float _provocationMaxTime = 0f;
    public bool _physicDamageImmune = false; // 데미지 무효화

    CharacterController _characterController;
    CharacterStats _characterStats;

    GameObject _slowEffect;
    GameObject _dotEffect;

    public StatusEffectsController(CharacterController c, CharacterStats s)
    {
        _characterController = c;
        _characterStats = s;
    }
    public void SlowOn()
    {
        _slowOn = true;
        _slowTime = 2f;
        if (_slowEffect == null) { _slowEffect = SimplePool.Spawn(CommonStaticDatas.RES_EX, "FX_Hit_09", _characterController._trans, Vector3.zero, Quaternion.identity); }
        if (_slowEffect != null) { _slowEffect.SetActive(true); }
    }
    public void SlowReAttack()
    {
        _slowTime = 2f;
    }
    public void KnockBackAttack()
    {
        _knockBackTime = 0.2f;
    }
    public void ShockAttack()
    {
        _shockTime = 2.0f;
    }
    public void SternReAttack()
    {
        _sternTime = 0.2f;
    }
    public void DotOn(float damage)
    {
        _dotOn = true;
        _dotTime = 2f;
        _dotDamage = damage * 0.2f;

        if (_dotEffect == null) { _dotEffect = SimplePool.Spawn(CommonStaticDatas.RES_EX, "FX_Hit_06", _characterController._trans, Vector3.zero, Quaternion.identity); }
        if (_dotEffect != null) { _dotEffect.SetActive(true); }
    }
    public void DotReAttack()
    {
        _dotTime = 2f;
    }
    public void ResetCustomAffect()
    {
        _dotOn = false;
        _dotTime = 2f;
        _dotDamage = 0f;
        _dotDamageTime = 0.3f;
        _sternOn = false;
        _sternTime = 0.2f;
        _slowOn = false;
        _slowTime = 2f;
        _provocation = false;
        _physicDamageImmune = false;
        if (_slowEffect != null) { _slowEffect.SetActive(false); }
        if (_dotEffect != null) { _dotEffect.SetActive(false); }
    }
    public IEnumerator IPhysiceDamageImmune(CharacterController c, float duration)
    {
        Transform physiceDamageImunne = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Reflection").transform;
        physiceDamageImunne.SetParent(c._trans);
        physiceDamageImunne.localPosition = Vector3.up;
        _physicDamageImmune = true;

        yield return YieldInstructionCache.WaitForSeconds(duration);

        _physicDamageImmune = false;
        physiceDamageImunne.SetParent(null);
        SimplePool.Despawn(physiceDamageImunne.gameObject);
    }

    public void CustomAffectUpdate()
    {
        // 지속딜을 맞는가?
        if (_dotOn)
        {
            _dotTime -= Time.deltaTime;
            _dotDamageTime -= Time.deltaTime;
            if (_dotDamageTime < 0f)
            {
                _dotDamageTime = 0.3f;
                if (!_characterStats.TakeDamage(_dotDamage))
                {
                    _characterController.DoDie();
                    return;
                }
            }
            if (_dotTime < 0f) { _dotOn = false; if (_dotEffect != null) { _dotEffect.SetActive(false); } }
        }
        if (_slowOn)
        {
            _slowTime -= Time.deltaTime;
            if (_slowTime < 0f) { _slowOn = false; if (_slowEffect != null) { _slowEffect.SetActive(false); } }
        }
        // 도발을 맞았는가?
        if (_provocation)
        {
            _provocationTime += Time.deltaTime;
            if (_provocationTime > _provocationMaxTime)
            {
                _provocation = false;
                _characterController.TargetNotting();
            }
        }
    }

    public bool GetSlowOn { get { return _slowOn; } }
    public bool GetDotOn { get { return _dotOn; } }
}