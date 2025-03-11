using Info;
using MyData;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExplosionPopup : PopupBase
{
    PlayerSkillSet _skillSet;
    InGameUIManager _uiManager;

    [Header("Boat Icon")]
    [SerializeField] UnitIcon[] _boatIconArr;
    List<Player> _playerList = new List<Player>();

    [Header("UI")]
    [SerializeField] Image[] _boatHpArr;
    [SerializeField] TextMeshProUGUI[] _boatHpTextArr;
    [SerializeField] TextMeshProUGUI[] _damageTextArr;

    [Header("Info")]
    [SerializeField] GameObject[] _selectSlotArr;
    [SerializeField] Sprite _normalBg;
    [SerializeField] Sprite _selectedBg;
    List<Image> _slotBgList = new List<Image>();
    List<Button> _buttonList = new List<Button>();

    int _selectedIdx = -1;
    int _targetPosNumber = -1;
    List<int> _posNumberList = new List<int>();

    bool _isInit = false;
    bool _executeExplosion = false;             // 폭파 예약상태 확인
    bool _isEmpty = false;

    public event PopupEventDelegate OnSelectEventListener;

    const string _effectName = "FX_Explosion_03";
    const string _damagetextFormat = "DMG : {0}";

    #region Init
    public void Init()
    {
        if (_isInit) return;
        _isInit = true;
        // 슬롯 배경 등록
        for (int j = 0; j < _selectSlotArr.Length; j++)
        {
            _slotBgList.Add(_selectSlotArr[j].GetComponent<Image>());
            _buttonList.Add(_selectSlotArr[j].GetComponent<Button>());
            int btnIndex = j;
            _buttonList[j].onClick.AddListener(() => OnTouchSelectButton(btnIndex));
        }

        _uiManager = InGameUIManager.GetInstance;

        ResetSlotList();
    }

    /// <summary>
    /// 슬롯 상태 초기화
    /// </summary>
    void ResetSlotList()
    {
        ResetSlotBG();

        // 슬롯 OFF 처리
        for (int i = 0; i < _selectSlotArr.Length; i++)
        {
            _selectSlotArr[i].gameObject.SetActive(false);
        }
        _posNumberList.Clear();
        _playerList.Clear();
        _selectedIdx = -1;
        _executeExplosion = false;
    }

    /// <summary>
    /// 슬롯 배경 초기화
    /// </summary>
    void ResetSlotBG(int exception = -1)
    {
        // 슬롯 배경 초기화
        for (int i = 0; i < _slotBgList.Count; i++)
        {
            if (exception == i)
            {
                _slotBgList[i].sprite = _selectedBg;
            }
            else
            {
                _slotBgList[i].sprite = _normalBg;
            }
        }
    }
    #endregion Init
    public void SetSkillScript(PlayerSkillSet skillSet)
    {
        _skillSet = skillSet;
    }

    /// <summary>
    /// 보트 정보 등록
    /// </summary>
    /// <param name="playerInfoList"></param>
    /// <param name="curHpList"></param>
    /// <param name="maxHpList"></param>
    public void AddBoatInfo(List<PlayerInfo> playerInfoList, List<Interactable> unitList)
    {
        ResetSlotList();

        for (int i = 1; i < unitList.Count; i++)
        {
            _playerList.Add(unitList[i] as Player);
        }
        Debug.Log("_playerList : " + _playerList.Count);

        int slotIdx = 0;
        // 보트 정보 등록 
        for (int i = 0; i < playerInfoList.Count; i++)
        {
            // 메인 보트 or 보트 정보가 없는 경우 스킵
            if (i == 0 || playerInfoList[i] == null)
            {
                continue;
            }

            // 슬롯 정보 등록
            var boatData = DataManager.GetInstance.GetData(DataManager.KEY_BOAT, playerInfoList[i].GetPlayerValue(ECoalescenceType.BOAT_ID), 1) as BoatData;
            _boatIconArr[slotIdx].UpdateGradeText(boatData.grade);
            _boatIconArr[slotIdx].UpdateGradeOutLine(boatData.grade);

            Sprite iconImage = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format("ItemIcon/{0}", UnitDataManager.GetInstance.GetStringValue(EItemList.BOAT, boatData.id1, StatusType.resName)));
            _boatIconArr[slotIdx].UpdateIconSprite(iconImage);
            _boatIconArr[slotIdx].UpdateLevelText(playerInfoList[i].GetPlayerValue(ECoalescenceType.BOAT_LEVEL));

            // 플레이어 HP 정보 표기
            bool isActive = false;
            for (int j = 0; j < _playerList.Count; j++)
            {
                if (i == _playerList[j]._playerStats.GetPosNumber)
                {
                    float hp = _playerList[j]._playerStats.hp;
                    isActive = hp > 0;
                    float maxHp = _playerList[j]._playerStats.maxHp;
                    _boatHpTextArr[slotIdx].text = string.Format("{0} / {1}", hp, maxHp);
                    _damageTextArr[slotIdx].text = string.Format(_damagetextFormat, maxHp * 3f);
                    _boatHpArr[slotIdx].fillAmount = hp / maxHp;
                    _posNumberList.Add(i);
                    break;
                }
            }
            _selectSlotArr[slotIdx].gameObject.SetActive(isActive);
            _selectSlotArr[slotIdx].GetComponent<RectTransform>().sizeDelta = new Vector2(140f, 240f);
            slotIdx++;
        }
    }

    /// <summary>
    /// 보트 폭파
    /// </summary>
    void Explosion()
    {
        var explosion = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _effectName, _playerList[_targetPosNumber]._PlayerController._trans.position, Quaternion.identity);
        var explosionTdd = new UnitDamageData()
        {
            attacker = _playerList[_targetPosNumber].transform,
            layerMask = 1 << LayerMask.NameToLayer(CommonStaticDatas.LAYERMASK_UNIT),
            targetTag = new string[] { CommonStaticDatas.TAG_ENEMY, CommonStaticDatas.TAG_TRAP },
            damage = _playerList[_targetPosNumber]._playerStats.maxHp * 3f,
            _ability = null
        };
        explosion.GetComponent<SphereCollider>().radius = 30f;
        explosion.GetComponent<Explosion>().HitRadius = 30f;
        explosion.GetComponent<Explosion>().InitData(_playerList[_targetPosNumber]._playerStats, explosionTdd);
        var stats = _playerList[_targetPosNumber]._playerStats;
        stats.SetIsInvincibility(false);
        _playerList[_targetPosNumber].TakeToDamage(_playerList[_targetPosNumber]._playerStats.hp);
    }

    /// <summary>
    /// 선택한 보트의 실물 위(Hp바 위)에 Explosion Notice 표시
    /// </summary>
    void ShowNotice()
    {
        if (_uiManager == null) _uiManager = InGameUIManager.GetInstance;
        var hpList = new List<HpBar>();
        // 대상 : 0 ~ 3 (메인 보트 제외)
        for (int i = 0; i < _playerList.Count; i++)
        {
            HpBar hp = _playerList[i]._playerStats._hpBar.GetComponent<HpBar>();
            hp.SetExplosionState(false);
            hpList.Add(hp);
        }
        if(_selectedIdx != -1)
        {
            hpList[_selectedIdx].SetExplosionState(true);
        }
    }


    public override void OpenPopup()
    {
        base.OpenPopup();
    }

    public override void ClosePopup()
    {
        _selectedIdx = -1;
        ShowNotice();
        base.ClosePopup();
    }

    #region Buttons
    public void OnTouchConfirmButton()
    {
        if (_selectedIdx <= -1) return;
        OnSelectEventListener?.Invoke();
        if (!_executeExplosion)
        {
            _executeExplosion = true;
            _targetPosNumber = _selectedIdx;
            _playerList[_targetPosNumber]._playerStats.SetIsInvincibility(true);
            _playerList[_targetPosNumber]._PlayerController.SetTraceState(false);
            _skillSet.ActivateSkill(Explosion, 2.0f);
        }
        ClosePopup();
    }
    public void OnTouchSelectButton(int idx)
    {
        _selectedIdx = idx;
        ResetSlotBG(idx);
        ShowNotice();
    }
    #endregion Buttons
}
