using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryObj : MonoBehaviour
{
    [SerializeField] GunFireTop _gunfireTop;
    Transform _trans;

    float _lifeTime = 5f;

    Animator _animator;
    AnimEnvetSender _animEnvetSender;

    bool _attack = false;

    public void Init(PlayerController playerController)
    {
        if (_animEnvetSender == null)
        {
            _animator = _gunfireTop.GetComponent<Animator>();
            _animEnvetSender = _animator.GetComponent<AnimEnvetSender>();
            _animEnvetSender.AddEvent("StartGunFire", StartGunFire);
            _animEnvetSender.AddEvent("EndObj", EndObj);
        }
        _gunfireTop.SetSentry = true;
        _gunfireTop.InitCharacterStates(playerController);
        _gunfireTop.Init();
        _trans = this.transform;
    }

    public void StartGunFire()
    {
        _lifeTime = 5f;
        _gunfireTop.SetSentryAttackOn = true;
        _attack = true;
    }

    public void OnObj(Vector3 pos)
    {
        this.gameObject.SetActive(true);
        _trans.position = new Vector3(pos.x, 0f, pos.z);
    }
    public void EndObj()
    {
        this.gameObject.SetActive(false);
    }
    public void EndAnime()
    {
        _attack = false;
        _gunfireTop.SetSentryAttackOn = false;
        _animator.SetTrigger(CommonStaticDatas.ANIMPARAM_CLOSEHATCH);
    }

    private void Update()
    {
        if (!_attack) { return; }

        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0f) { EndAnime(); }
    }
}
