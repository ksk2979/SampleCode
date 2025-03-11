using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 잉어가 쏘는 어뢰
// 플레이어가 들어오면 터지고 아니면 일정 시간 이후에 터진다
public class EliteCarpAttack : BaseAttack
{
    float _speed;
    bool _move = false;
    Transform _trans;

    float _boomTime = 0f;

    public void Init()
    {
        _speed = 5f;
        _move = true;
        if (_trans == null) { _trans = this.transform; }
        _boomTime = 5f;
        this.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_move)
        {
            _trans.Translate(Vector3.forward * _speed * Time.deltaTime);

            if (_speed > 0f) { _speed -= Time.deltaTime; }
            else { _speed = 0f; _move = false; }
        }
        _boomTime -= Time.deltaTime;
        if (_boomTime < 0f) { _boomTime = 0f; Destroy(); return; }
    }

    void Destroy()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "Enemy/EnemyTorpedoAttackLink", Vector3.zero, Quaternion.identity);
        obj.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        obj.transform.forward = this.transform.forward;
        EliteCarpAttackLink ef = obj.GetComponent<EliteCarpAttackLink>();
        ef.InitData(stats, stats.GetTDD1());
        ef.gameObject.SetActive(true);
        SimplePool.Despawn(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(CommonStaticDatas.TAG_PLAYER))
        {
            Destroy();
            return;
        }
    }
}
