using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public enum SkillRangeType
    {
        OneTarget, // 단일 타겟
        RangeTarget // 범위 타겟
    }

    public enum SkillAttackType
    {
        TriggerEnter, // 한번만 데미지를 줌
        TriggerStay // 여러시간에 걸쳐 데미지를 줌
    }

    public enum SkillMoveType
    {
        Fiexd, //움직이지 않음
        Direct, //직선
        Guide, //유도
    }

    public SkillMoveType moveType;
    public SkillAttackType attackType;
    public GameObject root;
    public string _fxHitResName = "FX_Normal_Hit";
    public float speed = 3;
    public bool onEnterDestroy = true;
    public bool ignoreFloorWall = false;


    protected CharacterStats stat;
    protected UnitDamageData tdd;
    protected List<Collider> targets = new List<Collider>();
    protected Transform _trans;


    private void Awake()
    {
        _trans = transform;
    }

    public void InitData(CharacterStats s, UnitDamageData t)
    {
        stat = s;
        tdd = t;
        if (0 < targets.Count)
            targets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (attackType == SkillAttackType.TriggerEnter)
        {
            if (tdd == null)
                return;

            // 데미지 주기
            if (ignoreFloorWall == false &&  (other.CompareTag(CommonStaticDatas.TAG_FLOOR)
                || other.CompareTag(CommonStaticDatas.TAG_WALL)))
            {
                if (onEnterDestroy)
                    Destroy();
            }
            // 데미지 주기
            //if (other.CompareTag(tdd.targetTag))
            if (StandardFuncUnit.IsTargetCompareTag(other.gameObject, tdd.targetTag))
            {
                DoDamage(other.GetComponent<Interactable>());
                if(onEnterDestroy)
                    Destroy();
            }
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (attackType == SkillAttackType.TriggerStay)
        {
            if (tdd == null)
                return;
            if (StandardFuncUnit.IsTargetCompareTag(other.gameObject, tdd.targetTag))
            {
                if (targets.Contains(other) == false)
                {
                    targets.Add(other);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (attackType == SkillAttackType.TriggerStay)
        {
            if (tdd == null)
                return;
            if (StandardFuncUnit.IsTargetCompareTag(other.gameObject, tdd.targetTag))
            {
                if (targets.Contains(other))
                {
                    targets.Remove(other);
                }
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].GetComponent<CharacterController>().IsDie())
            {
                targets.RemoveAt(i);
            }
        }
    }


    public void DoAffectStayEnemy()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            DoDamage(targets[i].GetComponent<Interactable>());
        }
    }

    protected virtual void DoDamage(Interactable interactable)
    {
        if (stat == null || tdd == null)
            return;

        interactable.TakeToDamage(tdd);
    }
    

    private void LateUpdate()
    {
        if(moveType == SkillMoveType.Direct)
        {
            _trans.position += _trans.forward * (Time.deltaTime * speed);
        }
    }
    public virtual void Destroy()
    {
        if (gameObject.activeSelf == false)
            return;
        if (_fxHitResName != string.Empty)
        {
            var pos = _trans.position;
            pos.y = 0;
            SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _fxHitResName, pos, Quaternion.identity);
        }
        SimplePool.Despawn(root);
    }
}
