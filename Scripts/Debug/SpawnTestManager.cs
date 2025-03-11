using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using MyData;
using Info;
using TMPro;

public class SpawnTestManager : SceneStaticObj<SpawnTestManager>
{
    PlayerController _playerController;
    PlayerStats _playerStat;
    List<Interactable> _listPlayers;
    public PlayerInfo[] _playerInfoArr;

    [SerializeField] DebugTestInfo _debugTestInfo;

    [SerializeField] Transform _cameraParent;

    [SerializeField] TextMeshProUGUI _timeText;
    private StopWatchManager _stopwatch; // �����ġ�� ������ ����µ��� ȥ�� �ð��� �귯���°ſ� ���� ������ �ؾ���

    public List<EnemyController> _enemysCtrl = new List<EnemyController>(); // ������ ���� �� ������ �ִ´�
    public List<EnemyController> _bossCtrl = new List<EnemyController>();
    public bool _isEnd = false; // é���� ���� ������ ���׾����� Ȯ���ϱ� ���� ��ġ

    public int _enemyCount = 0;
    public int _totalCount = 0; // ��ȯ�� ������ ��
    public int _maxUI = 0; // �� ��ȯ UI ǥ�ü�
    public int _killCount = 0;
    [Tooltip("���͵��� ���� ����")]
    public Transform[] appearPoint = null;

    bool _spawn = false;
    float _spawnEnemyTime = 0f;

    CSVFileManager _dataManager;
    public TestGameData _testGameData;

    public void Awake()
    {
        _stopwatch = new StopWatchManager();
        _listPlayers = new List<Interactable>();
        _playerInfoArr = new PlayerInfo[5];
        _dataManager = new CSVFileManager();
        _testGameData = new TestGameData();
        _debugTestInfo.Init(this);
        //TestGameData anotherData = new TestGameData
        //{
        //    _boatDataID = "3/0/0/0/0/0",
        //    _boatDataLevel = "3/0/0/0/0/0",
        //    _boatDamage = "750",
        //    _abilityCheck = "1/0/1",
        //    _enemyID = "5",
        //    _enemyCount = "70",
        //    _time = "07:30"
        //};
        //
        //_dataManager.AddGameData(anotherData);
        //_dataManager.ExportFile();
    }

