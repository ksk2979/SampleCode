using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : PopupBase
{
    private enum ButtonType
    {
        AD,
        DIA,
        CANCEL,
        HOME,
    }
    ResourceManager _resourceManager;
    public event PopupEventDelegate OnADEventListener;
    public event PopupEventDelegate OnDiaEventListener;
    public event PopupEventDelegate OnCancelEventListener;

    const string IconPath = CommonStaticDatas.RES_UI;
    const string SpritePath = "ItemIcon/{0}";
    const string IconPrev = "Inven";
    const string LogFormat = "StageReward_{0}";
    [Header("IconBox")]
    [SerializeField] RectTransform _iconRectTrans;
    [SerializeField] GameObject _emptyTextObj;

    [Header("Button")]
    [SerializeField] GameObject _checkButtonObj;
    [SerializeField] GameObject _bonusButtonSet;
    [SerializeField] Button[] _buttonArr;
    [Header("Quit")]
    [SerializeField] Button _quitButton;
    [SerializeField] TextMeshProUGUI _quitTMP;
    bool _isQuitCheck = false;
    const float _quitTime = 1.0f;
    float _tmpQuitTime = 0f;
    [SerializeField] TextMeshProUGUI _titleText;

    [SerializeField] GameObject[] _winLoseImg;

    /// <summary>
    /// 리워드 아이콘 생성 및 수량 표기
    /// </summary>
    /// <param name="rCountArr">리워드 수량 배열</param>
    public void SetRewardInfo(int[] rCountArr)
    {
        _quitButton.interactable = false;
        _quitTMP.gameObject.SetActive(false);
        _resourceManager = ResourceManager.GetInstance;
        bool isEmpty = true;
        List<MaterialIcon> iconList = new List<MaterialIcon>();
        for (int i = 0; i < rCountArr.Length; i++)
        {
            if (rCountArr[i] <= 0)
            {
                continue;
            }
            isEmpty = false;
            string iconName = ((EInvenType)i).ToString();
            GameObject icon = SimplePool.Spawn(IconPath, "MaterialIcon");
            MaterialIcon iconScript = icon.GetComponent<MaterialIcon>();
            iconList.Add(iconScript);

            icon.transform.SetParent(_iconRectTrans);
            icon.transform.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            iconScript.SetIconInfo(_resourceManager.GetSpriteClipForKey(string.Format(SpritePath, IconPrev + iconName)), rCountArr[i]);
        }

        // 비어있으면 텍스트 ON / 1개라도 존재시 OFF
        if (isEmpty)
        {
            _emptyTextObj.SetActive(true);
            _bonusButtonSet.SetActive(false);
            _checkButtonObj.SetActive(true);
        }
        else
        {
            _emptyTextObj.SetActive(false);
            _bonusButtonSet.SetActive(true);
            _checkButtonObj.SetActive(false);

            OnADEventListener += () =>
            {
                ADManager.GetInstance.ShowRewardedVideo(() =>
                {
                    Debug.Log("광고 보기 성공");
                    // 광고 보기 성공
                    FirebaseManager.GetInstance.LogEvent(string.Format(LogFormat, "AD"));
                    SetBonusReward(iconList, rCountArr, 1.5f);
                    _bonusButtonSet.SetActive(false);
                    _checkButtonObj.SetActive(true);

                    StartQuitCheck();
                    OnADEventListener = null;
                },
                () => { Debug.Log("광고 보기 실패"); },
                "BonusReward");
            };

            OnDiaEventListener += () =>
            {
                if (UserData.GetInstance.GetCurrency(EPropertyType.DIAMOND) < 50)
                {
                    MessageHandler.GetInstance.ShowMessage("다이아가 부족합니다", 1.5f);
                    return;
                }
                UserData.GetInstance.SubCurrency(EPropertyType.DIAMOND, 50);
                FirebaseManager.GetInstance.LogEvent(string.Format(LogFormat, "DIA"));
                SetBonusReward(iconList, rCountArr, 3.0f);
                _bonusButtonSet.SetActive(false);
                _checkButtonObj.SetActive(true);

                StartQuitCheck();
                OnDiaEventListener = null;
            };
        }
        _buttonArr[(int)ButtonType.AD].onClick.AddListener(OnTouchADbutton);
        _buttonArr[(int)ButtonType.DIA].onClick.AddListener(OnTouchDiaButton);
        _buttonArr[(int)ButtonType.CANCEL].onClick.AddListener(OnTouchCancelButton);
        _buttonArr[(int)ButtonType.HOME].onClick.AddListener(OnTouchCheckButton);
    }

    /// <summary>
    /// 광고, 보석 보너스 보상 텍스트 설정
    /// </summary>
    /// <param name="rCountArr"></param>
    public void SetBonusReward(List<MaterialIcon> iconList, int[] rCountArr, float rate)
    {
        UserData userData = UserData.GetInstance;
        int idx = 0;
        for(int i = 0; i < rCountArr.Length; i++)
        {
            if (rCountArr[i] <= 0)
            {
                continue;
            }
            int bonusReward = (int)(rCountArr[i] * rate);
            //Debug.Log((EInvenType)i + " : " + bonusReward);
            iconList[idx].SetBonusInfo(bonusReward);
            userData.AddCurrency((EInvenType)i, bonusReward);
            idx++;
        }
    }

    /// <summary>
    /// 터치 시 로비로 이동하는 백보드 버튼 활성화 체크
    /// </summary>
    /// <returns></returns>
    public void StartQuitCheck()
    {
        _isQuitCheck = true;
        StartCoroutine(CheckBackBoard());
    }

    IEnumerator CheckBackBoard()
    {
        while (_isQuitCheck)
        {
            _tmpQuitTime += 0.02f;
            if (_tmpQuitTime >= _quitTime)
            {
                _quitButton.interactable = true;
                _quitTMP.gameObject.SetActive(true);
                _isQuitCheck = false;
            }
            yield return YieldInstructionCache.RealTimeWaitForSeconds(0.02f);
        }
    }

    public void WinLoseImgChange(bool die)
    {
        for (int i = 0; i < _winLoseImg.Length; ++i)
        {
            _winLoseImg[i].SetActive(false);
        }
        if (die)
        {
            _winLoseImg[1].SetActive(true);
        }
        else
        {
            _winLoseImg[0].SetActive(true);
        }
    }

    #region Buttons
    /// <summary>
    /// 광고 보상 : 1.5배
    /// </summary>
    public void OnTouchADbutton()
    {
        _buttonArr[(int)ButtonType.AD].onClick.RemoveAllListeners();
        OnADEventListener?.Invoke();
		OnADEventListener = null;
    }

    /// <summary>
    /// 다이아 보상 : 3.0배
    /// </summary>
    public void OnTouchDiaButton()
    {
        _buttonArr[(int)ButtonType.DIA].onClick.RemoveAllListeners();
        OnDiaEventListener?.Invoke();
		OnADEventListener = null;
    }

    public void OnTouchCancelButton()
    {
        _buttonArr[(int)ButtonType.CANCEL].onClick.RemoveAllListeners();
        _bonusButtonSet.SetActive(false);
        _checkButtonObj.SetActive(true);

        StartQuitCheck();
    }

    public void OnTouchCheckButton()
    {
        _buttonArr[(int)ButtonType.HOME].onClick.RemoveAllListeners();
        ClosePopup();
    }

    public void TitleEndTextSetting(string text)
    {
        if (_titleText == null) { return; }
        _titleText.text = text;
    }
    #endregion Buttons
}
