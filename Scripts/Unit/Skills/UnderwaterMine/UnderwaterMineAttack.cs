using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterMineAttack : MonoBehaviour
{
    UMManager _umManager;
    UnderwaterMines _parentObj;
    GameObject _obj;
    Transform _trans;
    UnitDamageData tdd;
    float _time = 0f;

    public void InitData(UnitDamageData data, UnderwaterMines parent, UMManager umManager)
    {
        if (_obj == null) { _obj = this.gameObject; }
        if (_trans == null) { _trans = this.transform; }
        if (_parentObj == null) { _parentObj = parent; }
        if (_umManager == null) { _umManager = umManager; }
        tdd = data;
    }

    public void StartAbility(Vector3 pos)
    {
        _obj.SetActive(true);
        _trans.position = pos;
        _time = 0f;
    }

    public void EndAbility()
    {
        _obj.SetActive(false);
        _time = 0f;
        _umManager.UnderwaterMineEnqueue(_parentObj);
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
                other.GetComponent<Interactable>().TakeToDamage(tdd);
            }
        }
    }
}
