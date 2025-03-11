using MyData;
using MyStructData;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ItemEditorScript : PageBase
{
    private enum StatusBoxType
    {
        CURRENT,
        ARROW,
        NEXT,
    }

    private enum CategoryBoxType
    {
        UPGRADE,
        POTENTIAL,
    }

    public enum PotentialType
    {
        Status,
        Ability
    }

    InvenScript _inven;
    ItemUpgrade _itemUpgrade;

    [SerializeField] UnitIcon _viewIcon;
    UnitIcon _targetIcon = null;

    [Header("Upgrade")]
    [SerializeField] GameObject[] _statusBoxConents;    // curStats, arrow, nextStats
    [SerializeField] ItemStatus _currentStatus;
    [SerializeField] ItemStatus _nextStatus;
    [SerializeField] MaterialIcon[] _materialIconArr;
    [SerializeField] GameObject _unableUpgradeTextObj;
    [SerializeField] Button[] _upgradeButtonArr;

    [Header("Potential")]
    [SerializeField] PotentialInfo[] _potentialInfoArr;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI _titleTMP;
    [SerializeField] GameObject[] _categoryBoxArr;       // upgrade, potential
    [SerializeField] Button[] _selectBoxButtonArr;

    // Data
    List<ItemPotentialData> _potenStatusList = new List<ItemPotentialData>();
    List<ItemPotentialData> _potenAbilityList = new List<ItemPotentialData>();

    PotentialPopup _potentialPopup;

    const string upgradeTitle = "Upgrade";
    const string potentialTitle = "Potential";

    #region Init
    public override void Init(LobbyUIManager uiM)
    {
        base.Init(uiM);
        _inven = uiManager.GetInvenPage;
        InitUpgradeBox();
        InitPotentialBox();
        InitPotentialList();
        _selectBoxButtonArr[(int)CategoryBoxType.UPGRADE].onClick.AddListener(() => OnTouchSelectButton((int)CategoryBoxType.UPGRADE));
        _selectBoxButtonArr[(int)CategoryBoxType.POTENTIAL].onClick.AddListener(() => OnTouchSelectButton((int)CategoryBoxType.POTENTIAL));

        _potentialPopup = uiManager.GetPopup<PotentialPopup>(PopupType.POTENTIAL);
        _potentialPopup.Init(this);
    }

    /// <summary>
    /// 업그레이드 항목 초기화
    /// </summary>
    void InitUpgradeBox()
    {
        _itemUpgrade = GetComponent<ItemUpgrade>();

        _currentStatus.Init(_inven.StatusSupporter);
        _nextStatus.Init(_inven.StatusSupporter);

        _upgradeButtonArr[0].onClick.AddListener(OnTouchUpgrade);
        _upgradeButtonArr[1].onClick.AddListener(OnTouchBatchUpgrade);
    }

    /// <summary>
    /// 포텐셜 항목 초기화
    /// </summary>
    void InitPotentialBox()
    {
        for(int i = 0; i < _potentialInfoArr.Length; i++)
        {
            _potentialInfoArr[i].Init(this, i);
        }
    }

    /// <summary>
    /// 포텐셜 리스트 초기화(분류)
    /// </summary>
    void InitPotentialList()
    {
        _potenStatusList.Clear();
        _potenAbilityList.Clear();

        var dataList = DataManager.GetInstance.GetList<ItemPotentialData>(DataManager.KEY_POTENTIAL);
        for(int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].abilityEnum == 0)
            {
                _potenStatusList.Add(dataList[i]);
            }
            else
            {
                _potenAbilityList.Add(dataList[i]);
            }
        }
    }

    /// <summary>
    /// 페이지 오픈 이전에 대상 아이템 지정(필수)
    /// </summary>
    /// <param name="target"></param>
    public void SetUpgradeTarget(UnitIcon target)
    {
        if (target == null) return;
        _targetIcon = target;
        _viewIcon.Init(_targetIcon);
        _itemUpgrade.Init(this, _targetIcon, _inven.GetMyMaterial());

        if(_targetIcon.GetItemType <= EItemList.DEFENSE)
        {
            _selectBoxButtonArr[(int)CategoryBoxType.POTENTIAL].gameObject.SetActive(false);
        }
        else
        {
            _selectBoxButtonArr[(int)CategoryBoxType.POTENTIAL].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 스테이터스 박스 초기화
    /// </summary>
    public void ResetStatusBox()
    {
        foreach (var status in _statusBoxConents)
        {
            status.SetActive(true);
        }
    }
    #endregion Init

    #region Update
    /// <summary>
    /// 업그레이드 정보 업데이트
    /// </summary>
    public void UpdateUpgradeInfo()
    {
        ClearRequiredMatInfo();

        // Max Level
        if (userData.UnitLevelMaxCheck(_targetIcon))
        {
            _statusBoxConents[(int)StatusBoxType.ARROW].SetActive(false);
            _statusBoxConents[(int)StatusBoxType.NEXT].SetActive(false);

            UpdateStatus(StatusBoxType.CURRENT);   // 스테이터스 업데이트
            SetUpgradeButtonState(false);      // 업그레이드 버튼 상태 OFF
            SetRequiredMatInfo(new int[8], -1, 0, EItemList.NONE); // 필요 광물 OFF

        }
        else
        {
            _statusBoxConents[(int)StatusBoxType.ARROW].SetActive(true);
            _statusBoxConents[(int)StatusBoxType.NEXT].SetActive(true);

            UpdateStatus(StatusBoxType.CURRENT);   // 스테이터스 업데이트
            UpdateStatus(StatusBoxType.NEXT);
            SetUpgradeButtonState(true);      // 업그레이드 버튼 상태 OFF
            
            // 필요광물 세팅
            int level = userData.UnitLevelCheck(_targetIcon.GetItemType, _targetIcon.GetDataIndex);
            SetRequiredMatInfo(_itemUpgrade.GetNeedMatArray(), level, _targetIcon.GetGrade, _targetIcon.GetItemType);
        }
        UpdateIconInfo();   // 아이콘 업데이트
        OnTouchSelectButton((int)CategoryBoxType.UPGRADE);
         //       SetMyMatInfo(); 내 광물
    }

    /// <summary>
    /// 아이콘 정보 업데이트
    /// </summary>
    void UpdateIconInfo()
    {
        _viewIcon.ChangeID(_targetIcon.GetID);
        _viewIcon.ChangeItemType(_targetIcon.GetItemType);
        _viewIcon.ChangeLevel(_targetIcon.GetLevel);
        _viewIcon.UpdateUISetting();
    }
    
    /// <summary>
    /// 장비 스텟 정보 업데이트
    /// </summary>
    void UpdateStatus(StatusBoxType type)
    {
        if (type == StatusBoxType.ARROW) return;
        else
        {
            if (type == StatusBoxType.CURRENT)
            {
                _currentStatus.UpdateStatusText(_targetIcon);
            }
            else
            {
                _nextStatus.UpdateStatusText(_targetIcon, true);
            }
        }
    }

    /// <summary>
    /// 상단 타이틀 텍스트 업데이트
    /// </summary>
    void UpdateTitleText(CategoryBoxType type)
    {
        switch (type)
        {
            case CategoryBoxType.UPGRADE:
                _titleTMP.text = upgradeTitle;
                break;
            case CategoryBoxType.POTENTIAL:
                _titleTMP.text = potentialTitle;
                break;
        }
    }

    public void UpdatePotential()
    {
        for (int i = 0; i < _potentialInfoArr.Length; i++)
        {
            _potentialInfoArr[i].SetInfoState(false);
        }

        EItemList type = _targetIcon.GetItemType;
        if(type == EItemList.CAPTAIN || type == EItemList.SAILOR || type == EItemList.ENGINE)
        {
            var potentialList = _targetIcon.GetPotentialList;
            for(int i = 0; i < potentialList.Count; i++)
            {
                _potentialInfoArr[i].SetInfo(potentialList[i]);
                _potentialInfoArr[i].SetInfoState(true);
            }
        }
    }
    /// <summary>
    /// UI 갱신 (페이지 외부 UI)
    /// </summary>
    public void RefreshUI()
    {
        LobbyUIManager.GetInstance.UpdateUserInfo();
        _targetIcon.UpdateUISetting();
        _targetIcon.LevelUpEdiorUpdate();
    }
    #endregion Update

    #region Potential
    /// <summary>
    /// 포텐셜 교체
    /// </summary>
    /// <param name="idx">해당 순번</param>
    /// <param name="id">포텐셜 ID</param>
    public void ChangePotential(int idx, int id)
    {
        List<int> potList = _targetIcon.GetPotentialList;
        potList[idx] = id;
        _targetIcon.ChangePotential(potList);

        UnitIcon boat = null;
        if(_targetIcon.GetIngerenceID != 0)
        {
            List<UnitIcon> boatList = GetUnitList(EItemList.BOAT);
            for(int i = 0; i < boatList.Count; i++)
            {
                if (boatList[i].GetIngerenceID == _targetIcon.GetIngerenceID)
                {
                    boat = boatList[i];
                    boat._playerInfo.ChangePotential(_targetIcon.GetItemType, potList);
                }
            }
        }
        userData.ChangePotential(_targetIcon, idx, boat);
    }

    /// <summary>
    /// 포텐셜 항목 정보 갱신
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="potenID"></param>
    public void RefreshPotentialInfo(int idx, int potenID)
    {
        _potentialInfoArr[idx].SetInfo(potenID);
    }

    /// <summary>
    /// 포텐셜 ID로 데이터 찾아주는 메서드
    /// </summary>
    public ItemPotentialData FindPotentialData(int potentialID)
    {
        return DataManager.GetInstance.FindData(DataManager.KEY_POTENTIAL, potentialID) as ItemPotentialData;
    }

    /// <summary>
    /// 랜덤한 포텐셜을 불러오는 메서드
    /// </summary>
    /// <param name="type">타입</param>
    /// <returns></returns>
    public ItemPotentialData GetRandomPotential(PotentialType type)
    {
        switch (type)
        {
            case PotentialType.Status:
                {
                    int randomIndex = Random.Range(0, _potenStatusList.Count);
                    return _potenStatusList[randomIndex];
                }
            case PotentialType.Ability:
                {
                    int randomIndex = Random.Range(0, _potenAbilityList.Count);
                    return _potenAbilityList[randomIndex];
                }
        }
        return null;
    }

    /// <summary>
    /// 해당 슬롯의 포텐셜 타입 반환
    /// </summary>
    /// <param name="idx">슬롯 번호</param>
    /// <returns></returns>
    public PotentialType GetPotentialType(int idx)
    {
        if (idx == 0)
        {
            return PotentialType.Status;
        }
        else
        {
            return PotentialType.Ability;
        }
    }

    public PotentialPopup GetPotentialPopup => _potentialPopup;
    #endregion Potential

    #region Materials
    /// <summary>
    /// 업그레이드 필요 재화 정보 초기화
    /// </summary>
    void ClearRequiredMatInfo()
    {
        foreach (var icon in _materialIconArr)
        {
            icon.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 업그레이드 광물 요구 수량 세팅
    /// </summary>
    public void SetRequiredMatInfo(int[] matArr, int level, int grade, EItemList type)
    {
        if (level == -1) return;
        int[] changedArr = new int[8];
        for (int i = 0; i < matArr.Length; i++)
        {
            if (matArr[i] > 0)
            {
                if (i == (int)EInvenType.Money)
                {
                    float money = matArr[i] * Mathf.Pow(StandardFuncData.ItemUpgradeMoneyCalculateMultiplier(grade), level - 1);
                    Color textColor = Color.white;
                    if(userData.GetCurrency(EInvenType.Money) < money)
                    {
                        textColor = Color.red;
                    }
                    _materialIconArr[i].SetIconInfo((int)money, textColor);
                    changedArr[i] = (int)money;
                }
                else
                {
                    float mat = matArr[i] * Mathf.Pow(StandardFuncData.ItemUpgradeCalculateMultiplier(type), level - 1);
                    changedArr[i] = (int)mat;
                    Color textColor = Color.white;
                    if (userData.GetCurrency((EInvenType)i) < changedArr[i])
                    {
                        textColor = Color.red;
                    }
                    _materialIconArr[i].SetIconInfo(changedArr[i], textColor);
                    //var matData = DataManager.GetInstance.FindData(DataManager.KEY_UPGRADE, level) as UpgradeData;
                    //if (userData.GetCurrency((EInvenType)i) < matData.cost)
                    //{
                    //    textColor = Color.red;
                    //}
                    //_materialIconArr[i].SetIconInfo(matData.cost, textColor);
                    //changedArr[i] = matData.cost;
                }
                _materialIconArr[i].gameObject.SetActive(true);
            }
        }
        _itemUpgrade.SetNeedMaterials(changedArr);
    }
    

    #endregion Materials

    #region Buttons
    /// <summary>
    /// 업그레이드 버튼 상태 설정
    /// </summary>
    /// <param name="state">상태</param>
    public void SetUpgradeButtonState(bool state)
    {
        _upgradeButtonArr[0].gameObject.SetActive(state);
        _upgradeButtonArr[1].gameObject.SetActive(state);
        _unableUpgradeTextObj.SetActive(!state);
    }

    public void OnTouchUpgrade()
    {
        _itemUpgrade.Upgrade();
    }

    public void OnTouchBatchUpgrade()
    {
        _itemUpgrade.BatchUpgrade();
    }

    public void OnTouchSelectButton(int idx)
    {
        for(int i = 0; i < _selectBoxButtonArr.Length; i++)
        {
            if(i == idx)
            {
                UpdateTitleText((CategoryBoxType)i);
                _categoryBoxArr[i].SetActive(true);
            }
            else
            {
                _categoryBoxArr[i].SetActive(false);
            }
        }
    }

    #endregion Buttons

    #region Functions
    public override void OpenPage()
    {
        ResetStatusBox();
        UpdateUpgradeInfo();
        UpdatePotential();
        UpdateTitleText(CategoryBoxType.UPGRADE);
        base.OpenPage();
    }

    public override void ClosePage()
    {
        SortInventory();
        ResetPotentialInfo();
        if(_targetIcon != null && _targetIcon.GetItemType == EItemList.BOAT)
        {
            _inven.UpdateBoatInfoTitle();
        }
        base.ClosePage();
    }

    /// <summary>
    /// 포텐셜 각 항목 UI 초기화
    /// </summary>
    void ResetPotentialInfo()
    {
        for(int i = 0; i < _potentialInfoArr.Length; i++)
        {
            _potentialInfoArr[i].ResetInfo();
        }
    }

    /// <summary>
    /// 업그레이드 이후 인벤 정렬
    /// </summary>
    void SortInventory()
    {
        if (_inven.SearchType != EItemList.NONE)
        {
            var category = _inven.FindInvenCategory(_inven.SearchType);
            if (category.GetItemCount() <= 0)
            {
                _inven.ShowAllCategory();
            }
        }
        else
        {
            _inven.ShowAllCategory();
        }
        if (_targetIcon != null)
        {
            _inven.SortInven(_targetIcon.GetItemType);
        }
    }

    /// <summary>
    /// 해당 항목 리스트 서치
    /// </summary>
    /// <param name="type">타입</param>
    /// <returns></returns>
    public List<UnitIcon> GetUnitList(EItemList type)
    {
        if (type == EItemList.NONE || type == EItemList.MATERIAL) return null;
        return _inven.GetInven(type);
    }
    #endregion Functions
}
