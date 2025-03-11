using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;
using TMPro;
using System;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;
// 여기서 데이터 관리
// 각 씬에 맞는 저장된 데이터를 불러와서 좀비 혹은 보스 소환

public class StageManager : MonoBehaviour
{
    public ChapterWave[] _chapter; // 배열로 잡아서 실행해야할 듯
    public StageSetData _stageData;
    private PlayerStats _playerStat = null;
    private PlayerController _playerController = null;
    List<Interactable> _listPlayers; // 모든 플레이어 정보
    List<CharacterController> _listController; // 모든 플레이어 컨트롤러
    public static StageManager GetInstance;
    public int _stageNumber; // 몇 스테이지인가?

    /// <summary>
    /// ChapterCount 챕터 카운터 줄임말
    /// </summary>
    public int _cc;

    /// <summary>
    /// 현재 웨이브
    /// </summary>
    public int _nowWave = 1;

    /// <summary>
    /// 토탈 좀비 카운터
    /// </summary>
    int _maxEnemyTotle = 120;
    int _eliteCount = 0, _bossCount = 0, _bossMax; // 엘리트와 보스 횟수 카운터

    /// <summary>
    /// 페이드 인 아웃 (인게임용)
    /// </summary>
    public FadeDimmer _fade;

    public GameObject _hpCanvas;

    [Header("현재 설정된 스테이지 재료")]
    public int[] _material;

    public GameObject _showMaterialObj;
    public GameObject[] _uiMaterialPrefabs;
    Vector3 _materialStartPoint = new Vector3(-150, 150, 0);
    InGameUIManager _inGameUI;

    bool _endGame = false;

    const string FireBaseClearFormat = "Stage_Clear_{0}";
    const string FireBaseFailFormat = "Stage_Fail_{0}_{1}";
    const string FireBaseRetireFormat = "Stage_Retire_{0}_{1}";

    TimeSpan _playTime = new TimeSpan();
    bool _isTimeOver = false;
    public TimeSpan GetPlayTime => _playTime;

    // 장비 어빌리티, 추가능력치
    List<int> _addedAbility = new List<int>();

    // 재화와 재료 배율 적용
    public float[] _ingredientRatio = { 1, 1 };

    private void Awake()
    {
        GetInstance = this;
        _listPlayers = new List<Interactable>();
        _listController = new List<CharacterController>();
        _material = new int[(int)EInvenType.Evolution];
        _stageNumber = GameManager.GetInstance.GetNowStageNumber;
        _inGameUI = InGameUIManager.GetInstance;
        GameManager.GetInstance._nowScene = EScene.E_GAME;
    }

    private void OnEnable()
    {
        if (_inGameUI == null) { _inGameUI = GameObject.Find("InGameCanvas").GetComponent<InGameUIManager>(); }
        // 랜덤하게 맵 뽑고
        //NowChapterWaveRandomSelect();
        // 첫 맵 무조건 나오게
        _cc = 0;

        _inGameUI.InitPopup();

        // 보트 소환하고
        CreateBoat();
        if (UserData.GetInstance.TutorialCheck() != 2 && UserData.GetInstance.TutorialCheck() != 6)
        {
            var stage = DataManager.GetInstance.FindData(DataManager.KEY_STAGE, GameManager.GetInstance.GetNowStageNumber + 1) as StageData;
            _stageData.Init(stage);
        }
        else { _stageData.Init(); }

        _inGameUI.Init();

        // 이 맵의 브금틀고
        _chapter[_cc].SetCurStageBgm();
        // 전체 보스 설정
        _bossMax = _stageData.BossCountMax;
        CameraManager.GetInstance.InitPos(_chapter[_cc]._cameraParent);
        // 처음 딱 게임 시작 시 한번 들어오는 곳
        _chapter[_cc].Init(_stageData);
        PlayerPositionResetting();

        // 어빌리티 추가 구간
        UserData ud = UserData.GetInstance;
        if (ud.IsAdAbilityReserved)
        {
            _inGameUI.SetRemainAblityCount(1);
        }
        else if (ud.IsDiaAbilityReserved)
        {
            _inGameUI.SetRemainAblityCount(2);
        }
        _inGameUI.RandomAbilityFuntion();

        ud.SetAdAbilityReserved(false);
        ud.SetDiaAbilityReserved(false);

        // 재료 설정
        StageMaterialSetting();

        if(IsTutorial)
        {
            _inGameUI.StageNameSetting("Tutorial");
        }
        else
        {
            _inGameUI.StageNameSetting((_stageNumber + 1).ToString());
        }
        if (_stageNumber == 99) { _inGameUI.StageWaveSetting("Final"); } //Final
        else { _inGameUI.StageWaveSetting(_nowWave.ToString()); }

        CheckPlayTime();
    }

