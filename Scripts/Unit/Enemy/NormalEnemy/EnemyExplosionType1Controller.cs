using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 자폭병 스크립트
public class EnemyExplosionType1Controller : EnemyController
{
    public string effectName = string.Empty;
    public bool addChild = false;
    public Transform perent;
    private GameObject obj;
    bool _explosion = false; // 한번 폭파하면 끝

    public void OnEffectDream()
    {
        _explosion = true;
        obj = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, effectName, Vector3.zero, Quaternion.identity);
        obj.transform.position = perent.position;
        obj.transform.forward = perent.forward;
        if (addChild)
        {
            obj.transform.SetParent(perent);
        }
        obj.GetComponent<Explosion>().InitData(_enemyStats, _enemyStats.GetTDD1());
    }

    public override void OnStart()
    {
        base.OnStart();
        if (!_oneOnStartInit)
        {
            StateStart();
            if (_animaController != null)
            {
                _animaController.AnimEnventSender("OnEffectDream", OnEffectDream);
            }

            if (!GameManager.GetInstance.TestEditor) { _stageManager = StageManager.GetInstance; }
            _oneOnStartInit = true;
        }
        _explosion = false;
        SetState(eCharacterStates.Spawn);
    }

    // 이놈은 터진다
    public override void AttackFinishCall()
    {
        if (_curState == eCharacterStates.Die)
            return;
        if (_target != null && _target.GetComponent<CharacterController>().IsDie())
            _target = null;

        if (!_explosion) { OnEffectDream(); }

        _enemyStats.hp = 0;
        DoDie();
    }
    
    protected override void DieInit()
    {
        base.DieInit();

        //if (GameManager.GetInstance.TestEditor) { if (!GameManager.GetInstance.Production) { SpawnTestManager.GetInstance.GetEnemyRemove(this); } }
        //else { _stageManager._chapter[_stageManager._cc].GetEnemyRemove(this); }
    }

    public override void DieFinishCall()
    {
        testDieFinishCheck = true;
        //if (GameManager.GetInstance.TestEditor) { if (!GameManager.GetInstance.Production) { SpawnTestManager.GetInstance.GetEnemyRemove(this); } }
        //else
        //{
        //    if (_stageManager == null) { _stageManager = StageManager.GetInstance; }
        //    _stageManager._chapter[_stageManager._cc].GetEnemyRemove(this);
        //}
        if (_animaController != null)
            _animaController.PlayerBoolenAnimation(CommonStaticDatas.ANIMPARAM_IS_DIE, false);
        SimplePool.Despawn(gameObject);
    }
}
