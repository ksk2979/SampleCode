using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro.Examples;
using TMPro;

public static class StandardFuncUnit
{
    public static bool IsTargetCompareTag(GameObject ohter, string[] tags)
    {
        for (int i = 0; i < tags.Length; i++)
            if (ohter.CompareTag(tags[i]))
                return true;
        return false;
    }
    /// <summary>
    /// 공격할수 있는 적을 알아 transform을 리턴한다.
    /// </summary>
    public static Transform CheckTraceTarget(Transform trans, string[] tag, float range, int unitLayerMask)
    {
        Transform bestTraget = null;
        float bestDist = 99999;
        var targets = Physics.OverlapSphere(trans.position, range, unitLayerMask).ToList();
        targets.Remove(trans.GetComponent<Collider>());

        for (int i = 0; i < targets.Count; i++)
        {
            if (IsTargetCompareTag(targets[i].gameObject, tag))
            {
                var dist = Vector3.Distance(targets[i].transform.position, trans.position);
                if (dist < bestDist)
                {

                    bestTraget = targets[i].transform;
                    bestDist = dist;
                }
            }
        }
        return bestTraget;
    }

    /// <summary>
    /// 공격할수 있는 적을 알아 transform을 리턴한다.
    /// </summary>
    public static Transform GetCanAttackTarget(Transform trans, string[] tag, float range, float angle, int unitLayerMask)
    {
        Transform bestTraget = null;
        float bestDist = 99999;
        var targets = Physics.OverlapSphere(trans.position, range, unitLayerMask).ToList();
        targets.Remove(trans.GetComponent<Collider>());
        for (int i = 0; i < targets.Count; i++)
        {
            //if (targets[i].CompareTag(tag))
            if (IsTargetCompareTag(targets[i].gameObject, tag))
            {
                var dist = Vector3.Distance(targets[i].transform.position, trans.position);
                if (dist < bestDist)
                {
                    bestTraget = targets[i].transform;
                    bestDist = dist;
                }
            }
        }
        return bestTraget;
    }


    /// <summary>
    /// 주어진 각도와 범위 내에서 적군이 있는지를 확인
    /// </summary>
    public static bool CheckCanAttackAngle(Transform trans, string[] tag, float range, float angle, int unitLayerMask)
    {
        var targets = Physics.OverlapSphere(trans.position, range, unitLayerMask);
         
        for (int i = 0; i < targets.Length; i++)
        {
            //if (targets[i].CompareTag(tag))
            if(IsTargetCompareTag(targets[i].gameObject, tag))
            {
                var dir = targets[i].transform.position - trans.position;
                float direction = Vector3.Dot(dir, trans.forward);
                if (direction > Mathf.Cos((angle / 2) * Mathf.Deg2Rad))
                    return true;
                //Debug.Log(direction > Mathf.Cos((angle / 2) * Mathf.Deg2Rad));
            }
        }
        return false;
    }

