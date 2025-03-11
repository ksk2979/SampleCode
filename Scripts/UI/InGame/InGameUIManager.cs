using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyData;
using System;
using System.Linq;

public class InGameUIManager : SceneStaticObj<InGameUIManager>
{
    [Header("UIs")]
    public FloatingJoystick _floatingJoystick = null;
    [SerializeField] TextMeshProUGUI _remainCountTMP;
    [SerializeField] TextMeshProUGUI _stageNameT;
    [SerializeField] TextMeshProUGUI _stageWaveT;
    [SerializeField] TextLocalizeSetter _uniqueWarningObj;
    [SerializeField] GameObject _accelNoticeObj;
    [SerializeField] GameObject _videoBlocker;

    [Header("Player")]
    public Transform _hpBarParent = null;       // HpCanvas에서 가져옴
    private List<HpBar> _hpBarList = new List<HpBar>();

    PlayerAbility _playerAbility;   // 플레이어 능력치
    bool _oneChance = false;        // 부활 기회 사용여부 
    int _remainAbilityCount = 0;    // 남은 어빌리티 개수

    [Header("Active SKill")]
    [SerializeField] PlayerSkillSet[] _playerSkillSetArr;

    [Header("Boss")]
    public GameObject _bossHpObj;
    public Image _bossHp;

    [Header("Popup")]
    [SerializeField] PopupController _popupController;
    AbilityPopup _abilityPopup;

    [Header("Play Time")]
    [SerializeField] TextMeshProUGUI _playTimeTMP;
    TimeSpan _warningTime = new TimeSpan(0, 0, 10);
    bool _isWarningTime = false;

    StageManager _stageM;

    const string HpBar = "HpBar";
    const string StageStr = "Stage";
    const string WaveStr = "Wave";

    [SerializeField] DarkBlurScript _darkBlur;
    public DarkBlurScript GetDarkBlur => _darkBlur;

    [SerializeField] bool _isPause = false;
    bool _isAccel = false;
    public bool IsPause { get { return _isPause; } set { _isPause = value; } }
    public bool SetAccel(bool state) => _isAccel = state;
    public bool IsAccel => _isAccel;

    [SerializeField] WindowTargetPoint _targetPoint;
    [SerializeField] TextMeshProUGUI _endTextName;

    #region Init
    public void Init()
    {
        _videoBlocker.SetActive(false);
        _floatingJoystick.OnBackGround();
        if (_stageM == null) { _stageM = StageManager.GetInstance; }
        TutorialManager.GetInstance.Init();
        _playerAbility = _stageM.GetPlayersController()[0].GetComponent<PlayerAbility>();
        _targetPoint.Init();

        InitActiveSkill();
        InitAbilityPopup();
    }

    public void InitPopup()
    {
        _popupController.Init();
        GetPopup<OptionManager>(PopupType.OPTION).Init();
    }
    public void InitAbilityPopup()
    {
        _abilityPopup = GetPopup<AbilityPopup>(PopupType.ABILITY);
        _abilityPopup.Init(_playerAbility);
    }

    void InitActiveSkill()
    {
        UserData userData = UserData.GetInstance;
/*        if (userData.GetUserLevel < 15)
        {
            // 15 미만 숨김 처리
            for (int i = 0; i < _playerSkillSetArr.Length; i++)
            {
                _playerSkillSetArr[i].ActivateSkillUI(false);
            }
            return;
        }
        else
        {
            // 15이상 장착된 스킬 상태 보여줌
            var equipedSkill = userData.GetEquipedSkillArr();
            for (int j = 0; j < _playerSkillSetArr.Length; j++)
            {
                var data = DataManager.GetInstance.FindData(DataManager.KEY_ACTIVESKILL, equipedSkill[j]) as ActiveSkillData;
                _playerSkillSetArr[j].Init(data);
            }
        }*/

        // 임시 개방
        var equipedSkill = userData.GetEquipedSkillArr();
        for (int j = 0; j < _playerSkillSetArr.Length; j++)
        {
            var data = DataManager.GetInstance.FindData(DataManager.KEY_ACTIVESKILL, equipedSkill[j]) as ActiveSkillData;
            _playerSkillSetArr[j].Init(data);
        }
    }
    #endregion Init

