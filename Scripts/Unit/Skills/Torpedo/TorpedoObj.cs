using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoObj : MonoBehaviour
{
    [SerializeField] TorpedoMove[] _torpedo;
    TorpedoManager _torpedoManager;
    float _lifeTime = 3f;

    public void InitData(UnitDamageData data, TorpedoManager manager)
    {
        _torpedoManager = manager;
        for (int i = 0; i < _torpedo.Length; ++i) { _torpedo[i].InitData(data); }
    }

    private void FixedUpdate()
    {
        _lifeTime -= Time.deltaTime;
        if (_lifeTime < 0f)
        {
            EndAbility();
        }
    }

    public void StartAbility(Vector3 pos)
    {
        this.transform.position = pos;
        this.gameObject.SetActive(true);
        _lifeTime = 3;
        for (int i = 0; i < _torpedo.Length; ++i)
        {
            _torpedo[i].StartAbility();
        }
    }

    public void EndAbility()
    {
        this.gameObject.SetActive(false);
        for (int i = 0; i < _torpedo.Length; ++i)
        {
            _torpedo[i].EndAbility();
        }
        _torpedoManager.TorpedoObjEnqueue(this);
    }
}
