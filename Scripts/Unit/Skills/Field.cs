using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public Skill skill;
    private CharacterStats stat;
    private UnitDamageData tdd;

    private float _duration;
    private float _interval;

    private float flowTime = 0;

    private IEnumerator fieldCo;

    private void OnDisable()
    {
        StopCoroutine(fieldCo);
    }
    //private WaitForSeconds wfsInteval;
    ///
    public void InitData(CharacterStats s, UnitDamageData t, float duration, float interval)
    {
        skill.InitData(s,t);
        stat = s;
        tdd = t;
        _duration = duration;
        _interval = interval;
        //wfsInteval = new WaitForSeconds(_interval);
        flowTime = 0;

        fieldCo = FieldCo();
        StartCoroutine(fieldCo);
    }

    IEnumerator FieldCo()
    {
        while (flowTime < _duration)
        {
            //yield return wfsInteval;
            yield return YieldInstructionCache.WaitForSeconds(_interval);
            flowTime += _interval;
            skill.DoAffectStayEnemy();
        }
        skill.Destroy();
    }

}
