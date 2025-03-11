using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueBox : MonoBehaviour
{
    Transform _trans;

    Vector3 _startPos, _endPos = Vector3.zero;
    private bool _trigger = false;

    float _dis;
    float _time;
    float _speed = 1f;

    [SerializeField] GameObject[] _effect;
    int _effectIndex;

    public void Init(Vector3 startPos, Vector3 target)
    {
        if (_trans == null) { _trans = this.transform; }
        _startPos = startPos + new Vector3(0f, 0.5f, 0f);
        _trans.position = _startPos;
        _endPos = target;
        _dis = Vector3.Distance(_startPos, _endPos);
        _time = 0;
        _trigger = true;

        // 랜덤 상자의 파티클 설정과 결과 보여주기
        _effectIndex = Random.Range(0, _effect.Length);
        for (int i = 0; i < _effect.Length; ++i)
        {
            _effect[i].transform.SetParent(_trans);
            _effect[i].transform.localPosition = Vector3.zero;
            _effect[i].SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (_trigger == false)
            return;

        //_trans.position = MathParabola.Parabola(_startPos, _endPos + (Vector3.down * 0.5f), _correctHeight, _flowTime / _correctDuration);

        if (_dis > 0.2f)
        {
            _time += Time.deltaTime * _speed;
            Vector3 tempPos = MathParabola.Parabola(_startPos, _endPos, 7, _time);
            _trans.position = tempPos;
            _dis = Vector3.Distance(_trans.position, _endPos);

            if (_trans.position.y < 0) { _trigger = false; }
        }
        else { _trigger = false; }
     }
    void ResetObj()
    {
        _trigger = false;
        //_flowTime = 0;
        //_destoryTime = 0;
    }
    void Destroy()
    {
        // 터지는 이펙트를 넣거나 할 수 있어 나두게 됨
        //SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _hitEfx, _trans.position, Quaternion.identity);
        SimplePool.Despawn(this.gameObject);
        ResetObj();
        // _effectIndex에 따른 보상 획득
        StageManager.GetInstance._ingredientRatio[_effectIndex] += 0.1f;

        // 이펙트 리셋
        _effect[_effectIndex].transform.SetParent(null);
        _effect[_effectIndex].transform.position = _trans.position;
        _effect[_effectIndex].transform.rotation = Quaternion.identity;
        _effect[_effectIndex].SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(CommonStaticDatas.TAG_PLAYER))
        {
            //other.GetComponent<Interactable>().TakeToDamage(tdd);
            Destroy();
            return;
        }
    }
}
