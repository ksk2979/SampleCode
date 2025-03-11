using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwirlObj : MonoBehaviour
{
    Transform _trans;
    UnitDamageData tdd;
    SwirlManager _swirlManager;

    // ²ø¾î ´ç±æ Àû
    List<Transform> _enemyTransList;

    float _lifeTime = 5f;

    public void InitData(UnitDamageData data, SwirlManager manager)
    {
        _trans = this.transform;
        tdd = data;
        _swirlManager = manager;
        _enemyTransList = new List<Transform>();
    }
    public void StartAbility(Vector3 pos)
    {
        this.gameObject.SetActive(true);
        _trans.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
        _trans.position = pos;
        _lifeTime = 5f;
    }
    public void EndAbility()
    {
        this.gameObject.SetActive(false);
        _trans.rotation = Quaternion.identity;
        _swirlManager.SwirlObjEnqueue(this);
        _enemyTransList.Clear();
    }

    private void FixedUpdate()
    {
        _lifeTime -= Time.deltaTime;
        if (_lifeTime < 0f) { EndAbility(); return; }
        _trans.Translate(Vector3.forward * Time.deltaTime * 3f);
        for (int i = 0; i < _enemyTransList.Count; ++i)
        {
            _enemyTransList[i].position = Vector3.MoveTowards(_enemyTransList[i].position, _trans.position, 7.0f * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (tdd != null)
        {
            if (other.CompareTag(tdd.targetTag[0]))
            {
                if (other.GetComponent<Enemy>()._unit == UNIT.Boss) return;
                _enemyTransList.Add(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (tdd != null)
        {
            if (other.CompareTag(tdd.targetTag[0]))
            {
                _enemyTransList.Remove(other.transform);
            }
        }
    }
}
