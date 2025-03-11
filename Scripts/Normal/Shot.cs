using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    protected CharacterStats stats;
    protected UnitDamageData tdd;
    protected Transform _transform;
    protected GameObject _gameObject;

    public string FireEffect = "FX_Muzzle_Flash";
    public string hitEffect = "FX_Normal_Hit";

    protected bool _didExplosion = false;
    protected int _counteractionCount = 0;
    protected bool _didReflection = false;

    // Start is called before the first frame update
    void Awake()
    {
        _transform = transform;
        _gameObject = gameObject;
    }

    internal CharacterStats GetState()
    {
        return stats;
    }

    internal UnitDamageData GetTdd()
    {
        return tdd;
    }

    public virtual void DestroyThis()
    {
        _didExplosion = false; // 폭팔 유무 초기화
        _didReflection = false;
        _counteractionCount = 0;
        SimplePool.Despawn(_gameObject);
    }


    internal virtual void InitData(CharacterStats data,UnitDamageData tdd)
    {
        if (string.IsNullOrEmpty(FireEffect) == false)
            SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, FireEffect, _transform.position, Quaternion.identity);
        stats = data;
        this.tdd = tdd;
    }

    public virtual void CreateHitObject(Vector3 pos)
    {

    }
}
