using MyData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uTools;

public class Boomerang : BaseAttack
{
    public enum State
    {
        Init = 0,
        FlyToTarget,
        ReturnMerc
    }
    public State m_state = State.Init;

    private Transform m_myTran;
    public Transform _myTran
    {
        get
        {
            if (m_myTran == null)
                m_myTran = transform;
            return m_myTran;
        }
    }

    private GameObject m_myGo;
    public float _arriveDuration = 1;
    public float _returnSpeed = 20;

    public GameObject _myGo
    {
        get
        {
            if (m_myGo == null)
                m_myGo = gameObject;
            return m_myGo;
        }
    }

    public string _hitEffectStr = string.Empty;
    private void OnTriggerEnter(Collider other)
    {
        if (tdd == null)
            return;

        if (other.CompareTag(CommonStaticDatas.TAG_OBSTACLE) || other.CompareTag(CommonStaticDatas.TAG_WALL))
        {
            if (string.IsNullOrEmpty(_hitEffectStr) == false)
            {
                var obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _hitEffectStr);
                obj.transform.position = other.transform.position;
            }
            if (tp != null)
            {
                Destroy(tp);
                StartCoroutine(ReturnTarget());
            }
        }
        // 데미지 주기
        if (StandardFuncUnit.IsTargetCompareTag(other.gameObject, tdd.targetTag))
        {
            if (string.IsNullOrEmpty(_hitEffectStr) == false)
            {
                var obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _hitEffectStr);
                obj.transform.position = _myTran.position;
            }
            other.GetComponent<Interactable>().TakeToDamage(tdd);
        }
    }
    private TweenPosition tp = null;
    public void InitData(CharacterStats _characterStats, UnitDamageData tdd, Transform target)
    {
        m_state = State.FlyToTarget;
        base.InitData(_characterStats, tdd);
        var fromPos = _myTran.position;
        fromPos.y = 1;
        var targetPos = target.position;
        targetPos.y = 1;
        tp = TweenPosition.Begin(_myGo, fromPos, targetPos, _arriveDuration);
        tp.SetOnFinished(UI.UiTools.ConvertUnityEvent(() =>
        {
            m_state = State.ReturnMerc;
            StartCoroutine(ReturnTarget());
        }));
    }

    private IEnumerator ReturnTarget()
    {
        while(0.55f < Vector3.Distance(tdd.attacker.position , _myTran.position))
        {
            var dir = (tdd.attacker.position - _myTran.position).normalized;
            _myTran.position += dir * _returnSpeed * Time.deltaTime;
            yield return null;
        }
        //tdd.attacker.GetComponent<Merc10Ctrl>().ReturnBoomerang();
        SimplePool.Despawn(_myGo);
    }
}
