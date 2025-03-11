using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyData;

public class ReadyScript : PageBase
{
    private enum ToggleType
    {
        Diamond = 0,
        AD = 1,
    }

    public delegate void ReadyEventDelegate();
    public event ReadyEventDelegate OnStartEventListener;

    InvenScript _inven;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI _titleTMP;

    [Header("Boat Setting")]
    [SerializeField] RectTransform _boatListRect;
    [SerializeField] UnitPosition[] _unitPositionArr;
    [Space]
    [SerializeField] GameObject _placementBlocker;
    [SerializeField] GameObject _boatListBlocker;
    [SerializeField] GameObject _positionBlocker;

    List<UnitIcon> _boatIconList = new List<UnitIcon>();
    EUnitPosition _selectedPosition;

    [Header("Ability")]
    [SerializeField] Toggle[] _abilityToggleArr;

    [Header("Active Skill")]
    [SerializeField] GameObject[] _activeSkillObjArr;
    [SerializeField] Image[] _skillIconImgArr;
    [SerializeField] GameObject _unlockBloker;
    ActiveSkillData[] _equipedSkillDataArr = new ActiveSkillData[2];
    int skillSlotIdx = 0;

    [Header("Buttons")]
    [SerializeField] Button _startButton;
    [SerializeField] Button _closeButton;

    const string _titleStageFormat = "STAGE {0}";
    const string _skillSpritePathFormat = "ItemIcon/Skill{0:D2}";
    public override void Init(LobbyUIManager uiM)
    {
        base.Init(uiM);
        _inven = uiManager.GetInvenPage;

        // Boat Placement
        _selectedPosition = EUnitPosition.NONE;
        for (int i = 0; i < _unitPositionArr.Length; i++)
        {
            _unitPositionArr[i].Init(this);
        }

        // Ability
        for (int i = 0; i <= (int)ToggleType.AD; i++)
        {
            int idx = i;
            _abilityToggleArr[i].onValueChanged.AddListener((bool state) => OnTouchToggle(idx));
        }

        // Active Skill
        InitSkillSlot();

        _startButton.onClick.AddListener(OnTouchStartButton);
        _closeButton.onClick.AddListener(ClosePage);

        // Blocker
        //_placementBlocker.GetComponent<Button>().onClick.AddListener(OnTouchPlacementBlocker);
        _boatListBlocker.GetComponent<Button>().onClick.AddListener(OnTouchPlacementBlocker);
        _positionBlocker.GetComponent<Button>().onClick.AddListener(OnTouchPlacementBlocker);
        //_placementBlocker.SetActive(false);
        _boatListBlocker.SetActive(false);
        _positionBlocker.SetActive(false);
    }

    public void RestartInit()
    {
        SortBoatList();
        for (int i = 0; i < _unitPositionArr.Length; i++)
        {
            SetBoatSlot(i);
        }
    }

    public override void OpenPage()
    {
        //SoundManager.GetInstance.PlayAudioEffectSound("UI_Button_Touch");
        SortBoatList();
        for (int i = 0; i < _unitPositionArr.Length; i++)
        {
            SetBoatSlot(i);
        }
        CheckToggleState();
        base.OpenPage();
    }

    public override void ClosePage()
    {
        //SoundManager.GetInstance.PlayAudioEffectSound("UI_Button_Touch");
        ResetIconButtons();
        for (int i = 0; i < _unitPositionArr.Length; i++)
        {
            _unitPositionArr[i].ClearPosition();
        }
        base.ClosePage();
    }

