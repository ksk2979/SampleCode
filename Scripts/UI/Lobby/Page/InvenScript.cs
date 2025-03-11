using MyData;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 개별 카테고리를 컨트롤하는 인벤 클래스
/// 생성 이후의 아이템 관리는 카테고리에서 처리
/// </summary>
public class InvenScript : PageBase
{
    ItemStatusSupporter _statusSupporter;

    [Header("Category")]
    [SerializeField] InvenCategory[] _invenCategoryArr;
    Dictionary<EItemList, InvenCategory> _invenDictionary;

    [Header("Scroll")]
    [SerializeField] RectTransform _contentPos;
    [SerializeField] RectTransform _heightPos;
    [SerializeField] GameObject _searchBoardObj;        // 검색용 버튼 셋
    bool _isSearching = false;

    [Header("Popup")]
    ItemPopup _itemPopup;
    SynthesisPopup _synthesisPopup;

    [Header("Buttons")]
    [SerializeField] Button _boatUpgradeBtn;
    [SerializeField] Button _showSpecButton;
    [SerializeField] Button[] _categoryBtnArr;
    [SerializeField] TextLocalizeSetter _searchButtonText;
    // 유닛 선택된것 있으면 있는것이 있으면 지우고?
    public Transform _createUnitRawImagePosition;

    // 선택된 보트가 있으면 무기, 방어는 그 안에 추가가 되어야한다
    [HideInInspector] public GameObject _boatObj;

    // 유닛 장비 창
    [Header("Equip")]
    [SerializeField] InvenIcon[] _invenIconArr;
    RectTransform[] _editorPos; // 0: 보트, 1:무기, 2:방어, 3:캡틴, 4:선원, 5:엔진 (0보트는 이 값을 사용해서 업그레이드를 함)
    public UnitIcon[] _editorObj; // 장비 착용중인거 체크 (보트가 없어지면 전부 벗어야하기 때문에 모아놔야함)
    [SerializeField] TextLocalizeSetter _boatInfoText;

    // 업그레이드 될 녀석 체크
    [HideInInspector] public UnitIcon _unitUpgrade;

    bool _notSave = false;
    bool _basicWeaponChange = false; // 무기에만 해당되는 것 (다른 보트에서 이쪽 보트로 무기 교체를 할때 필요)

    // 합성 관련 데이터
    bool _mainWear = false;

    // 보트 업그레이트 버튼
    [SerializeField] ImageRotater _imageRotater;
    const string StrSearch = "검색";
    [SerializeField] EItemList _searchType = EItemList.NONE;

    #region Page
    public override void OpenPage()
    {
        OnTouchCategoryButton(0);
        CloseSearchBoard();
        CheckCategoryButtonState();
        InitContentScrollView();
        UpdateBoatInfoTitle();
        base.OpenPage();
    }
    public override void ClosePage()
    {
        base.ClosePage();
    }

    public void UpdateBoatInfoTitle()
    {
        _boatInfoText.key = string.Format("Class {0} Lv.{1:D2}\n{2}", StandardFuncData.GradeCheck(_editorObj[0].GetGrade, true), _editorObj[0].GetLevel, _editorObj[0].GetName);
    }
    #endregion Page

    #region Init
    public override void Init(LobbyUIManager uiM)
    {
        base.Init(uiM);
        _invenDictionary = new Dictionary<EItemList, InvenCategory>();
        _statusSupporter = new ItemStatusSupporter();
        InitPopup();

        _imageRotater.Init(uiM.GetTargetRotation());

        for (int i = 0; i < _invenCategoryArr.Length; i++)
        {
            _invenCategoryArr[i].Init(this);
            _invenDictionary.Add(_invenCategoryArr[i].ItemListType, _invenCategoryArr[i]);
        }

        InitIcons();
        InitButtons();
    }

    /// <summary>
    /// 팝업 초기화
    /// </summary>
    void InitPopup()
    {
        _itemPopup = uiManager.GetPopup<ItemPopup>(PopupType.ITEM);
        _synthesisPopup = uiManager.GetPopup<SynthesisPopup>(PopupType.SYNTHESIS);
        _itemPopup.Init(this);
    }

