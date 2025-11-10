using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class CommonFunc : MonoBehaviour
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
    /// 완전 랜덤한 공격할수 있는 적을 알아 transform을 리턴한다.
    /// </summary>
    public static Transform CheckRandomTraceTarget(Transform trans, string[] tag, float range, int unitLayerMask)
    {
        Transform bestTraget = null;
        var targets = Physics.OverlapSphere(trans.position, range, unitLayerMask).ToList();
        targets.Remove(trans.GetComponent<Collider>());

        int rand = UnityEngine.Random.Range(0, targets.Count);
        if (targets.Count == 0) { return null; }
        bestTraget = targets[rand].transform;

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
    /// 내가 가려는 방향에 적군이 없는지를 확인하여 알려준다
    /// </summary>
    public static bool CheckCanAttackAngle(Transform trans, string[] tag, float range, float angle, int unitLayerMask)
    {
        var targets = Physics.OverlapSphere(trans.position, range, unitLayerMask);

        for (int i = 0; i < targets.Length; i++)
        {
            //if (targets[i].CompareTag(tag))
            if (IsTargetCompareTag(targets[i].gameObject, tag))
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
    /// 내가 가려는 방향에 적군이 없는지를 확인하여 알려준다
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
    static public Vector3 GetDropPoint(Transform trans, int layer)
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

    static public GameObject GetChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }

    static public void FaceRotation(Transform body, Vector3 direction, float trunSpeed)
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        body.rotation = Quaternion.Slerp(body.rotation, lookRotation, Time.deltaTime * trunSpeed);
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
    static public double OperatorValue(double defualtValue, double v, OperatorCategory operatorCategory)
    {
        switch (operatorCategory)
        {
            case OperatorCategory.persent: return defualtValue * (v / 100);
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

    static public int RandomRange(int min, int max)
    {
        if (min == max) return min;
        return UnityEngine.Random.Range(min, max + 1);
    }

    static public GameObject SpawnObj(string path, string name, Transform parent, int objSetMove, bool uiRect = false)
    {
        GameObject obj = SimplePool.Spawn(path, name, parent, Vector3.zero, Quaternion.identity);
        obj.name = name;
        if (uiRect)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.offsetMin = new Vector2(0f, 0f); // left, bottom
            rect.offsetMax = new Vector3(0f, 0f); // right, top
        }

        if (objSetMove == -1)
        {
            ObjHierarchyChange.LastObjMove(obj.transform);
        }
        else
        {
            ObjHierarchyChange.SetObjMove(obj.transform, objSetMove);
        }

        return obj;
    }

    #region UI 오브젝트 풀링
    static Dictionary<string, Queue<RectTransform>> _cachedTransforms = new Dictionary<string, Queue<RectTransform>>();
    static public void BtnEffectCreate(string name, Transform targetUIRect, RectTransform topCanvasParent)
    {
        // 캐싱된 RectTransform을 가져오거나, 없으면 새로 생성하여 캐싱
        RectTransform rectTransform = GetOrCreateEffectTransform(name);
        
        if (rectTransform.parent != topCanvasParent)
        {
            rectTransform.SetParent(topCanvasParent);
        }

        // 버튼의 위치를 Canvas 상의 위치로 변환하여 이펙트 위치에 적용
        Vector2 canvasPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            topCanvasParent,
            RectTransformUtility.WorldToScreenPoint(Camera.main, targetUIRect.position),
            Camera.main,
            out canvasPosition))
        {
            rectTransform.anchoredPosition3D = new Vector3(canvasPosition.x, canvasPosition.y, 0);
        }

        rectTransform.localScale = Vector3.one;
        rectTransform.gameObject.SetActive(true);
    }
    static RectTransform GetOrCreateEffectTransform(string name)
    {
        if (!_cachedTransforms.ContainsKey(name))
        {
            _cachedTransforms[name] = new Queue<RectTransform>();
        }

        if (_cachedTransforms[name].Count > 0)
        {
            RectTransform cachedTransform = _cachedTransforms[name].Dequeue();
            if (cachedTransform != null)
            {
                return cachedTransform;
            }
        }

        GameObject obj = ResourceManager.GetInstance.GetObjClipForKey(string.Format("{0}{1}", CommonStaticKey.FILEPATH_UI_EFFECT, name));
        GameObject effectObj = Instantiate(obj);
        effectObj.name = name;
        RectTransform rectTransform = effectObj.transform as RectTransform;
        _cachedTransforms[name].Enqueue(rectTransform);

        return rectTransform;
    }

    // 사용한 오브젝트를 다시 풀로 반환하여 재사용
    static public void ReturnEffectToPool(RectTransform rectTransform, string name)
    {
        rectTransform.gameObject.SetActive(false);
        //rectTransform.SetParent(null);  // 부모에서 분리하여 재사용 준비

        if (_cachedTransforms.ContainsKey(name))
        {
            _cachedTransforms[name].Enqueue(rectTransform);
        }
    }
    #endregion
}
