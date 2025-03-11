using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 레벨업시 도발 범위 증가
public class GhostShipObj : MonoBehaviour
{
    float _lifeTime = 6f;
    const string TAG_ENEMY = "Enemy";
    Transform _trans;

    List<EnemyController> _provocationTarget;

    float _provocationTime = 0f;

    public void InitData()
    {
        _provocationTarget = new List<EnemyController>();
        _trans = this.transform;
        _provocationTime = 0f;
    }

    private void FixedUpdate()
    {
        _lifeTime -= Time.deltaTime;
        if (_lifeTime < 0f)
        {
            EndAbility();
            return;
        }
        _provocationTime -= Time.deltaTime;
        if (_provocationTime < 0f)
        {
            _provocationTime = 1f;
            for (int i = 0; i < _provocationTarget.Count; ++i)
            {
                if (_provocationTarget[i] != null) { _provocationTarget[i].TargetSetting(_trans); }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TAG_ENEMY))
        {
            _provocationTarget.Add(other.GetComponent<EnemyController>());
        }
    }

    public void StartAbility(Vector3 pos)
    {
        this.transform.position = pos;
        this.gameObject.SetActive(true);
        _lifeTime = 6f;
        _provocationTime = 0f;
    }
    public void EndAbility()
    {
        this.gameObject.SetActive(false);
        //for (int i = 0; i < _provocationTarget.Count; ++i)
        //{
        //    if (_provocationTarget[i] != null) { _provocationTarget[i].TargetNotting(); }
        //}
        _provocationTarget.Clear();
        _provocationTime = 0f;
    }
}
