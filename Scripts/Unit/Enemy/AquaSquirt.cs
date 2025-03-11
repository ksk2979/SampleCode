using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;

// Å©¶óÄË ¸Ô¹°
public class AquaSquirt : BaseAttack
{
    Transform _trans;
    Vector3 _startPos, _endPos;
    public float _minHeight = 4f;
    public string _hitEfx;

    bool _trigger = false;
    public float _speed = 3f;
    float _time = 0f;

    public void SetData(Vector3 startPos, Vector3 endPos)
    {
        if (_trans == null) { _trans = this.transform; }
        Reset();
        _startPos = startPos;
        _endPos = endPos;
        _trans.position = _startPos;
        TriggerrOn();
    }

    public void Destroy()
    {
        // ¸¶Áö¸· ÀÌÆÑÆ® »ý¼º ±¸°£
        var bullet = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _hitEfx, _trans.position, Quaternion.identity);
        bullet.transform.Find("Collider").GetComponent<BossAttackEffect>().InitData(stats, tdd);
        bullet.transform.Find("Collider").GetComponent<BossAttackEffect>().DarkSet = true;
        Reset();
        SimplePool.Despawn(gameObject);
    }

    public void TriggerrOn()
    {
        _trigger = true;
    }
    public void Reset()
    {
        _time = 0f;
        _trigger = false;
        _trans.position = _startPos;
    }

    void FixedUpdate()
    {
        if (_trigger == false)
            return;

        _time += Time.deltaTime * _speed;
        Vector3 tempPos = MathParabola.Parabola(_startPos, _endPos, 4, _time);
        _trans.position = tempPos;

        if (_trans.position.y < 0) { Destroy(); return; }
    }
}