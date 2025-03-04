﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 화염병 : 투사체속도, 범위 / 화염병 2개 투척 / 화염병 1개 추가\n화염 범위 증가\n피해량 증가 180씩 0 180 / 120씩 30 150 270 / 90씩 0 90 180 270 / 70씩 0 70 140 210 280 / 60씩 30 90 150 210 270 330 / 12개는 30씩 0 30 60 90 120 150 180 210 240 270 300 330
public class Molotov : Ability
{
    AbilityAssistant _assistant = AbilityAssistant.E_MOLOTOVASSISTANT;
    Transform _targetTrans;
    Vector3 _startPos;
    Vector3 _targetPos;
    MolotovAssistant _rangeObj;
    [SerializeField] MeshRenderer _mesh;

    int _levelCount = 0;

    public override void DateSetting(AbilityData data)
    {
        base.DateSetting(data);
        Init();
    }

    void Init()
    {
        if (_trans == null) { _trans = this.transform; }
        if (_obj == null) { _obj = this.gameObject; }
        if (_rangeObj == null)
        {
            _rangeObj =  GameObject.Instantiate(GameManager.GetInstance._skillAssistantPrefabs[(int)_assistant]).GetComponent<MolotovAssistant>();
        }
        _rangeObj.Init(_data, _obj);
    }

    public void StartAbility(int addY)
    {
        _obj.SetActive(true);
        if (_mesh != null) { _mesh.enabled = true; }
        _trans.position = _playerTrans.position;
        _trans.localScale = new Vector3(_data.GetScale, _data.GetScale, _data.GetScale);
        _speed = _data.GetProjectile;
        _rangeObj.GetObj.SetActive(false);
        _startPos = _trans.position + new Vector3(0f, 0.5f, 0f);

        _trans.localRotation = Quaternion.Euler(0f, _targetTrans.localRotation.eulerAngles.y + addY, 0f);
        _trans.Translate(Vector3.forward * (2.5f + _data.GetScale));
        _targetPos = _trans.position;
        _trans.position = _playerTrans.position;

        StartCoroutine(StartParabola());
        GetSound().PlayAudioEffectSound("SFX_Water_Throw");
    }

    public int LevelNormalProjectile()
    {
        if (_data.GetLevel == 1) { _levelCount = 1; }
        else if (_data.GetLevel == 2) { _levelCount = 2; }
        else if (_data.GetLevel == 3) { _levelCount = 3; }
        else if (_data.GetLevel == 4) { _levelCount = 4; }
        else if (_data.GetLevel == 5) { _levelCount = 5; }
        else { _levelCount = 0; }

        return _levelCount;
    }

    IEnumerator StartParabola()
    {
        _rangeObj.StartAbility();
        float dis = Vector3.Distance(_trans.position, _targetPos);
        float time = 0f;

        while (dis > 0.2f)
        {
            time += Time.deltaTime * _speed;
            Vector3 tempPos = StaticScript.Parabola(_startPos, _targetPos, 4, time);
            _trans.position = tempPos;
            dis = Vector3.Distance(_trans.position, _targetPos);

            if (_trans.position.y < 0) { break; }

            yield return YieldInstructionCache.WaitForEndOfFrame;
        }

        _trans.position = new Vector3(_trans.position.x, 0f, _trans.position.z);
        _mesh.enabled = false;
        _rangeObj.GetObj.SetActive(true);
        _rangeObj.GetTrans.position = _trans.position;
    }

    public Transform GetTargetTrans { get { return _targetTrans; } set { _targetTrans = value; } }
}
