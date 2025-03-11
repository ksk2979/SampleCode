using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirMachineGunScript : MonoBehaviour
{
    GameObject _obj;
    Transform _trans;
    [SerializeField] float _speed = 1f;
    [SerializeField] AirBoxAttackEffect _boxAttack;

    bool _move = false;
    float _moveTime = 0f;

    public void InitData(UnitDamageData data)
    {
        if (_obj == null) { _obj = this.gameObject; }
        if (_trans == null) { _trans = this.transform; }
        _obj.SetActive(false);
        _boxAttack.Init(data);
    }

    public void StartAbility(Vector3 target)
    {
        _obj.SetActive(true);
        _trans.position = target;
        _trans.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360), 0f));
        Vector3 offset = _trans.forward * 20f;
        _trans.position -= offset;
        _moveTime = 0f;
        _move = true;
        _boxAttack.StartBoxHit();
    }

    private void Update()
    {
        if (!_move) { return; }

        _trans.position += _trans.forward * (Time.deltaTime * _speed);
        _moveTime += Time.deltaTime;
        if (_moveTime > 2f) { EndAbility(); }
    }

    public void EndAbility()
    {
        _move = false;
        _moveTime = 0f;
        _obj.SetActive(false);
        _boxAttack.EndBoxHit();
    }
}