    #region BoatSetting
    /// <summary>
    /// 보트 아이콘 생성(수량 맞추는 용도)
    /// </summary>
    public void CreateBoatIcon(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_UI_ITEMICON, CommonStaticDatas.RES_ITEMICON, Vector3.zero, Quaternion.identity);
            obj.transform.SetParent(_boatListRect);
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
            _boatIconList.Add(obj.GetComponent<UnitIcon>());
        }
    }

    /// <summary>
    /// 보트 아이콘 삭제(수량 맞추는 용도)
    /// </summary>
    public void DestroyBoatIcon()
    {
        Destroy(_boatIconList[_boatIconList.Count - 1].gameObject);
        _boatIconList.RemoveAt(_boatIconList.Count - 1);
    }


    /// <summary>
    /// 보트 리스트에 아이콘 추가
    /// </summary>
    void AddBoatIcon(List<UnitIcon> unitList, int grade, ref int count, ref int synthesisCount)
    {
        for (int j = count; j < unitList.Count; ++j)
        {
            if (unitList[count].GetGrade == grade)
            {
                _boatIconList[synthesisCount].Init(unitList[count]);
                _boatIconList[synthesisCount].GetOriginalData = unitList[count];
                _boatIconList[synthesisCount].GetMyBtn.onClick.AddListener(_boatIconList[synthesisCount].BoatPositionSelectFunction);

                count++;
                synthesisCount++;
            }
            else { break; }
        }
    }

    /// <summary>
    /// 슬롯 보트 초기화
    /// </summary>
    void SetBoatSlot(int arr)
    {
        var data = userData.GetSelectData;
        var boatList = _inven.GetInven(EItemList.BOAT);
        if (data._boatSPId[arr] != 0)
        {
            for (int i = 0; i < boatList.Count; ++i)
            {
                if (_boatIconList[i].GetOriginalData.GetIngerenceID == data._boatSPId[arr])
                {
                    userData.BoatTemp = _boatIconList[i];
                    _unitPositionArr[arr].PositionBtn();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 보유 중인 보트 리스트 정렬
    /// </summary>
    void SortBoatList()
    {
        int synthesisCount = 0;
        int boatCount = 0;
        // 12등급 부터 1등급 까지 내림차순
        for (int i = 12; i > 0; --i)
        {
            AddBoatIcon(_inven.GetInven(EItemList.BOAT), i, ref boatCount, ref synthesisCount);
        }
    }

    /// <summary>
    /// 보트 아이콘 버튼 기능 제거
    /// </summary>
    void ResetIconButtons()
    {
        for (int i = 0; i < _boatIconList.Count; ++i)
        {
            _boatIconList[i].GetMyBtn.onClick.RemoveAllListeners();
        }
    }

    #endregion BoatSetting

    #region Ability
    /// <summary>
    /// 어빌리티 토글 상태 확인
    /// </summary>
    void CheckToggleState()
    {
        // 둘다 ON 상태이면 둘다 OFF 처리(중복 불가 옵션이므로)
        if (userData.CheckAdAbility && userData.CheckDiaAbility)
        {
            userData.SetAdAbilityCheck(false);
            userData.SetDiaAbilityCheck(false);
        }
        // 둘중 하나 or 아무것도 체크 되어 있지 않은 상태
        _abilityToggleArr[(int)ToggleType.Diamond].isOn = userData.CheckDiaAbility;
        _abilityToggleArr[(int)ToggleType.AD].isOn = userData.CheckAdAbility;
    }

    public void DisableADToggle()
    {
        _abilityToggleArr[(int)ToggleType.AD].isOn = false;
        userData.SetAdAbilityCheck(false);
    }

    public void DisableDiaToggle()
    {
        _abilityToggleArr[(int)ToggleType.Diamond].isOn = false;
        userData.SetDiaAbilityCheck(false);
    }

    public void OnTouchDiaToggle()
    {
        if (_abilityToggleArr[0].isOn)
        {
            _abilityToggleArr[1].isOn = false;
        }
    }
    public void OnTouchADToggle()
    {
        if (_abilityToggleArr[1].isOn)
        {
            _abilityToggleArr[0].isOn = false;
        }
    }

    /// <summary>
    /// 어빌리티 토글 터치 이벤트
    /// </summary>
    /// <param name="idx"></param>
    public void OnTouchToggle(int idx)
    {
        if (!_abilityToggleArr[idx].isOn)
        {
            return;
        }
        ToggleType type = (ToggleType)idx;
        switch (type)
        {
            case ToggleType.Diamond:
                {
                    _abilityToggleArr[(int)ToggleType.AD].isOn = false;
                }
                break;
            case ToggleType.AD:
                {
                    _abilityToggleArr[(int)ToggleType.Diamond].isOn = false;
                }
                break;
        }
    }
    public bool DiaToggleState => _abilityToggleArr[(int)ToggleType.Diamond].isOn;
    public bool ADToggleState => _abilityToggleArr[(int)ToggleType.AD].isOn;
    #endregion Ability

    #region ActiveSkill

    void InitSkillSlot()
    {
/*        // 15레벨 이후 개방
        if(userData.GetUserLevel < 15)
        {
            _unlockBloker.SetActive(true);
            return;
        }
        else
        {
            _unlockBloker.SetActive(false);
        }*/
        _unlockBloker.SetActive(false);
        for (int i = 0; i < _equipedSkillDataArr.Length; i++)
        {
            int id = userData.GetEquipedSkill(i);
            if(id > 0)
            {
                var data = DataManager.GetInstance.FindData(DataManager.KEY_ACTIVESKILL, id) as ActiveSkillData;
                _equipedSkillDataArr[i] = data;
                _skillIconImgArr[i].sprite = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(_skillSpritePathFormat, data.nId));
                _activeSkillObjArr[i].SetActive(true);
            }
            else
            {
                _activeSkillObjArr[i].SetActive(false);
            }
        }
    }
    /// <summary>
    /// 스킬 슬롯 상태 설정
    /// </summary>
    /// <param name="data">스킬 데이터</param>
    public void SetSkillSlot(ActiveSkillData data)
    {
        int id = 0;
        if(data != null)
        {
            _equipedSkillDataArr[skillSlotIdx] = data;
            _skillIconImgArr[skillSlotIdx].sprite = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(_skillSpritePathFormat, data.nId));
            _activeSkillObjArr[skillSlotIdx].SetActive(true);
            id = data.nId;
        }
        else
        {
            _equipedSkillDataArr[skillSlotIdx] = null;
            _skillIconImgArr[skillSlotIdx].sprite = null;
            _activeSkillObjArr[skillSlotIdx].SetActive(false);
        }
        Debug.Log(skillSlotIdx);
        userData.SetEquipedSkill(skillSlotIdx, id);
        userData.SaveUnitSelect();
    }
    #endregion ActiveSkill

    #region Buttons
    /// <summary>
    /// 게임 시작 버튼
    /// </summary>
    public void OnTouchStartButton()
    {
        OnStartEventListener?.Invoke();
        //_uiManager.GameStart();
    }

    /// <summary>
    /// 보트 선택 제외 Blocker
    /// </summary>
    void OnTouchPlacementBlocker()
    {
        //_placementBlocker.SetActive(false);
        _boatListBlocker.SetActive(false);
        _positionBlocker.SetActive(false);
        userData.BoatTemp = null;
        _selectedPosition = EUnitPosition.NONE;
    }

    public void OnTouchActiveSkillSlot(int idx)
    {
        skillSlotIdx = idx;
        uiManager.GetPopup<ActiveSkillPopup>(PopupType.ACTIVESKILL).OpenPopup();
    }
    #endregion Buttons

    #region Getter
    public List<UnitIcon> GetBoatList => _boatIconList;
    public UnitPosition GetUnitPosition(int idx) => _unitPositionArr[idx];
    public UnitPosition GetUnitPosition(EUnitPosition posType) => _unitPositionArr[(int)posType];
    public EUnitPosition GetTargetPosType => _selectedPosition;
    #endregion Getter

    #region Setter
    /// <summary>
    /// 보트 리스트 가림 처리
    /// </summary>
    /// <param name="state"></param>
    public void SetBoatListBlockerState(bool state)
    {
        _boatListBlocker.SetActive(state);
        //_placementBlocker.SetActive(state);
    }

    /// <summary>
    /// 포지션 가림 처리
    /// </summary>
    /// <param name="state"></param>
    public void SetPositionBlockerState(bool state)
    {
        _positionBlocker.SetActive(state);
        //_placementBlocker.SetActive(state);
    }

    /// <summary>
    /// 보트가 배치될 위치 정보 설정
    /// </summary>
    /// <param name="pos"></param>
    public void SetTargetPosition(EUnitPosition pos)
    {
        _selectedPosition = pos;
    }

    public void SetTitleTMP(int stage)
    {
        _titleTMP.text = string.Format(_titleStageFormat, stage);
    }
    #endregion Setter
}
