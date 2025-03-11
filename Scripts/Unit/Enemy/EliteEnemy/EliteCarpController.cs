using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 원거리 잉어느낌
/// </summary>

// 물거품 수뢰(지뢰) 공격을 하면 앞으로 살짝 수뢰를 내보낸다 닿는것이 없으면 일정 시간 이후 터진다
public class EliteCarpController : EnemyController
{
    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            _oneOnStartInit = true;
        }
        SetState(eCharacterStates.Spawn);
    }

    public override void BasicAttack()
    {
        // 물거품을 앞으로 내보내기만 하면 됨 투~ 침뱉듯이
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Enemy/EnemyTorpedoAttack", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        EliteCarpAttack ef = obj.GetComponent<EliteCarpAttack>();
        ef.InitData(_enemyStats, _enemyStats.GetTDD1());
        ef.Init();
    }
}