    public void CheckPlayTime()
    {
        _playTime = _stageData._limitTime;
        StartCoroutine(CountDownSecond());
    }

    IEnumerator CountDownSecond()
    {
        while(true)
        {
            yield return YieldInstructionCache.WaitForSeconds(1f);
            if(InGameUIManager.GetInstance.GetPopup<RevivalPopup>(PopupType.REVIVAL).IsOpened)
            {
                continue;
            }
            _playTime = _playTime.Subtract(new TimeSpan(0, 0, 1));
            if(_playTime.Minutes <= 0 && _playTime.Seconds < 0) // 시간이 다 되면
            {
                _isTimeOver = true;
                EndStageMaterialShow(true);
                break;
            }
            else if(_endGame)
            {
                break;
            }
        }
    }

    public void NextStage()
    {
        /* 잠시 맵을 안바꾸기로 하신건지 모르겠지만 맵을 하나만 사용 (21.7.4)
        StartCoroutine(DelayChapter(_cc));
        NowChapterWaveRandomSelect(); // 맵 바꾸고
        _chapter[_cc]._mapObj.SetActive(true);
        */
        _chapter[_cc].Init(_stageData); // 그 맵 셋팅하고hd uncensored
        PlayerPositionResetting();
        _fade.FadeIn();
    }
    IEnumerator DelayChapter(int chapter)
    {
        yield return YieldInstructionCache.WaitForSeconds(2f);
        _chapter[chapter]._mapObj.SetActive(false);
    }

    // 시작 자리 정리를 위해서
    void PlayerPositionResetting()
    {
        ListPlayerActive(false); // 잠시 유닛 끄고
        // 자리 정리
        _listPlayers[0].transform.position = _chapter[_cc]._startPos.position;
        for (int i = 1; i < _listPlayers.Count; ++i)
        {
            _listPlayers[i].transform.position = _chapter[_cc]._colleagueStartPos[i - 1].position;
        }
        CameraManager.GetInstance.ResetPos(_chapter[_cc]._cameraParent);
        ListPlayerActive(true);
    }

    public void ListPlayerActive(bool on)
    {
        for (int i = 0; i < _listController.Count; ++i)
        {
            if (_listController[i]._curState != eCharacterStates.Die) // 죽지 않은 자만 돌아다니게
            {
                _listController[i].ColliderEnalbed(on);
            }
        }
    }

