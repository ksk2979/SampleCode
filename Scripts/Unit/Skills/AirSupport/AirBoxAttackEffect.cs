using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBoxAttackEffect : MonoBehaviour
{
    BoxCollider _collider;
    UnitDamageData tdd;

    public void Init(UnitDamageData tdd)
    {
        this.tdd = tdd;
        if (_collider == null) { _collider = this.transform.GetComponent<BoxCollider>(); }
    }

    public void StartBoxHit()
    {
        _collider.enabled = true;
    }
    public void EndBoxHit()
    {
        _collider.enabled = false;
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

    //public float DownTime { get { return _downTime; } set { _downTime = value; } }
    //public bool DownSpeed { get { return _downSpeed; } set { _downSpeed = value; } }
}
