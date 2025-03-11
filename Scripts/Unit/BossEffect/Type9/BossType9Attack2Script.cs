using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 거북이가 위로 쏘아 올린 장판 개수만큼 내려와서 터진다
public class BossType9Attack2Script : BaseAttack
{
    [SerializeField] float _radius = 5f;

    public void SpawnAttack()
    {
        GameObject area = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type9/Boss_Turtle_Attack_02_Area", Vector3.zero, Quaternion.identity);
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type9/Boss_Turtle_Attack_02_A", Vector3.zero, Quaternion.identity);
        obj.transform.position = StandardFuncUnit.GenerateRandomPositionAroundTarget(this.transform, _radius);
        area.transform.position = obj.transform.position;
        obj.transform.Find("Collider").GetComponent<BossNormalAttack>().InitData(stats, tdd);
    }

    public void StartAttack()
    {
        StartCoroutine(IDelayStart());
    }

    IEnumerator IDelayStart()
    {
        int spawn = 3;

        for (int i = 0; i < spawn; ++i)
        {
            SpawnAttack();
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        SimplePool.Despawn(this.gameObject);
    }
}
