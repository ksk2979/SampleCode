using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Shot
{
    public float explosionTime = 3;
    public float explosionRange = 3;

    private float time = 0;
    private Transform core;
    private readonly string FX_Explosion_01 = "FX_Explosion_01";
    public string sound = "Colleague4Fire";

    private void Start()
    {
        core = transform;
    }
    private void OnEnable()
    {
        time = Time.realtimeSinceStartup;
    }
    private void OnTriggerEnter(Collider other)
    {
        // 데미지 주기
        //if (other.CompareTag(GetTdd().targetTag))
        if(StandardFuncUnit.IsTargetCompareTag(other.gameObject, GetTdd().targetTag))
        {
            Explosion();
           
        }
    }

    

    private void Update()
    {
        if(explosionTime < Time.realtimeSinceStartup - time)
        {
            Explosion();
        }
    }

    private void Explosion()
    {
        var eff = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, FX_Explosion_01, core.position, Quaternion.identity);
        var tdd = stats.GetTDD1();
        tdd.explosionRange = explosionRange;
        //stats.GetTDD().explosionRange = explosionRange;
        eff.GetComponent<Explosion>().InitData(stats, tdd);
        SimplePool.Despawn(gameObject);
        AudioController.Play(sound);
    }
}
