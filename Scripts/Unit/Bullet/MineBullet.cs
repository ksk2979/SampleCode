using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;

public class MineBullet : MonoBehaviour
{
    public int _bodyRotateSpeed = 10;
    public Transform _body;
    public string _hitEfx;
    float _minHeightThreshold = 0f;

    Transform _trans;
    float _delayDestroy = 0f;
    WeaponData _data;
    PlayerStats _playerStats;

    bool _idle = false;
    bool _isHoming = false;

    float _autoDestroyTime = 0f;

    public void SetData(Vector3 startPos, Vector3 endPos, WeaponData data, PlayerStats playerStats)
    {
        if (_trans == null) { _trans = this.transform; }
        Reset();
        if (_playerStats == null) { _playerStats = playerStats; }
        if (_data == null) { _data = data; }
        _trans.position = startPos;
    }

    public void Destroy()
    {
        // 마지막 이팩트 생성 구간
        var bullet = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _hitEfx, _trans.position, Quaternion.identity);
        bullet.GetComponent<Explosion>().InitData(_playerStats, _playerStats.GetTDD1());
        Reset();
        SimplePool.Despawn(gameObject);
    }
    public void Reset()
    {
        _delayDestroy = 0f;
        _isHoming = false;
        _idle = false;
        _autoDestroyTime = 0f;
    }
    void FixedUpdate()
    {
        if (_idle)
        {
            var targets = Physics.OverlapSphere(transform.position, 2.5f, _playerStats.GetTDD1().layerMask);
            for (int i = 0; i < targets.Length; i++)
            {
                Destroy();
                _idle = false;
                return;
            }
            _delayDestroy += Time.deltaTime;
            if (_delayDestroy > 2f)
            {
                _delayDestroy = 0f;
                Destroy();
                _idle = false;
            }
        }

        if (!_isHoming)
        {
            _trans.position += _trans.forward * 2f * Time.fixedDeltaTime;
            _trans.position += Vector3.down * 1f * Time.fixedDeltaTime;
            if (_trans.position.y <= _minHeightThreshold)
            {
                _isHoming = true;
                _idle = true;
            }
        }

        _autoDestroyTime += Time.deltaTime;
        if (_autoDestroyTime > 8f) { _autoDestroyTime = 0f; Debug.Log("자동 폭파"); Destroy(); }
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