using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoMove : MonoBehaviour
{
    Transform _trans;
    UnitDamageData tdd;
    [SerializeField] bool _left = false;
    [SerializeField] float _speed = 3f;
    [SerializeField] float _rotationSpeed = 10f;
    float _y = 0;

    public void InitData(UnitDamageData data)
    {
        tdd = data;
        _trans = this.transform;
    }

    public void StartAbility()
    {
        _y = 0f;
    }

    private void FixedUpdate()
    {
        if (_left) { _trans.Translate(Vector3.back * (Time.deltaTime * _speed)); }
        else { _trans.Translate(Vector3.forward * (Time.deltaTime * _speed)); }
        _y += Time.deltaTime * _rotationSpeed;
        _trans.rotation = Quaternion.Euler(0, _y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tdd != null)
        {
            if (other.CompareTag(tdd.targetTag[0]))
            {
                other.GetComponent<Interactable>().TakeToDamage(tdd);
            }
        }
    }

    public void EndAbility()
    {
        _trans.localPosition = Vector3.zero;
        _trans.localRotation = Quaternion.Euler(Vector3.zero);
        _y = 0;
    }
}