    /// <summary>
    /// 아이콘 초기화
    /// </summary>
    void InitIcons()
    {
        _editorPos = new RectTransform[_invenIconArr.Length];
        for (int i = 0; i < _invenIconArr.Length; i++)
        {
            _invenIconArr[i].Init(this);
            _editorPos[i] = _invenIconArr[i].GetRectTrans;
        }

        var unitData = userData.GetUnitData;
        var levelData = userData.GetLevelData;
        var potentialData = userData.GetPotentialData;
        var coalescenceData = userData.GetCoalescenceData;

        CreateItemIcon(unitData._boat, levelData._boatLevel, EItemList.BOAT, coalescenceData._boatNumID);
        CreateItemIcon(unitData._weapon, levelData._weaponLevel, EItemList.WEAPON);
        CreateItemIcon(unitData._defense, levelData._defenseLevel, EItemList.DEFENSE);
        CreateItemIcon(unitData._captain, levelData._captainLevel, EItemList.CAPTAIN, potentialData._capPotentialID);
        CreateItemIcon(unitData._sailor, levelData._sailorLevel, EItemList.SAILOR, potentialData._salPotentialID);
        CreateItemIcon(unitData._engine, levelData._engineLevel, EItemList.ENGINE, potentialData._engPotentialID);

        uiManager.GetReadyPage.CreateBoatIcon(GetInven(EItemList.BOAT).Count);

        ItemWearingIcon();
        // 융합 아이콘 생성하기
        //InvenSynthesisCreateIcon();
        // 스크롤 정리
        ShowAllCategory();
    }

    /// <summary>
    /// 버튼 초기화
    /// </summary>
    void InitButtons()
    {
        for (int i = 0; i < _categoryBtnArr.Length; i++)
        {
            int num = i + 1;
            _categoryBtnArr[i].onClick.AddListener(() =>
            {
                OnTouchCategoryButton(num);
            });
        }

        CheckInvenIconState();
    }
    #endregion Init

    #region Category
    /// <summary>
    /// 카테고리 내부 아이템 리스트 반환
    /// </summary>
    /// <param name="itemList"></param>
    /// <returns></returns>
    public List<UnitIcon> GetInven(EItemList itemList)
    {
        if (itemList == EItemList.NONE) return null;

        return FindInvenCategory(itemList).GetList;
    }

    /// <summary>
    /// 카테고리 반환(아이템 리스트 관리 클래스)
    /// </summary>
    /// <param name="itemList"></param>
    /// <returns></returns>
    public InvenCategory FindInvenCategory(EItemList itemList)
    {
        if (_invenDictionary.ContainsKey(itemList))
        {
            return _invenDictionary[itemList];
        }
        return null;
    }

    /// <summary>
    /// 인벤(각 카테고리) 상태 처리 메서드(전체)
    /// </summary>
    void CheckAllCategoryState()
    {
        foreach (var category in _invenDictionary)
        {
            InvenCategory inven = category.Value;
            inven.SetCategoryState(!inven.IsEmpty);
        }
    }

    /// <summary>
    /// 특정 카테고리 상태 확인
    /// </summary>
    /// <param name="itemList">타입</param>
    /// <returns></returns>
    public bool GetCategoryState(EItemList itemList)
    {
        if (itemList == EItemList.NONE)
        {
            return false;
        }
        return !_invenDictionary[itemList].IsEmpty;
    }

