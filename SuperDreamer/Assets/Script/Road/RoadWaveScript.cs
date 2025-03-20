using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadWaveScript : MonoBehaviour
{
    [SerializeField] LineCreate[] _line;
    [SerializeField] Transform _enemyParent;
    [SerializeField] Player _player;

    // 웨이브에 따른 적 체력 및 공격력 증가
    int _wave = 1;
    int _bossWave = 5;
    float _waveTime = 15f;
    int _waveEnemyCount = 12;
    float _enemySpawnTime = 1f;
    bool _startGame = false;
    string[] _enemySpawnPath = { "Enemy", "Bounty_1", "Bounty_2", "Boss" };
    string[] _shinusPath = { "Shinsu_1" };

    StageManager _stageManager;
    UIManager _uiManager;

    int _myGold = 100;
    int _subGold = 20;
    int _soul = 0;

    bool _bot = true;
    bool _bossPass = false;

    public void Init(StageManager stageManager)
    {
        if (_uiManager == null) { _uiManager = UIManager.GetInstance; }
        if (_stageManager == null) { _stageManager = stageManager; }

        if (_player != null) { _player.Init(); _player.InRoadWaveScript(this); }
        for (int i = 0; i < _line.Length; i++) { _line[i].Init(); }
        _startGame = true;
        if (this.name == "PlayerRoad") { _bot = false; }
        if (!_bot)
        {
            if (_uiManager != null)
            {
                _uiManager.InfoUpdate(0, _myGold);
                _uiManager.InfoUpdate(1, _soul);
                _uiManager.GoldUpdate(_subGold);
            }
        }
    }

    private void Update()
    {
        if (!_startGame) { return; }
        WaveUpdate();
        EnemySpawnUpdate();
        if (_bot)
        {
            if (_myGold > _subGold)
            {
                AddSpellBtn();
            }
        }
    }
    void WaveUpdate()
    {
        if (IsBossWave(_wave) && _bossPass && _stageManager.ClearCount == _stageManager.LifePlayer)
        {
            _bossPass = false;
            _waveTime = 0f;
        }
        _waveTime -= Time.deltaTime;
        if (_waveTime < 0f)
        {
            _wave++;
            if (IsBossWave(_wave))
            {
                _stageManager.ClearCount = 0;
                _waveTime = 20f;
                _waveEnemyCount = 1;
            }
            else
            {
                _waveTime = 15f;
                _waveEnemyCount = 12;
                _myGold += CalculateWaveReward(20);
                if (!_bot) { if (_uiManager != null) { _uiManager.InfoUpdate(0, _myGold); } }
            }
        }
        if (!_bot && _uiManager != null)
        {
            _uiManager.GetPlayerUIInfo.WaveCountUpdate(_wave);
            _uiManager.GetPlayerUIInfo.WaveTimeUpdate(_waveTime);
        }
    }
    void EnemySpawnUpdate()
    {
        if (_waveEnemyCount > 0)
        {
            _enemySpawnTime -= Time.deltaTime;
            if (_enemySpawnTime < 0f)
            {
                if (IsBossWave(_wave))
                {
                    EnemyCreate(3);
                }
                else
                {
                    EnemyCreate(0);
                }
                _enemySpawnTime = 1f;
            }
        }
    }
    public void PlayerDie()
    {
        _startGame = false;
        _stageManager.LifePlayer--;
    }
    bool IsBossWave(int wave)
    {
        return wave % _bossWave == 0;
    }
    public void EnemyCreate(int id)
    {
        _waveEnemyCount--;
        GameObject obj = SimplePool.Spawn(CommonStaticKey.RESOURCES_ENEMY, _enemySpawnPath[id]);
        Enemy enemy = obj.GetComponent<Enemy>();
        if (id == 0) { enemy.Init(50, 100, _wave); }
        else if (id == 1 || id == 2) { enemy.Init(50, 1000, _wave); enemy.BossFunction(); enemy.name = _enemySpawnPath[id]; }
        else if (id == 3) { enemy.Init(50, 2000, _wave); enemy.BossFunction(); }
        enemy.InRoadWaveScript(this);

        if (_enemyParent != null)
        {
            obj.transform.SetParent(_enemyParent);
            obj.transform.localPosition = Vector3.zero;
        }
        else
        {
            SimplePool.Despawn(obj);
        }
    }
    public void ShinusCreate(int id)
    {
        GameObject obj = SimplePool.Spawn(CommonStaticKey.RESOURCES_SHINUS, _shinusPath[id]);
        ShinsuController sh = obj.GetComponent<ShinsuController>();
        sh.OnStart();
        sh.transform.position = _line[1].GetLineObj.transform.position;
    }

    int GetRarityIndex()
    {
        float[] probabilities = { 84.98f, 11.32f, 3f, 0.5f, 0.2f, 0f };
        float randomValue = Random.Range(0f, 100f);
        float cumulative = 0f;

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulative += probabilities[i];
            if (randomValue <= cumulative)
                return i;
        }
        return 0;
    }
    public AttackType GetRandomAttackType()
    {
        return (AttackType)Random.Range(0, 3);
    }
    public void AddSpellBtn()
    {
        if (_subGold > _myGold) { MessageHandler.Getinstance.ShowMessage("Not enough gold ", 2f); return; }
        _myGold -= _subGold;
        _subGold += 1;
        _player.GetPlayerController.AddSpell(GetRarityIndex(), GetRandomAttackType());
        if (!_bot)
        {
            if (_uiManager != null)
            {
                _uiManager.InfoUpdate(0, _myGold);
                _uiManager.GoldUpdate(_subGold);
            }
        }
    }

    public int CalculateWaveReward(int gold)
    {
        int waveIndex = (_wave - 1) / 5;
        double additionalGold = gold * Mathf.Pow(1.25f, waveIndex);
        return Mathf.CeilToInt((float)additionalGold);
    }
    public void GetEnemyGold(int gold)
    {
        _myGold += CalculateWaveReward(gold);
        if (!_bot) { if (_uiManager != null) { _uiManager.InfoUpdate(0, _myGold); } }
    }
    public void GetBossSoul(int soul)
    {
        _soul += soul;
        if (!_bot) { if (_uiManager != null) { _uiManager.InfoUpdate(1, _soul); } }
        _bossPass = true;
        _stageManager.ClearCount++;
    }
    public void GetBountyGold(int gold)
    {
        _myGold += gold;
        if (!_bot) { if (_uiManager != null) { _uiManager.InfoUpdate(0, _myGold); } }
    }
    public void GetBountySoul(int soul)
    {
        _soul += soul;
        if (!_bot) { if (_uiManager != null) { _uiManager.InfoUpdate(1, _soul); } }
    }
    public bool GetBossPass { get { return _bossPass; } }
    public bool GetBot { get { return _bot; } }
    public int GetMyGold { get { return _myGold; } set { _myGold = value; } }
    public bool GetStartGame { get { return _startGame; } }
    public Player GetPlayer { get { return _player; } }
}
