using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;
using System;

/// <summary>
/// 스테이지
/// 월드 -> 스테이지 -> 챕터 점령
/// 시작, 일반, 탈출 지점으로 나뉨
/// 스테이지는 점령된후 일정 시간이 지나면 좀비 땅으로 변함
/// 보호막을 가질수 잇음 
/// - 클리어 하고 나면 생김
/// - 보호막이 있을 동안 보호막 내구력(시간)이 깍임
/// - 방어벽 등급마다 방어력 내구력(시간)이 다름 
/// 앱을꺼도 계산되어야함
/// 일반 스테이지는 클리어 챕터 좀비 챕터로 나뉨
/// 방금 점령한곳에 표시
/// 탈출 지점을 클리어하면 다음 스테이지로 갈수 있다.
/// </summary>
public class Chapter : MonoBehaviour
{
    public Transform _cameraParent = null; // 카메라 포인트 챕터가 바뀔때 마다 해당위치로 위치시킨다
    public Transform _startPos = null; // 탱크 시작 포인트
    public Transform[] _colleagueStartPos = null;
    public GameObject _mapObj;

    protected List<EnemyController> enemyControllerList = new List<EnemyController>(); // 생성한 적들 을 가지고 있는다
    protected bool _isEnd = false; // 챕터의 끝에 적들이 다죽었는지 확인하기 위해 설치

    protected int _enemyCount = 0;
    protected int _totalCount = 0; // 소환된 몬스터의 수
    protected int _maxCount = 0; // 총 소환 UI 표시수
    protected int _killCount = 0;

    public virtual void Init(StageSetData stageData)
    {
        _isEnd = false;
        CameraManager.GetInstance.InitPos(_cameraParent);
 
        StartCoroutine(CreateEnemys());
    }
    /// <summary>
    ///  시작시 살짝 딜레이를 준다.
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateEnemys()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        enemyControllerList.Clear();
    }

    internal void ClearEnemys()
    {
        foreach (var item in enemyControllerList)
        {
            item.DoDie();
        }
    }

    public int GetAppearZombieCount()
    {
        return enemyControllerList.Count;
    }

    public List<EnemyController> GetEnemys()
    {
        return enemyControllerList;
    }

    public void GetEnemyRemove(EnemyController enemy)
    {
        enemyControllerList.Remove(enemy);
        _enemyCount--;
        _maxCount--;
        //Debug.Log($"_enemyCount: {_enemyCount}, _maxCount: {_maxCount}");
        if (_maxCount <= 0) { _maxCount = 0; }
        InGameUIManager.GetInstance.UpdateEnemyCount(_maxCount.ToString());
        _killCount++;
    }

    public bool IsClearGame => _maxCount == 0;
    public int KillCount => _killCount;
}