    /// <summary>
    /// 특정 카테고리만 보여주는 메서드
    /// </summary>
    /// <param name="itemList"></param>
    bool SetHighlightCategory(EItemList itemList)
    {
        if (_invenDictionary.ContainsKey(itemList))
        {
            if(_invenDictionary[itemList].IsEmpty)
            {
                MessageHandler.GetInstance.ShowMessage("해당 카테고리에 아이템이 없습니다.", 1.5f);
                return false;
            }

            foreach (var category in _invenDictionary)
            {
                category.Value.SetCategoryState(false);
            }
            _invenDictionary[itemList].SetCategoryState(true);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 아이템 정렬 시켜주는 메서드
    /// </summary>
    /// <param name="itemList"></param>
    public void SortInven(EItemList itemList)
    {
        if (itemList != EItemList.NONE)
        {
            FindInvenCategory(itemList).SortInven();
        }
    }

    /// <summary>
    /// 카테고리로 이동 가능한 상단의 + 버튼 상태 체크
    /// </summary>
    public void CheckInvenIconState()
    {
        for (int j = 0; j < _invenIconArr.Length; j++)
        {
            _invenIconArr[j].CheckButtonState();
        }
    }

    public void RefreshInvenCategory()
    {
        if(_searchType == EItemList.NONE || FindInvenCategory(_searchType).IsEmpty)
        {
            OnTouchCategoryButton(0);

        }
        CheckCategoryButtonState();
    }
    #endregion Category

    #region Scroll
    /// <summary>
    /// 아이템을 보유하고 있는 모든 카테고리 보여주는 메서드
    /// </summary>
    public void ShowAllCategory()
    {
        _searchType = EItemList.NONE;
        CheckAllCategoryState();
        RefreshInvenScrollView();
    }

    /// <summary>
    /// 인벤 스크롤 뷰 배치 갱신
    /// </summary>
    public void RefreshInvenScrollView()
    {
        int length = 0;
        for (int i = 0; i < _invenDictionary.Count; i++)
        {
            var category = FindInvenCategory((EItemList)i);
            if (category.IsActivated)
            {
                category.SetPosition(-length);
                length += category.GetLength();
            }
        }
        if (length <= 100)
        {
            length = 150;
        }
        _heightPos.sizeDelta = new Vector2(_heightPos.sizeDelta.x, length);
    }

    /// <summary>
    /// 인벤 컨텐츠 스크롤 초기화 (화면 맨 위로 올라오게 하는 것)
    /// </summary>
    public void InitContentScrollView()
    {
        _contentPos.anchoredPosition = new Vector2(0f, 0f);
    }

    /// <summary>
    /// 검색의 카테고리 버튼 활성화 / 비활성화
    /// </summary>
    void CheckCategoryButtonState()
    {
        for (int i = 0; i < _categoryBtnArr.Length; i++)
        {
            _categoryBtnArr[i].interactable = GetCategoryState((EItemList)i);
        }
    }

    /// <summary>
    /// 검색 버튼 보드 닫기
    /// </summary>
    public void CloseSearchBoard()
    {
        _isSearching = false;
        _searchBoardObj.SetActive(false);
    }
    #endregion Scroll

    #region Create
    /// <summary>
    /// 인벤 유닛 아이콘 생성
    /// <summary>
    void CreateItemIcon(List<int> item, List<int> itemLevel, EItemList itemList, List<string> ingerenceID = null)
    {
        for (int i = 0; i < item.Count; ++i)
        {
            if (itemList == EItemList.BOAT)
            {
                CreateItemIcon(item[i], itemLevel[i], itemList, i, int.Parse(ingerenceID[i]));
            }
            else if (itemList == EItemList.CAPTAIN || itemList == EItemList.SAILOR || itemList == EItemList.ENGINE)
            {
                CreateItemIcon(item[i], itemLevel[i], itemList, i, ingerenceID[i]);
            }
            else
            {
                CreateItemIcon(item[i], itemLevel[i], itemList, i);
            }
        }
    }
    /// <summary>
    /// 함선, 무기, 철판의 경우
    /// </summary>
    public UnitIcon CreateItemIcon(int id, int level, EItemList itemList, int numberArr, int ingerenceID = 0)
    {
        UnitIcon unit = CreateEmptyIcon();
        unit.Init(id, level, numberArr, itemList, _editorPos[(int)itemList], this, ingerenceID);
        RegisterUnitIcon(unit);
        return unit;
    }

    /// <summary>
    /// 선장, 선원, 갑판장의 경우
    /// </summary>
    public UnitIcon CreateItemIcon(int id, int level, EItemList itemList, int numberArr, string potentialList)
    {
        UnitIcon unit = CreateEmptyIcon();
        unit.Init(id, level, numberArr, itemList, _editorPos[(int)itemList], this, potentialList);
        RegisterUnitIcon(unit);
        return unit;
    }

    /// <summary>
    /// 비어있는 아이콘 생성
    /// </summary>
    UnitIcon CreateEmptyIcon()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_UI_ITEMICON, CommonStaticDatas.RES_ITEMICON, Vector3.zero, Quaternion.identity);
        UnitIcon unit = obj.GetComponent<UnitIcon>();
        return unit;
    }

    /// <summary>
    /// 아이콘 등록 메서드
    /// </summary>
    void RegisterUnitIcon(UnitIcon icon)
    {
        var category = FindInvenCategory(icon.GetItemType);
        if (category != null)
        {
            category.AddInvenIcon(icon, icon.gameObject);
        }
        _synthesisPopup.CreateSynthesisIcon();
    }

