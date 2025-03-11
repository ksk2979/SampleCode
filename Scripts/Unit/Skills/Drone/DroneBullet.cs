using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBullet : MonoBehaviour
{
    GameObject _obj;
    Transform _trans;
    UnitDamageData tdd;
    [SerializeField] float _speed = 2f;
    Collider _collider;
    DroneObj _parent;
    //[SerializeField] AirSupportAttack _attack;

    float _lifeTime = 0f;

    public void Init(UnitDamageData data, DroneObj parent)
    {
        if (_obj == null) { _obj = this.gameObject; }
        if (_trans == null) { _trans = this.transform; }
        if (_collider == null) { _collider = this.GetComponent<Collider>(); }
        _parent = parent;
        tdd = data;
        //_attack.Init(tdd);
    }

    public void StartAbility(Vector3 pos, Vector3 targetPos)
    {
        _obj.SetActive(true);
        _trans.position = pos;

        Vector3 direction = targetPos - pos;
        _trans.rotation = Quaternion.LookRotation(direction);
        _lifeTime = 0f;
    }
    public void EndAbility()
    {
        _obj.SetActive(false);
        _parent.EnqueueBullet(this);
    }

    private void FixedUpdate()
    {
        _lifeTime += Time.deltaTime;
        if (_lifeTime > 3f)
        {
            EndAbility();
            return;
        }

        _trans.position += _trans.forward * _speed * Time.deltaTime;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (tdd != null)
        {
            if (other.CompareTag(tdd.targetTag[0]))
            {
                if (other.GetComponent<Interactable>() == null) { other.GetComponent<TentacleScript>().HitDamage(tdd.damage); }
                else { other.GetComponent<Interactable>().TakeToDamage(tdd); }
                EndAbility();
            }
        }
    }
}
