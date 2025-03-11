using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ¿ø·¡´Â °íÆøÅºÀÌ ¿´´Âµ¥ ¸¶ÀÎÀ¸·Î ±³Ã¼µÊ
public class ArtilleryObj : MonoBehaviour
{
    GameObject _obj;
    Transform _trans;
    UnitDamageData tdd;
    [SerializeField] ExplosiveShell _attack;
    Vector3 _spawnPos;
    Transform _parentObj;

    [SerializeField] Animator _animator;
    AnimEnvetSender _animEnvetSender;
    bool _on = false;

    float _lifeTime = 0f;

    public void Init(UnitDamageData data, Transform parnet, int number)
    {
        if (_obj == null) { _obj = this.gameObject; }
        if (_trans == null) { _trans = this.transform; }
        _parentObj = parnet;
        _spawnPos = _trans.localPosition;
        tdd = data;
        _attack.Init(tdd, number);
        _obj.SetActive(false);
        if (_animEnvetSender == null && _animator != null)
        { 
            _animEnvetSender = _animator.GetComponent<AnimEnvetSender>();
            _animEnvetSender.AddEvent("OnTrigger", OnTrigger);
        }
    }

    public void OnTrigger()
    {
        _on = true;
    }

    public void StartAbility(Vector3 pos)
    {
        _obj.SetActive(true);
        _trans.SetParent(null);
        _trans.position = pos;
        _lifeTime = 0f;
        //_trans.position += new Vector3(0f, 30f, -20f);
    }
    public void EndAbility()
    {
        _obj.SetActive(false);
        _trans.SetParent(_parentObj);
        _trans.localPosition = _spawnPos;
        _on = false;
    }

    private void FixedUpdate()
    {
        _lifeTime += Time.deltaTime;
        if (_lifeTime > 5f)
        {
            _lifeTime = 0f;
            _attack.StartAbility(_trans.position);
            EndAbility();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_on) { return; }

        if (tdd != null)
        {
            if (other.CompareTag(tdd.targetTag[0]))
            {
                _attack.StartAbility(_trans.position);
                EndAbility();
                //if (other.GetComponent<Interactable>() == null) { other.GetComponent<TentacleScript>().HitDamage(tdd.damage); }
                //else { other.GetComponent<Interactable>().TakeToDamage(tdd); }
            }
        }
    }
}

/*
 °íÆøÅº ÄÚµå
 GameObject _obj;
    Transform _trans;
    UnitDamageData tdd;
    [SerializeField] ExplosiveShell _attack;
    Vector3 _spawnPos;
    Transform _parentObj;
    [SerializeField] float _speed = 2f;

    bool _go = false;

    public void Init(UnitDamageData data, Transform parnet, int number)
    {
        if (_obj == null) { _obj = this.gameObject; }
        if (_trans == null) { _trans = this.transform; }
        _parentObj = parnet;
        _spawnPos = _trans.localPosition;
        tdd = data;
        _attack.Init(tdd, number);
        _obj.SetActive(false);
        _go = false;
    }

    public void StartAbility(Vector3 pos)
    {
        _obj.SetActive(true);
        _trans.SetParent(null);
        _trans.position = pos;
        _trans.position += new Vector3(0f, 30f, -20f);
        _go = true;
    }
    public void EndAbility()
    {
        _obj.SetActive(false);
        _trans.SetParent(_parentObj);
        _trans.localPosition = _spawnPos;
        _go = false;
    }

    private void FixedUpdate()
    {
        if (!_go) { return; }
        _trans.Translate(Vector3.up * _speed * Time.deltaTime);
        if (_trans.position.y < 0)
        {
            _attack.StartAbility(_trans.position);
            EndAbility();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tdd != null)
        {
            if (other.CompareTag(tdd.targetTag[0]))
            {
                if (other.GetComponent<Interactable>() == null) { other.GetComponent<TentacleScript>().HitDamage(tdd.damage); }
                else { other.GetComponent<Interactable>().TakeToDamage(tdd); }
            }
        }
    }
 */