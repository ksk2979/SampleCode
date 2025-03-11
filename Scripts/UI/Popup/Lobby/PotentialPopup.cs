using MyData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

/// <summary>
/// ������ȭ�� ����� ���ټ� ���� �˾�
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
    /// �˾� ���� �� �ش� ���ټ� �ε��� ����
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
    /// ���ټ� ���� �ʱ�ȭ(���� ���ټ� 2�� ����)
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
    /// ���ټ� �ؽ�Ʈ ����
    /// </summary>
    /// <param name="idx">����</param>
    /// <param name="title">Ÿ��Ʋ Ű</param>
    /// <param name="explanation">���� Ű</param>
    /// <param name="buttonText">��ư �ؽ�Ʈ Ű</param>
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
    /// ���ټ� ���� ��ư
    /// </summary>
    /// <param name="idx"></param>
    public void OnTouchPotentialButton(int idx)
    {
        // ����
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
            MessageHandler.GetInstance.ShowMessage("���̾ư� �����մϴ�", 1.5f);
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
    /// ���ټ� �˾����� Ȯ�� 
    /// </summary>
    public void OnTouchCloseButton()
    {
        var popup = popupController.GetPopup<ChoiceAskPopup>(PopupType.ASK);
        popup.SetContext("��� �Ͻðڽ��ϱ�? \n(�Ҹ�� ��ȭ�� ��ȯ���� �ʽ��ϴ�)");
        popup.OnAgreeEventListener += ClosePopup;
        popup.OpenPopup();
    }
    #endregion Button
}
