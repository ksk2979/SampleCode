using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 화염병의 직접적인 데미지 클래스 : 데미지, 범위, 지속시간
public class MolotovAssistant : MonoBehaviour
{
    ContinuousAbility _eContinuous = ContinuousAbility.E_MOLOTOV;
    AbilityData _data;
    float _duration = 0f;
    Transform _trans;
    GameObject _obj;
    GameObject _targetObj;

    SoundManager _soundManager;
    SoundManager GetSound()
    {
        if (_soundManager == null) { _soundManager = SoundManager.GetInstance; }
        return _soundManager;
    }

    public void Init(AbilityData data, GameObject targetObj)
    {
        _data = data;
        if (_obj == null) { _obj = this.gameObject; }
        if (_trans == null) { _trans = this.transform; }
        _targetObj = targetObj;
        StartAbility();
    }
    public void StartAbility()
    {
        _trans.localScale = new Vector3(2f + _data.GetScale, 2f + _data.GetScale, 2f + _data.GetScale);
        _duration = _data.GetLifeTime;
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
    }

    public GameObject GetObj { get { return _obj; } }
    public Transform GetTrans { get { return _trans; } set { _trans = value; } }
}
