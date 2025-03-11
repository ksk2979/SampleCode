using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 떨어지는 미사일
public class AirSupportMissile : MonoBehaviour
{
    GameObject _obj;
    Transform _trans;
    UnitDamageData tdd;
    [SerializeField] AirSupportAttack _attack;
    Vector3 _spawnPos;
    Transform _parentObj;
    [SerializeField] float _speed = 2f;

    public void Init(UnitDamageData data, Transform parnet)
    {
        if (_obj == null) { _obj = this.gameObject; }
        if (_trans == null) { _trans = this.transform; }
        _parentObj = parnet;
        _spawnPos = _trans.localPosition;
        tdd = data;
        _attack.Init(tdd);
    }

    public void StartAbility()
    {
        _obj.SetActive(true);
        _trans.SetParent(null);
    }
    public void EndAbility()
    {
        _obj.SetActive(false);
        _trans.SetParent(_parentObj);
        _trans.localPosition = _spawnPos;
    }

    private void FixedUpdate()
    {
        _trans.Translate(Vector3.up * _speed * Time.deltaTime);
        if (_trans.position.y < 0)
        {
            _attack.StartAbility(_trans.position);
            EndAbility();
        }
    }
}