    /// <summary>
    /// 원 범위 내와 특정 각도 내에서 적군이 있는지를 확인
    /// </summary>
    public static bool CheckRoundAndAngle(Transform trans, string tag, float roundRnage, float range, float angle, int unitLayerMask)
    {
        var roundTargets = Physics.OverlapSphere(trans.position, roundRnage, unitLayerMask);
        for (int i = 0; i < roundTargets.Length; i++)
            if (roundTargets[i].transform != trans && roundTargets[i].CompareTag(tag))
                return true;

        var targets = Physics.OverlapSphere(trans.position, range, unitLayerMask);
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].transform != trans && targets[i].CompareTag(tag))
            {
                var dir = targets[i].transform.position - trans.position;
                float direction = Vector3.Dot(dir, trans.forward);
                if (direction > Mathf.Cos((angle / 2) * Mathf.Deg2Rad))
                    return true;
                //Debug.Log(direction > Mathf.Cos((angle / 2) * Mathf.Deg2Rad));
            }
        }
        return false;
    }


    /// <summary>
    /// 해당 트랜스의 아래쪽 방향에 위치를받아온다
    /// </summary>
    static public Vector3 GetDropPoint(Transform trans , int layer)
    {
        if (Physics.Raycast(trans.position, -trans.up, out RaycastHit hit, 1000999, layer))
            return hit.point;
        return Vector3.zero;

    }
    /// <summary>
    /// 해당 유닛 아래쪽 방향에 범위의 적들을 가져온다
    /// </summary>
    static public Collider[] GetTargetOverlapSphere(Transform trans, float range, int layer)
    {
        return Physics.OverlapSphere(GetDropPoint(trans, layer), range, layer);
    }

    /// <summary>
    /// true = range보다 거리가 짧아지면 true / false = range보다 거리가 멀어지면 true
    /// </summary>
    /// <param name="thisTrans"></param>
    /// <param name="finelPos"></param>
    /// <param name="range"></param>
    /// <param name="up"></param>
    /// <returns></returns>
    static public bool GetDistance(Vector3 thisTrans, Vector3 finelPos, float range, bool up)
    {
        float dir = Vector3.Distance(thisTrans, finelPos);
        if (up) { if (dir < range) { return true; } }
        else { if (dir > range) { return true; } }

        return false;
    }

    static public GameObject GetChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }

    internal static Transform CheckTraceTarget(Transform core, string[] tag, float findEnemyRange, int lAYERMASK_UNIT, Transform turretTarget)
    {
        var targets = Physics.OverlapSphere(core.position, findEnemyRange, lAYERMASK_UNIT).ToList();
        if (targets != null && turretTarget != null && 1 < targets.Count)
            targets.Remove(turretTarget.GetComponent<Collider>());
        for (int i = 0; i < targets.Count; i++)
        {
            if(StandardFuncUnit.IsTargetCompareTag(targets[i].gameObject, tag))
            {
                return targets[i].transform;
            }
        }
        return null;
    }

    /// <summary>
    /// 타겟 방향으로 즉시 바라보게 한다
    /// </summary>
    static public Quaternion LookRotation(Transform target, Transform trans)
    {
        Vector3 direction = (target.position - trans.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        return lookRotation;
    }
    /// <summary>
    /// 대상으로 서서히 trunSpeed만큼 회전한다
    /// </summary>
    /// <param name="body"></param>
    /// <param name="direction"></param>
    /// <param name="trunSpeed"></param>
    static public void FaceRotation(Transform myTrans, Transform target, float trunSpeed)
    {
        var targetPos = target.position;
        targetPos.y = myTrans.position.y;

        Vector3 direction = (targetPos - myTrans.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        myTrans.rotation = Quaternion.Slerp(myTrans.rotation, lookRotation, Time.deltaTime * trunSpeed);
    }
    static public void WatchTarget(Transform myTrans, Transform target)
    {
        myTrans.forward = (target.position - myTrans.position).normalized;
    }

    static public bool InAttackArea(Transform me, Transform target, float dist, int layerMast)
    {
        var targets = Physics.OverlapSphere(me.position, dist, layerMast);
        bool inTarget = false;
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].transform == target)
            {
                inTarget = true;
                break;
            }
        }
        return inTarget;
    }

    /// <summary>
    /// persent 기준값의 퍼센테이지 만큼값을 리턴
    /// add 기준값에 해당값을 더한값을 리턴
    /// </summary>
    static public float OperatorValue(float defualtValue, float v, OperatorCategory operatorCategory)
    {
        switch (operatorCategory)
        {
            case OperatorCategory.persent: return defualtValue * (v / 100.0f);
            case OperatorCategory.add: return defualtValue + v;
            case OperatorCategory.sub: return defualtValue - v;
            case OperatorCategory.partial: return v / defualtValue * 100; // 전체값에서 일부값의 퍼센트 계산
            case OperatorCategory.PersentAdd: return defualtValue * (1 + v / 100); // 숫자(전체값)를 몇 퍼센트 증가시키기
            case OperatorCategory.PersentSub: return defualtValue * (1 - v / 100); // 숫자(전체값)를 몇 퍼센트 감소시키기
        }
        return 0;
    }

    static public bool IsInDist(Transform target, Transform me, float dist)
    {
        if (target == null)
            return false;
        return Vector3.Distance(target.position, me.position) < dist;
    }

    /// <summary>
    /// 센터를 기준으로 최소~최대 안에 포인트를 잡아주는 함수 (원형)
    /// </summary>
    /// <param name="center"></param>
    /// <param name="minRadius"></param>
    /// <param name="maxRadius"></param>
    /// <returns></returns>
    static public Vector3 GetRandomPointInDonut(Vector3 center, float minRadius, float maxRadius)
    {
        float radius = UnityEngine.Random.Range(minRadius, maxRadius);
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        return new Vector3(center.x + x, center.y, center.z + z);
    }

    /// <summary>
    /// 센터에 반경만큼 랜덤한 포인트 return 
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    static public Vector3 GenerateRandomPositionAroundTarget(Transform center, float radius)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection.y = 0f;

        Vector3 randomPosition = center.position + randomDirection;

        return randomPosition;
    }

    static public Vector3 NavMeshPositionRandom(Transform trans, float raduius)
    {
        Vector3 rand = UnityEngine.Random.insideUnitSphere * raduius;
        rand += trans.position;

        UnityEngine.AI.NavMeshHit hit;
        Vector3 target = Vector3.zero;

        if (UnityEngine.AI.NavMesh.SamplePosition(rand, out hit, raduius, UnityEngine.AI.NavMesh.AllAreas))
        {
            target = hit.position;
        }

        return target;
    }
}
