using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UnitIcon : MonoBehaviour, IComparable
{
    [Header("Components")]
    UnitIconUI _iconUI;

    // 공통적인 정보
    [Header("Info")]
    [SerializeField] int _id;
    [SerializeField] int _level;
    [SerializeField] EItemList _itemType;
    [SerializeField] int _ingerenceId; // 고유번호 (보트고유번호 이지만 다른 착용물들은 어느 보트에 종속되어있나 알기 위한 변수)
    [SerializeField] int _hierarchyIdx; // 유니티 에디터 상의 자리 배치
    [SerializeField] int _dataIdx; // 실제 데이터 배열 자리
    bool _wear = false; // 인벤 차제에 착용하고 있는 여부
    bool _wearReally = false; // 실제로 착용 확인 (보트는 적용 안됨)

    [Header("UI")]
    public GameObject _check; // 장착중일때는 체크 혹은 선택창일때는 좌표
    RectTransform _myEditorPos; // 장비 장착 아이템
    RectTransform _myTrans;
    GameObject _myObj;

    // 장비 장착할때 보여주기 아이콘
    UnitIcon _myEdiorUnitIcon;
    Button _myEditorBtn;
    GameObject _myPrefabsObj;

    InvenScript _inven;
    UnitIcon _originalData;
    Button _myBtn;

    public UnitIcon GetOriginalData { get { return _originalData; } set { _originalData = value; } }
    public Button GetMyBtn { get { if (_myBtn == null) { _myBtn = this.GetComponent<Button>(); } return _myBtn; } }

    // 보트 전용
    // 보트 무기 착용 여부
    public Info.PlayerInfo _playerInfo;
    GameObject _boatBasicWeapon;

    // 로비에 보트 업그레이드 버튼
    Button _invenBoatUpgradeBtn;

    // 합성페이지에서 사용 - 클릭 못하는 것에 대한 블러 처리용
    [SerializeField] GameObject _synthesisBlurObj;

    // 선장, 선원, 갑판장
    [SerializeField] List<int> _potentialList = new List<int>();

    #region Init
    /// <summary>
    /// 기본적인 초기화 메서드
    /// </summary>
    public void Init(int id, int level, int arr, EItemList itemType, RectTransform edior, InvenScript script, int ingerenceId = 0)
    {
        // Components
        _iconUI = GetComponent<UnitIconUI>();
        _iconUI.Init(this);
        _myTrans = GetComponent<RectTransform>();
        _myObj = gameObject;

        // Data
        _id = id;
        _level = level;
        _dataIdx = arr;
        _itemType = itemType;
        if (edior == null) Debug.LogError("Editor is NULL");
        _myEditorPos = edior;
        _inven = script;
        _ingerenceId = ingerenceId;

        // UI
        UpdateUISetting();

        if (_itemType == EItemList.BOAT)
        {
            UserData userData = UserData.GetInstance;
            _playerInfo = new Info.PlayerInfo(userData.FindCoalescence(_ingerenceId));
            List<string> potentials = userData.FindPotential(_ingerenceId);
            if (potentials.Count == 3)
            {
                _playerInfo.InitPotential(potentials[0], potentials[1], potentials[2]);
            }
            _invenBoatUpgradeBtn = _inven.GetBoatUpgradeBtn;
        }
        else
        {
            // 장착될 위치 지정 (상단)
            InvenIcon icon = _myEditorPos.transform.GetComponent<InvenIcon>();
            _myEdiorUnitIcon = icon.GetUnitIcon;
            _myEditorBtn = _myEdiorUnitIcon.GetComponent<Button>();
        }

        if (_itemType == EItemList.CAPTAIN || _itemType == EItemList.SAILOR || _itemType == EItemList.ENGINE)
        {
            if (_potentialList.Count <= 0)
            {
                // 포텐셜 정보가 없는 경우 새로 생성
                string potential = _inven.CreateEmptyPotential(GetGrade);
                _potentialList = potential.Split(',').Select(int.Parse).ToList();
            }
        }
    }

    /// <summary>
    /// 포텐셜을 가진 선장, 선원, 갑판장 전용 초기화 메서드
    /// </summary>
    public void Init(int id, int level, int arr, EItemList itemType, RectTransform editor, InvenScript script, string idList)
    {
        if (idList.Length > 0)
        {
            _potentialList = idList.Split(',').Select(int.Parse).ToList();
        }

        // 기존 초기화 메서드 사용하여 초기화
        Init(id, level, arr, itemType, editor, script, 0);
    }

    /// <summary>
    /// 복사 전용
    /// </summary>
    public void Init(UnitIcon unitIcon)
    {
        _myTrans = GetComponent<RectTransform>();
        _iconUI = GetComponent<UnitIconUI>();
        _iconUI.Init(this);
        _iconUI.CopyUISetting(unitIcon._iconUI);

        _id = unitIcon.GetID;
        _level = unitIcon.GetLevel;
        _dataIdx = unitIcon.GetDataIndex;
        _itemType = unitIcon.GetItemType;
        _ingerenceId = unitIcon.GetIngerenceID;
        _wear = unitIcon.GetWearState;
        _wearReally = unitIcon.GetWearReally;
        _inven = unitIcon.GetInven;
        _myObj = gameObject;

        if (unitIcon._itemType == EItemList.BOAT)
        {
            _playerInfo = unitIcon._playerInfo;
            _invenBoatUpgradeBtn = unitIcon.GetBoatUpgradeBtn;
        }
        else if (unitIcon._itemType == EItemList.CAPTAIN || unitIcon._itemType == EItemList.SAILOR || unitIcon._itemType == EItemList.ENGINE)
        {
            _potentialList = unitIcon._potentialList;
        }
    }
    #endregion Init

    /// <summary>
    /// 인벤토리에서 아이콘 선택
    /// 보트는 바로 장착(업그레이드 버튼 선택 시)
    /// </summary>
    public void SelectInvenIcon()
    {
        if (_inven.IsSearching)
        {
            _inven.CloseSearchBoard();
        }
        _inven._unitUpgrade = this;
        var popup = _inven.ItemPopup;
        popup.SetViewIcon(this);
        popup.SetViewIconList(_inven.GetInven(_itemType));
        popup.SetInvenSetting();
        popup.OpenPopup();
    }

    /// <summary>
    /// 합성 팝업의 아래 아이콘
    /// </summary>
    public void SelectSynthesis()
    {
        var popup = _inven.ItemPopup;
        popup.SetViewIcon(this);
        popup.SetViewIconList(LobbyUIManager.GetInstance.GetPopup<SynthesisPopup>(PopupType.SYNTHESIS).GetActivatedIconList());
        popup.SetSynthesisSetting();
        popup.OpenPopup();
    }

    /// <summary>
    /// 장비 장착 / 해제
    /// </summary>
    public void IconEditor()
    {
        if (_itemType == EItemList.BOAT)
        {
            _inven._unitUpgrade = this;
        }
        if (_itemType == EItemList.BOAT && _wear) { /*Debug.Log("보트는 빠지지 않는다 다만 체크될뿐");*/ return; }
        if (_itemType == EItemList.WEAPON && _wear) { /*Debug.Log("기본무기로 체인지");*/ _inven.BasicWeaponChange = true; } // 단일로 그냥 무기가 빼지는 거니 기본무기로 변경
        if (_itemType == EItemList.BOAT && !_wear) { /*Debug.Log("단일 보트 세이브 못함");*/ _inven.NotSave = true; }
        // 입기
        if (!_wear)
        {
            WearEditor(true);
        }
        // 벗기
        else
        {
            WearEditor(false);
        }
        _inven.CheckInvenIconState();
        _inven.BoatInvenSave();
    }

    // 착용하는거? ok:true 벗는거 no:false
    public void WearEditor(bool ok, bool boat = false)
    {
        if (ok)
        {
            if (_itemType != EItemList.BOAT && _inven._boatObj == null) { MessageHandler.GetInstance.ShowMessage("보트가 선택되지 않았습니다", 1.5f); _inven.NotSave = true; return; }

            // 보트 파츠 체크
            if (_itemType == EItemList.BOAT)
            {
                // 만약 입기 전에 전에 같은 타입이 있으면 그것은 벗어주기
                if (_inven._editorObj[(int)_itemType] != null)
                {
                    _inven.NotSave = true; // 보트 교체이기 때문에 세이브는 굳이 안해줘도된다
                    _inven._editorObj[(int)_itemType].WearEditor(false);
                }

                _wear = true;
                _check.gameObject.SetActive(true);
                if (CheckList()) { _myPrefabsObj = _inven.CreatePrefabsObj(_id, _itemType, GetGrade); }
                _inven._editorObj[(int)_itemType] = this;

                // 착용 되어있는 아이템 확인하고 장착
                int[] arr = UserData.GetInstance.FindCoalescence(_ingerenceId);
                int arr1 = (int)ECoalescenceType.WEAPON_ID;
                int arr2 = (int)ECoalescenceType.WEAPON_LEVEL;
                for (int i = (int)EItemList.WEAPON; i <= (int)EItemList.ENGINE; i++)
                {
                    var targetList = _inven.GetInven((EItemList)i);
                    for (int j = 0; j < targetList.Count; j++)
                    {
                        if (CheckBoatParts(j, arr, (EItemList)i, arr1, arr2))
                        {
                            break;
                        }
                    }
                    arr1 += 2;
                    arr2 += 2;
                }

                // 보트에 무기가 없을시 기본 무기 보여주기
                if (arr[2] == 0)
                {
                    _boatBasicWeapon = _inven.CreatePrefabsObj(_inven._editorObj[0].GetBasicWeaponType, EItemList.WEAPON, GetGrade);
                }

                if (_invenBoatUpgradeBtn != null)
                {
                    _invenBoatUpgradeBtn.onClick.AddListener(SelectInvenIcon);
                }
                else { Debug.Log("_invenBoatUpgradeBtn = null"); }
                _inven.UpdateBoatInfoTitle();
                //_inven.InitContentScrollView(); // 스크롤 위로 올려주기
                return;
            }
            else
            {
                // 교체
                if (_inven._editorObj[(int)_itemType] != null)
                {
                    // 입기 전에 전에 같은 타입이 있으면 그것은 벗어주기
                    _inven._editorObj[(int)_itemType].WearEditor(false);
                }
            }

            _wear = true;
            _myObj.SetActive(false);
            EditorIconUpdate(true);
            // 무기 없던거였으면 기본무기 끼고있던 보트 무기 이미지 없애주기
            if (_itemType == EItemList.WEAPON)
            {
                if (_inven._editorObj[(int)EItemList.BOAT]._boatBasicWeapon != null)
                {
                    Destroy(_inven._editorObj[(int)EItemList.BOAT]._boatBasicWeapon);
                    _inven._editorObj[(int)EItemList.BOAT]._boatBasicWeapon = null;
                }
            }
            if (CheckList()) { _myPrefabsObj = _inven.CreatePrefabsObj(_id, _itemType, GetGrade); }
            _inven._editorObj[(int)_itemType] = this;
            if (_itemType > EItemList.DEFENSE)
            {
                _check.gameObject.SetActive(false);
                _wearReally = true;

                var potentials = UserData.GetInstance.FindPotential(_inven._editorObj[(int)EItemList.BOAT].GetIngerenceID.ToString());
                if (potentials != null)
                {
                    UserData ud = UserData.GetInstance;
                    potentials[(int)GetItemType - 3] = GetPotentialString;
                    int idx = ud.FindBoatIndex(_inven._editorObj[(int)EItemList.BOAT].GetIngerenceID.ToString());
                    if (idx == -1)
                    {
                        Debug.Log("Boat Data Error");
                    }
                    else
                    {
                        ud.GetCoalescenceData._potentials[idx] = string.Join("_", potentials);
                    }
                }
            }
            //_inven.InitContentScrollView();// 스크롤 위로 올려주기
        }
        else
        {
            _wear = false;
            if (_itemType != EItemList.BOAT)
            {
                _myObj.SetActive(true);
                EditorIconUpdate(false);
            }
            if (_itemType == EItemList.BOAT)
            {
                // 단일로 뺄때도 있을꺼니깐 세이브 하면안됨
                _inven.NotSave = true;
                // 보트가 벗어지면 다 벗어져야한다 (하지만 세이브된 보트면 나중에 다시 장착될때 다 장착되어야함)
                for (int i = 0; i < _inven._editorObj.Length; ++i)
                {
                    if (_inven._editorObj[i] != null)
                    {
                        if (_inven._editorObj[i]._itemType != EItemList.BOAT)
                        {
                            _inven._editorObj[i].WearEditor(false, true);
                        }
                    }
                }
                _inven._boatObj = null;
                _check.gameObject.SetActive(false);
                if (_invenBoatUpgradeBtn != null)
                {
                    _invenBoatUpgradeBtn.onClick.RemoveAllListeners();
                }
            }
            else
            {
                if (!boat) { _ingerenceId = 0; _check.gameObject.SetActive(false); _wearReally = false; }
                else { _myObj.SetActive(false); _check.gameObject.SetActive(true); _wearReally = true; }
            }

            //if (_eitemList != EItemList.BOAT) { MyAnchorSetting(false); }
            if (CheckList())
            {
                if (_itemType == EItemList.DEFENSE) // 방패는 다른 곳에도 있을수 있으니 체크해서 뺀다
                {
                    EDefensePoint t = (EDefensePoint)GetDefensePoint;
                    if (t == EDefensePoint.FRONT || t == EDefensePoint.BACK || t == EDefensePoint.LEFT || t == EDefensePoint.RIGHT) { Destroy(_myPrefabsObj); }
                    else if (t == EDefensePoint.FRONT_BACK)
                    {
                        EDefensePoint[] point = { EDefensePoint.FRONT, EDefensePoint.BACK };
                        DefenseObjDestory(2, point);
                    }
                    else if (t == EDefensePoint.LEFT_RIGHT)
                    {
                        EDefensePoint[] point = { EDefensePoint.LEFT, EDefensePoint.RIGHT };
                        DefenseObjDestory(2, point);
                    }
                    else if (t == EDefensePoint.FRONT_LEFT_RIGHT)
                    {
                        EDefensePoint[] point = { EDefensePoint.FRONT, EDefensePoint.LEFT, EDefensePoint.RIGHT };
                        DefenseObjDestory(3, point);
                    }
                    else if (t == EDefensePoint.ALL)
                    {
                        EDefensePoint[] point = { EDefensePoint.FRONT, EDefensePoint.BACK, EDefensePoint.LEFT, EDefensePoint.RIGHT };
                        DefenseObjDestory(4, point);
                    }
                    _myPrefabsObj = null;
                }
                else { Destroy(_myPrefabsObj); }
            }
            if (_inven.BasicWeaponChange) // 그냥 무기를 빼면 들어와야해
            {
                _inven.BasicWeaponChange = false;
                // 무기를 뺄때 무기교체가 아닌 그냥 쌩자로 빼는거면 보트의 기본 무기로 변경해야함
                if (EItemList.WEAPON == _itemType)
                {
                    _inven._editorObj[(int)EItemList.BOAT]._boatBasicWeapon = _inven.CreatePrefabsObj(_inven._editorObj[0].GetBasicWeaponType, EItemList.WEAPON, GetGrade);
                }
            }
            _inven._editorObj[(int)_itemType] = null;
        }

        //_inven.InitContentScrollView();// 스크롤 위로 올려주기
        if (_itemType != EItemList.BOAT)
        {
            _inven._unitUpgrade = null;
            _inven.RefreshInvenScrollView();
        }
    }

    void DefenseObjDestory(int index, EDefensePoint[] point)
    {
        for (int i = 0; i < index; ++i)
        {
            GameObject obj = _myPrefabsObj.transform.parent.transform.parent.GetChild((int)point[i]).transform.GetChild(0).gameObject;
            Destroy(obj);
        }
    }

    /// <summary>
    /// 보트에 장착된 파츠 체크
    /// </summary>
    bool CheckBoatParts(int index, int[] arr, EItemList itemList, int arr1, int arr2)
    {
        var iconList = _inven.GetInven(itemList);
        if (iconList[index]._id == arr[arr1] && iconList[index]._level == arr[arr2] && _ingerenceId == iconList[index].GetIngerenceID)
        {
            iconList[index].gameObject.SetActive(true);
            iconList[index].WearEditor(true);
            return true;
        }
        return false;
    }

    bool CheckList()
    {
        if (_itemType == EItemList.BOAT || _itemType == EItemList.WEAPON || _itemType == EItemList.DEFENSE) { return true; }
        else { return false; }
    }

    public void EditorIconUpdate(bool on)
    {
        if (LobbyUIManager.GetInstance.GetPopup<SynthesisPopup>(PopupType.SYNTHESIS).IsOpened)
        {
            return;
        }
        _myEdiorUnitIcon.gameObject.SetActive(on);
        if (on)
        {
            _myEditorBtn.onClick.AddListener(SelectInvenIcon);
            _myEdiorUnitIcon.GetIconUI().CopyUISetting(_iconUI);
            _myObj.SetActive(false);
        }
        else
        {
            _myEditorBtn.onClick.RemoveAllListeners();
            _myObj.SetActive(true);
        }
    }

    /// <summary>
    /// 팀 선택창의 버튼
    /// </summary>
    public void BoatPositionSelectFunction()
    {
        if (_inven.SelectPositionCheck() != null)
        {
            // 포지션에서 선택했을때 들어오는 함수
            UserData.GetInstance.BoatTemp = this;
            _inven.SelectPositionBtnAction();
        }
        else
        {
            // 보관하고 있는 전체 데이터
            // 이것으로 자리 배치할때 들어가는 이미지, 데이터 정보 등 건네주기
            UserData.GetInstance.BoatTemp = this;
            LobbyUIManager.GetInstance.GetReadyPage.SetBoatListBlockerState(true);
        }
    }

    #region UI Update
    public void LevelUpEdiorUpdate()
    {
        if (!_wear) { return; }
        if (_myEdiorUnitIcon != null)
        {
            _myEdiorUnitIcon.UpdateLevelText(_level);
            _myEdiorUnitIcon.UpdateGradeText(GetGrade);
        }
    }

    /// <summary>
    /// Icon UI 통합 업데이트
    /// </summary>
    public void UpdateUISetting() => _iconUI.UpdateUISetting();

    /// <summary>
    /// 레벨 텍스트 업데이트
    /// </summary>
    public void UpdateLevelText(int level = 0) => _iconUI.UpdateLevelText(level);
    public void UpdateNameText(string name) => _iconUI.UpdateLevelText(name);

    /// <summary>
    /// 등급 텍스트 업데이트
    /// </summary>
    /// <param name="grade"></param>
    public void UpdateGradeText(int grade = 0) { if (_iconUI == null) { _iconUI = GetComponent<UnitIconUI>(); } _iconUI.UpdateGradeText(grade); }

    /// <summary>
    /// 등급 OutLine 업데이트
    /// </summary>
    public void UpdateGradeOutLine(int grade = 0) { if (_iconUI == null) { _iconUI = GetComponent<UnitIconUI>(); } _iconUI.UpdateGradeOutLine(grade); }

    /// <summary>
    /// 아이템 이미지 업데이트
    /// </summary>
    public void UpdateIconSprite(Sprite iconSprite = null) => _iconUI.UpdateIconSprite(iconSprite);
    #endregion UI Update

    public void ChangeID(int id) => _id = id;
    public void ChangeLevel(int level) => _level = level;
    public void ChangeItemType(EItemList type) => _itemType = type;
    public void ChangeHierarchyIndex(int idx) => _hierarchyIdx = idx;
    public void ChangeDataIndex(int idx) => _dataIdx = idx;
    public void ChangeIngerenceID(int id) => _ingerenceId = id;
    public void ChangePotential(List<int> poten) => _potentialList = poten;
    public void SetWearState(bool state) => _wear = state;
    public void SetWearReally(bool state) => _wearReally = state;

    public GameObject GetObj { get { return _myObj; } }
    public RectTransform GetTrans { get { if (_myTrans == null) { _myTrans.GetComponent<RectTransform>(); } return _myTrans; } }
    public InvenScript GetInven { get { return _inven; } }
    public UnitIcon GetMyEdiorUnitIcon { get { return _myEdiorUnitIcon; } }

    public GameObject GetMyPrefabsObj { get { return _myPrefabsObj; } }
    public Button GetBoatUpgradeBtn { get { return _invenBoatUpgradeBtn; } }
    public GameObject GetSynthesisBlurObj()
    {
        return _synthesisBlurObj;
    }

    #region Item Values
    public UnitIconUI GetIconUI()
    {
        if (_iconUI == null)
        {
            _iconUI = GetComponent<UnitIconUI>();
            _iconUI.Init(this);
        }
        return _iconUI;
    }
    public int GetID => _id;
    public int GetLevel => _level;
    public EItemList GetItemType => _itemType;
    public int GetHierarchyIndex => _hierarchyIdx;
    public int GetDataIndex => _dataIdx;
    public int GetIngerenceID => _ingerenceId;
    public bool GetWearState => _wear;
    public bool GetWearReally => _wearReally;
    public List<int> GetPotentialList => _potentialList;
    public string GetPotentialString => string.Join(",", _potentialList);
    public int GetPotential(int idx) => _potentialList[idx];

    public int GetMaxLevel => UnitDataManager.GetInstance.GetIntValue(_itemType, _id, StatusType.maxLevel);
    public string GetName => UnitDataManager.GetInstance.GetStringValue(_itemType, _id, StatusType.name);
    public List<int> GetNeedMatAmount => UnitDataManager.GetInstance.GetIntListValue(_itemType, _id, StatusType.needMatAmount);
    public int GetDefenseType => UnitDataManager.GetInstance.GetIntValue(_itemType, _id, StatusType.defenseType);
    public int GetGrade => UnitDataManager.GetInstance.GetIntValue(_itemType, _id, StatusType.grade);
    public int GetEquipType => UnitDataManager.GetInstance.GetIntValue(_itemType, _id, StatusType.equipType);
    public int GetBasicWeaponType => UnitDataManager.GetInstance.GetIntValue(_itemType, _id, StatusType.basicWeaponType);
    public int GetWeaponType => UnitDataManager.GetInstance.GetIntValue(_itemType, _id, StatusType.weaponType);
    public float GetDefensive => UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.defensive);
    public float GetMoveSpeed => UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.moveSpeed);
    public float GetShootingRange => UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.shootingRange);
    public float GetFireRate => UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.fireRate);
    public string GetResName => UnitDataManager.GetInstance.GetStringValue(_itemType, _id, StatusType.resName);
    public int GetDefensePoint => UnitDataManager.GetInstance.GetIntValue(_itemType, _id, StatusType.defensePoint);
    public float GetRealValue()
    {
        float value = 0;
        switch (_itemType)
        {
            case EItemList.BOAT:
                value = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.hp);
                break;
            case EItemList.WEAPON:
                value = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.damage);
                break;
            case EItemList.DEFENSE:
                value = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.value);
                break;
            case EItemList.CAPTAIN:
                value = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.damage);
                break;
            case EItemList.SAILOR:
                value = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.value);
                break;
            case EItemList.ENGINE:
                value = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.value);
                break;
        }
        return value;
    }
    public float GetServeValue()
    {
        float value = 0;
        switch (_itemType)
        {
            case EItemList.BOAT:
                value = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.damage);
                break;
        }
        return value;
    }

    public float GetAddValue()
    {
        float addValue = 0;
        switch (_itemType)
        {
            case EItemList.BOAT:
                addValue = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.addHp);
                break;
            case EItemList.WEAPON:
                addValue = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.addDamage);
                break;
            case EItemList.DEFENSE:
                addValue = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.addValue);
                break;
            case EItemList.CAPTAIN:
                addValue = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.addDamage);
                break;
            case EItemList.SAILOR:
                addValue = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.addValue);
                break;
            case EItemList.ENGINE:
                addValue = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.addValue);
                break;
        }
        return addValue;
    }
    public float GetAddServeValue()
    {
        float addValue = 0;
        switch (_itemType)
        {
            case EItemList.BOAT:
                addValue = UnitDataManager.GetInstance.GetFloatValue(_itemType, _id, StatusType.addDamage);
                break;
        }
        return addValue;
    }
    public float GetValue => GetRealValue() + ((_level - 1) * GetAddValue());
    public float GetTwoValue => GetServeValue() + ((_level - 1) * GetAddServeValue());
    public float GetNextLevelValue => GetRealValue() + (_level * GetAddValue());
    public float GetNextTwoLevelValue => GetServeValue() + (_level * GetAddServeValue());
    #endregion Item Values

    /// <summary>
    /// 등급, 타입, 레벨에 따라 내림차순 처리
    /// </summary>
    /// <returns></returns>
    public int CompareTo(object obj)
    {
        UnitIcon targetIcon = (UnitIcon)obj;
        int myGrade = GetGrade;
        int myType = GetEquipType;
        int myLevel = GetLevel;

        int tGrade = targetIcon.GetGrade;
        int tType = targetIcon.GetEquipType;
        int tLevel = targetIcon.GetLevel;

        if (myGrade > tGrade)
        {
            return -1;
        }
        else if (myGrade == tGrade)
        {
            if (myType > tType)
            {
                return -1;
            }
            else if (myType == tType)
            {
                if (myLevel > tLevel)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }
        else
        {
            return 1;
        }
    }
}