    public void CreateBoat(int num, PlayerInfo info)
    {
        _playerInfoArr[num] = info;

        Info.PlayerInfo playerinfo = _playerInfoArr[num];
        var boatData = DataManager.GetInstance.GetData(DataManager.KEY_BOAT, playerinfo.GetPlayerValue(ECoalescenceType.BOAT_ID), 1) as BoatData;
        _playerInfoArr[num].SetEquipValue(EItemList.BOAT, boatData.addHp);
        _playerInfoArr[num].SetEquipBoatValue(boatData.addDamage);
        var boat = SimplePool.Spawn(CommonStaticDatas.RES_BOAT, boatData.resName, Vector3.zero, Quaternion.identity);
        var player = boat.GetComponent<Player>();

        // ������ ���� ��ȯ
        if (playerinfo.GetPlayerValue(ECoalescenceType.WEAPON_ID) == 0) 
        { 
            playerinfo.SetPlayerValue(ECoalescenceType.WEAPON_ID, boatData.basicWeaponType); 
            playerinfo.SetPlayerValue(ECoalescenceType.WEAPON_LEVEL, 1); 
        } // �⺻���� �����ÿ�
        var weaponData = DataManager.GetInstance.GetData(DataManager.KEY_WEAPON, playerinfo.GetPlayerValue(ECoalescenceType.WEAPON_ID), 1) as WeaponData;
        _playerInfoArr[num].SetEquipValue(EItemList.WEAPON, weaponData.addDamage);
        var weapon = SimplePool.Spawn(CommonStaticDatas.RES_WEAPON, weaponData.resName, Vector3.zero, Quaternion.identity);

        // ������ ���� ��ȯ
        DefenseData defenseData = null;
        GameObject defense = null;
        if (playerinfo.GetPlayerValue(ECoalescenceType.DEFENSE_ID) != 0)
        {
            // �̰� ����� �ٸ��� �⺻���� ���� ������ ����
            defenseData = DataManager.GetInstance.GetData(DataManager.KEY_DEFENSE, playerinfo.GetPlayerValue(ECoalescenceType.DEFENSE_ID), 1) as DefenseData;
            _playerInfoArr[num].SetEquipValue(EItemList.DEFENSE, defenseData.addValue); 
            defense = SimplePool.Spawn(CommonStaticDatas.RES_DEFENSE, defenseData.resName, Vector3.zero, Quaternion.identity);
            if (defense != null)
            {
                boat.GetComponent<PlayerController>()._defensePoint.GetPointSetting((EDefensePoint)defenseData.defensePoint, CommonStaticDatas.RES_DEFENSE, defenseData.resName, defense);
            }
        }
        // ������ ����
        CaptainData captainData = null;
        if (playerinfo.GetPlayerValue(ECoalescenceType.CAPTAIN_ID) != 0)
        {
            captainData = DataManager.GetInstance.GetData(DataManager.KEY_CAPTAIN, playerinfo.GetPlayerValue(ECoalescenceType.DEFENSE_ID), 1) as CaptainData;
            _playerInfoArr[num].SetEquipValue(EItemList.CAPTAIN, captainData.addDamage);
        }
        // ������ ����
        SailorData sailorData = null;
        if (playerinfo.GetPlayerValue(ECoalescenceType.SAILOR_ID) != 0)
        {
            sailorData = DataManager.GetInstance.GetData(DataManager.KEY_SAILOR, playerinfo.GetPlayerValue(ECoalescenceType.SAILOR_ID), 1) as SailorData;
            _playerInfoArr[num].SetEquipValue(EItemList.SAILOR, sailorData.addValue);
        }
        // ������ ����
        EngineData engineData = null;
        if (playerinfo.GetPlayerValue(ECoalescenceType.ENGINE_ID) != 0)
        {
            engineData = DataManager.GetInstance.GetData(DataManager.KEY_ENGINE, playerinfo.GetPlayerValue(ECoalescenceType.ENGINE_ID), 1) as EngineData;
            _playerInfoArr[num].SetEquipValue(EItemList.ENGINE, engineData.addValue);
        }

        PlayerController playerController = boat.GetComponent<PlayerController>();
        if (num == 0)
        {
            _playerController = playerController;
            _playerController._gunFireTop = weapon.GetComponent<GunFireTop>();
            _playerStat = boat.GetComponent<PlayerStats>();
            _playerController._gunFireTop.GetWeaponData = weaponData;
            _playerController.GetMovement.avoidancePriority--;
            _playerController._main = true;
            CameraManager.GetInstance.InitPos(_cameraParent);
        }
        else
        {
            playerController._gunFireTop = weapon.GetComponent<GunFireTop>();
            playerController._gunFireTop.GetWeaponData = weaponData;
        }
        // ����� ��Ʈ�� ������ �ջ�
        playerController._gunFireTop.InitCharacterStates(playerController);
        weapon.transform.SetParent(boat.GetComponent<PlayerController>()._weaponPosPoint.transform);
        weapon.transform.localPosition = new Vector3(0, 0, 0);
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = new Vector3(1, 1, 1);
        if (num != 0) { player.SetNumber(num); player._PlayerController.GetSupporter = true; player._PlayerController.GetMainPlayer = _playerController; }
        player.Init(boatData, weaponData, playerinfo, defenseData, captainData, sailorData, engineData);
        _listPlayers.Add(player);

        if (num == 0) { _testGameData._boatDamage = _playerStat.damage.ToString(); }
    }

    public void GetEnemyRemove(EnemyController enemy)
    {
        _enemysCtrl.Remove(enemy);
        _enemyCount--;
        _maxUI--;
        InGameUIManager.GetInstance.UpdateEnemyCount(_maxUI.ToString());
        _killCount++;
        if (_maxUI == 0)
        {
            SaveData();
        }
    }

