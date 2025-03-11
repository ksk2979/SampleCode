using MyData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

/// <summary>
/// 유료재화를 사용한 포텐셜 선택 팝업
/// </summary>
public class PotentialPopup : PopupBase
{
    ItemEditorScript _itemEditor;
    ItemPotentialData[] _potentialDataArr;

    [Header("UI")]
    [SerializeField] Button[] _buttonArr;
    [SerializeField] Button _closeButton;
    [SerializeField] Button _rerollButton;

    [Header("Potential")]
    [SerializeField] TextLocalizeSetter[] _potentialTitleArr;
    [SerializeField] TextLocalizeSetter[] _potentialDescArr;
    [SerializeField] Image[] _potenBGArr;
    [SerializeField] Image[] _buttonImageArr;

    int _potentialIndex = -1;

    const int costDia = 30;

    /// <summary>
    /// 팝업 열기 전 해당 포텐셜 인덱스 설정
    /// </summary>
    /// <param name="idx"></param>
    public void SetPotentialIndex(int idx) => _potentialIndex = idx;

    public void Init(ItemEditorScript itemEditor)
    {
        _itemEditor = itemEditor;
        for (int i = 0; i < _buttonArr.Length; i++)
        {
            int idx = i;
            _buttonArr[i].onClick.AddListener(()=> OnTouchPotentialButton(idx));
        }
        _potentialDataArr = new ItemPotentialData[2];
        _closeButton.onClick.AddListener(OnTouchCloseButton);
        _rerollButton.onClick.AddListener(OnTouchRerollButton);
    }

    /// <summary>
    /// 포텐셜 정보 초기화(랜덤 포텐셜 2개 등장)
    /// </summary>
    void SetRandomPotentialInfo()
    {
        for (int i = 0; i < 2; i++)
        {
            _potentialDataArr[i] = _itemEditor.GetRandomPotential(_itemEditor.GetPotentialType(_potentialIndex));
            if(i == 1)
            {
                while (_potentialDataArr[0].nId == _potentialDataArr[i].nId)
                {
                    _potentialDataArr[i] = _itemEditor.GetRandomPotential(_itemEditor.GetPotentialType(_potentialIndex));
                }
            }
            SetText(i, _potentialDataArr[i].name, _potentialDataArr[i].explanation);
        }
    }

    #region UI
    /// <summary>
    /// 포텐셜 텍스트 설정
    /// </summary>
    /// <param name="idx">순번</param>
    /// <param name="title">타이틀 키</param>
    /// <param name="explanation">설명 키</param>
    /// <param name="buttonText">버튼 텍스트 키</param>
    void SetText(int idx, string title, string explanation)
    {
        _potentialTitleArr[idx].key = title;
        _potentialDescArr[idx].key = explanation;
    }
    #endregion UI

    public override void OpenPopup()
    {
        SetRandomPotentialInfo();
        base.OpenPopup();
    }
    public override void ClosePopup()
    {
        for(int i = 0; i < _potentialDataArr.Length; i++)
        {
            _potentialDataArr[i] = null;
        }
        _potentialIndex = -1;
        base.ClosePopup();
    }

    #region Button
    /// <summary>
    /// 포텐셜 선택 버튼
    /// </summary>
    /// <param name="idx"></param>
    public void OnTouchPotentialButton(int idx)
    {
        // 선택
        if (_potentialDataArr[idx] == null || _potentialIndex == -1)
        {
            Debug.LogError("PotentialData is null");
            return;
        }
        _itemEditor.ChangePotential(_potentialIndex, _potentialDataArr[idx].nId);
        _itemEditor.RefreshPotentialInfo(_potentialIndex, _potentialDataArr[idx].nId);
        ClosePopup();
    }

    public void OnTouchRerollButton()
    {
        UserData userData = UserData.GetInstance;
        if (userData.GetCurrency(EInvenType.Diamond) < costDia)
        {
            MessageHandler.GetInstance.ShowMessage("다이아가 부족합니다", 1.5f);
            return;
        }
        else
        {
            userData.SubCurrency(EInvenType.Diamond, costDia);
            userData.SaveUserData();
            LobbyUIManager.GetInstance.UpdateUserInfo();

            SetRandomPotentialInfo();
        }
    }

    /// <summary>
    /// 포텐셜 팝업종료 확인 
    /// </summary>
    public void OnTouchCloseButton()
    {
        var popup = popupController.GetPopup<ChoiceAskPopup>(PopupType.ASK);
        popup.SetContext("취소 하시겠습니까? \n(소모된 재화는 반환되지 않습니다)");
        popup.OnAgreeEventListener += ClosePopup;
        popup.OpenPopup();
    }
    #endregion Button
}
