using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 전기 고양이
/// </summary>

// 전기 지역 야옹거리며 자기 주변 전기 장판대미지
// 느낌이 일반 공격은 없는거 같고 전기 장판이 꺼졌다가 켜졌다가 반복하면 될꺼 같음
public class EliteElectroCatController : EnemyController
{
    [SerializeField] EliteElectroCatAttack _attackObj; // 장판 대미지
    float _attackOnTime = 0f;
    float _attackOnMaxTime = 2f;

    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            _attackObj.InitData(_enemyStats, _enemyStats.GetTDD1());
            _oneOnStartInit = true;
            if (_animaController != null)
            {
                _animaController.AnimEnventSender("AttackEnd", AttackEnd);
            }
        }

        SetState(eCharacterStates.Spawn);
    }

    //private void Update()
    //{
    //    _attackOnTime += Time.deltaTime;
    //    if (_attackOnTime > _attackOnMaxTime)
    //    {
    //        _attackOnTime = 0f;
    //        if (_attackObj.gameObject.activeSelf) { _attackObj.gameObject.SetActive(false); _attackOnMaxTime = 5f; }
    //        else { _attackObj.gameObject.SetActive(true); _attackOnMaxTime = 2f; }
    //    }
    //}

    // 일단 장판 딜만 하기 때문에 일반 공격은 안한다
    public override void BasicAttack()
    {
        _attackObj.gameObject.SetActive(true);
    }
    public void AttackEnd()
    {
        _attackObj.gameObject.SetActive(false);
    }
}