    private void Update()
    {
        if (_stageM != null && _stageM.GetEndGame) { return; }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseBtn();
        }

        if (!IsPause)
        {
            if (_remainAbilityCount > 0)
            {
                _remainAbilityCount--;
                RandomAbilityFuntion();
            }
            CheckPlayTime();
        }
    }

    #region UI Info
    /// <summary>
    /// 플레이어의 HP Bar 생성 및 설정
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="upValue"></param>
    internal void CreateHpBarUi(CharacterStats stat, float upValue = 2)
    {
        var obj = SimplePool.Spawn(CommonStaticDatas.RES_UI, HpBar, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(_hpBarParent);
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        HpBar hpBar = obj.GetComponent<HpBar>();
        hpBar.SetTarget(stat, upValue);
        stat._hpBar = obj;
        _hpBarList.Add(hpBar);
    }

    /// <summary>
    /// 보스 체력 설정
    /// </summary>
    /// <param name="hpRatio"></param>
    public void SetBossHp(float hpRatio)
    {
        if (GameManager.GetInstance.TestEditor)
        {
            if (_bossHpObj.activeSelf == false)
            {
                _bossHpObj.SetActive(true);
            }
            _bossHp.fillAmount = hpRatio;

            if (hpRatio == 0)
            {
                _bossHpObj.SetActive(false);
            }
            return;
        }

        if (_stageM == null) { _stageM = StageManager.GetInstance; }

        if (_bossHpObj.activeSelf == false)
        {
            // 보스 나올때 나오는 브금
            _stageM._chapter[_stageM._cc].SetStageBgm(CommonStaticDatas.MUSIC_BOSS_APPEARANCE);
            _bossHpObj.SetActive(true);
        }
        _bossHp.fillAmount = hpRatio;

        if (hpRatio == 0)
        {
            // 보스 죽으면 종료고 다시 맵의 브금 틀기
            _stageM._chapter[_stageM._cc].SetCurStageBgm();
            _bossHpObj.SetActive(false);
        }
    }

    /// <summary> 몬스터 카운트 </summary>
    public void UpdateEnemyCount(string count) => _remainCountTMP.text = count;

    /// <summary> 스테이지 이름 </summary>
    public void StageNameSetting(string stage) => _stageNameT.text = string.Format("{0} {1}", StageStr, stage);

    /// <summary> 현재 웨이브 정보 </summary>
    public void StageWaveSetting(string wave) => _stageWaveT.text = string.Format("{0} {1}", WaveStr, wave);

    #endregion UI Info

    #region ChangeScene / Lobby
    /// <summary> 로비로 나가는 기능 </summary>
    public void LobbyReturnScene()
    {
        StartCoroutine(ChangeLoobyScene());
        SoundManager.GetInstance.DestroyEffectSoundObj();
        LocalizeText.DeleteChangeLocalize();
    }
    IEnumerator ChangeLoobyScene()
    {
        yield return null;
        Time.timeScale = 1;
        IsPause = false;
        cSceneManager.GetInstance.ChangeScene("Loading_Lobby", () => { });
    }
    #endregion ChangeScene / Lobby

    #region Retry

    /// <summary>
    /// 첫 사망시 부활 여부 확인(광고)
    /// </summary>
    public bool CheckRetry()
    {
        if (_stageM == null) { _stageM = StageManager.GetInstance; }
        if (_oneChance) { return false; }
        else
        {
            _oneChance = true;
            UserData userData = UserData.GetInstance;
            RevivalPopup popup = GetPopup<RevivalPopup>(PopupType.REVIVAL);

            string logFormat = "Revival_{0}";
            popup.OnOpenEventListener += () =>
            {
                _stageM._hpCanvas.SetActive(false);
                //Time.timeScale = 0;
            };
            popup.OnADEventListener += () =>
            {
                ADManager.GetInstance.ShowRewardedVideo(
                    () =>
                    {
                        // 광고 보기 성공
                        FirebaseManager.GetInstance.LogEvent(string.Format(logFormat, "AD"));
                        RetryOk();
                        popup.GetSuccessRevival();
                    },
                    () => { },
                    "Revival");
            };
            popup.OnDiaEventListener += () =>
            {
                // 다이아로 부활
                if (userData.GetCurrency(EPropertyType.DIAMOND) > 30)
                {
                    FirebaseManager.GetInstance.LogEvent(string.Format(logFormat, "Dia"));
                    userData.SubCurrency(EPropertyType.DIAMOND, 30);
                    userData.SaveUserData();
                    RetryOk();
                    popup.GetSuccessRevival();
                }
                else
                {
                    MessageHandler.GetInstance.ShowMessage("다이아가 부족합니다", 1.5f);
                    return;
                }
            };
            popup.OnCancelEventListener += GiveUpRetry; // 포기
            popup.OpenPopup();
            return true;
        }
    }

    /// <summary>
    /// 부활 기능 (광고 / 다이아)
    /// </summary>
    public void RetryOk()
    {
        Time.timeScale = 1;
        //_oneChanceObj.SetActive(false);
        _stageM._hpCanvas.SetActive(true);
        _stageM.GetEndGame = false;
        var player = StageManager.GetInstance.GetPlayers();
        for (int i = 0; i < player.Count; ++i)
        {
            Player p = player[i] as Player;
            p.GetCharacterCtrl().Revival();
            p._PlayerController.SetTraceState(true);
        }
        if(_stageM.GetCurrentChapter.IsClearGame)
        {
            _stageM.GetCurrentChapter.CheckNextPhase();
        }
    }

    /// <summary>
    /// 부활 포기 (결과창 표기)
    /// </summary>
    public void GiveUpRetry() => _stageM.EndStageMaterialShow(true);

    #endregion Retry

    #region Ability
    /// <summary>
    /// 랜덤 어빌리티 설정 및 팝업 오픈
    /// </summary>
    public void RandomAbilityFuntion()
    {
        IsPause = true;
        _abilityPopup.SetRandomAbility();
        _abilityPopup.OnCloseEventListener += () => { IsPause = false; };
    }

    /// <summary>
    /// 랜덤 어빌리티 1개 추가
    /// </summary>
    public void AddOneAbility()
    {
        AbilityData data = _playerAbility.AddRandomAbility();
        Sprite iconSprite = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format("ItemIcon/Ability{0:D2}", data.nId));
        string abilityName = data.name;
        _stageM.PlayerStat._hpBar.GetComponent<HpBar>().SetAbilityInfo(iconSprite, abilityName);
    }

    /// <summary>
    /// 타겟에 대한 화살표 UI 생성
    /// </summary>
    /// <param name="trans"></param>
    public void TargetArrowPosShowCreate(Transform trans)
    {
        _targetPoint.Show(trans);
    }
    public void ArrowDestroy()
    {
        _targetPoint.ClearObj();
    }

    public void SetRemainAblityCount(int count) => _remainAbilityCount = count;

    public int GetAbilityCount { get { return _playerAbility._abilityCount; } set { _playerAbility._abilityCount = value; } }
    public int GetAbilityCountMax { get { return _playerAbility._abilityCountMax; } }
    public int GetRemainAbilityCount => _remainAbilityCount;
    #endregion Ability

    #region Buttons

    /// <summary>
    /// 일시정지 버튼(팝업 실행)
    /// </summary>
    public void PauseBtn()
    {
        if (_stageM == null) { _stageM = StageManager.GetInstance; }

        _stageM._hpCanvas.SetActive(false);
        Time.timeScale = 0;
        IsPause = true;

        PausePopup popup = GetPopup<PausePopup>(PopupType.PAUSE);
        popup.OnPlayEventListener += () =>
        {
            StageManager.GetInstance._hpCanvas.SetActive(true);
            Time.timeScale = 1;
            IsPause = false;
        };
        popup.OnLobbyEventListener += LobbyReturnScene;
        popup.OnLobbyEventListener += () =>
        {
            _stageM.SaveQuestInfo(true);
            _stageM.SaveSeasonPassData();
        };
        popup.OpenPopup();
    }

    #endregion Buttons

    #region Play Time
    /// <summary>
    /// 플레이 시간 체크
    /// </summary>
    void CheckPlayTime()
    {
        if (!GameManager.GetInstance.TestEditor)
        {
            if (_playTimeTMP == null) return;
            if (!_playTimeTMP.gameObject.activeSelf)
            {
                _playTimeTMP.gameObject.SetActive(true);
            }
            TimeSpan playTime = StageManager.GetInstance.GetPlayTime;
            _playTimeTMP.text = string.Format("{0:00}:{1:00}", playTime.Minutes, playTime.Seconds);
            if (!_isWarningTime && playTime <= _warningTime)
            {
                SetWarningPlayTime();
            }
        }
    }
    /// <summary>
    /// 남은 플레이 시간 일정 이하일때 경고 시작(텍스트 색상 변화)
    /// </summary>
    void SetWarningPlayTime()
    {
        _isWarningTime = true;
        StartCoroutine(ChangePlayTimeTextColor());
    }

    IEnumerator ChangePlayTimeTextColor(float changeTime = 0.2f)
    {
        while (!StageManager.GetInstance.GetEndGame)
        {
            yield return YieldInstructionCache.WaitForSeconds(changeTime);
            if (_playTimeTMP.color == Color.white)
            {
                _playTimeTMP.color = Color.red;
            }
            else
            {
                _playTimeTMP.color = Color.white;
            }
        }
    }
    #endregion Play Time

    public void Accelerate(bool state)
    {
        if (_stageM == null) { _stageM = StageManager.GetInstance; }

        _accelNoticeObj.SetActive(state);
        if (state)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        IsPause = state;
        if (state)
        {
            _isAccel = state;
        }
        else
        {
            StartCoroutine(DelayDisableAccel(1.0f));
        }
    }

    IEnumerator DelayDisableAccel(float time)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        _isAccel = false;
    }

    /// <summary>
    /// 특수 몬스터 등장 알림 텍스트
    /// </summary>
    /// <param name="key">텍스트 키</param>
    /// <param name="time">지속 시간</param>
    public void SetEnemyAppearText(string key, float time)
    {
        _uniqueWarningObj.key = key;
        StartCoroutine(DelayTextSetting(time));
    }
    IEnumerator DelayTextSetting(float time)
    {
        GetUniqueWarningObj.SetActive(true);
        yield return YieldInstructionCache.WaitForSeconds(time);
        GetUniqueWarningObj.SetActive(false);
    }



    public bool OneChange { get { return _oneChance; } set { _oneChance = value; } }
    public GameObject GetUniqueWarningObj => _uniqueWarningObj.transform.parent.gameObject;
    public PopupController GetPopupController() { return _popupController; }
    public GameObject GetVideoBlocker => _videoBlocker;
    public HpBar GetHpBar(int idx) => _hpBarList[idx];
    public List<HpBar> GetHpBarList => _hpBarList;
    public T GetPopup<T>(PopupType type)
    {
        var popup = _popupController.GetPopup<T>(type);
        if (popup != null)
        {
            return popup;
        }
        else
        {
            return default(T);
        }
    }
    public void TitleEndTExtSetting(string text)
    {
        ResultPopup popup = GetPopup<ResultPopup>(PopupType.RESULT);
        popup.TitleEndTextSetting(text);
    }
}
