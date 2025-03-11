using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyData;
using TMPro;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

public class SynthesisPopup : PopupBase
{
    private enum SynthesisType
    {
        Main,
        Sub01,
        Sub02,
        Result,
    }
    [Header("Synthesis")]
    [SerializeField] UnitIcon[] _synthesisIconArr;
    [SerializeField] UnitIcon[] _invenIconArr;
    [SerializeField] RectTransform _synthesisObj;

    int _itemCount = 0;
    [SerializeField] bool _isFilled = false;     // 합성 아이콘이 꽉 찼는지 여부

    int[] _returnMatArr = new int[(int)EInvenType.Evolution];

    [Header("Result Page")]
    [SerializeField] GameObject _synthesisResult;
    [SerializeField] UnitIcon[] _pageIcon; // 0 메인, 1 서브(0), 2 서브(1) 3 다음 레벨 아이콘
    [SerializeField] TextMeshProUGUI[] _levelMaxText; // 0 해당 레벨, 1 바뀔 레벨
    [SerializeField] TextMeshProUGUI[] _nameText; // 0 이름 변경(공격력, 체력), 1 해당 값, 2 바뀔 값
    [SerializeField] TextMeshProUGUI _skillText; // 0 바뀔 스킬 설명
    [SerializeField] GameObject _mainCloseBtn;
    [SerializeField] GameObject _synthesisPageCloseBtn;

    [Header("Inven")]
    [SerializeField] List<UnitIcon> _synthesisList;
    [SerializeField] GameObject[] _synthesisBtnObj;
    Dictionary<EItemList, List<UnitIcon>> _iconListDictionary;

    [Header("Scripts")]
    LobbyUIManager _uiManager;
    InvenScript _inven;
    RewardResultPopup _rewardResultPopup;
    UserData _userData;

    [Header("Search")]
    [SerializeField] GameObject _searchBtnSetObj;
    [SerializeField] GameObject _searchBoardObj;
    [SerializeField] Button[] _searchButtonArr;
    [SerializeField] TextLocalizeSetter _searchButtonText;
    List<string> _searchNameList;
    bool _isSearching = false;      // 검색 버튼 활성화 여부

    const int _maxIconCount = 2;
    const string StrSearch = "검색";

    #region Init
    public void Init(LobbyUIManager uiManager)
    {
        _uiManager = uiManager;
        _inven = _uiManager.GetInvenPage;
        _rewardResultPopup = popupController.GetPopup<RewardResultPopup>(PopupType.REWARDRESULT);
        _userData = UserData.GetInstance;
        _invenIconArr = new UnitIcon[3];

        _iconListDictionary = new Dictionary<EItemList, List<UnitIcon>>();
        for (int i = 0; i < (int)EItemList.MATERIAL; i++)
        {
            _iconListDictionary.Add((EItemList)i, new List<UnitIcon>());
        }
        InitSearchButtons();
        InitSynthesisIconButton();
    }

    /// <summary>
    /// 합성 팝업 리스트 초기화
    /// </summary>
    void InitSynthesisList()
    {
        for (int i = 0; i < _synthesisList.Count; ++i)
        {
            _synthesisList[i].GetMyBtn.onClick.RemoveAllListeners();
        }

        foreach (var list in _iconListDictionary)
        {
            list.Value.Clear();
        }
    }

    /// <summary>
    /// 검색 버튼 초기화
    /// </summary>
    void InitSearchButtons()
    {
        _searchNameList = new List<string>();

        for (int i = 0; i < _searchButtonArr.Length; i++)
        {
            int num = i + 1;
            _searchButtonArr[i].onClick.AddListener(() => OnTouchCategoryButton(num));
            _searchNameList.Add(_searchButtonArr[i].transform.GetChild(0).GetComponent<TextLocalizeSetter>().key);
        }
        CloseSearchBoard();
    }

