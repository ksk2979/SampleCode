using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirMover : MonoBehaviour
{
    // Ȱ��ȭ �Ǹ� 2�� ������ ������ ���� ����
    bool _move = false;

    string[] _targetTag; // ���� �� �±�
    int _layerMask; // ���� ���̾�

    Transform _target;
    Transform _trans;

    float _settingTime = 0f;
    [SerializeField] float _speed = 5f;

    float _dieTime = 0f;

    private void Awake()
    {
        _layerMask = 1 << LayerMask.NameToLayer(CommonStaticDatas.LAYERMASK_PLAYER);
        _targetTag = new string[] { CommonStaticDatas.TAG_PLAYER };
        _trans = this.transform;
    }

    private void OnEnable()
    {
        Invoke("StartMove", 2f);
    }

    void StartMove()
    {
        _move = true;
    }

    private void Update()
    {
        if (_move)
        {
            _dieTime += Time.deltaTime;
            _settingTime += Time.deltaTime;
            if (_settingTime > 0.5f)
            {
                _settingTime = 0f;
                // Ÿ���� ã�´�
                _target = StandardFuncUnit.CheckTraceTarget(_trans, _targetTag, 10000f, _layerMask);
            }
            if (_target != null)
            {
                _trans.position = Vector3.MoveTowards(_trans.position, _target.position, Time.deltaTime * _speed);
            }
            if (_dieTime > 10f)
            {
                _dieTime = 0f;
                _move = false;
                SimplePool.Despawn(this.gameObject);
            }
        }
    }
}
