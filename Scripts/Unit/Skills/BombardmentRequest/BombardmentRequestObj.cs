using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombardmentRequestObj : MonoBehaviour
{
    GameObject _obj;
    Transform _trans;
    UnitDamageData tdd;
    Vector3 _spawnPos;
    Transform _parentObj;
    //[SerializeField] float _speed = 2f;
    [SerializeField] GameObject _lineEffectObj;

    bool _go = false;

    Transform _targetTrans;

    [SerializeField] BombardmentRequestAttack _attack;

    float _damageTime = 0f;

    public void Init(UnitDamageData data, Transform parnet)
    {
        if (_obj == null) { _obj = this.gameObject; }
        if (_trans == null) { _trans = this.transform; }
        _parentObj = parnet;
        _spawnPos = _trans.localPosition;
        tdd = data;
        _obj.SetActive(false);
        _attack.Init();
        _go = false;
    }

    public void StartAbility(Vector3 pos, Transform target)
    {
        _obj.SetActive(true);
        _trans.SetParent(null);
        _trans.position = pos;
        _go = true;
        //_lineEffectObj.SetActive(true);
        _targetTrans = target;
        //_speed = 5f;
        _damageTime = 0f;
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
        // 현재 위치에서 타겟 위치로 속도만큼 서서히 이동
        Vector3 targetPosition = new Vector3(_targetTrans.position.x, _trans.position.y, _targetTrans.position.z);
        _trans.position = Vector3.MoveTowards(_trans.position, targetPosition, 10f * Time.deltaTime);
        // y값은 내려온다

        _damageTime += Time.deltaTime;
        if (_damageTime > 2.5f)
        {
            DamageSetting();
        }

        //if (_speed < 60f) { _speed += Time.deltaTime * 20f; }
        //_trans.Translate(Vector3.down * _speed * Time.deltaTime);
        //if (_lineEffectObj.activeSelf) { if (_trans.position.y < 30f) { _lineEffectObj.SetActive(false); } }
        //if (_trans != null)
        //{
        //    if (_trans.position.y < 0)
        //    {
        //        DamageSetting();
        //    }
        //}
    }

    

    void DamageSetting()
    {
        var tddTemp = new UnitDamageData
        {
            attacker = tdd.attacker,
            layerMask = tdd.layerMask,
            targetTag = tdd.targetTag,
            damage = StandardFuncUnit.OperatorValue(_targetTrans.GetComponent<EnemyStats>().maxHp, 10f, OperatorCategory.persent),
            _ability = tdd._ability
        };
        _attack.StartAbility(_trans.position, tddTemp);
        EndAbility();
    }
}

/*
GameObject _obj;
    Transform _trans;
    UnitDamageData tdd;
    Vector3 _spawnPos;
    Transform _parentObj;
    [SerializeField] float _speed = 2f;
    [SerializeField] GameObject _lineEffectObj;

    bool _go = false;

    Transform _targetTrans;

    [SerializeField] BombardmentRequestAttack _attack;

    public void Init(UnitDamageData data, Transform parnet)
    {
        if (_obj == null) { _obj = this.gameObject; }
        if (_trans == null) { _trans = this.transform; }
        _parentObj = parnet;
        _spawnPos = _trans.localPosition;
        tdd = data;
        _obj.SetActive(false);
        _attack.Init();
        _go = false;
    }

    // 조준선이 먼저 보이고 2초정도 뒤에 포격이 된다
    public void StartAbility(Vector3 pos, Transform target)
    {
        _obj.SetActive(true);
        _trans.SetParent(null);
        _trans.position = pos;
        _trans.position += new Vector3(pos.x, 60f, pos.z);
        _go = true;
        _lineEffectObj.SetActive(true);
        _targetTrans = target;
        _speed = 5f;
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
        // 현재 위치에서 타겟 위치로 속도만큼 서서히 이동
        Vector3 targetPosition = new Vector3(_targetTrans.position.x, _trans.position.y, _targetTrans.position.z);
        _trans.position = Vector3.MoveTowards(_trans.position, targetPosition, 10f * Time.deltaTime);
        // y값은 내려온다
        
        if (_speed < 60f) { _speed += Time.deltaTime * 20f; }
        _trans.Translate(Vector3.down * _speed * Time.deltaTime);
        if (_lineEffectObj.activeSelf) { if (_trans.position.y < 30f) { _lineEffectObj.SetActive(false); } }
        if (_trans.position.y < 0)
        {
            var tddTemp = new UnitDamageData
            {
                attacker = tdd.attacker,
                layerMask = tdd.layerMask,
                targetTag = tdd.targetTag,
                damage = StandardFuncUnit.OperatorValue(_targetTrans.GetComponent<EnemyStats>().maxHp, 10f, OperatorCategory.persent),
                _ability = tdd._ability
            };
            _attack.StartAbility(_trans.position, tddTemp);
            EndAbility();
        }
    } 

 */
