using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] Transform[] _posMove;
    [SerializeField] Player[] _player;
    [SerializeField] BackgroundLoop[] _backgroundLoopArr;
    [SerializeField] TruckAnima _truckAnima;

    float _tempTime = 0f;
    bool _backgroundMove = false;

    private void Start()
    {
        for (int i = 0; i < _player.Length; ++i)
        {
            _player[i].Init();
        }
    }

    private void Update()
    {
        _tempTime -= Time.deltaTime;
        if (_tempTime < 0f)
        {
            if (_backgroundMove)
            {
                _tempTime = Random.Range(0.2f, 1f);
                CreateEnemy(Random.Range(0, 3));
            }
            else
            {
                _tempTime = Random.Range(3f, 7f);
                CreateEnemy(Random.Range(0, 3));
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            // 멈추면 좀비가 쏟아져 나오게 구현
            MoveOnOff(_backgroundMove == true ? false : true);
            if (_backgroundMove) { _tempTime = 0f; }
        }
    }

    public void CreateEnemy(int number)
    {
        GameObject obj = SimplePool.Spawn(CommonStaticKey.RESOURCES_ENEMY, string.Format("{0}{1}", "Enemy_", number + 1));
        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Init(50, 3000, 1);

        if (_posMove[number] != null)
        {
            obj.transform.SetParent(_posMove[number]);
            obj.transform.localPosition = Vector3.zero;
        }
        else
        {
            SimplePool.Despawn(obj);
        }
    }

    public void MoveOnOff(bool active)
    {
        _backgroundMove = active;
        for (int i = 0; i < _backgroundLoopArr.Length; ++i)
        {
            _backgroundLoopArr[i].AnimaController(active);
        }
        if (_truckAnima != null) { _truckAnima.AnimaController(active); }
    }
}
