using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EEnergyTextType
{
    ValueText = 0,
    TimeText,
    AdReadyText,
    None,
}
public class EnergyPopup : PopupBase
{
    private enum ButtonType
    {
        AD,
        PAID,
    }
    LobbyUIManager _lobbyUIManager;
    [SerializeField] TextMeshProUGUI[] _actionText;

    [SerializeField] GameObject[] _inAppActionObj;
    [SerializeField] GameObject[] _actionADObj;

    [SerializeField] Button[] _buttonArr;

    bool _isWorking = false;
    public void Init(LobbyUIManager uiManager)
    {
        _lobbyUIManager = uiManager;
        _buttonArr[(int)ButtonType.AD].onClick.AddListener(OnTouchAdButton);
        _buttonArr[(int)ButtonType.PAID].onClick.AddListener(OnTouchBuyButton);
    }
    private void Update()
    {
        if (IsOpened)
        {
            _lobbyUIManager.RefreshEnergyPopup();
        }
    }

    #region Text
    /// <summary>
    /// 텍스트 내용 입력
    /// </summary>
    public void SetEnergyText(EEnergyTextType type, string text)
    {
        _actionText[(int)type].text = text;
    }

    /// <summary>
    /// 텍스트 On/Off 상태 설정
    /// </summary>
    /// <param name="type">텍스트 타입</param>
    /// <param name="state">텍스트 상태</param>
    public void SwitchTextState(EEnergyTextType type, bool state)
    {
        _actionText[(int)type].gameObject.SetActive(state);
    }
    #endregion Text

    #region Buttons
    public void OnTouchAdButton()
    {
        if (_isWorking) return;
        _isWorking = true;
        // 활성화 체크 필요함
        ShowAD();
        _isWorking = false;
    }

    public void OnTouchBuyButton()
    {
        if (_isWorking) return;
        _isWorking = true;
        ShopBuyPopupWindows();
        _isWorking = false;
    }
    #endregion Buttons
    
    void ShowAD()
    {
        ADManager.GetInstance.ShowRewardedVideo(ERewardType.ActionEnergy);
    }

    void ShopBuyPopupWindows()
    {
        ShopScript shop = _lobbyUIManager.GetShopPage;
        var popup = shop.GetBuyQuestionPopup();
        if (popup.IsOpened) return;

        if (popup != null)
        {
            popup.OnAgreeEventListener += () => shop.BuyShopItem(EShopProduct.ENERGY_02);
            popup.SetContext(shop.GetBuyAskText(EShopProduct.ENERGY_02));
            popup.OpenPopup();
        }
    }

    public override void OpenPopup()
    {
        if(_lobbyUIManager == null)
        {
            _lobbyUIManager = LobbyUIManager.GetInstance;
        }
        SoundManager.GetInstance.PlayAudioEffectSound("UI_Button_Touch");
        base.OpenPopup();
    }

    public override void ClosePopup()
    {
        base.ClosePopup();
    }
}
