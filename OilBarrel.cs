using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilBarrel : Ability
{
    AbilityAssistant _assistant = AbilityAssistant.E_OILBARRELATTACK;
    Transform _targetTrans;
    Vector3 _startPos;
    Vector3 _targetPos;
    OilBarrelAssistant _rangeObj;
    [SerializeField] MeshRenderer _mesh;

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
            _rangeObj = GameObject.Instantiate(GameManager.GetInstance._skillAssistantPrefabs[(int)_assistant]).GetComponent<OilBarrelAssistant>();
        }
        _rangeObj.Init(_data, _obj, _playerTrans);
    }

    public void StartAbility(int addY)
    {
        _obj.SetActive(true);
        if (_mesh != null) { _mesh.enabled = true; }
        _trans.position = _playerTrans.position;
        _trans.localScale = new Vector3(_data.GetScale, _data.GetScale, _data.GetScale);
        _speed = _data.GetProjectile;
        _rangeObj.GetObj.SetActive(false);
        _startPos = _trans.position;

        _trans.localRotation = Quaternion.Euler(0f, _targetTrans.localRotation.eulerAngles.y + addY, 0f);
        _trans.Translate(Vector3.forward * (4f + _data.GetScale));
        _targetPos = _trans.position;
        _trans.position = _playerTrans.position;

        StartCoroutine(StartParabola());
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