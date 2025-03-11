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
    /// ���׷��̵� �׸� �ʱ�ȭ
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
    /// ���ټ� �׸� �ʱ�ȭ
    /// </summary>
    void InitPotentialBox()
    {
        for(int i = 0; i < _potentialInfoArr.Length; i++)
        {
            _potentialInfoArr[i].Init(this, i);
        }
    }

    /// <summary>
    /// ���ټ� ����Ʈ �ʱ�ȭ(�з�)
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
    /// ������ ���� ������ ��� ������ ����(�ʼ�)
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
    /// �������ͽ� �ڽ� �ʱ�ȭ
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
    /// ���׷��̵� ���� ������Ʈ
    /// </summary>
    public void UpdateUpgradeInfo()
    {
        ClearRequiredMatInfo();

        // Max Level
        if (userData.UnitLevelMaxCheck(_targetIcon))
        {
            _statusBoxConents[(int)StatusBoxType.ARROW].SetActive(false);
            _statusBoxConents[(int)StatusBoxType.NEXT].SetActive(false);

            UpdateStatus(StatusBoxType.CURRENT);   // �������ͽ� ������Ʈ
            SetUpgradeButtonState(false);      // ���׷��̵� ��ư ���� OFF
            SetRequiredMatInfo(new int[8], -1, 0, EItemList.NONE); // �ʿ� ���� OFF

        }
        else
        {
            _statusBoxConents[(int)StatusBoxType.ARROW].SetActive(true);
            _statusBoxConents[(int)StatusBoxType.NEXT].SetActive(true);

            UpdateStatus(StatusBoxType.CURRENT);   // �������ͽ� ������Ʈ
            UpdateStatus(StatusBoxType.NEXT);
            SetUpgradeButtonState(true);      // ���׷��̵� ��ư ���� OFF
            
            // �ʿ䱤�� ����
            int level = userData.UnitLevelCheck(_targetIcon.GetItemType, _targetIcon.GetDataIndex);
            SetRequiredMatInfo(_itemUpgrade.GetNeedMatArray(), level, _targetIcon.GetGrade, _targetIcon.GetItemType);
        }
        UpdateIconInfo();   // ������ ������Ʈ
        OnTouchSelectButton((int)CategoryBoxType.UPGRADE);
         //       SetMyMatInfo(); �� ����
    }

    /// <summary>
    /// ������ ���� ������Ʈ
    /// </summary>
    void UpdateIconInfo()
    {
        _viewIcon.ChangeID(_targetIcon.GetID);
        _viewIcon.ChangeItemType(_targetIcon.GetItemType);
        _viewIcon.ChangeLevel(_targetIcon.GetLevel);
        _viewIcon.UpdateUISetting();
    }
    
    /// <summary>
    /// ��� ���� ���� ������Ʈ
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
    /// ��� Ÿ��Ʋ �ؽ�Ʈ ������Ʈ
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
    /// UI ���� (������ �ܺ� UI)
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
    /// ���ټ� ��ü
    /// </summary>
    /// <param name="idx">�ش� ����</param>
    /// <param name="id">���ټ� ID</param>
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
    /// ���ټ� �׸� ���� ����
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="potenID"></param>
    public void RefreshPotentialInfo(int idx, int potenID)
    {
        _potentialInfoArr[idx].SetInfo(potenID);
    }

    /// <summary>
    /// ���ټ� ID�� ������ ã���ִ� �޼���
    /// </summary>
    public ItemPotentialData FindPotentialData(int potentialID)
    {
        return DataManager.GetInstance.FindData(DataManager.KEY_POTENTIAL, potentialID) as ItemPotentialData;
    }

    /// <summary>
    /// ������ ���ټ��� �ҷ����� �޼���
    /// </summary>
    /// <param name="type">Ÿ��</param>
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
    /// �ش� ������ ���ټ� Ÿ�� ��ȯ
    /// </summary>
    /// <param name="idx">���� ��ȣ</param>
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
    /// ���׷��̵� �ʿ� ��ȭ ���� �ʱ�ȭ
    /// </summary>
    void ClearRequiredMatInfo()
    {
        foreach (var icon in _materialIconArr)
        {
            icon.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ���׷��̵� ���� �䱸 ���� ����
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
    /// ���׷��̵� ��ư ���� ����
    /// </summary>
    /// <param name="state">����</param>
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
    /// ���ټ� �� �׸� UI �ʱ�ȭ
    /// </summary>
    void ResetPotentialInfo()
    {
        for(int i = 0; i < _potentialInfoArr.Length; i++)
        {
            _potentialInfoArr[i].ResetInfo();
        }
    }

    /// <summary>
    /// ���׷��̵� ���� �κ� ����
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
    /// �ش� �׸� ����Ʈ ��ġ
    /// </summary>
    /// <param name="type">Ÿ��</param>
    /// <returns></returns>
    public List<UnitIcon> GetUnitList(EItemList type)
    {
        if (type == EItemList.NONE || type == EItemList.MATERIAL) return null;
        return _inven.GetInven(type);
    }
    #endregion Functions
}
