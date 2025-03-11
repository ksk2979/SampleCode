using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimationController
{
    private Animator _animator;
    public AnimEnvetSender _animEnvetSender;
    float _velocityY = 0f;

    public AnimationController(Animator animator)
    {
        _animator = animator;
        if (_animator.GetComponent<AnimEnvetSender>() != null)
            _animEnvetSender = _animator.GetComponent<AnimEnvetSender>();
    }

    public void AnimEnventSender(string eventKey, Action act)
    {
        if (_animEnvetSender != null) { _animEnvetSender.AddEvent(eventKey, act); }
    }
    public void PlayTriggerAnimation(int animation)
    {
        _animator.SetTrigger(animation);
    }
    public void ResetTriggerAnimation(int animation)
    {
        _animator.ResetTrigger(animation);
    }
    public void PlayIntegerAnimation(int animation, int index)
    {
        _animator.SetInteger(animation, index);
    }
    public void PlayerBoolenAnimation(int animation, bool active)
    {
        _animator.SetBool(animation, active);
    }
    public void PlayFloatAnimation(int animation, float value)
    {
        _animator.SetFloat(animation, value);
    }
    // 파라미터가 있는지 체크
    private bool AnimatorHasParameter(Animator animator, int hash)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.nameHash == hash) { return true; }
        }
        return false;
    }
    public void Rebind()
    {
        _animator.Rebind();
    }
    public void SpeedSetting(float speed)
    {
        _animator.speed = speed;
    }

    public void AnimVelocityYUpdate()
    {
        _animator.SetFloat(CommonStaticDatas.ANIMPARAM_VELOCITY_Y, _velocityY);
    }

    public Transform AnimatorTrans => _animator.transform;
    public float Speed => _animator.speed;
    public int ShortNameHash_0 => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
    public float VelocityY { get { return _velocityY; } set { _velocityY = value; } }
}