    /// <summary>
    /// 합성칸에 들어가는 아이콘 버튼 기능 초기화
    /// </summary>
    void InitSynthesisIconButton()
    {
        for (int i = 0; i < (int)SynthesisType.Result; i++)
        {
            int idx = i;
            _synthesisIconArr[i].GetMyBtn.onClick.AddListener(() => CancelSynthesis((SynthesisType)idx));
        }
    }

    /// <summary>
    /// 결과창에서 나갈때 초기화 하는 내용
    /// </summary>
    void ResetResultPage()
    {
        _synthesisResult.SetActive(false);
        _synthesisPageCloseBtn.SetActive(false);
        _mainCloseBtn.SetActive(true);
        _levelMaxText[0].transform.parent.transform.parent.gameObject.SetActive(false);
        _nameText[0].gameObject.SetActive(false);
        _skillText.gameObject.SetActive(false);
        for (int i = 0; i < _pageIcon.Length - 1; ++i)
        {
            _pageIcon[i].gameObject.SetActive(true);
        }
        _pageIcon[3].gameObject.SetActive(false);
    }

    /// <summary>
    /// 합성 아이콘 초기화
    /// </summary>
    void ResetSynthesisIcon()
    {
        for (int i = 0; i < _synthesisIconArr.Length; i++)
        {
            if(_invenIconArr.Length > i)
            {
                _invenIconArr[i] = null;
            }
            _synthesisIconArr[i].gameObject.SetActive(false);
            if (i == (int)SynthesisType.Result)
            {
                _synthesisIconArr[i].GetSynthesisBlurObj().SetActive(true);
            }
        }
    }
    #endregion Init

    /// <summary>
    /// 업그레이드에 사용한 재료 돌려받는 메서드
    /// </summary>
    /// <param name="item">아이템</param>
    void GetBackMaterial(UnitIcon item)
    {
        var matList = item.GetNeedMatAmount;
        int level = item.GetLevel - 1;
        float money = 0;
        float[] mat;

        // 광물 타입 찾기
        List<EInvenType> matType = new List<EInvenType>();
        for (int j = 1; j < matList.Count; j++)
        {
            if (matList[j] > 0)
            {
                matType.Add((EInvenType)j);
            }
        }

        mat = new float[matType.Count];
        for (int i = level; i >= 1; i--)
        {
            money += matList[0] * Mathf.Pow(StandardFuncData.ItemUpgradeMoneyCalculateMultiplier(item.GetGrade), i);

            for (int j = 0; j < matType.Count; ++j)
            {
                mat[j] += matList[(int)matType[j]] * Mathf.Pow(StandardFuncData.ItemUpgradeCalculateMultiplier(item.GetItemType), i - 1);
            }
            //var data = DataManager.GetInstance.FindData(DataManager.KEY_UPGRADE, i) as UpgradeData;
            //mat += data.cost;
            //money += (matList[0] * i);
        }

        // 업그레이드 재료 반환
        _userData.AddCurrency(EInvenType.Money, (int)money);
        for (int i = 0; i < matType.Count; ++i)
        {
            if (matType[i] != EInvenType.None)
            {
                _userData.AddCurrency(matType[i], (int)mat[i]);
            }
        }

        if (money > 0)
        {
            _returnMatArr[(int)EInvenType.Money] += (int)money;
        }
        for (int i = 0; i < matType.Count; ++i)
        {
            if (mat[i] > 0)
            {
                _returnMatArr[(int)matType[i]] += (int)mat[i];
            }
        }
        //if (matType != EInvenType.None)
        //{
        //    _userData.AddCurrency(matType, mat);
        //}
        //if (money > 0 && mat > 0)
        //{
        //    _returnMatArr[(int)EInvenType.Money] += (int)money;
        //    _returnMatArr[(int)matType] += mat;
        //}
        _userData.SaveUserData();
        _uiManager.UpdateUserInfo();

        // ** 현재는 재료 1개만 사용하지만 나중에는 여러개 사용할 예정임
        // 스크립트 변경 필요함
    }

