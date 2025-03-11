using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyData;
using System.Xml;
using MyStructData;

public class ChapterWave : Chapter
{
    private enum EnemyType
    {
        NORMAL,
        SPAWN,
        BOSS,
        UNIQUE,
    }

    [Header("Components")]
    StageSetData _stageData;
    InGameUIManager _uiManager;
    StageManager _stageManager;
    List<GameObject> _abilityBoxList;

    [Header("Spawn Point")]
    [SerializeField] Transform[] appearPoint = null;

    [Header("Unique")]
    UniqueTypeController _unique;
    bool _spawnUnique = false;      // 유니크 생성 여부
    int _percentageUnique = 30;      // 유니크 등장 확률

    // 플레이에 필요한 기본 규칙이 실행되는 WaveUpdate실행되는 딜레이 타임
    private float chapterPlayCoDelay = 1.0f; 

    [SerializeField] string _stageBGM;

    bool _isSelectAbility = false;

    Vector3 _playerScaleOffset = new Vector3(30f, 0f, 30f);

    [Header("Correction")]          // 보정 시간(몬스터들의 비정상적 작동 체크)
    float _correctionTime = 0f;
    const float _correctionMaxTime = 30f;

    [SerializeField] float[] _settingDistance = new float[] { 10, 20 };

    [Header("AbilityBox")]
    float _cycleTmpTime = 0f;
    float _cycleTime = 0f;
    int _boxCount = 0;
    int _boxMaxCount = 6;

    public override void Init(StageSetData stageData)
    {
        _uiManager = InGameUIManager.GetInstance;
        _stageManager = StageManager.GetInstance;

        _isEnd = false;
        _stageData = stageData;
        _cycleTime = (float)_stageData._limitTime.TotalSeconds / 8;
        //Debug.Log("_stageData._limitTime.TotalSeconds : " + _cycleTime);
        StartCoroutine(WaveUpdate());
    }

    /// <summary>
    /// 필요한 변수 초기화
    /// </summary>
    void ResetValues()
    {
        _correctionTime = 0;
        _enemyCount = 0;
        _totalCount = 0;
    }

    #region Update
    private void Update()
    {
        _correctionTime += Time.deltaTime;
        if (_correctionTime > _correctionMaxTime)
        {
            _correctionTime = 0f;
            if (enemyControllerList.Count != 0)
            {
                for (int i = 0; i < enemyControllerList.Count; ++i)
                {
                    // 안죽었는데 콜라이더가 꺼져있는 상태 체크
                    if (!enemyControllerList[i].IsDie() && !enemyControllerList[i].GetCollider().enabled)
                    {
                        enemyControllerList[i].SetState(eCharacterStates.Idle);
                        enemyControllerList[i].GetCollider().enabled = true;
                    }
                }
            }
        }

        // 어빌리티 상자
        _cycleTmpTime += Time.deltaTime;
        if(_cycleTmpTime > _cycleTime)
        {
            _cycleTmpTime = 0;
            if(_boxCount < _boxMaxCount)
            {
                _boxCount++;
                DropAbilityBox();
            }
        }
    }

    /// <summary>
    /// 웨이브 업데이트 메서드
    /// </summary>
    IEnumerator WaveUpdate()
    {
        //DropAbilityBox();
        ResetValues();

        int[] enemyCount = new int[6];
        for (int i = 0; i < enemyCount.Length; ++i)
        {
            enemyCount[i] = _stageData.GetEnemyCount(i);
            _maxCount += enemyCount[i];
        }
        int allEnemy = _maxCount;
        //Debug.Log($"allEnemy: {allEnemy}");

        _uiManager.UpdateEnemyCount(_maxCount.ToString());

        yield return null;

        enemyControllerList.Clear();
        bool isFinish = false;
        float flowTime = 0;
        bool allSpawn = false;

        CheckUniqueEnemy();

        //챕터에 등장한 모든 몹을 잡으면 다음 웨이브로 가고 웨이브 마지막까지 오면 승리
        while (!isFinish)
        {
            CheckBossCreate();
            if (!allSpawn && _enemyCount < _stageManager.MaxEnemyTotle)
            {
                if (allEnemy - _totalCount <= 0)
                {
                    allSpawn = true;
                }
                else
                {
                    int[] enemyMakeCounts = new int[6];
                    for (int i = 0; i < enemyMakeCounts.Length; i++)
                    {
                        enemyMakeCounts[i] = Mathf.Clamp(enemyCount[i], 0, 3);
                    }

                    for (int j = 0; j < enemyMakeCounts.Length; j++)
                    {
                        enemyCount[j] -= enemyMakeCounts[j];
                        for (int k = 0; k < enemyMakeCounts[j]; k++)
                        {
                            CreateEnemy(EnemyType.NORMAL, _stageData.GetWaveEnemyID(_stageManager.NowWave, j), CommonStaticDatas.RES_ENEMY , string.Empty);
                        }
                    }
                    yield return YieldInstructionCache.WaitForSeconds(1f);
                }
            }
            // 공격 보내기
            flowTime += chapterPlayCoDelay;
            if (10f < flowTime)
            {
                OrderAttackToEnemy();
                flowTime = 0;
            }

            // 현재 챕터 클리어 체크
            if (IsClearGame)
            {
                yield return YieldInstructionCache.WaitForSeconds(2f); // 혹시 몬스터와 동시에 죽을수도 있어서 죽었을때를 대비 1초 휴식
                isFinish = true;
                if (StageManager.GetInstance.PlayerStat.hp > 0)
                {
                    CheckNextPhase();
                }
            }
            yield return YieldInstructionCache.WaitForSeconds(0.5f);
        }
    }
    #endregion Update

