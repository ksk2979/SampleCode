using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 문어가 포물선 공격을 위한 스크립트 (근데 일직선이였네?.. 민달팽이가 가져가는걸로)
public class EnemyLDAttack : BaseAttack
{
    Transform _trans;

    Vector3 _startPos, _endPos = Vector3.zero;

    public int _bodyRotateSpeed = 10; // 몸체 회전속도
    public float _height; // 포물선의 최고 높이
    public float _duration; // 포물선 공격의 총 시간
    public float _minDuration = 0.5f; // 최소 지속 시간
    public float _minHeight = 3f; // 최소 높이
    public Transform _body; // 몸체 돌아가게 하는 것
    public string _hitEfx; // 타격 이펙트

    private float _flowTime = 0; // 경과 시간
    private bool _trigger = false; // 공격 활성화 여부
    private float _destoryTime; // 파괴 시간
    private float _correctDuration = 0; // 보정된 지속 시간
    private float _correctHeight = 0; // 보정된 높이

    public void Init(Vector3 startPos, Vector3 endPos, EnemyStats enemyStats)
    {
        if (_trans == null) { _trans = this.transform; }
        _startPos = startPos;
        _endPos = endPos;
        if (stats == null) { stats = enemyStats; }
        _trans.position = this._startPos;
        _correctDuration = Mathf.Clamp(_duration * Vector3.Distance(startPos, endPos) / enemyStats._AttackRange, _minDuration, _duration);
        _correctHeight = Mathf.Clamp(_height * Vector3.Distance(startPos, endPos) / enemyStats._AttackRange, _minHeight, _height);
        InitData(stats, stats.GetTDD1());
        _trigger = true;
    }

    private void Update()
    {
        if (_trigger == false)
            return;
        _destoryTime += Time.deltaTime;
        _flowTime += Time.deltaTime;
        _flowTime = _flowTime % _correctDuration;

        if (_correctDuration < _destoryTime)
            Destroy();
    }

    private void FixedUpdate()
    {
        if (_trigger == false)
            return;

        _trans.position = MathParabola.Parabola(_startPos, _endPos + (Vector3.down * 0.5f), _correctHeight, _flowTime / _correctDuration);

        var halfPoint = (Vector3.Lerp(_startPos, _endPos, 0.5f) + Vector3.up * _correctHeight);
        if (_flowTime < _correctDuration * 0.5f)
            _body.rotation = Quaternion.Slerp(_body.rotation, Quaternion.LookRotation(halfPoint - _startPos), Time.deltaTime * _bodyRotateSpeed);
        else
            _body.rotation = Quaternion.Slerp(_body.rotation, Quaternion.LookRotation(_endPos - halfPoint), Time.deltaTime * _bodyRotateSpeed);
    }
    void ResetObj()
    {
        _trigger = false;
        _flowTime = 0;
        _destoryTime = 0;
    }

    void Destroy()
    {
        // 터지는 이펙트를 넣거나 할 수 있어 나두게 됨
        //SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _hitEfx, _trans.position, Quaternion.identity);
        SimplePool.Despawn(this.gameObject);
        ResetObj();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tdd.targetTag[0]))
        {
            other.GetComponent<Interactable>().TakeToDamage(tdd);
            Destroy();
            return;
        }
    }
}