    /// <summary>
    /// 합성할 아이템 생성
    /// </summary>
    public void CreateSynthesisIcon()
    {
        GameObject obj = SimplePool.Spawn(CommonStaticDatas.RES_UI_ITEMICON, CommonStaticDatas.RES_ITEMICON, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(_synthesisObj);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        rt.localScale = new Vector3(1f, 1f, 1f);
        _synthesisList.Add(obj.GetComponent<UnitIcon>());
    }

    /// <summary>
    /// 합성 인벤 정렬
    /// </summary>
    void SortInvenUnitIcon()
    {
        int synthesisCount = 0;
        int[] itemCount = new int[(int)EItemList.MATERIAL];
        // 12등급 부터 1등급 까지 내림차순
        for (int i = 12; i > 0; --i)
        {
            for (int j = 0; j < (int)EItemList.MATERIAL; j++)
            {
                RegisterInvenItem((EItemList)j, i, ref itemCount[j], ref synthesisCount);
            }
        }
        CheckCategoryButton();
    }
    /// <summary>
    /// 인벤에 있는 아이템을 합성 팝업 하단 아이콘으로 불러오는 메서드
    /// </summary>
    void RegisterInvenItem(EItemList invenType, int grade, ref int count, ref int synthesisCount)
    {
        List<UnitIcon> unitList = _inven.GetInven(invenType);
        for (int i = count; i < unitList.Count; ++i)
        {
            if (unitList[count].GetGrade == grade)
            {
                _synthesisList[synthesisCount].Init(unitList[count]);
                _synthesisList[synthesisCount].GetOriginalData = unitList[count];
                _synthesisList[synthesisCount].GetMyBtn.onClick.AddListener(_synthesisList[synthesisCount].SelectSynthesis);
                _synthesisList[synthesisCount].GetSynthesisBlurObj().SetActive(false);
                // 착용되어있는거는 블러 처리 해준다
                if (_synthesisList[synthesisCount].GetItemType == EItemList.BOAT)
                {
                    int[] coalescence = _userData.FindCoalescence(_synthesisList[synthesisCount].GetIngerenceID);
                    if(coalescence != null)
                    {
                        for (int j = 2; j < coalescence.Length; ++j)
                        {
                            if (coalescence[j] != 0)
                            {
                                _synthesisList[synthesisCount].GetSynthesisBlurObj().SetActive(true);
                                break;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Null Coalescence : " + _synthesisList[synthesisCount].GetIngerenceID);
                    }
                }
                else
                {
                    if (_synthesisList[synthesisCount].GetWearState || _synthesisList[synthesisCount].GetWearReally)
                    {
                        _synthesisList[synthesisCount].GetSynthesisBlurObj().SetActive(true);
                    }
                }
                _iconListDictionary[invenType].Add(_synthesisList[synthesisCount]);
                count++;
                synthesisCount++;
            }
            else { break; }
        }
    }

    /// <summary>
    /// 아이콘 선택
    /// </summary>
    /// <param name="unit"></param>
    public void SelectUnitIcon(UnitIcon unit)
    {
        if (_isFilled) { return; }

        if (_invenIconArr[(int)SynthesisType.Main] == null)
        {
            CloseSearchBoard();
            SetSearchButtonState(false);

            SetInvenIconArray(SynthesisType.Main, unit);
            CopyIconInfo(SynthesisType.Main, unit);
            CopyIconInfo(SynthesisType.Result, unit);

            _synthesisIconArr[(int)SynthesisType.Result].ChangeID(unit.GetID + 1);
            _synthesisIconArr[(int)SynthesisType.Result].ChangeLevel(1);
            _synthesisIconArr[(int)SynthesisType.Result].UpdateUISetting();

            SetHighlightIcon(_invenIconArr[(int)SynthesisType.Main]);
        }
        else
        {
            SynthesisType type = SynthesisType.Sub01 + _itemCount;
            SetInvenIconArray(type, unit);
            CopyIconInfo(type, unit);
            
            _itemCount++;
            if (_itemCount == _maxIconCount)
            {
                _isFilled = true;
                SetSynthesisButtonState(true);
            }
            //SoundManager.GetInstance.PlayAudioEffectSound("UI_Icon_select");
        }
        unit.gameObject.SetActive(false);
        _synthesisObj.anchoredPosition3D = Vector3.zero;
    }

    /// <summary>
    /// 아이콘 합성대상 등록 취소 기능
    /// </summary>
    /// <param name="type"></param>
    void CancelSynthesis(SynthesisType type)
    {
        switch (type)
        {
            case SynthesisType.Main:
                {
                    _itemCount = 0;

                    SetInvenIconArray(SynthesisType.Sub01, null);
                    SetInvenIconArray(SynthesisType.Sub02, null);

                    ResetSynthesisIcon();
                    OnTouchCategoryButton(0);
                    SetSearchButtonState(true);
                }
                break;
            case SynthesisType.Sub01:
                {
                    _itemCount--;
                    // 일단 빠지는 원래 재료는 켜주고 없애준다
                    SetInvenIconArray(SynthesisType.Sub01, null);

                    // 만약 Sub02번자리가 있다고 한다면
                    if (_invenIconArr[(int)SynthesisType.Sub02] != null)
                    {
                        // Sub01번 자리로 이동 시켜준다. Sub02번은 꺼주고 리셋
                        CopyIconInfo(SynthesisType.Sub01, _invenIconArr[(int)SynthesisType.Sub02]);

                        // 원본 자리 교체
                        SetInvenIconArray(SynthesisType.Sub01, _invenIconArr[(int)SynthesisType.Sub02]);

                        _synthesisIconArr[(int)SynthesisType.Sub02].gameObject.SetActive(false);
                        _invenIconArr[(int)SynthesisType.Sub02] = null;
                    }
                    else
                    {
                        _synthesisIconArr[(int)SynthesisType.Sub01].gameObject.SetActive(false);
                    }
                }
                break;
            case SynthesisType.Sub02:
                {
                    _itemCount--;
                    _synthesisIconArr[(int)SynthesisType.Sub02].gameObject.SetActive(false);
                    SetInvenIconArray(SynthesisType.Sub02, null);
                }
                break;
        }
        _isFilled = false;
        SetSynthesisButtonState(false);
        SoundManager.GetInstance.PlayAudioEffectSound("UI_Button_Touch");
    }

    // 연출 부분
    IEnumerator StartFusionPage()
    {
        _synthesisPageCloseBtn.SetActive(false);
        _synthesisResult.SetActive(true);
        OnCloseEventListener += ResetResultPage;
        //SoundManager.GetInstance.PlayAudioEffectSound("UI_Compose");
        for (int i = 0; i < _pageIcon.Length - 1; ++i)
        {
            _pageIcon[i].gameObject.SetActive(false);
        }
        _pageIcon[3].gameObject.SetActive(true);

        if (_pageIcon[0].GetItemType == EItemList.WEAPON || _pageIcon[0].GetItemType == EItemList.CAPTAIN)
        {
            _nameText[0].text = LocalizeText.Get("공격력");
        }
        else if (_pageIcon[0].GetItemType == EItemList.BOAT || _pageIcon[0].GetItemType == EItemList.SAILOR)
        {
            _nameText[0].text = LocalizeText.Get("체력");
        }
        else if (_pageIcon[0].GetItemType == EItemList.ENGINE)
        {
            _nameText[0].text = LocalizeText.Get("스피드");
        }
        else
        {
            _nameText[0].text = LocalizeText.Get("방어력");
        }
        // 착용중인 장비 체크가 되어있으면 빼줘야함 
        if (_inven.GetMainWear) { NextIconWearFuntion(_pageIcon[0].GetItemType); }
        string nextLevel = NextLevelFuntion(_pageIcon[0].GetItemType);
        string nextStats = NextStatsFuntion(_pageIcon[0].GetItemType);

        // 레벨
        _levelMaxText[0].transform.parent.transform.parent.gameObject.SetActive(true);
        _levelMaxText[0].text = _pageIcon[0].GetMaxLevel.ToString();
        _levelMaxText[1].text = nextLevel;
        // 스텟
        _nameText[0].gameObject.SetActive(true);
        _nameText[1].text = _pageIcon[0].GetValue.ToString();
        _nameText[2].text = nextStats;
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        // 나가기
        _synthesisPageCloseBtn.SetActive(true);
        _mainCloseBtn.SetActive(true);
    }
    string NextLevelFuntion(EItemList type)
    {
        UnitIcon lastestItem = _inven.FindInvenCategory(type).LastestIcon;

        if (lastestItem == null)
        {
            return "";
        }

        return lastestItem.GetMaxLevel.ToString();
    }
    string NextStatsFuntion(EItemList type)
    {
        UnitIcon item = _inven.FindInvenCategory(type).LastestIcon;
        if (item == null)
        {
            return "";
        }

        return item.GetValue.ToString();
    }
    void NextIconWearFuntion(EItemList type)
    {
        _inven.FindInvenCategory(type).LastestIcon.IconEditor();
        _inven.GetMainWear = false;
    }

    #region Setters
    /// <summary>
    /// 합성버튼 활성화 상태 설정
    /// </summary>
    void SetSynthesisButtonState(bool state)
    {
        for (int i = 0; i < _synthesisBtnObj.Length; i++)
        {
            _synthesisBtnObj[i].SetActive(state);
        }
        _synthesisIconArr[(int)SynthesisType.Result].GetSynthesisBlurObj().SetActive(!state);
    }

    /// <summary>
    /// 인벤 아이콘 정보 리스트 설정
    /// </summary>
    /// <param name="targetIcon">대상 아이콘</param>
    /// <param name="type">합성 슬롯 타입</param>
    void SetInvenIconArray(SynthesisType type, UnitIcon targetIcon)
    {
        if(type < SynthesisType.Result)
        {
            _invenIconArr[(int)type]?.gameObject.SetActive(targetIcon == null);
            _invenIconArr[(int)type] = targetIcon;
        }
    }

    /// <summary>
    /// 합성 아이콘에 인벤 아이콘 정보 복사
    /// </summary>
    void CopyIconInfo(SynthesisType type, UnitIcon targetIcon)
    {
        _synthesisIconArr[(int)type].Init(targetIcon);
        _synthesisIconArr[(int)type].gameObject.SetActive(true);
    }
    #endregion Setters

    #region Search
    /// <summary>
    /// 카테고리 선택 버튼 체크
    /// </summary>
    void CheckCategoryButton()
    {
        // All Off
        foreach (var btn in _searchButtonArr)
        {
            btn.interactable = false;
        }

        for (int j = 0; j < (int)EItemList.MATERIAL; j++)
        {
            if (_iconListDictionary[(EItemList)j].Count > 0)
            {
                _searchButtonArr[j].interactable = true;
            }
        }
    }

    /// <summary>
    /// 카테고리의 아이템 표시 
    /// 0 : 모두 표시 / 1 ~ : 카테고리 한정
    /// </summary>
    /// <param name="idx"></param>
    string SearchCategory(int idx)
    {
        string buttonText = "";
        if (idx == 0)
        {
            for (int i = 0; i < (int)EItemList.MATERIAL; i++)
            {
                var list = _iconListDictionary[(EItemList)i];
                for (int j = 0; j < list.Count; j++)
                {
                    // 엘리트 등급 위로는 안보이게 (나중에 업데이트로 해제)
                    if (list[j].GetGrade > 5) // 나중에는 11로 해서 레전더리는 합성에 안뜨게
                    {
                        list[j].gameObject.SetActive(false);
                    }
                    else
                    {
                        list[j].gameObject.SetActive(true);
                    }
                }
            }
            buttonText = StrSearch;
        }
        else
        {
            int num = idx - 1;
            SetHighlightCategory((EItemList)num);
            buttonText = _searchNameList[num];
        }
        return buttonText;
    }

    /// <summary>
    /// 특정 카테고리만 보여주는 메서드
    /// </summary>
    /// <param name="itemList"></param>
    void SetHighlightCategory(EItemList itemList)
    {
        // all off 처리
        for (int i = 0; i < (int)EItemList.MATERIAL; i++)
        {
            var list = _iconListDictionary[(EItemList)i];
            for (int j = 0; j < list.Count; j++)
            {
                list[j].gameObject.SetActive(false);
            }
        }

        if (_iconListDictionary[itemList].Count > 0)
        {
            var list = _iconListDictionary[itemList];

            for (int i = 0; i < list.Count; i++)
            {
                // 엘리트 등급 위로는 안보이게 (나중에 업데이트로 해제)
                if (list[i].GetGrade > 5) // 나중에는 11로 해서 레전더리는 합성에 안뜨게
                {
                    list[i].gameObject.SetActive(false);
                }
                else
                {
                    list[i].gameObject.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// 특정 아이콘만 보여주는 메서드
    /// </summary>
    void SetHighlightIcon(UnitIcon targetIcon = null)
    {
        if (targetIcon == null)
        {
            if (_invenIconArr[(int)SynthesisType.Main] != null)
            {
                targetIcon = _invenIconArr[(int)SynthesisType.Main];
            }
            else
            {
                return;
            }
        }
        bool isBoat = targetIcon.GetItemType == EItemList.BOAT;

        int tGrade = targetIcon.GetGrade;
        int tType = targetIcon.GetEquipType;
        // 그 타입에 해당하는 물품으로만 보여주기
        for (int i = 0; i < _synthesisList.Count; ++i)
        {
            int sGrade = _synthesisList[i].GetGrade;
            int sType = _synthesisList[i].GetEquipType;
            _synthesisList[i].gameObject.SetActive(false);

            if (_synthesisList[i] != targetIcon)
            {
                if (_synthesisList[i].GetItemType == EItemList.BOAT && isBoat)
                {
                    bool isContinue = false;
                    int[] coalescence = _userData.FindCoalescence(_synthesisList[i].GetIngerenceID);
                    for (int j = 2; j < coalescence.Length; ++j)
                    {
                        if (coalescence[j] != 0)
                        {
                            isContinue = true;
                            break;
                        }
                    }

                    if (isContinue)
                    {
                        continue;
                    }
                }

                // 아이템 타입이 다를 때
                if (targetIcon.GetItemType != _synthesisList[i].GetItemType)
                {
                    continue; 
                }
                // 등급이 다를 때
                else if (tGrade != sGrade)
                {
                    continue;
                }
                // 아이템 유형이 다를 때
                else if (tType != sType)
                {
                    continue;
                }
                // 착용 중인 경우
                else if (_synthesisList[i].GetWearState || _synthesisList[i].GetWearReally)
                {
                    if(!isBoat)
                    {
                        continue;
                    }
                }
                _synthesisList[i].gameObject.SetActive(true);
            }
        }
        //SoundManager.GetInstance.PlayAudioEffectSound("UI_Icon_Insert");
    }

    /// <summary>
    /// 검색버튼 상태 설정
    /// </summary>
    public void SetSearchButtonState(bool state) => _searchBtnSetObj.SetActive(state);

    /// <summary>
    /// 검색 버튼 보드 닫기
    /// </summary>
    public void CloseSearchBoard()
    {
        _isSearching = false;
        _searchBoardObj.SetActive(false);
    }
    #endregion Search

    #region Popup
    public override void OpenPopup()
    {
        SoundManager.GetInstance.PlayAudioEffectSound("UI_Button_Touch");
        SortInvenUnitIcon();
        if (_userData.TutorialCheck() == 8)
        {
            _mainCloseBtn.SetActive(false);
        }
        else
        {
            _mainCloseBtn.SetActive(true);
            OnTouchCategoryButton(0);
        }
        ResetSynthesisIcon();
        base.OpenPopup();
    }

    public override void ClosePopup()
    {
        SoundManager.GetInstance.PlayAudioEffectSound("UI_Button_Touch");
        if (_invenIconArr[(int)SynthesisType.Main] != null)
        {
            CancelSynthesis(SynthesisType.Main);
        }
        SetSearchButtonState(true);
        InitSynthesisList();
        OpenRewardResultPopup();
        base.ClosePopup();
    }

    /// <summary>
    /// 재료 반환 결과 팝업
    /// </summary>
    void OpenRewardResultPopup()
    {
        bool isOpen = false;
        if (_returnMatArr[(int)EInvenType.Money] > 0)
        {
            isOpen = true;
            for (int i = 0; i < (int)EInvenType.Evolution; i++)
            {
                if (_returnMatArr[i] > 0)
                {
                    ERewardType rType = (ERewardType)i;
                    if (i >= 2)
                    {
                        rType += 1;
                    }
                    _rewardResultPopup.SetPopup("재료 반환", rType, _returnMatArr[i]);
                }
            }
        }

        // 결과 재화 초기화
        for (int j = 0; j < _returnMatArr.Length; j++)
        {
            _returnMatArr[j] = 0;
        }
        if (isOpen)
        {
            _rewardResultPopup.OpenPopup();
        }
    }
    #endregion Popup
    
    #region Buttons
    /// <summary>
    /// 합성 버튼
    /// </summary>
    public void OnTouchSynthesisButton()
    {
        _pageIcon[0].Init(_invenIconArr[(int)SynthesisType.Main]);
        _pageIcon[1].Init(_invenIconArr[(int)SynthesisType.Sub01]);
        _pageIcon[2].Init(_invenIconArr[(int)SynthesisType.Sub02]);
        _pageIcon[3].Init(_synthesisIconArr[(int)SynthesisType.Result]);
        GetBackMaterial(_invenIconArr[(int)SynthesisType.Main]);
        GetBackMaterial(_invenIconArr[(int)SynthesisType.Sub01]);
        GetBackMaterial(_invenIconArr[(int)SynthesisType.Sub02]);
        _inven.FusionFuntion(_invenIconArr[(int)SynthesisType.Main], _invenIconArr[(int)SynthesisType.Sub01], _invenIconArr[(int)SynthesisType.Sub02]);
        CancelSynthesis(SynthesisType.Main);

        for (int i = 0; i < 3; ++i)
        {
            Destroy(_synthesisList[_synthesisList.Count - 1].gameObject);
            _synthesisList.RemoveAt(_synthesisList.Count - 1);

            if (_pageIcon[0].GetItemType == EItemList.BOAT)
            {
                _uiManager.GetReadyPage.DestroyBoatIcon();
            }
        }

        InitSynthesisList();
        SortInvenUnitIcon();
        // 결과창
        StartCoroutine(StartFusionPage());
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
        _searchButtonText.key = SearchCategory(idx);
        _synthesisObj.anchoredPosition3D = Vector3.zero;
        CloseSearchBoard();
    }

    /// <summary>
    /// 결과창 닫기 버튼
    /// </summary>
    public void OnTouchCloseResultButton()
    {
        SoundManager.GetInstance.PlayAudioEffectSound("UI_Button_Touch");
        if (_userData.TutorialCheck() == 8) { _userData.SaveTutorial(9); TutorialManager.GetInstance.TypingAction(); }
        ClosePopup();
    }
    #endregion Buttons

    public List<UnitIcon> GetSynthesisIconList => _synthesisList;
    public List<UnitIcon> GetActivatedIconList()
    {
        List<UnitIcon> activatedIconList = new List<UnitIcon>();
        for (int i = 0; i < _synthesisList.Count; i++)
        {
            if (_synthesisList[i].gameObject.activeSelf)
            {
                activatedIconList.Add(_synthesisList[i]);
            }
        }
        return activatedIconList;
    }
}
