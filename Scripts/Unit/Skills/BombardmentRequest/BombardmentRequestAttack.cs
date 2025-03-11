using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombardmentRequestAttack : MonoBehaviour
{
    GameObject _obj;
    Transform _trans;
    UnitDamageData tdd;
    float _time = 0f;

    public void Init()
    {
        _obj = this.gameObject;
        _trans = this.transform;
        _trans.SetParent(null);
    }

    public void StartAbility(Vector3 pos, UnitDamageData data)
    {
        _obj.SetActive(true);
        _trans.position = pos;
        tdd = data;
        _time = 0f;
    }
    public void EndAbility()
    {
        _obj.SetActive(false);
        _time = 0f;
    }

    private void FixedUpdate()
    {
        _time += Time.deltaTime;
        if (_time > 0.5f)
        {
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
}