    #region BGM    
    /// <summary>
    /// BGM 설정
    /// </summary>
    /// <param name="bgmName">BGM 이름</param>
    public void SetStageBgm(string bgmName)
    {
        if (!AudioController.IsPlaying(bgmName))
        {
            SoundManager.GetInstance.PlayAudioBackgroundSound(bgmName, CameraManager.GetInstance.transform);
        }
    }

    /// <summary>
    /// 현재 스테이지의 BGM으로 설정
    /// </summary>
    public void SetCurStageBgm()
    {
        if (_stageBGM == "" || _stageBGM == null)
        {
            _stageBGM = "01_InGame";
        }
        SetStageBgm(_stageBGM);
    }
    #endregion BGM

    #region Drop
    /// <summary>
    /// 어빌리티 상자 생성
    /// </summary>
    void AbilityBoxCreate()
    {
        //InGameUIManager.GetInstance.ArrowDestroy();
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_PLAYERABILITY, "AbilityBox");
        obj.GetComponent<AbilityBoxScript>().Init();
        obj.transform.position = GetRandomPlayerDisPos();
        InGameUIManager.GetInstance.TargetArrowPosShowCreate(obj.transform);
        _abilityBoxList.Add(obj);
    }

    /// <summary>
    /// 어빌리티 상자 드랍
    /// </summary>
    void DropAbilityBox()
    {
        if (_abilityBoxList == null)
        {
            _abilityBoxList = new List<GameObject>();
        }
/*        else if (_abilityBoxList.Count != 0)
        {
            for (int i = 0; i < _abilityBoxList.Count; ++i)
            {
                Destroy(_abilityBoxList[i]);
            }
            _abilityBoxList.Clear();
        }*/
        AbilityBoxCreate();
    }
    #endregion Drop

    #region Unique Enemy
    /// <summary>
    /// 유니크(도감)몬스터 생성
    /// </summary>
    void CheckUniqueEnemy()
    {
        if (!_spawnUnique)
        {
            if (!_stageManager.IsTutorial)
            {
                _spawnUnique = true;
                StartCoroutine(DelayCreateUnique());
                //int rand = Random.Range(0, 100);
                ////int rand = 4;
                //if (rand < _percentageUnique)
                //{
                //    _spawnUnique = true;
                //    StartCoroutine(DelayCreateUnique());
                //}
            }
        }
    }

    /// <summary>
    /// 소환할 유니크(도감)몬스터 이름 반환
    /// </summary>
    /// <returns></returns>
    string GetSpawnUniqueName(int id)
    {
        // 튜토리얼에는 강제 1번소환
        //if (UserData.GetInstance.TutorialCheck() == 9) { return "Q01"; }
        string nameFormat = string.Format("Q{0:00}", id);
        return nameFormat;
    }

    /// <summary>
    /// 유니크 생성 코루틴
    /// </summary>
    IEnumerator DelayCreateUnique()
    {
        _uiManager.SetEnemyAppearText("유니크 등장", 4f);
        yield return YieldInstructionCache.WaitForSeconds(2f);
        int id = Random.Range(1, 22);
        if (UserData.GetInstance.TutorialCheck() == 9) { id = 1; }
        CreateEnemy(EnemyType.UNIQUE, id + 20000, CommonStaticDatas.RES_UNIQUE, GetSpawnUniqueName(id));
    }
    #endregion Unique Enemy

    #region Enemy
    /// <summary>
    /// 몬스터 생성 메서드(공통)
    /// </summary>
    /// <param name="type">몬스터 타입</param>
    /// <param name="id">ID</param>
    /// <param name="path">프리팹 경로</param>
    /// <param name="name">이름(선택사항)</param>
    Enemy CreateEnemy(EnemyType type, int id, string path, string name, Transform center = null)
    {
        var enemyData = DataManager.GetInstance.FindData(DataManager.KEY_ENEMY, id) as EnemyData;
        GameObject enemyObject = null;
        if (name.Length <= 0)
        {
            enemyObject = SimplePool.Spawn(path, enemyData.resName);
        }
        else
        {
            enemyObject = SimplePool.Spawn(path, name);
        }

        var lookVector = new Vector3(UnityEngine.Random.Range(-1.00f, 1.00f), 1, UnityEngine.Random.Range(-1.00f, 1.00f));
        if (lookVector == Vector3.zero)
            lookVector = Vector3.forward;
        Quaternion lookRot = Quaternion.LookRotation(lookVector);
        enemyObject.transform.rotation = lookRot;

        var enemyController = enemyObject.GetComponent<EnemyController>();
        Vector3 warpPos = Vector3.zero;
        _enemyCount++;
        switch (type)
        {
            case EnemyType.NORMAL:
                {
                    warpPos = GetRandomPlayerDisPos();
                    enemyControllerList.Add(enemyController);
                    _totalCount++;
                }
                break;
            case EnemyType.SPAWN:
                {
                    warpPos = GetRandomPos(center.position, _playerScaleOffset);
                    enemyControllerList.Add(enemyController);
                    //GameObject effect = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, "FX_Bounce_01");
                    //effect.transform.position = enemyObject.transform.position;
                }
                break;
            case EnemyType.BOSS:
                {
                    warpPos = GetBossPos();
                    _stageManager.GetPlayers()[0].GetComponent<Player>()._playerAbility.AbilityBombardmentSetting(enemyController);
                    _uiManager.SetEnemyAppearText("보스 등장", 4f);
                }
                break;
            case EnemyType.UNIQUE:
                {
                    warpPos = GetRandomPos();
                    _unique = enemyController as UniqueTypeController;
                    _enemyCount--;
                }
                break;
        }

        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemyController.GetMovement.AgentWarp(warpPos);
        enemyController.GetMovement.AgentEnabled(true);
        enemy.Init(enemyData);
        return enemy;
    }

    public void CreateForcedSpawnEnemy(int id, Transform player, EnemyStats parent, float value = 2)
    {
        Enemy enemy = CreateEnemy(EnemyType.SPAWN, id, CommonStaticDatas.RES_ENEMY, string.Empty, player);
        UnitDamageData tdd = enemy._enemyStats.GetTDD1();
        tdd.damage = parent.damage;
        enemy._enemyStats.damage = parent.damage;
        enemy._enemyStats.InitUnitDamageData1(tdd);
        enemy._enemyStats.maxHp = StandardFuncUnit.OperatorValue(parent.maxHp, value, OperatorCategory.persent);
        enemy._enemyStats.hp = enemy._enemyStats.maxHp;
        _maxCount++;
        _uiManager.UpdateEnemyCount(_maxCount.ToString());
    }
    #endregion Enemy

    #region Boss    
    /// <summary>
    /// 보스 소환 체크
    /// </summary>
    public void CheckBossCreate()
    {
        // 튜토리얼에서는 보스 생성 X
        if (_stageManager.IsTutorial)
        {
            return;
        }

        if (_stageManager.BossMax != 0)
        {
            if (_stageManager.NowWave == _stageData.MaxWave)
            {
                // 전체 몬스터 수의 %만큼이 되었을때 보스 출현
                // 획득한 재료가 없으면 (20% 정도도 못얻는 웨이브면 재료 없음 뜨게하기)
                int index = (int)StandardFuncUnit.OperatorValue(_stageData.GetEnemyMaxCount(), _maxCount, OperatorCategory.partial);
                if (index > 80) 
                {
                    // 현재는 80% 이상일때는 소환 불가
                    return; 
                } 
                else
                {
                    CreateBoss();
                }
            }
        }
    }

    /// <summary>
    /// 보스 생성
    /// </summary>
    void CreateBoss()
    {
        CreateEnemy(EnemyType.BOSS, _stageData._bossId[_stageManager.BossCount], CommonStaticDatas.RES_ENEMY, string.Empty);
        _stageManager.BossMax--;
        _stageManager.BossCount++;
        _maxCount++;
        _uiManager.UpdateEnemyCount(_maxCount.ToString());
    }
    #endregion Boss

    #region Spawn
    public Vector3 GetRandomPlayerDisPos()
    {
        for (int i = 0; i < 5; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            float radian = randomAngle * Mathf.Deg2Rad;

            float randomDistance = Random.Range(_settingDistance[0], _settingDistance[1]);

            Vector3 randomDirection = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian));
            Vector3 randomPosition = _stageManager.GetPlayers()[0].transform.position + randomDirection * randomDistance;

            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(randomPosition, out hit, 2.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.Log("유효한 NavMesh 위치를 찾지 못함");
        float fallbackDistance = Random.Range(2f, 3f);
        float fallbackAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 fallbackDirection = new Vector3(Mathf.Cos(fallbackAngle), 0, Mathf.Sin(fallbackAngle));
        Vector3 fallbackPosition = _stageManager.GetPlayers()[0].transform.position + fallbackDirection * fallbackDistance;

        // Y값 고정
        fallbackPosition.y = _stageManager.GetPlayers()[0].transform.position.y;
        return fallbackPosition;
    }

    public Vector3 GetRandomPos()
    {
        int randomAppearPointIdx = StandardFuncData.RandomRange(0, appearPoint.Length - 1);
        return GetRandomPos(appearPoint[randomAppearPointIdx].position, appearPoint[randomAppearPointIdx].localScale);
    }

    public Vector3 GetRandomPos(Vector3 mainPos, Vector3 scale, float offset = 0.5f)
    {
        var offsetPos = mainPos;
        var xHalf = scale.x * offset;
        var zHalf = scale.z * offset;
        var x = offsetPos.x + UnityEngine.Random.Range(-xHalf, xHalf);
        var z = offsetPos.z + UnityEngine.Random.Range(-zHalf, zHalf);
        return new Vector3(x, 0.0f, z);
    }

    public Vector3 GetBossPos()
    {
        List<CharacterController> pc = _stageManager.GetPlayersController();
        Vector3 temp;
        if (pc[0].transform.position.z >= 40) { temp = new Vector3(pc[0].transform.position.x, 0, pc[0].transform.position.z - 10); } // 밑에 소환
        else { temp = new Vector3(pc[0].transform.position.x, 0, pc[0].transform.position.z + 10); } // 위에 소환

        return temp;
    }
    #endregion Spawn

    /// <summary>
    /// 다음 페이즈로 진행하는 메서드
    /// </summary>
    public void CheckNextPhase()
    {
        StartCoroutine(SetNextPhase());
    }

    IEnumerator SetNextPhase()
    {
        _stageManager.CountUpWave();
        if (_stageManager.NowWave > _stageData.MaxWave)
        {
            // 승리 UI 버튼 클릭 로비 나가기
            _stageManager.EndStageMaterialShow(false);
        }
        else
        {
            _isSelectAbility = true;
            _uiManager.RandomAbilityFuntion();
            while (_isSelectAbility)
            {
                yield return null;
            }
            // 다음 전투로 넘어가기
            /*                    if (_unique != null)
                                {
                                    _unique.gameObject.SetActive(false);
                                }*/
            _stageManager._fade.FadeOut();

            yield return YieldInstructionCache.WaitForSeconds(0.9f);

            if (_stageManager.NowWave == _stageData.MaxWave)
            {
                _uiManager.StageWaveSetting("Final");
            }
            else
            {
                _uiManager.StageWaveSetting(_stageManager.NowWave.ToString());
            }
            // 로딩 배경 페이드 인 아웃
            _stageData.AddEnemyCount(_stageManager.NowWave);
            _stageManager.NextStage();
        }
    }

    private void OrderAttackToEnemy()
    {
        for (int i = 0; i < enemyControllerList.Count; i++)
        {
            enemyControllerList[i].OrderAttack();
        }
    }

    #region Setter
    public void SetSelectAbilityState(bool state) => _isSelectAbility = state;
    #endregion Setter

    #region Getter
    public bool IsSelectAbility => _isSelectAbility;
    #endregion Getter
}

public class EnemyCount
{
    public int enemyId;
    public int count;
    public int appearCount;
    public bool isBossOrElite = false;
    public EnemyCount(int v1, int v2, bool isBossOrElite)
    {
        enemyId = v1;
        count = v2;
        appearCount = v2;
        this.isBossOrElite = isBossOrElite;
    }

    public int GetId()
    {
        SubAppearCount();
        return enemyId;
    }

    public void SubAppearCount() => appearCount = Mathf.Clamp(appearCount - 1, 0, appearCount);
}