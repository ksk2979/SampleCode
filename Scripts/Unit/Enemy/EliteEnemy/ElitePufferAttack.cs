using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElitePufferAttack : BaseAttack
{
    [SerializeField] float _speed = 5f;
    Transform _trans;
    float _attackDelayTime;

    public void OnStart()
    {
        if (_trans == null) { _trans = this.transform; }
        _attackDelayTime = 2f;
    }

    private void FixedUpdate()
    {
        _attackDelayTime -= Time.deltaTime;
        if (_attackDelayTime < 0f)
        {
            _attackDelayTime = 2f;
            Destroy();
        }
        _trans.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    void Destroy()
    {
        SimplePool.Despawn(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tdd != null)
        {
            if (other.CompareTag(tdd.targetTag[0]))
            {
                other.GetComponent<Interactable>().TakeToDamage(tdd);
                Destroy();
            }
        }
    }
}
