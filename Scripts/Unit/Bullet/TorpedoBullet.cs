using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;

// ���
// ���̾�����Ʈ���� ������ �����鼭 ������ ��������
// Y��ǥ ������ -�� ������������ ��ó�� �ִ� ���� ������ ������
// ���߿� �� ��ڴ� Ÿ���� ���� ���󰣴�
// ������� �����ð��� ������ �ڵ����� ���ĵȴ� (���� ȸ�� ����)

public class TorpedoBullet : MonoBehaviour
{
    public int _bodyRotateSpeed = 10;
    public Transform _body;
    public string _hitEfx;
    float _minHeightThreshold = -1f;
    bool _isHoming = false;

    Transform _trans;
    bool _trigger = false;
    float _destroyDelay;
    WeaponData _data;
    PlayerStats _playerStats;
    Transform _targetTrans;

    public void SetData(Vector3 startPos, Vector3 endPos, WeaponData data, PlayerStats playerStats)
    {
        if (_trans == null) { _trans = this.transform; }
        Reset();
        if (_playerStats == null) { _playerStats = playerStats; }
        if (_data == null) { _data = data; }
        _trans.position = startPos;
        TriggerrOn();
    }

    public void Destroy()
    {
        // ������ ����Ʈ ���� ����
        var bullet = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _hitEfx, _trans.position, Quaternion.identity);
        bullet.GetComponent<Explosion>().InitData(_playerStats, _playerStats.GetTDD1());
        Reset();
        SimplePool.Despawn(gameObject);
    }

    public void TriggerrOn()
    {
        _trigger = true;
    }
    public void Reset()
    {
        _trigger = false;
        _destroyDelay = 0;
        _targetTrans = null;
        _isHoming = false;
    }

    private void Update()
    {
        if (!_trigger)
            return;
        _destroyDelay += Time.deltaTime;
        if (_destroyDelay > 5f)
        {
            Destroy();
        }
    }

    void FixedUpdate()
    {
        if (!_isHoming)
        {
            _trans.position += _trans.forward * 2f * Time.fixedDeltaTime;
            _trans.position += Vector3.down * 1f * Time.fixedDeltaTime;
            if (_trans.position.y <= _minHeightThreshold)
            {
                _isHoming = true;
                _targetTrans = StandardFuncUnit.CheckTraceTarget(_trans, _playerStats.GetTDD1().targetTag, _playerStats.turretShootingRange, _playerStats.GetTDD1().layerMask);
            }
        }

        if (_isHoming)
        {
            if (_targetTrans != null)
            {
                Vector3 directionToTarget = (_targetTrans.position - _trans.position).normalized;
                _trans.position += directionToTarget * 20f * Time.fixedDeltaTime;
                _trans.rotation = Quaternion.LookRotation(directionToTarget);
                float targetDir = Vector3.Distance(_trans.position, _targetTrans.position);
                if (targetDir < 0.5f) { Destroy(); }
            }
            else
            {
                Destroy();
            }
        }
    }

    const string TAG_ENEMY = "Enemy";
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TAG_ENEMY))
        {
            Destroy();
        }
    }
}