    private void Update()
    {
        if (_stopwatch.IsRunning)
        {
            UpdateStopwatchText();
        }
        if (_spawn)
        {
            _spawnEnemyTime += Time.deltaTime;
            if (_spawnEnemyTime > 3f)
            {
                _spawnEnemyTime = 0f;
                _spawn = false;
                for (int i = 0; i < _enemysCtrl.Count; ++i)
                {
                    _enemysCtrl[i].OrderAttack();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Pause();
        }
    }
    

    public void Pause()
    {
        Time.timeScale = 1 == Time.timeScale ? 0 : 1;
        if (Time.timeScale == 0) { StopStopwatch(); }
        else { StartStopwatch(); }
    }

    public void ResetBtn()
    {
        _spawn = false;
        _spawnEnemyTime = 0f;
        _killCount = 0;
        _enemyCount = 0;
        _maxUI = 0;
        ResetStopwatch();
        for (int i = 0; i < _enemysCtrl.Count; ++i)
        {
            Destroy(_enemysCtrl[i].gameObject);
        }
        _enemysCtrl.Clear();
        for (int i = 0; i < _listPlayers.Count; ++i)
        {
            Destroy(_listPlayers[i].GetStats()._hpBar);
            Destroy(_listPlayers[i].gameObject);
        }
        _listPlayers.Clear();
        for (int i = 0; i < _bossCtrl.Count; ++i)
        {
            Destroy(_bossCtrl[i].gameObject);
        }
        _bossCtrl.Clear();
        _playerController = null;
        _playerStat = null;
        for (int i = 0; i < _playerInfoArr.Length; ++i)
        {
            if (_playerInfoArr[i] != null) { _playerInfoArr[i] = null; }
        }
        InGameUIManager.GetInstance._bossHpObj.SetActive(false);
    }

    public void SpawnEnemyGo()
    {
        _spawn = true;
    }

    public void CreateEnemy(int id)
    {
        //����
        var enemyData = DataManager.GetInstance.FindData(DataManager.KEY_ENEMY, id) as EnemyData;
        var enemyGo = SimplePool.Spawn(CommonStaticDatas.RES_ENEMY, enemyData.resName);
        var enemyCtrl = enemyGo.GetComponent<EnemyController>();
        if (enemyGo.GetComponent<Enemy>()._unit != UNIT.Boss) { _enemysCtrl.Add(enemyCtrl); }
        else { _bossCtrl.Add(enemyCtrl); }
        _enemyCount++;

        var lookVector = new Vector3(UnityEngine.Random.Range(-1.00f, 1.00f), 1, UnityEngine.Random.Range(-1.00f, 1.00f));
        if (lookVector == Vector3.zero)
            lookVector = Vector3.forward;
        Quaternion lookRot = Quaternion.LookRotation(lookVector);

        Enemy enemy = enemyGo.GetComponent<Enemy>();
        enemyGo.transform.rotation = lookRot;
        if (enemy._unit != UNIT.Boss) { _totalCount++; }
        if (enemy._unit != UNIT.Boss) { enemyCtrl.GetMovement.AgentWarp(GetRandomPos()); }
        else { enemyCtrl.GetMovement.AgentWarp(GetBossPos()); _listPlayers[0].GetComponent<Player>()._playerAbility.AbilityBombardmentSetting(enemy._enemyController); }

        enemyCtrl.GetMovement.AgentEnabled(true);
        enemy.Init(enemyData);
    }
    public Vector3 GetRandomPos()
    {
        int randomAppearPointIdx = StandardFuncData.RandomRange(0, appearPoint.Length - 1);

        return GetRandomPos(appearPoint[randomAppearPointIdx].position, appearPoint[randomAppearPointIdx].localScale);
    }

    Vector3 GetRandomPos(Vector3 mainPos, Vector3 scale, float offset = 0.5f)
    {
        var offsetPos = mainPos;
        var xHalf = scale.x * offset;
        var zHalf = scale.z * offset;
        var x = offsetPos.x + UnityEngine.Random.Range(-xHalf, xHalf);
        var z = offsetPos.z + UnityEngine.Random.Range(-zHalf, zHalf);
        return new Vector3(x, 0.0f, z);
    }
    // ���� ��ȯ ��ġ �� ���� �ؾ���
    public Vector3 GetBossPos()
    {
        CharacterController pc = _listPlayers[0].GetController();
        Vector3 temp;
        if (pc.transform.position.z >= 40) { temp = new Vector3(pc.transform.position.x, 0, pc.transform.position.z - 10); } // �ؿ� ��ȯ
        else { temp = new Vector3(pc.transform.position.x, 0, pc.transform.position.z + 10); } // ���� ��ȯ

        return temp;
    }

    Vector3 _playerScaleOffset = new Vector3(30f, 0f, 30f);
    private void EliteEnemyCreate(int id, Transform Player)
    {
        //����
        var enemyData = DataManager.GetInstance.FindData(DataManager.KEY_ENEMY, id) as EnemyData;
        var enemyGo = SimplePool.Spawn(CommonStaticDatas.RES_ENEMY, enemyData.resName);
        var enemyCtrl = enemyGo.GetComponent<EnemyController>();
        if (enemyGo.GetComponent<Enemy>()._unit != UNIT.Boss) { _enemysCtrl.Add(enemyCtrl); }
        _enemyCount++;
        //_totalCount++; // ������ ��ȯ�̱⶧���� �Ϲ������� ������ ������ ����

        // ����Ʈ ����
        GameObject effect = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "FX_Bounce_01");
        effect.transform.position = enemyGo.transform.position;

        var lookVector = new Vector3(UnityEngine.Random.Range(-1.00f, 1.00f), 1, UnityEngine.Random.Range(-1.00f, 1.00f));
        if (lookVector == Vector3.zero)
            lookVector = Vector3.forward;
        Quaternion lookRot = Quaternion.LookRotation(lookVector);

        Enemy enemy = enemyGo.GetComponent<Enemy>();
        enemyGo.transform.rotation = lookRot;
        Vector3 pos = GetRandomPos(Player.transform.position, _playerScaleOffset);
        enemyCtrl.GetMovement.AgentWarp(pos);

        enemyCtrl.GetMovement.AgentEnabled(true);
        enemy.Init(enemyData);
        enemyCtrl.OrderAttack();
    }
    public void BossType2CreateEnemy(int id, Transform player)
    {
        EliteEnemyCreate(id, player);
        _maxUI++;
        InGameUIManager.GetInstance.UpdateEnemyCount(_maxUI.ToString());
    }

    public void PlayerAbilitySelect(int num)
    {
        if (_listPlayers.Count <= 0) { return; }
        _listPlayers[0].GetComponent<Player>()._playerAbility.TestAbilitySelectAll();
        _listPlayers[0].GetComponent<Player>()._playerAbility.AbilityBtnSelect(num);
        _testGameData._abilityCheck = StringIsNullOrEmptyReturn(_testGameData._abilityCheck, num);
        //_listPlayers[0].AddAbility()
    }

    public string StringIsNullOrEmptyReturn(string value, int number)
    {
        string str = value;
        if (string.IsNullOrEmpty(str))
        {
            str = number.ToString();
        }
        else
        {
            str += "/" + number.ToString();
        }
        return str;
    }

    public void SaveData(bool die = false)
    {
        _stopwatch.StopStopwatch();
        if (die) { _testGameData._time = _stopwatch.StopwatchTextReturn() + "/Die"; }
        else { _testGameData._time = _stopwatch.StopwatchTextReturn(); }
        _dataManager.AddGameData(_testGameData);
        _testGameData = new TestGameData();
    }

    public List<Interactable> GetPlayerList() => _listPlayers;
    public void StartStopwatch() => _stopwatch.StartStopwatch();
    public void StopStopwatch() => _stopwatch.StopStopwatch();
    public void ResetStopwatch() => _stopwatch.ResetStopwatch(_timeText);
    public void UpdateStopwatchText() => _stopwatch.UpdateStopwatchText(_timeText);
    public void ExportFile() => _dataManager.ExportFile();
    public bool GetSpawn() => _spawn;
}
