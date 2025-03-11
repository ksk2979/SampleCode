using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterMines : MonoBehaviour
{
    GameObject _obj;
    Transform _trans;
    Collider _collider;
    [SerializeField] UnderwaterMineAttack _attack;
    UnitDamageData tdd;
    float _time = 0f;

    public void InitData(UnitDamageData data, UMManager umManager)
    {
        if (_collider == null) { _collider = this.GetComponent<Collider>(); }
        if (_obj == null) { _obj = this.gameObject; }
        if (_trans == null) { _trans = this.transform; }
        tdd = data;
        _attack.InitData(tdd, this, umManager);
        _attack.transform.SetParent(null);
    }

    public void StartAbility(Transform target)
    {
        _obj.SetActive(true);
        _trans.rotation = target.rotation;
        _trans.position = target.position;
        _trans.Translate(Vector3.back * 2.5f);
        //_trans.position += Vector3. * 2.5f;
        //Vector3.forward * (Time.deltaTime * _speed)
        //_trans.Translate(Vector3.forward * (4f + _data.GetScale));
        _collider.enabled = true;
        _time = 0f;
    }

    public void EndAbility()
    {
        _collider.enabled = false;
        _time = 0f;
        _obj.SetActive(false);
        _attack.StartAbility(_trans.position);
    }

    public void FixedUpdate()
    {
        _time += Time.deltaTime;
        if (_time > 3f)
        {
            EndAbility();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tdd.targetTag[0]))
        {
            EndAbility();
        }
    }
}
