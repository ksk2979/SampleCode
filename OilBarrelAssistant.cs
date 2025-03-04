using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilBarrelAssistant : MonoBehaviour
{
    ContinuousAbility _eContinuous = ContinuousAbility.E_OILBARRELRANGE;
    AbilityData _data;
    float _duration = 0f;
    Transform _trans;
    GameObject _obj;
    GameObject _targetObj;
    Transform _playerTrans;

    SoundManager _soundManager;
    SoundManager GetSound()
    {
        if (_soundManager == null) { _soundManager = SoundManager.GetInstance; }
        return _soundManager;
    }

    public void Init(AbilityData data, GameObject targetObj, Transform playerTrans)
    {
        _data = data;
        if (_obj == null) { _obj = this.gameObject; }
        if (_trans == null) { _trans = this.transform; }
        _targetObj = targetObj;
        _playerTrans = playerTrans;
        StartAbility();
    }
    public void StartAbility()
    {
        _trans.localScale = new Vector3(3f + _data.GetScale, 3f + _data.GetScale, 3f + _data.GetScale);
        _duration = _data.GetLifeTime + 2;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(CommonStaticKey.TAG_ENEMY))
        {
            other.GetComponent<Enemy>().TakeDamage(_data, 0.5f, (int)_eContinuous);
        }
        if (other.CompareTag(CommonStaticKey.TAG_BOX))
        {
            other.GetComponent<BoxScript>().StartBox();
        }
    }

    private void FixedUpdate()
    {
        _duration -= Time.deltaTime;
        if (_duration < 0f)
        {
            _obj.SetActive(false);
            _targetObj.SetActive(false);
        }

        _trans.position = Vector3.MoveTowards(_trans.position, _playerTrans.position, (Time.deltaTime * 1));
    }

    public GameObject GetObj { get { return _obj; } }
    public Transform GetTrans { get { return _trans; } set { _trans = value; } }
}