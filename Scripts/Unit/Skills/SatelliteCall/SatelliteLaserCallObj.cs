using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteLaserCallObj : MonoBehaviour
{
    UnitDamageData _tdd;
    string[] _targetTag = null;
    int _unitLayerMask;
    Transform _target;
    Transform _trans;

    SatelliteLaserCallManager _manager;

    float _time = 0f;
    bool _on = false;
    Collider _collider;

    public void Init(UnitDamageData tdd, SatelliteLaserCallManager manager)
    {
        _tdd = tdd;
        _targetTag = _tdd.targetTag;
        _unitLayerMask = _tdd.layerMask;
        _trans = this.transform;
        _manager = manager;
        this.gameObject.SetActive(false);
        _collider = this.GetComponent<Collider>();
    }

    public void StartAbility()
    {
        _time = 3f;
        _collider.enabled = false;
        _on = false;
        this.gameObject.SetActive(true);
        _target = StandardFuncUnit.CheckTraceTarget(_trans, _targetTag, 10000f, _unitLayerMask);
        if (_target != null) { _trans.position = _target.position; }
        StartCoroutine(OnLaser());
    }

    IEnumerator OnLaser()
    {
        yield return YieldInstructionCache.WaitForSeconds(1.7f);
        _collider.enabled = true;
        _on = true;
    }

    private void Update()
    {
        _target = StandardFuncUnit.CheckTraceTarget(_trans, _targetTag, 10000f, _unitLayerMask);

        if (_target != null)
        {
            Vector3 targetPosition = new Vector3(_target.position.x, _trans.position.y, _target.position.z);
            _trans.position = Vector3.MoveTowards(_trans.position, targetPosition, 10f * Time.deltaTime);
        }

        if (!_on) { return; }

        _time -= Time.deltaTime;
        if (_time < 0f)
        {
            _time = 0f;
            EndAbility();
        }
    }

    public void EndAbility()
    {
        _manager.OnReset();
        this.gameObject.SetActive(false);
    }

    public void OnTriggerStay(Collider other)
    {
        if (_tdd != null)
        {
            if (other.CompareTag(_tdd.targetTag[0]))
            {
                if (other.GetComponent<Interactable>() == null) { other.GetComponent<TentacleScript>().HitDamage(_tdd.damage); }
                else
                {
                    Interactable enemy = other.GetComponent<Interactable>();
                    if (enemy.GetDelayHit) { return; }
                    enemy.GetDelayHit = true;
                    enemy.DelayCheckTime(0.2f);
                    enemy.TakeToDamage(_tdd);
                }
            }
        }
    }
}