    public void CreateBoat()
    {
        UserData ud = UserData.GetInstance;
        // 선택한 보트 소환
        for (int i = 0; i < ud.UnitInfo.Count; ++i)
        {
            if (ud.UnitInfo[i] == null) { continue; }

            Dictionary<EItemList, int> dataIdDic = new Dictionary<EItemList, int>();
            Info.PlayerInfo playerinfo = ud.UnitInfo[i];
            Player player = null;
            PlayerController playerController = null;
            GameObject boat = null;

            WeaponData weaponData = null;
            GameObject weapon = null;

            int[] playerInfoArr = playerinfo.GetPlayerInfoArr;
            for (int j = (int)EItemList.BOAT; j <= (int)EItemList.ENGINE; j++)
            {
                int id = playerInfoArr[j * 2];
                switch ((EItemList)j)
                {
                    case EItemList.BOAT:
                        {
                            boat = SimplePool.Spawn(CommonStaticDatas.RES_BOAT,
                                UnitDataManager.GetInstance.GetStringValue(EItemList.BOAT, id, StatusType.resName),
                                _chapter[_cc]._startPos.position, Quaternion.identity);
                            player = boat.GetComponent<Player>();
                            playerController = boat.GetComponent<PlayerController>();
                        }
                        break;
                    case EItemList.WEAPON:
                        {
                            if (id == 0)
                            {
                                playerinfo.SetPlayerValue(ECoalescenceType.WEAPON_ID, UnitDataManager.GetInstance.GetIntValue(EItemList.BOAT, playerInfoArr[(int)EItemList.BOAT], StatusType.basicWeaponType));
                                playerinfo.SetPlayerValue(ECoalescenceType.WEAPON_LEVEL, 1);
                                id = playerinfo.GetPlayerValue(ECoalescenceType.WEAPON_ID);
                            }
                            weaponData = DataManager.GetInstance.GetData(DataManager.KEY_WEAPON, id, 1) as WeaponData;
                            weapon = SimplePool.Spawn(CommonStaticDatas.RES_WEAPON, weaponData.resName, new Vector3(0, 0, 0), Quaternion.identity);
                        }
                        break;
                    case EItemList.DEFENSE:
                        {
                            if (id == 0) continue;

                            string resName = UnitDataManager.GetInstance.GetStringValue(EItemList.DEFENSE, id, StatusType.resName);
                            int defensePoint = UnitDataManager.GetInstance.GetIntValue(EItemList.DEFENSE, id, StatusType.defensePoint);
                            GameObject defense = SimplePool.Spawn(CommonStaticDatas.RES_DEFENSE, resName, new Vector3(0, 0, 0), Quaternion.identity);
                            if (defense != null)
                            {
                                boat.GetComponent<PlayerController>()._defensePoint.GetPointSetting((EDefensePoint)defensePoint, CommonStaticDatas.RES_DEFENSE, resName, defense);
                            }
                        }
                        break;
                }

                if (id != 0)
                {
                    dataIdDic.Add((EItemList)j, id);
                }
            }

            if (i == 0)
            {
                // 메인 보트
                _playerController = playerController;
                _playerController._gunFireTop = weapon.GetComponent<GunFireTop>();
                _playerStat = boat.GetComponent<PlayerStats>();
                _playerController._gunFireTop.GetWeaponData = weaponData;
                _playerController.GetMovement.avoidancePriority--;
                _playerController._main = true;
            }
            else
            {
                // 서브 보트
                playerController._gunFireTop = weapon.GetComponent<GunFireTop>();
                playerController._gunFireTop.GetWeaponData = weaponData;

                player.SetNumber(i);
                player._PlayerController.GetSupporter = true;
                player._PlayerController.GetMainPlayer = _playerController;
            }

            playerController._gunFireTop.InitCharacterStates(playerController);
            weapon.transform.SetParent(boat.GetComponent<PlayerController>()._weaponPosPoint.transform);
            weapon.transform.localPosition = new Vector3(0, 0, 0);
            weapon.transform.localRotation = Quaternion.identity;
            weapon.transform.localScale = new Vector3(1, 1, 1);

            player.Init(playerinfo, dataIdDic);
            _listPlayers.Add(player);
            _listController.Add(player.GetComponent<CharacterController>());
        }
        _playerStat.ApplyPotenAbility();
    }

    public List<Interactable> GetPlayers()
    {
        return _listPlayers;
    }
    public List<CharacterController> GetPlayersController()
    {
        return _listController;
    }

    void NowChapterWaveRandomSelect()
    {
        bool ok = false;
        while (!ok)
        {
            int rand = UnityEngine.Random.Range(0, _chapter.Length);
            if (_cc != rand)
            {
                _cc = rand;
                ok = true;
            }
        }
    }

    /// <summary>
    /// 스테이지 재료 설정
    /// </summary>
    void StageMaterialSetting()
    {
        float material = 30 * Mathf.Pow(1.056f, _stageNumber);
        int materialMax = (int)material;

        _material[0] = _stageData._materialSetting[0];

        List<int> availableIndices = new List<int> { 2, 3, 4, 5, 6, 7 };

        while (materialMax > 0)
        {
            if (availableIndices.Count == 0)
            {
                availableIndices = new List<int> { 2, 3, 4, 5, 6, 7 };
            }
            int randomIndex = UnityEngine.Random.Range(0, availableIndices.Count);
            int randomValue = UnityEngine.Random.Range(0, materialMax + 1);
            int chosenIndex = availableIndices[randomIndex];
            _material[chosenIndex] += randomValue;

            availableIndices.RemoveAt(randomIndex);
            materialMax -= randomValue;
        }
    }

    public void KillAllBoat()
    {
        for(int i = 0; i < _listPlayers.Count; i++)
        {
            Player p = _listPlayers[i] as Player;
            if (p != null && p._playerStats.hp > 0)
            {
                p.TakeToDamage(p._playerStats.hp + 1);
            }
        }
    }

