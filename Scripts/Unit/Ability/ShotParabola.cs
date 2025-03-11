using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotParabola : BaseAttack
{
    public float _shotMaxHeight = 4;
    public Transform _shoterPos;
    public string prefabs = "FX_Enemy_Projectile_01";
    private Vector3 _targetPos;

    public void SetOnEnableTarget(Vector3 targetPos)
    {
        //Debug.Log("샷 타겟 셋팅");
        _targetPos = targetPos;
        enabled = true;
    }


    private void OnEnable()
    {
        //GetAttackTarget();
        //Debug.Log(this.name + "enable On");
        DoFire(_targetPos);
    }


    public void GetAttackTarget()
    {
        if (stats == null)
            return;
        //trans.forward 
        var trans = GetComponentInParent<EnemyController>()._trans;
        var goShot = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, prefabs, trans.TransformPoint(Vector3.up), Quaternion.identity);
        goShot.transform.forward = tdd.attacker.forward;

        var para = stats.GetPara();
        para._origin = trans.TransformPoint(Vector3.up);
        para._targetPos = _targetPos;
        para._parabolaThrowDuration = Mathf.Clamp(Vector3.Distance(para._origin, para._targetPos), 0.01f, para.attackDist) / 10.0f; // 20 미터를 2초에 날라가게 될때 10의 속도로 포탄이 날라간다
        para._parabolaBoomDist = 1; // 폭팔시 나 잔해물 공격시 범위

        var dist = Vector3.Distance(trans.position, _targetPos); //거리 별로 높이를 쫌올려줘야 하나?
        para._parabolaThrowHeight = _shotMaxHeight * (para._parabolaThrowDuration / 2);

        goShot.GetComponent<ParabolaBoll>().Shot(stats, para,  tdd);

        enabled = false;
    }

    public void DoFire(Vector3 targetPos)
    {
        if (stats == null)
            return;
        //trans.forward 
        var goShot = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, prefabs, _shoterPos.position, Quaternion.identity);
        goShot.transform.forward = tdd.attacker.forward;

        var para = stats.GetPara();
        para._origin = _shoterPos.position;
        para._targetPos = targetPos;
        para._parabolaThrowDuration = Mathf.Clamp(Vector3.Distance(para._origin, para._targetPos), 0.01f, para.attackDist) / 10.0f; // 20 미터를 2초에 날라가게 될때 10의 속도로 포탄이 날라간다
        para._parabolaBoomDist = 1; // 폭팔시 나 잔해물 공격시 범위

        var dist = Vector3.Distance(_shoterPos.position, targetPos); //거리 별로 높이를 쫌올려줘야 하나?
        para._parabolaThrowHeight = _shotMaxHeight * (para._parabolaThrowDuration / 2);

        goShot.GetComponent<ParabolaBoll>().Shot(stats, para, tdd);

        enabled = false;
    }

    public void DoFire(Vector3 oriPos, Vector3 targetPos ,float attackdist)
    {
        if (stats == null)
            return;
        //trans.forward 
        var goShot = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, prefabs, oriPos, Quaternion.identity);
        goShot.transform.position = oriPos;
        goShot.transform.forward = tdd.attacker.forward;

        var para = new ParabolaData();
        para._origin = oriPos;
        para._targetPos = targetPos;
        para.attackDist = attackdist;
        para._parabolaThrowDuration = Mathf.Clamp(Vector3.Distance(oriPos, targetPos), 0.01f, attackdist) / 10.0f; // 20 미터를 2초에 날라가게 될때 10의 속도로 포탄이 날라간다
        para._parabolaBoomDist = 1; // 폭팔시 나 잔해물 공격시 범위

        var dist = Vector3.Distance(oriPos, targetPos); //거리 별로 높이를 쫌올려줘야 하나?
        para._parabolaThrowHeight = _shotMaxHeight * (para._parabolaThrowDuration / 2);

        goShot.GetComponent<ParabolaBoll>().Shot(stats, para, tdd);

        enabled = false;
    }
}

