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
            // 포텐셜이 할당되어 있는 상태
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
            // 포텐셜이 할당되어 있지 않는 상태
            SetUI(_noneString, _noneString, _potentialBGArr[0]);
            SetButtonState(true);
        }
    }

    /// <summary>
    /// 포텐셜 정보 갱신
    /// </summary>
    public void SetUI(string title, string desc, Sprite titleBG)
    {
        _potentialTitleText.key = title;
        _potentialDescText.key = desc;
        _potentialTitleBG.sprite = titleBG;
    }

    /// <summary>
    /// 일회성 포텐셜 할당 메서드(무료, 골드)
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
    /// 버튼 초기화
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
    /// 다이아 사용 포텐셜(양자택일. 선택시 차감)
    /// </summary>
    public void OnTouchDiaButton()
    {
        if (ButtonCheck())
        {
            UserData userData = UserData.GetInstance;
            if (userData.GetCurrency(EInvenType.Diamond) < costDia)
            {
                MessageHandler.GetInstance.ShowMessage("다이아가 부족합니다", 1.5f);
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
    /// 인게임 재화 사용(1회)
    /// </summary>
    public void OnTouchMoneyButton()
    {
        if (ButtonCheck())
        {
            UserData userData = UserData.GetInstance;
            if (userData.GetCurrency(EInvenType.Money) < costMoney)
            {
                MessageHandler.GetInstance.ShowMessage("재화가 부족합니다", 1.5f);
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
