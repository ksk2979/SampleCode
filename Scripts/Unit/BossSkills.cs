using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BossSkills : MonoBehaviour
{
    public BossSkill[] skills;

    void Update()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].Update();
        }
    }

    /// <summary>
    /// 준비된 스킬을 가져오는데 -1이면 준비된 스킬이 없는것
    /// </summary>
    /// <returns></returns>
    public BossSkill GetReadyInnerDistSkill()
    {
        var readySkills =  skills.Where(x => x.IsReady()).ToArray();
        if(readySkills == null || readySkills.Length <= 0)
            return null;
        return readySkills[StandardFuncData.RandomRange(0, readySkills.Length-1)];
    }

    internal BossSkill GetSkill(eCharacterStates state)
    {
        return skills.Where(x => x._state == state).ToArray()[0];
    }
}

[System.Serializable]
public class BossSkill
{
    public float _delayTime = 10;
    public float _distanse = 4;
    public eCharacterStates _state;
    public bool _isReady = false;

    private float _flowTime = 0;

    public void Update()
    {
        if (_delayTime < _flowTime)
            return;
        _flowTime += Time.deltaTime;
        _isReady = _delayTime < _flowTime;
    }

    public bool IsReady()
    {
        return _delayTime < _flowTime;
    }

    public eCharacterStates GetAnimParam()
    {
        return _state;
    }
    public bool IsInDist(float dist)
    {
        return dist < _distanse;
    }

    public void ResetTime()
    {
        _flowTime = 0;
    }
}
