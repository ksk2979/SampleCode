using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 위에서 떨어지는 독을 쏟아 내려지가 한다

public class BossType6AttackSkill : BaseAttack
{
    [SerializeField] GameObject _attackPrefabsObj;
    [SerializeField] float _radius = 5f;

    public void SpawnAttack()
    {
        GameObject area = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type6/Boss_Snake_Attack1_Area", Vector3.zero, Quaternion.identity);
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Boss/Type6/Boss_Snake_Attack1_A", Vector3.zero, Quaternion.identity);
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
        int spawn = 30;

        for (int i = 0; i < spawn; ++i)
        {
            SpawnAttack();
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        SimplePool.Despawn(this.gameObject);
    }
}
