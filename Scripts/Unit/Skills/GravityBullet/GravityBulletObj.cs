using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBulletObj : MonoBehaviour
{
    Transform _trans;
    UnitDamageData tdd;
    GravityManager _gravityManager;

    // 끌어 당길 적
    List<Transform> _enemyTransList;

    float _lifeTime = 2f;
    bool _open = false;
    Collider _collider;

    // 포물선 관련
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

    GameObject _gravityEffect;

    public void InitData(UnitDamageData data, GravityManager manager)
    {
        _trans = this.transform;
        tdd = data;
        _gravityManager = manager;
        _enemyTransList = new List<Transform>();
        _collider = this.GetComponent<Collider>();
    }
    public void Init(Vector3 startPos, Vector3 endPos)
    {
        if (_trans == null) { _trans = this.transform; }
        _startPos = startPos;
        _endPos = endPos;
        _trans.position = this._startPos;
        _correctDuration = Mathf.Clamp(_duration * Vector3.Distance(startPos, endPos) / 5f, _minDuration, _duration);
        _correctHeight = Mathf.Clamp(_height * Vector3.Distance(startPos, endPos) / 5f, _minHeight, _height);
        _trigger = true; 
        _collider.enabled = false;
        this.gameObject.SetActive(true);
        _trans.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
        _lifeTime = 2f;
        _open = false;
        _flowTime = 0;
        _destoryTime = 0;
    }

    public void EndAbility()
    {
        this.gameObject.SetActive(false);
        _trans.rotation = Quaternion.identity;
        _gravityManager.GravityObjEnqueue(this);
        _enemyTransList.Clear();

        _trigger = false;
        _flowTime = 0;
        _destoryTime = 0;
        if (_gravityEffect != null) { _gravityEffect.SetActive(false); }
    }

    private void Update()
    {
        if (_open) { return; }

        if (_trigger == false)
            return;
        _destoryTime += Time.deltaTime;
        _flowTime += Time.deltaTime;
        _flowTime = _flowTime % _correctDuration;

        // 여기서 끌어당기는거 활성화
        if (_correctDuration < _destoryTime)
        {
            _open = true;
            _collider.enabled = true;
            if (_gravityEffect == null) { _gravityEffect = SimplePool.Spawn(CommonStaticDatas.RES_EX, "FX_Hit_08", _trans, Vector3.zero, Quaternion.identity); }
            if (_gravityEffect != null) { _gravityEffect.SetActive(true); }
        }
    }

    private void FixedUpdate()
    {
        if (_open)
        {
            _lifeTime -= Time.deltaTime;
            if (_lifeTime < 0f) { EndAbility(); return; }
            //_trans.Translate(Vector3.forward * Time.deltaTime * 3f);
            for (int i = 0; i < _enemyTransList.Count; ++i)
            {
                _enemyTransList[i].position = Vector3.MoveTowards(_enemyTransList[i].position, _trans.position, 7.0f * Time.deltaTime);
            }
        }
        else
        {
            // 포물선 업데이트
            if (_trigger == false)
                return;

            _trans.position = MathParabola.Parabola(_startPos, _endPos + (Vector3.down * 0.5f), _correctHeight, _flowTime / _correctDuration);

            var halfPoint = (Vector3.Lerp(_startPos, _endPos, 0.5f) + Vector3.up * _correctHeight);
            if (_flowTime < _correctDuration * 0.5f)
                _body.rotation = Quaternion.Slerp(_body.rotation, Quaternion.LookRotation(halfPoint - _startPos), Time.deltaTime * _bodyRotateSpeed);
            else
                _body.rotation = Quaternion.Slerp(_body.rotation, Quaternion.LookRotation(_endPos - halfPoint), Time.deltaTime * _bodyRotateSpeed);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (tdd != null)
        {
            if (other.CompareTag(tdd.targetTag[0]))
            {
                if (other.GetComponent<Enemy>()._unit == UNIT.Boss) return;
                _enemyTransList.Add(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (tdd != null)
        {
            if (other.CompareTag(tdd.targetTag[0]))
            {
                _enemyTransList.Remove(other.transform);
            }
        }
    }
}