    /// <summary>
    /// 인벤 상단 미리보기(View) 생성
    /// </summary>
    /// <param name="id"></param>
    /// <param name="itemList"></param>
    /// <returns></returns>
    public GameObject CreatePrefabsObj(int id, EItemList itemList, int grade)
    {
        GameObject obj = null;
        switch (itemList)
        {
            case EItemList.BOAT:
                var data = DataManager.GetInstance.GetData(DataManager.KEY_BOAT, id, 1) as BoatData;
                obj = SimplePool.Spawn(CommonStaticDatas.RES_BOATLOBBY, data.resName, Vector3.zero, Quaternion.identity);
                obj.transform.SetParent(_createUnitRawImagePosition);
                ObjResetSetting(obj, grade);
                _boatObj = obj;
                break;
            case EItemList.WEAPON:
                var data1 = DataManager.GetInstance.GetData(DataManager.KEY_WEAPON, id, 1) as WeaponData;
                obj = SimplePool.Spawn(CommonStaticDatas.RES_WEAPONLOBBY, data1.resName, Vector3.zero, Quaternion.identity);
                obj.transform.SetParent(_boatObj.GetComponent<BoatInfo>()._boatWeaponPos.transform);
                ObjResetSetting(obj, grade);
                break;
            case EItemList.DEFENSE:
                var data2 = DataManager.GetInstance.GetData(DataManager.KEY_DEFENSE, id, 1) as DefenseData;
                obj = SimplePool.Spawn(CommonStaticDatas.RES_DEFENSE, data2.resName, Vector3.zero, Quaternion.identity);
                _boatObj.GetComponent<BoatInfo>()._boatDefensePos.GetPointSetting((EDefensePoint)data2.defensePoint, CommonStaticDatas.RES_DEFENSE, data2.resName, obj);
                break;
        }
        return obj;
    }

