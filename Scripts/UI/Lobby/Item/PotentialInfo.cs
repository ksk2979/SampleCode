using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyData;

public enum EPotentialType
{
    AtkMultiple = 0,
    AtkAdded = 1,
    HpMultiple = 2,
    HpAdded = 3,
    Ability = 4,
}

public class PotentialInfo : MonoBehaviour
{
    private enum PayType
    {
        Free,
        Diamond,
        Money,
    }

    [SerializeField] int _potentialIndex = 0;
    ItemPotentialData _potentialData = null;
    UnitIcon _targetIcon;

    [Header("Info")]
    [SerializeField] Image _potentialTitleBG;
    [SerializeField] TextLocalizeSetter _potentialTitleText;
    [SerializeField] TextLocalizeSetter _potentialDescText;

    [Header("Buttons")]
    [SerializeField] GameObject[] _buttonSetArr;
    [SerializeField] Button[] _buttonArr;

    [Header("Resource")]
    [SerializeField] Sprite[] _potentialBGArr;

    ItemEditorScript _itemEditor;

    const string _noneString = "???";

    const int costMoney = 5000;
    const int costDia = 30;

    bool _isWorking = false;
    public void Init(ItemEditorScript itemEditor, int idx)
    {
        _itemEditor = itemEditor;
        _potentialIndex = idx;
        InitButton();
    }

    public void ResetInfo()
    {
        _targetIcon = null;
    }

    public void SetInfo(int potentialID)
    {
        if (potentialID > 0)
        {
            // ���ټ��� �Ҵ�Ǿ� �ִ� ����
            _potentialData = _itemEditor.FindPotentialData(potentialID);
            if (_potentialData == null)
            {
                gameObject.SetActive(false);
                return;
            }
            SetUI(_potentialData.name, _potentialData.explanation, _potentialBGArr[1]);
            SetButtonState(false);
        }
        else
        {
            // ���ټ��� �Ҵ�Ǿ� ���� �ʴ� ����
            SetUI(_noneString, _noneString, _potentialBGArr[0]);
            SetButtonState(true);
        }
    }

    /// <summary>
    /// ���ټ� ���� ����
    /// </summary>
    public void SetUI(string title, string desc, Sprite titleBG)
    {
        _potentialTitleText.key = title;
        _potentialDescText.key = desc;
        _potentialTitleBG.sprite = titleBG;
    }

    /// <summary>
    /// ��ȸ�� ���ټ� �Ҵ� �޼���(����, ���)
    /// </summary>
    void SetRandomPotential()
    {
        _potentialData = _itemEditor.GetRandomPotential(_itemEditor.GetPotentialType(_potentialIndex));
        if (_potentialData != null)
        {
            _itemEditor.ChangePotential(_potentialIndex, _potentialData.nId);
            SetUI(_potentialData.name, _potentialData.explanation, _potentialBGArr[1]);
        }
    }

    #region Button
    /// <summary>
    /// ��ư �ʱ�ȭ
    /// </summary>
    void InitButton()
    {
        _buttonArr[(int)PayType.Free].onClick.AddListener(OnTouchFreeButton);
        _buttonArr[(int)PayType.Diamond].onClick.AddListener(OnTouchDiaButton);
        _buttonArr[(int)PayType.Money].onClick.AddListener(OnTouchMoneyButton);
    }

    void SetButtonState(bool isFree)
    {
        _buttonSetArr[0].SetActive(isFree);
        _buttonSetArr[1].SetActive(!isFree);
    }

    public void OnTouchFreeButton()
    {
        if (ButtonCheck())
        {
            SetRandomPotential();
            SetButtonState(false);
            _isWorking = false;
        }
    }

    /// <summary>
    /// ���̾� ��� ���ټ�(��������. ���ý� ����)
    /// </summary>
    public void OnTouchDiaButton()
    {
        if (ButtonCheck())
        {
            UserData userData = UserData.GetInstance;
            if (userData.GetCurrency(EInvenType.Diamond) < costDia)
            {
                MessageHandler.GetInstance.ShowMessage("���̾ư� �����մϴ�", 1.5f);
                _isWorking = false;
                return;
            }
            else
            {
                userData.SubCurrency(EInvenType.Diamond, costDia);
                userData.SaveUserData();
                LobbyUIManager.GetInstance.UpdateUserInfo();

                var potentialPopup = _itemEditor.GetPotentialPopup;
                potentialPopup.SetPotentialIndex(_potentialIndex);
                potentialPopup.OpenPopup();
            }
            _isWorking = false;
        }
    }

    /// <summary>
    /// �ΰ��� ��ȭ ���(1ȸ)
    /// </summary>
    public void OnTouchMoneyButton()
    {
        if (ButtonCheck())
        {
            UserData userData = UserData.GetInstance;
            if (userData.GetCurrency(EInvenType.Money) < costMoney)
            {
                MessageHandler.GetInstance.ShowMessage("��ȭ�� �����մϴ�", 1.5f);
                _isWorking = false;
                return;
            }
            else
            {
                userData.SubCurrency(EInvenType.Money, costMoney);
                userData.SaveUserData();
                LobbyUIManager.GetInstance.UpdateUserInfo();
                SetRandomPotential();
            }
            _isWorking = false;
        }
    }

    bool ButtonCheck()
    {
        if (_isWorking)
        {
            return false;
        }
        else
        {
            _isWorking = true;
            return true;
        }
    }
    #endregion Button
    public void SetInfoState(bool state) => gameObject.SetActive(state);
}
