using MyData;
using MyStructData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopup : PopupBase
{
    private enum ButtonType
    {
        Equip,
        Select,
        Upgrade,
        Close,
    }

    private enum DetailType
    {
        Status,
        Potential,
    }

    [Header("UI")]
    [SerializeField] RectTransform _popupRect;
    [SerializeField] TextLocalizeSetter _titleText;
    [SerializeField] TextLocalizeSetter[] _potentialTitleArr;
    [SerializeField] GameObject[] _detailObjectArr;     // 스테이터스, 포텐셜의 상위 오브젝트
    [SerializeField] Button[] _buttonArr;
    [SerializeField] TextMeshProUGUI _selelctButtonTMP;
    [SerializeField] TextMeshProUGUI _equipButtonTMP;

    [Header("Move")]
    [SerializeField] GameObject[] _arrowObjArr;
    Button[] _arrowButtonArr;
    int _currentIdx = -1;

    [Header("Components")]
    [SerializeField] UnitIcon _viewIcon;
    [SerializeField] ItemStatus _itemStatus;
    UnitIcon _targetIcon = null;

    // Target Item List
    List<UnitIcon> _viewTargetList;

    InvenScript _inven;
    UserData _userData;

    public event PopupEventDelegate OnSelectEventListener;

    const float _popupSizeY = 650f;
    const float _onlyStatusSizeY = 450f;

    #region Init
    public void Init(InvenScript inven)
    {
        InitButtons();
        _inven = inven;
        _userData = UserData.GetInstance;
        _itemStatus.Init(_inven.StatusSupporter);
        _viewTargetList = new List<UnitIcon>();
    }

    /// <summary>
    /// 버튼 초기화
    /// </summary>
    void InitButtons()
    {
        _buttonArr[(int)ButtonType.Equip].onClick.AddListener(EquipItem);
        _buttonArr[(int)ButtonType.Select].onClick.AddListener(SelectItem);
        _buttonArr[(int)ButtonType.Upgrade].onClick.AddListener(UpgradeItem);
        _buttonArr[(int)ButtonType.Close].onClick.AddListener(ClosePopup);

        _arrowButtonArr = new Button[2];
        for (int i = 0; i < _arrowObjArr.Length; i++)
        {
            _arrowButtonArr[i] = _arrowObjArr[i].transform.GetChild(0).GetComponent<Button>();
            
            bool isLeft = i == 0;
            _arrowButtonArr[i].onClick.AddListener(() => OnTouchArrowButton(isLeft));
        }
    }

    /// <summary>
    /// 팝업 정보 초기화
    /// </summary>
    void ResetPopup()
    {
        _targetIcon = null;
    }
    #endregion Init

    #region Info
    /// <summary>
    /// 팝업 세팅
    /// </summary>
    /// <param name="icon">대상 아이콘</param>
    public void SetViewIcon(UnitIcon icon)
    {
        if (icon == null) return;

        _targetIcon = icon;
        //_titleText.text = _targetIcon.GetItemType.ToString();
        _titleText.key = _targetIcon.GetName;

        _itemStatus.UpdateStatusText(_targetIcon);
        SetPotential();
        _viewIcon.Init(_targetIcon);
    }

    /// <summary>
    /// 좌우이동으로 볼수있는 아이콘 리스트 설정
    /// </summary>
    /// <param name="targetList"></param>
    public void SetViewIconList(List<UnitIcon> targetList)
    {
        _viewTargetList = targetList;

        // 버튼
        // 이동 할 수 없는 경우(이전, 다음으로) 좌우버튼 비활성화
        bool btnState = true;
        if(_viewTargetList == null || _viewTargetList.Count <= 1)
        {
            btnState = false;
        }
        else
        {
            for(int i = 0; i < _viewTargetList.Count; i++)
            {
                if (_viewTargetList[i] == _targetIcon)
                {
                    _currentIdx = i;
                }
            }
        }

        for (int j = 0; j < _arrowObjArr.Length; j++)
        {
            _arrowObjArr[j].gameObject.SetActive(btnState);
        }

    }

    /// <summary>
    /// 스테이터스, 포텐셜 표기/미표기 상태 체크
    /// 하단 버튼 상태도 체크
    /// </summary>
    void CheckDetailState()
    {
        if (_targetIcon.GetItemType <= EItemList.DEFENSE)
        {
            _detailObjectArr[(int)DetailType.Potential].SetActive(false);
            _popupRect.sizeDelta = new Vector2(_popupRect.sizeDelta.x, _onlyStatusSizeY);
        }
        else
        {
            _detailObjectArr[(int)DetailType.Potential].SetActive(true);
            _popupRect.sizeDelta = new Vector2(_popupRect.sizeDelta.x, _popupSizeY);
        }

        if (_targetIcon.GetWearState)
        {
            _equipButtonTMP.text = "Unequip";
        }
        else
        {
            _equipButtonTMP.text = "Equip";
        }
    }

    /// <summary>
    /// 포텐셜 설정
    /// </summary>
    void SetPotential()
    {
        for (int i = 0; i < _potentialTitleArr.Length; i++)
        {
            _potentialTitleArr[i].gameObject.SetActive(false);
        }

        var potentialList = _targetIcon.GetPotentialList;
        for (int i = 0; i < potentialList.Count; i++)
        {
            _potentialTitleArr[i].gameObject.SetActive(true);
            var data = FindPotentialData(potentialList[i]);
            if (data != null)
            {
                _potentialTitleArr[i].key = data.name;
            }
            else
            {
                _potentialTitleArr[i].key = "-";
            }
        }
    }

    ItemPotentialData FindPotentialData(int potentialID)
    {
        return DataManager.GetInstance.FindData(DataManager.KEY_POTENTIAL, potentialID) as ItemPotentialData;
    }
    #endregion Info

    public override void OpenPopup()
    {
        if (_targetIcon == null) return;
        CheckDetailState();
        base.OpenPopup();
    }

    public override void ClosePopup()
    {
        ResetPopup();
        OnSelectEventListener = null;
        base.ClosePopup();
    }

    #region ButtonFunc
    /// <summary>
    /// 장비 장착
    /// </summary>
    void EquipItem()
    {
        _targetIcon.IconEditor();
        _inven.CheckInvenIconState();
        _inven.RefreshInvenCategory();
        ClosePopup();
    }

    void UnequipItem()
    {
        UserData userData = UserData.GetInstance;
        var ingerenceID = _targetIcon.GetIngerenceID;
        var iconList = popupController.GetPopup<SynthesisPopup>(PopupType.SYNTHESIS).GetSynthesisIconList;

        if (_targetIcon.GetItemType == EItemList.BOAT)
        {
            // 인벤에 장착중(상단 배 모델)이 아닌 경우 장착상태로 변경
            if(!_targetIcon.GetOriginalData.GetWearState)
            {
                _targetIcon.GetOriginalData.IconEditor();
            }
            _targetIcon.GetSynthesisBlurObj().SetActive(false);

            // 장착되어있는 부품 해제
            for (int i = 0; i < iconList.Count; i++)
            {
                if (iconList[i].GetIngerenceID == ingerenceID)
                {
                    iconList[i].GetSynthesisBlurObj().SetActive(false);
                    iconList[i].GetOriginalData.IconEditor();
                }
            }
        }
        else
        {
            // 장비의 경우 보트를 찾아서 장착상태로 변경
            for(int i = 0; i < iconList.Count; i++)
            {
                if (iconList[i].GetItemType == EItemList.BOAT && iconList[i].GetIngerenceID == ingerenceID)
                {
                    iconList[i].GetSynthesisBlurObj().SetActive(false);
                    iconList[i].GetOriginalData.IconEditor();
                    break;
                }
            }
            _targetIcon.GetSynthesisBlurObj().SetActive(false);
            _targetIcon.GetOriginalData.IconEditor();
        }
        _inven.RefreshInvenCategory();
    }

    /// <summary>
    /// 업그레이드 페이지로 이동
    /// </summary>
    public void UpgradeItem()
    {
        var itemEditorPage = LobbyUIManager.GetInstance.GetItemEditorPage;
        itemEditorPage.SetUpgradeTarget(_targetIcon);
        itemEditorPage.OpenPage();
        ClosePopup();
    }

    /// <summary>
    /// 아이템을 선택하는 버튼 기능
    /// </summary>
    public void SelectItem()
    {
        OnSelectEventListener?.Invoke();
        OnSelectEventListener = null;
        ClosePopup();
    }

    /// <summary>
    /// 인벤에서의 설정 값
    /// </summary>
    public void SetInvenSetting()
    {
        if (_targetIcon.GetItemType == EItemList.BOAT)
        {
            EnableEquipButton(false);
        }
        else
        {
            EnableEquipButton(true);
        }
        EnableSelectButton(false);
        EnableUpgradeButton(true);
        EnableCloseButton(true);
    }

    /// <summary>
    /// 합성 팝업에서의 설정 값
    /// </summary>
    public void SetSynthesisSetting()
    {
        EnableEquipButton(false);
        EnableUpgradeButton(false);

        EnableSelectButton(true);
        EnableCloseButton(true);

        OnSelectEventListener = null;

        if (_targetIcon.GetSynthesisBlurObj().activeSelf)
        {
            _selelctButtonTMP.text = "UnEquip";
            OnSelectEventListener += () =>
            {
                UnequipItem();
            };
        }
        else
        {
            _selelctButtonTMP.text = "Select";
            OnSelectEventListener += () =>
            {
                popupController.GetPopup<SynthesisPopup>(PopupType.SYNTHESIS).SelectUnitIcon(_targetIcon);
            };
        }
        //OnSelectEventListener += () => buttonCallback?.Invoke();
    }

    /// <summary>
    /// 좌우 이동 버튼
    /// </summary>
    /// <param name="isLeft"></param>
    public void OnTouchArrowButton(bool isLeft)
    {
        if (isLeft)
        {
            MoveLeft();
        }
        else
        {
            MoveRight();
        }
    }

    void MoveLeft()
    {
        _currentIdx--;
        if (_currentIdx <= -1)
        {
            _currentIdx = _viewTargetList.Count - 1;
        }
        ChangeTargetIcon();
    }

    void MoveRight()
    {
        _currentIdx++;
        if (_currentIdx >= _viewTargetList.Count)
        {
            _currentIdx = 0;

        }
        ChangeTargetIcon();
    }

    void ChangeTargetIcon()
    {
        SetViewIcon(_viewTargetList[_currentIdx]);

        if (_buttonArr[(int)ButtonType.Select].gameObject.activeSelf)
        {
            SetSynthesisSetting();
        }
        else
        {
            if(_targetIcon.GetItemType == EItemList.BOAT)
            {
                _targetIcon.IconEditor();
            }
        }
        CheckDetailState();
    }

    public void EnableEquipButton(bool state) => _buttonArr[(int)ButtonType.Equip].gameObject.SetActive(state);
    public void EnableSelectButton(bool state) => _buttonArr[(int)ButtonType.Select].gameObject.SetActive(state);
    public void EnableUpgradeButton(bool state) => _buttonArr[(int)ButtonType.Upgrade].gameObject.SetActive(state);
    public void EnableCloseButton(bool state) => _buttonArr[(int)ButtonType.Close].gameObject.SetActive(state);
    #endregion ButtonFunc
}