    /// <summary>
    /// 미리보기(View)에서 오브젝트 위치 재조정
    /// </summary>
    /// <param name="obj"></param>
    void ObjResetSetting(GameObject obj, int grade)
    {
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        if (grade == 1) { obj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); }
        else if (grade == 2) { obj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); }
        else { obj.transform.localScale = new Vector3(1f, 1f, 1f); }
    }
    #endregion Create

    #region Save/Load Info
    /// <summary>
    /// 보트에 착용중이었던 유닛 정보 불러오기
    /// 처음 초기화시 사용
    /// </summary>
    public void ItemWearingIcon()
    {
        var boatInven = GetInven(EItemList.BOAT);
        int numID = boatInven[0].GetDataIndex;
        // 일단 처음 보트 꺼내고
        var data = userData.GetCoalescenceData;
        int[] arr = userData.FindCoalescence(data._boatNumID[numID]);
        var potential = userData.FindPotential(data._boatNumID[numID]);
        PartsCheckEditor(arr, potential, 0);

        // 처음 보트 장착이 되면 위에 설정했던 파츠들도 다 들어가있음
        boatInven[0].IconEditor();
        // 다른 보트를 검사해서 다른 보트에도 착용된 정보가 있으면 표시해주기

        for (int i = 1; i < data._boatNumID.Count; ++i)
        {
            numID = boatInven[i].GetDataIndex; // 실제 데이터를 찾는다
            int[] tempArr = userData.FindCoalescence(data._boatNumID[numID]);
            var tempPotential = userData.FindPotential(data._boatNumID[numID]);
            //Debug.Log("tempArr : " + userData.CoalesceneceStr(tempArr));
            PartsCheckEditor(tempArr, tempPotential, i, true);
        }
    }

    void PartsCheckEditor(int[] coalescence, List<string> potential, int boatIdx, bool wear = false)
    {
        int ingerenceId = GetInven(EItemList.BOAT)[boatIdx].GetIngerenceID;
        for (int i = 2; i < coalescence.Length;)
        {
            if (coalescence[i] != 0)
            {
                List<UnitIcon> invenList = null;
                bool checkPotential = false;
                int potenIndex = -1;
                switch ((ECoalescenceType)i)
                {
                    case ECoalescenceType.WEAPON_ID:
                        {
                            invenList = GetInven(EItemList.WEAPON);
                        }
                        break;
                    case ECoalescenceType.DEFENSE_ID:
                        {
                            invenList = GetInven(EItemList.DEFENSE);
                        }
                        break;
                    case ECoalescenceType.CAPTAIN_ID:
                        {
                            invenList = GetInven(EItemList.CAPTAIN);
                            checkPotential = true;
                            potenIndex = 0;
                        }
                        break;
                    case ECoalescenceType.SAILOR_ID:
                        {
                            invenList = GetInven(EItemList.SAILOR);
                            checkPotential = true;
                            potenIndex = 1;
                        }
                        break;
                    case ECoalescenceType.ENGINE_ID:
                        {
                            invenList = GetInven(EItemList.ENGINE);
                            checkPotential = true;
                            potenIndex = 2;
                        }
                        break;
                }
                for (int j = 0; j < invenList.Count; j++)
                {
                    if (invenList[j].GetID != coalescence[i]) continue;
                    if (invenList[j].GetLevel != coalescence[i + 1]) continue;
                    if (invenList[j].GetWearReally) continue;
                    bool isSameItem = true;
                    if (checkPotential)
                    {
                        // 위 조건을 만족하고 들어오면 포텐셜 자리수는 같다
                        var itemPotential = potential[potenIndex].Split(',');
                        var targetPotentials = invenList[j].GetPotentialList;
                        isSameItem = false;
                        // 포텐셜 비교
                        for (int k = 0; k < itemPotential.Length; k++)
                        {
                            if (int.Parse(itemPotential[k]) != targetPotentials[k])
                            {
                                isSameItem = false;
                                break;
                            }
                            else
                            {
                                isSameItem = true;
                            }
                        }
                    }

                    if(isSameItem)
                    {
                        invenList[j].ChangeIngerenceID(ingerenceId);

                        if (wear)
                        {
                            invenList[j].gameObject.SetActive(false);
                            invenList[j].SetWearReally(true);
                            invenList[j]._check.gameObject.SetActive(true);
                            break;
                        }
                    }
                }
            }
            i += 2;
        }
    }

    // 세이브 함수
    public void BoatInvenSave()
    {
        if (_notSave)
        {
            _notSave = false;
            return;
        }

        string id = _editorObj[(int)EItemList.BOAT].GetIngerenceID.ToString();

        var data = userData.GetCoalescenceData;
        for (int i = 0; i < data._boatNumID.Count; ++i)
        {
            if (data._boatNumID[i] == id)
            {
                // 세이브 진행
                SaveFuntion(i);
                break;
            }
        }
    }

    /// <summary>
    /// 착용 중인 보트정보 세이브
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="ud"></param>
    /// <param name="index"></param>
    /// <param name="coalescenece"></param>
    void SaveFuntion(int index)
    {
        int[] arr = new int[System.Enum.GetValues(typeof(ECoalescenceType)).Length];
        List<string> itemPotens = new List<string>();

        for (int i = (int)EItemList.BOAT; i <= (int)EItemList.ENGINE; i++)
        {
            int equipIdx = i * 2;
            int levelIdx = equipIdx + 1;
            if (_editorObj[i] != null)
            {
                arr[equipIdx] = _editorObj[i].GetID;
                arr[levelIdx] = _editorObj[i].GetLevel;
                _editorObj[i].ChangeIngerenceID(_editorObj[(int)EItemList.BOAT].GetIngerenceID);
            }
            else
            {
                arr[equipIdx] = 0;
                arr[levelIdx] = 0;
            }

            if ((EItemList)i > EItemList.DEFENSE)
            {
                if (_editorObj[i] != null && _editorObj[i].GetPotentialList.Count > 0)
                {
                    itemPotens.Add(_editorObj[i].GetPotentialString);
                }
                else
                {
                    itemPotens.Add("0");
                }
            }
        }
        List<string> coalescenece = userData.GetCoalescenceData._coalescence;
        List<string> potentials = userData.GetCoalescenceData._potentials;
        coalescenece[index] = string.Join(",", arr);
        potentials[index] = string.Join("_", itemPotens);
        //Debug.Log("Coalescene Changed : " + coalescenece[index]);
        userData.SaveCoalescenece(coalescenece, potentials);
        //Debug.Log(coalescenece[index]);
        _editorObj[(int)EItemList.BOAT]._playerInfo = new Info.PlayerInfo(arr);
    }
    #endregion Save/Load Info

    #region Buttons

    /// <summary>
    /// 합성 버튼
    /// </summary>
    public void OnTouchSynthesisButton()
    {
        if (_isSearching)
        {
            CloseSearchBoard();
        }
        _synthesisPopup.OpenPopup();
    }

    /// <summary>
    /// 검색 버튼
    /// </summary>
    public void OnTouchSearchButton()
    {
        if (_isSearching || _searchBoardObj.activeSelf)
        {
            _isSearching = false;
        }
        else
        {
            _isSearching = true;
        }
        _searchBoardObj.SetActive(_isSearching);
    }

    /// <summary>
    /// 검색 보드의 각 항목 버튼
    /// </summary>
    /// <param name="idx"></param>
    public void OnTouchCategoryButton(int idx)
    {
        string buttonText = "";
        if (idx == 0)
        {
            CheckAllCategoryState();
            buttonText = StrSearch;
            _searchType = EItemList.NONE;
        }
        else
        {
            int num = idx - 1;
            if(!SetHighlightCategory((EItemList)num))
            {
                return;
            }
            buttonText = FindInvenCategory((EItemList)num).GetTitle;
            _searchType = (EItemList)num;
        }
        _searchButtonText.key = buttonText;
        RefreshInvenScrollView();
        InitContentScrollView();
        CloseSearchBoard();
    }
    #endregion Buttons

    public string BoatStatsShow(float hp, float def, float spd)
    {
        return string.Format("HP: {0}\nATK: {1}\nSPD: {2}", hp, def, spd);
    }
    public string ItemStatsShow(EItemList type, float value, float range, float atkSpd)
    {
        if (type == EItemList.DEFENSE)
        {
            return string.Format("HP: {0}\nATK: {1}", value, range);
        }
        else if (type == EItemList.CAPTAIN)
        {
            return string.Format("ATK: {0}", value);
        }
        else if (type == EItemList.SAILOR)
        {
            return string.Format("HP: {0}", value);
        }
        else if (type == EItemList.ENGINE)
        {
            return string.Format("SPD: {0}", value);
        }
        else
        {
            return string.Format("ATK: {0}\nRANGE: {1}\nATKSPD: {2}", value, range, atkSpd);
        }
    }

    public void UnequipAllBoatParts(UnitIcon targetIcon)
    {
        var ingerenceID = targetIcon.GetIngerenceID;


    }

    // 합성
    public void FusionFuntion(UnitIcon mainIcon, UnitIcon material1, UnitIcon material2)
    {
        // 다 없어지고 새로운 등급이 추가가된다
        // 유저 실제 데이터 정리
        userData.FusionUnit(mainIcon.GetItemType, mainIcon.GetOriginalData.GetDataIndex, material1.GetOriginalData.GetDataIndex, material2.GetOriginalData.GetDataIndex);
        DestroyUnitChange(mainIcon.GetItemType, mainIcon, material1, material2);
    }

    void DestroyUnitChange(EItemList itemListType, UnitIcon mainIcon, UnitIcon material1, UnitIcon material2)
    {
        var category = FindInvenCategory(itemListType);

        if (mainIcon.GetItemType == EItemList.BOAT)
        {
            if (mainIcon.GetOriginalData.GetWearState || material1.GetOriginalData.GetWearState || material2.GetOriginalData.GetWearState)
            {
                if (mainIcon.GetOriginalData.GetMyPrefabsObj != null) { Destroy(mainIcon.GetOriginalData.GetMyPrefabsObj); }
                else if (material1.GetOriginalData.GetMyPrefabsObj != null) { Destroy(material1.GetOriginalData.GetMyPrefabsObj); }
                else if (material2.GetOriginalData.GetMyPrefabsObj != null) { Destroy(material2.GetOriginalData.GetMyPrefabsObj); }
                _mainWear = true;
            }
        }
        else
        {
            if (mainIcon.GetOriginalData.GetWearReally)
            {
                mainIcon.GetOriginalData.IconEditor();
                _mainWear = true;
            } // 원래 장비가 착용중이였다면 장비를 해제
        }

        int originMaxCount = category.AllItemCount;
        category.ChangeItemDataIndex(mainIcon.GetDataIndex, -1);
        category.ChangeItemDataIndex(material1.GetDataIndex, -1);
        category.ChangeItemDataIndex(material2.GetDataIndex, -1);

        // 데이터 순서 정보 재배치
        if (category.AllItemCount != 0)
        {
            int dataCount = userData.GetUnitCountByType(itemListType);

            int idx = 0;
            for (int i = 0; i < originMaxCount; ++i)
            {
                UnitIcon item = category.FindItemByDataIndex(i);
                if (item.GetDataIndex <= -1 || item == null)
                {
                    continue;
                }
                item.ChangeDataIndex(idx);
                idx++;
                if (idx >= dataCount)
                {
                    break;
                }
            }
        }
        category.DestoryItem();
        EItemList type = mainIcon.GetItemType;
        // 새로운 등급 만들어 주기
        if (type == EItemList.BOAT)
        {
            var data = DataManager.GetInstance.GetData(DataManager.KEY_BOAT, mainIcon.GetID + 1, 1) as BoatData;
            int ingerenceID = userData.CreateUnitIngerenceID(mainIcon.GetID + 1, data.grade);
            // 오브젝트 생성 - 생성이 되면서 _all 변수에도 추가됨
            userData.BoatCoalesceneceAdd(ingerenceID, mainIcon.GetID + 1);
            CreateItemIcon(mainIcon.GetID + 1, 1, mainIcon.GetItemType, GetInven(mainIcon.GetItemType).Count, ingerenceID);
            // 출전 아이콘 생성
            uiManager.GetReadyPage.CreateBoatIcon();
        }
        else if (type == EItemList.CAPTAIN || type == EItemList.SAILOR || type == EItemList.ENGINE)
        {
            CreateItemIcon(mainIcon.GetID + 1, 1, mainIcon.GetItemType, GetInven(mainIcon.GetItemType).Count, CreateEmptyPotential(mainIcon.GetGrade + 1));
        }
        else
        {
            CreateItemIcon(mainIcon.GetID + 1, 1, mainIcon.GetItemType, GetInven(mainIcon.GetItemType).Count);
        }
        userData.UnitTypeAdd(mainIcon.GetItemType, mainIcon.GetID + 1, 1);
        userData.UnitPotentialAdd(mainIcon.GetItemType, mainIcon.GetPotentialString);
        userData.UnitTypeSave(mainIcon.GetItemType);

        RefreshInvenScrollView();
    }

    public UnitPosition SelectPositionCheck()
    {
        var posType = uiManager.GetReadyPage.GetTargetPosType;
        if (posType == EUnitPosition.NONE)
        {
            return null;
        }
        return uiManager.GetReadyPage.GetUnitPosition(posType);
    }
    public void SelectPositionBtnAction()
    {
        Debug.Log("SelectPositionBtnAction");
        var targetPosition = uiManager.GetReadyPage.GetUnitPosition(uiManager.GetReadyPage.GetTargetPosType);
        targetPosition.PositionBtn();
    }

    public void ShowMyMaterials()
    {
        var category = FindInvenCategory(EItemList.MATERIAL);
        int[] matArr = GetMyMaterial();
        int count = matArr.Length - 2;
        category.RefreshMaterial(matArr.ToList().GetRange(2, count));
    }
    public int[] GetMyMaterial()
    {
        int[] my = new int[(int)EInvenType.Evolution];

        for (int i = 0; i < my.Length; i++)
        {
            my[i] = userData.GetCurrency((EInvenType)i);
        }

        return my;
    }

    public string CreateEmptyPotential(int grade)
    {
        string newPotential = string.Empty;
        if (grade <= 4)
        {
            newPotential = "0";
        }
        else if (grade <= 6)
        {
            newPotential = "0,0";
        }
        else
        {
            newPotential = "0,0,0";
        }
        return newPotential;
    }

    #region property
    public bool NotSave { get { return _notSave; } set { _notSave = value; } }
    public bool BasicWeaponChange { get { return _basicWeaponChange; } set { _basicWeaponChange = value; } }
    public bool GetMainWear { get { return _mainWear; } set { _mainWear = value; } }
    public Button GetBoatUpgradeBtn { get { return _boatUpgradeBtn; } }
    #endregion

    public ItemStatusSupporter StatusSupporter => _statusSupporter;
    public ItemPopup ItemPopup => _itemPopup;
    public bool IsSearching => _isSearching;
    public EItemList SearchType => _searchType;
}

public enum EItemList
{
    BOAT = 0, // 지금은 안쓰는 공간 하지만 해놓은 코드가 많아서 남겨놓은 보트 6번째 칸의 어떤것이 생기면 그것으로 이름을 바꿀수도?
    WEAPON = 1,
    DEFENSE,
    CAPTAIN,
    SAILOR,
    ENGINE,
    MATERIAL,
    NONE
}