    // 게임이 종료되고 획득한 재료 보여주기
    public void EndStageMaterialShow(bool die)
    {
        UserData ud = UserData.GetInstance;
        _endGame = true;

        if (die)
        {
            // 튜토중에는 죽어도 다른거 못하게 (원래 죽는 씬이라서)
            if (ud.TutorialCheck() == 7)
            {
                _hpCanvas.SetActive(false);
                // 튜토 내용 시작
                TutorialManager.GetInstance.TypingAction();
                return;
            }

            if (!_isTimeOver && _inGameUI.CheckRetry()) 
            { 
                return; 
            }
            _nowWave--;
        }
        else
        {
            if (ud.TutorialCheck() != 3 && ud.TutorialCheck() != 7)
            {
                // 다음 스테이지 열어주기
                if (_stageNumber != 99)
                {
                    var nextLevelData = DataManager.GetInstance.FindData(DataManager.KEY_LEVEL, ud.GetUserLevel + 1) as LevelData;
                    Debug.Log("next Level Data : " + nextLevelData.nId + " : " + nextLevelData.exp);
                    if (GameManager.GetInstance.GetNowStageNumber >= ud.StagePos)
                    {
                        GameManager.GetInstance.GetFirstCleared = true;
                        ud.StagePosSave();
                        if(nextLevelData != null)
                        {
                            ud.ExpUp((int)_stageData._exp, nextLevelData.exp);
                        }
                    }
                    else
                    {
                        GameManager.GetInstance.GetPrevStage = GameManager.GetInstance.GetNowStageNumber;
                        if(nextLevelData != null)
                        {
                            float exp = StandardFuncUnit.OperatorValue(_stageData._exp, 50, OperatorCategory.persent);
                            ud.ExpUp((int)exp, nextLevelData.exp);
                        }
                    }
                }
            }
        }
        Time.timeScale = 0;
        _showMaterialObj.SetActive(true);

        ResultPopup popup = _inGameUI.GetPopup<ResultPopup>(PopupType.RESULT);
        popup.WinLoseImgChange(die);
        // 첫번째 인게임 튜토와 두번째 인게임 튜토
        if (ud.TutorialCheck() == 3 || ud.TutorialCheck() == 7)
        {
            popup.OnCloseEventListener += _inGameUI.LobbyReturnScene;
            popup.OnTouchCheckButton();
            return;
        }

        // 보트들의 hpUI 꺼주기
        _hpCanvas.SetActive(false);
        
        // 획득한 재료가 없으면 (20% 정도도 못얻는 웨이브면 재료 없음 뜨게하기)
        int index = (int)StandardFuncUnit.OperatorValue(_stageData.MaxWave, _nowWave, OperatorCategory.partial);
        if (index < 0) { index = 0; }
        float money = _material[0] * _ingredientRatio[0];
        float[] material = { _material[2], _material[3], _material[4], _material[5], _material[6], _material[7] };
        _material[0] = (int)money;
        for (int i = 0; i < _material.Length; ++i)
        {
            material[i] = material[i] * _ingredientRatio[1];
            _material[i + 2] = (int)material[i];
        }

        int[] saveIndex = new int[(int)EInvenType.Evolution];
        // 웨이브에 따라 재료의 수량 변경
        for (int i = 0; i < _material.Length; ++i)
        {
            if (index == 100) { saveIndex = _material; break; }
            int value = (int)StandardFuncUnit.OperatorValue(_material[i], index, OperatorCategory.persent);
            saveIndex[i] = value;
        }

        // 결과 팝업 
        popup.SetRewardInfo(saveIndex);
        popup.OnCloseEventListener += _inGameUI.LobbyReturnScene;
        popup.OpenPopup();

        // 획득한 것 저장
        int[] myMaterials = MyMaterialFind(ud);

        for (int i = 0; i < myMaterials.Length; ++i)
        {
            myMaterials[i] += saveIndex[i];
        }
        ud.MaterialSave(myMaterials);

        if (die)
        {
            FirebaseManager.GetInstance.LogEvent(string.Format(FireBaseFailFormat, _stageNumber, _cc));
        }
        else
        {
            FirebaseManager.GetInstance.LogEvent(string.Format(FireBaseClearFormat, _stageNumber));
        }

        SaveQuestInfo();
        SaveSeasonPassData();
        if (ud.GetServer) { if (ud.StagePercentCheck()) { ud.ServerSave(); } }
        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// 퀘스트 정보 저장
    /// </summary>
    /// <param name="isRetire">포기 여부</param>
    public void SaveQuestInfo(bool isRetire = false)
    {
        UserData ud = UserData.GetInstance;
        // 퀘스트 카운트 저장
        // 일일퀘 : 킬 카운트
        var dailyQuestData = DataManager.GetInstance.FindData(DataManager.KEY_QUEST, (int)(EDailyQuest.KILLENEMY + 1)) as QuestData;
        var userQuestData = ud.GetDailyQuest();

        if (dailyQuestData.clearCount > userQuestData[(int)EDailyQuest.KILLENEMY])
        {
            int targetCount = dailyQuestData.clearCount - userQuestData[(int)EDailyQuest.KILLENEMY];
            if (targetCount <= _chapter[_cc].KillCount)
            {
                ud.SetDailyQuest(EDailyQuest.KILLENEMY, userQuestData[(int)EDailyQuest.KILLENEMY] + targetCount);
            }
            else
            {
                ud.SetDailyQuest(EDailyQuest.KILLENEMY, userQuestData[(int)EDailyQuest.KILLENEMY] + _chapter[_cc].KillCount);
            }
        }

        // 메인퀘 : 클리어 상태(웨이브)
        int nowWave = _nowWave;
        if (isRetire)
        {
            nowWave -= 1;
        }

        // 1웨이브에서 종료처리되면 기록 X
        if (nowWave <= 0)
        {
            return;
        }

        int stageN = _stageNumber + 1;
        string key01 = stageN.ToString();
        string key02 = string.Format("{0:D2}", 3);
        var mainQuestData = DataManager.GetInstance.FindData
            (DataManager.KEY_QUEST, int.Parse(key01 + key02)) as QuestData;
        int clearedWave = ud.GetMainQuest(stageN);

        if (clearedWave <= -1) return;          // 튜토리얼 예외상황
        if (mainQuestData.clearCount > clearedWave)
        {
            if (mainQuestData.clearCount < nowWave)
            {
                nowWave = mainQuestData.clearCount;
            }
            if (nowWave > clearedWave)
            {
                ud.SetMainQuest(stageN, nowWave);
            }
        }
        ud.SaveQuestData();
    }

    /// <summary>
    /// 시즌패스 정보 저장(킬 카운트)
    /// </summary>
    public void SaveSeasonPassData()
    {
        UserData ud = UserData.GetInstance;
        DataManager dataManager = DataManager.GetInstance;

        // 시즌패스 활성화 시
        if (ud.IsPassActivated)
        {
            int current = ud.GetCurrentProgress + _chapter[_cc].KillCount;
            int next = ud.GetNextPassLevel(dataManager.GetList<SeasonPassData>(DataManager.KEY_SEASONPASS).Count);
            if (next > 0)
            {
                // 목표수량 체크해서 레벨 업 계산
                int addLevel = 0;
                while (true)
                {
                    SeasonPassData passData = dataManager.FindData(DataManager.KEY_SEASONPASS, next) as SeasonPassData;
                    if (passData == null) break;
                    if (passData.max <= current)
                    {
                        current = current - passData.max;
                        addLevel++;
                        next++;
                    }
                    else
                    {
                        break;
                    }
                }
                ud.LevelUpSeasonPass(addLevel);
                ud.SetCurrentProgress(current);
                ud.SaveSeasonPassData();
            }
        }
    }

    int[] MyMaterialFind(UserData ud)
    {
        List<int> myMaterialList = new List<int>();

        for (int i = 0; i < (int)EInvenType.Evolution; i++)
        {
            myMaterialList.Add(ud.GetCurrency((EInvenType)i));
        }
        return myMaterialList.ToArray();
    }

    #region property
    public PlayerStats PlayerStat
    {
        get
        {
            if (_playerStat == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(CommonStaticDatas.TAG_PLAYER);
                if (player != null)
                    _playerStat = player.GetComponent<PlayerStats>();
            }
            return _playerStat;
        }
    }
    
    public void UniqueDie()
    {
        if (_inGameUI == null) { _inGameUI = GameObject.Find("InGameCanvas").GetComponent<InGameUIManager>(); }
        _inGameUI.SetEnemyAppearText("유니크 처지", 3f);
    }

    public int MaxEnemyTotle { get { return _maxEnemyTotle; } set { _maxEnemyTotle = value; } }
    public int EliteCount { get { return _eliteCount; } set { _eliteCount = value; } }
    public int BossCount { get { return _bossCount; } set { _bossCount = value; } }
    //public int EliteMax { get { return _eliteMax; } set { _eliteMax = value; } }
    public int BossMax { get { return _bossMax; } set { _bossMax = value; } }
    public bool GetEndGame { get { return _endGame; } set { _endGame = value; } }
    public DarkBlurScript GetInGameUIManager => _inGameUI.GetDarkBlur;
    public ChapterWave GetCurrentChapter => _chapter[_cc];
    public int NowWave => _nowWave;
    public void CountUpWave() => _nowWave++;
    public bool IsTutorial => UserData.GetInstance.TutorialCheck() <= 8;       // 튜토리얼 여부 체크
    #endregion

    #region Potential    
    /// <summary>
    /// 장비 포텐셜(어빌리티) 등록
    /// </summary>
    /// <param name="abilityID">어빌리티 ID</param>
    public void SetAddedPotential(int abilityID)
    {
        _addedAbility.Add(abilityID);
    }

    public List<int> GetAddedAbility => _addedAbility;
    #endregion Potential
}

