using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 미사일을 내보내는 스크립트
public class AirSupportObj : MonoBehaviour
{
    [SerializeField] GameObject _meshObj;
    [SerializeField] AirSupportMissile[] _missile;
    GameObject _obj;
    Transform _trans;
    UnitDamageData tdd;
    [SerializeField] float _speed = 0.05f;

    public void InitData(UnitDamageData data)
    {
        if (_obj == null) { _obj = this.gameObject; }
        if (_trans == null) { _trans = this.transform; }
        tdd = data;
        for (int i = 0; i < _missile.Length; ++i)
        {
            _missile[i].Init(tdd, _trans);
        }
        _meshObj.SetActive(false);
    }

    public void StartAbility(Vector3 target)
    {
        _meshObj.SetActive(true); // 비행기 날라가기
        _trans.position = target;
        _trans.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360), 0f));
        Vector3 offset = _trans.forward * 5f;
        _trans.position -= offset;

        StartCoroutine(DelayStartAbility());
    }

    IEnumerator DelayStartAbility()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.1f);

        for (int i = 0; i < _missile.Length; ++i)
        {
            _missile[i].StartAbility();
            yield return YieldInstructionCache.WaitForSeconds(_speed);
        }
    }
}
