using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 지속형 어빌리티일 경우 스킬 쿨타임을 사용 하고 있는 와중에는 안되다가 사용이 끝나면 
public class AbilitySphereSkill : MonoBehaviour
{
    public GameObject[] _objList;
    public GameObject[] _particleObjList;
    public SphereCollider[] _sphereColiiderList;
    public AttackSphere[] _attackSphereList;
    public Transform _transform;

    public int _skillCount;
    public bool _changeRotaiton; // 반대 방향으로 돌릴지
    float _rotationY = 0f;

    float _durationTime;
    public float _maxDurationTime = 5f;
    float _coolTime;
    public float _maxCoolTime = 10f;

    public bool _on; // 스킬이 켜져있는가

    public float _rotationSpeed = 1f;

    public Transform _target;
    public UnitDamageData _udd;

    public void Init(PlayerAbility ability, UnitDamageData playerUdd, Transform target)
    {
        _transform = this.transform;
        _coolTime = _maxCoolTime;

        _target = target;
        _transform.position = _target.position;

        var udd = new UnitDamageData()
        {
            attacker = _target,//transform.parent.transform,
            layerMask = 1 << LayerMask.NameToLayer(CommonStaticDatas.LAYERMASK_UNIT),
            targetTag = new string[] { CommonStaticDatas.TAG_ENEMY, CommonStaticDatas.TAG_TRAP },
            damage = playerUdd.damage,
        };
        _udd = udd;
        for (int i = 0; i < _attackSphereList.Length; ++i)
        {
            _attackSphereList[i].InitData(ability, _udd);
        }
    }

    private void Update()
    {
        _coolTime += Time.deltaTime;

        _transform.position = _target.position;
        if (_coolTime > _maxCoolTime)
        {
            if (!_on)
            {
                _on = true;
                _coolTime = 0f;
                for (int i = 0; i < _sphereColiiderList.Length; ++i)
                {
                    _sphereColiiderList[i].enabled = true;
                    _particleObjList[i].SetActive(true);
                }
            }
        }

        if (_on)
        {
            _durationTime += Time.deltaTime;
            if (_durationTime > _maxDurationTime)
            {
                _on = false;
                _durationTime = 0f;
                for (int i = 0; i < _sphereColiiderList.Length; ++i)
                {
                    _sphereColiiderList[i].enabled = false;
                    _particleObjList[i].SetActive(false);
                }
            }
        }
    }

    // 그냥 계속 돈다
    private void FixedUpdate()
    {
        if (!_changeRotaiton)
        {
            _rotationY += Time.timeScale * _rotationSpeed;
            if (_rotationY > 360f)
            {
                _rotationY = 0;
            }
        }
        else
        {
            _rotationY -= Time.timeScale * _rotationSpeed;
            if (_rotationY < -360f)
            {
                _rotationY = 0;
            }
        }
        _transform.localRotation = Quaternion.Euler(0, _rotationY, 0);
    }

    public void AddSkillCount()
    {
        if (_skillCount >= 2)
        {
            _skillCount = 2;
            return;
        }
        _sphereColiiderList[_skillCount + 2].gameObject.SetActive(true);
        _skillCount++;
    }
}
