using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��� ������ ������ ���� ��ũ��Ʈ (�ٵ� �������̿���?.. �δ����̰� �������°ɷ�)
public class EnemyLDAttack : BaseAttack
{
    Transform _trans;

    Vector3 _startPos, _endPos = Vector3.zero;

    public int _bodyRotateSpeed = 10; // ��ü ȸ���ӵ�
    public float _height; // �������� �ְ� ����
    public float _duration; // ������ ������ �� �ð�
    public float _minDuration = 0.5f; // �ּ� ���� �ð�
    public float _minHeight = 3f; // �ּ� ����
    public Transform _body; // ��ü ���ư��� �ϴ� ��
    public string _hitEfx; // Ÿ�� ����Ʈ

    private float _flowTime = 0; // ��� �ð�
    private bool _trigger = false; // ���� Ȱ��ȭ ����
    private float _destoryTime; // �ı� �ð�
    private float _correctDuration = 0; // ������ ���� �ð�
    private float _correctHeight = 0; // ������ ����

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
        // ������ ����Ʈ�� �ְų� �� �� �־� ���ΰ� ��
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