using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RoulettePopup : PopupBase
{
    [Header("Components")]
    LobbyUIManager _uiManager;
    UserData _userData;

    [Header("Roulette")]
    [SerializeField] GameObject[] _iconObjList;
    [SerializeField] Image _rouletteImage;

    [Header("Button")]
    [SerializeField] Button _controlButton;
    [SerializeField] Button _disabledButton;
    [SerializeField] GameObject _closeButtonObj;
    [SerializeField] TextLocalizeSetter _buttonText;
    [SerializeField] TextMeshProUGUI _timeTMP;
    [SerializeField] Image _startIconImage;
    [SerializeField] Sprite[] _startIconSpriteArr;

    [Header("Description")]
    [SerializeField] GameObject _descriptionBoard;
    [SerializeField] TextLocalizeSetter _descriptionNameText;
    [SerializeField] TextMeshProUGUI _descriptionCountTMP;
    Transform _rouletteTrans;

    float _rotateSpeed = 25f;       // �ִ� ���ǵ�
    float _curSpeed;                // ���� ���ǵ�
    float _randStopRate;            // ���ߴ� �ӵ�
    bool _isRotated;                // ȸ�� ������ ����
    bool _isReady;                  // ���� ���� �غ�
    bool _isStop;                   // ���ߴ� ���·� ���� ���� ����
    bool _canTouchable;             // ���� ��ȯ(ȸ�� -> ����)�� ���� ��ġ ����
    int _prevIdx = -1;
    int _costDia = 50;
    int _rouletteCount = 0;

    const string keyStart = "����";
    const string keyAD = "���� ��û";
    const string keyCost = "50";
    const string keyStop = "����";
    const string keyMoney01 = "���";
    const string keyMoney02 = "���� ���";
    const string keyDia = "���̾Ƹ��";
    const string keyNormalBox = "������ ����(�⺻)";
    const string keyMaterial = "���� ����";
    const string keyResult = "�귿 ���";

    // Auto Stop
    const float _autoStopTime = 5f;
    float _stopTmpTime = 0f;

    public void Init(LobbyUIManager uiManager)
    {
        _uiManager = uiManager;
        _userData = UserData.GetInstance;
        _isRotated = false;
        _buttonText.key = keyStart;
        _rouletteTrans = _rouletteImage.rectTransform;

        _curSpeed = 0;
        _randStopRate = Random.Range(0.02f, 0.03f);
        _canTouchable = false;
        _descriptionBoard.SetActive(false);

        _rouletteCount = _userData.GetRouletteCount();
        _isReady = true;
        for (int i = 0; i < _iconObjList.Length; i++)
        {
            int b = i;
            Button btn = _iconObjList[i].transform.GetChild(0).GetComponent<Button>();
            btn.onClick.AddListener(() => { OnTouchItemButton(b); });
        }
        CheckAlram();
    }

    #region Setter
    /// <summary>
    /// ��ư ���� ����
    /// </summary>
    /// <param name="isActivate">Ȱ��ȭ ����</param>
    public void SetButtonState(bool isActivate)
    {
        if (!_isReady) return;

        if (isActivate)
        {
            Debug.Log("SetButtonState : " + isActivate);
            // ���� Ƚ�� 0ȸ  
            _buttonText.key = keyStart;
            _controlButton.gameObject.SetActive(isActivate);
            _disabledButton.gameObject.SetActive(!isActivate);
            _startIconImage.gameObject.SetActive(false);
        }
        else
        {
            if (_isRotated) return;

            if (_rouletteCount > 2)
            {
                _controlButton.gameObject.SetActive(isActivate);
                _disabledButton.gameObject.SetActive(!isActivate);
                return;
            }
            else
            {
                _startIconImage.gameObject.SetActive(true);
                _controlButton.gameObject.SetActive(true);
                _startIconImage.rectTransform.sizeDelta = new Vector2(50f, 50f);
                if (_rouletteCount == 1)
                {
                    // ���� ��û
                    _startIconImage.sprite = _startIconSpriteArr[0];
                    _buttonText.key = keyAD;
                }
                else if(_rouletteCount == 2)
                {
                    // ���̾�
                    _startIconImage.sprite = _startIconSpriteArr[1];
                    _buttonText.key = keyCost;
                }
            }
        }
    }

    /// <summary>
    /// �귿 ���� �޼���
    /// </summary>
    public void ResetRoullet()
    {
        _userData.SetRouletteCount(0);
        _userData.SaveDailyRewardData();
        _rouletteCount = 0;
    }

    /// <summary>
    /// �귿 �ʱ�ȭ ���� �ð� �ؽ�Ʈ
    /// </summary>
    /// <param name="timeText"></param>
    public void SetTimeText(string timeText)
    {
        _timeTMP.text = timeText;
    }

    #endregion Setter

    private void Update()
    {
        if (_isRotated)
        {
            if (_isStop)
            {
                if (_curSpeed > 0.01f)
                {
                    _curSpeed = Mathf.Lerp(_curSpeed, 0, _randStopRate);
                }
                else
                {
                    _isRotated = false;
                }
            }
            else
            {
                if (_curSpeed < _rotateSpeed)
                {
                    _curSpeed = Mathf.Lerp(_curSpeed, _rotateSpeed, 0.05f);
                    if (_curSpeed > _rotateSpeed - 0.5f)
                    {
                        _curSpeed = _rotateSpeed;
                        _canTouchable = true;
                        _controlButton.interactable = _canTouchable;
                    }
                }
                else
                {
                    // �ִ� �ӵ� ���� ����
                    if(_stopTmpTime < _autoStopTime)
                    {
                        _stopTmpTime += Time.deltaTime;
                        if(_stopTmpTime >= _autoStopTime)
                        {
                            _isStop = true;
                        }
                    }
                }
            }
            _rouletteTrans.Rotate(new Vector3(0, 0, _curSpeed));

            if (!_isRotated)
            {
                // ���� ����
                _isStop = false;
                _stopTmpTime = 0;
                _canTouchable = false;
                _curSpeed = 0;
                _randStopRate = Random.Range(0.02f, 0.03f);

                PresentReward();
                _uiManager.ResetCheckTime(ETimeCheckType.DAILY_ROULETTE);
                SetButtonState(false);

                // ���� ����Ʈ
                if(_userData.GetDailyQuest()[(int)EDailyQuest.ROULETTE] <= 0)
                {
                    _userData.SetDailyQuest(EDailyQuest.ROULETTE, 1);
                    popupController.GetPopup<QuestPopup>(PopupType.QUEST).CheckDailyQuestSet();
                    _userData.SaveQuestData();
                }
                StartCoroutine(DelayReady());
            }
        }
    }

    IEnumerator DelayReady()
    {
        yield return new WaitForSeconds(0.5f);
        _isReady = true;
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    void PresentReward()
    {
        float angle = _rouletteTrans.localEulerAngles.z;
        ERewardType rType = ERewardType.None;
        int rValue = 0;
        bool isPopupOpen = true;
        if (angle > 360)
        {
            Debug.Log("Over Range");
            angle %= 360;
        }

        if (angle > 330 && angle <= 360 || angle >= 0 && angle <= 30)
        {
            //Debug.Log("Range 1 : Money 1000");
            rType = ERewardType.Money;
            rValue = 1000;
        }
        else if (angle > 30 && angle <= 90)
        {
            //Debug.Log("Range 2 : Dia 50");
            rType = ERewardType.Diamond;
            rValue = 50;
        }
        else if (angle > 90 && angle <= 150)
        {
            //Debug.Log("Range 3 : Money 10000");
            rType = ERewardType.Money;
            rValue = 10000;
        }
        else if (angle > 150 && angle <= 210)
        {
            //Debug.Log("Range 4 : Normal Box");
            rType = ERewardType.NormalBox;
            rValue = 1;
            isPopupOpen = false;
        }
        else if (angle > 210 && angle <= 270)
        {
            //Debug.Log("Range 5 : Money Random Box 1000 ~ 100000");
            int randMoney = Random.Range(1000, 100001);
            rType = ERewardType.Money;
            rValue = randMoney;
        }
        else
        {
            //Debug.Log("Range 6 : Material Random 1 ~ 5");
            int randMats = Random.Range((int)EMaterialType.COPPER, (int)EMaterialType.NONE);
            int randCount = Random.Range(1, 6);
            rType = (ERewardType)(randMats + (int)ERewardType.Copper);
            rValue = randCount;
        }
        _uiManager.GetShopPage.ProvideNotPurchasingReward(EPayType.NONE, rType, rValue, keyResult, false, true);
       if(isPopupOpen)
        {
            _uiManager.OpenRewardResultPopup(0.2f);
        }
        _closeButtonObj.SetActive(true);
    }

    /// <summary>
    /// �˶� üũ
    /// </summary>
    void CheckAlram()
    {
        if (_rouletteCount <= 1)
        {
            _uiManager.SetBadge(BadgeType.Roulette, true);
        }
        else
        {
            _uiManager.SetBadge(BadgeType.Roulette, false);
        }
    }

    /// <summary>
    /// �귿 ��Ʈ�� ���
    /// </summary>
    void ControllRoulette()
    {
        if (!_isRotated)
        {
            _isRotated = true;
            _isReady = false;
            _buttonText.key = keyStop;
            _closeButtonObj.SetActive(false);
            _descriptionBoard.SetActive(false);
        }
        else
        {
            _isStop = true;
        }
    }

    /// <summary>
    /// �귿 ������ ��ư
    /// </summary>
    public void OnTouchRouletteButton()
    {
        if (_isStop || (_isRotated && !_canTouchable)) return;

        // �귿 ���߱�
        if(_isRotated)
        {
            ControllRoulette();
            return;
        }

        // �귿 ������
        if (_rouletteCount > 2 || !_isReady) return;

        _controlButton.interactable = _canTouchable;
        if (_rouletteCount == 0)
        {
            // ����
            ControllRoulette();
            CountUpRoulette();
        }
        else if(_rouletteCount == 1)
        {
            ADManager.GetInstance.ShowRewardedVideo(() => 
            {
                _startIconImage.gameObject.SetActive(false);
                ControllRoulette();
                CountUpRoulette();
                FirebaseManager.GetInstance.LogEvent("Roulette_AD");
            }, 
            () => 
            {
                _controlButton.interactable = true;
            });
        }
        else
        {
            // ���̾�
            if(_userData.GetCurrency(EPropertyType.DIAMOND) < _costDia)
            {
                _uiManager.GetShopPage.ShowMessageNotEnoughDia();
                _controlButton.interactable = true;
                return;
            }
            _userData.SubCurrency(EPropertyType.DIAMOND, _costDia);
            _userData.SaveUserData();
            _uiManager.UpdateUserInfo();

            _startIconImage.gameObject.SetActive(false);
            ControllRoulette();
            CountUpRoulette();
            FirebaseManager.GetInstance.LogEvent("Roulette_Dia");
        }
    }

    /// <summary>
    /// �귿 Ƚ�� ī��Ʈ ��
    /// </summary>
    void CountUpRoulette()
    {
        _rouletteCount++;
        _userData.SetRouletteCount(_rouletteCount);
        _userData.SaveDailyRewardData();
        CheckAlram();
    }

    /// <summary>
    /// �귿 ������ ��ġ ��
    /// </summary>
    public void OnTouchItemButton(int idx)
    {
        if(_prevIdx == idx)
        {
            OnTouchCloseDescription();
            return;
        }
        else
        {
            _prevIdx = idx;
        }
        Transform targetTrans = _iconObjList[idx].transform;

        _descriptionBoard.transform.position =
            new Vector3(targetTrans.position.x, targetTrans.position.y + 0.25f, 0);

        switch (idx)
        {
            case 0:
                {
                    _descriptionNameText.key = keyMoney01;
                    _descriptionCountTMP.text = string.Format("X {0:N0}", 1000);
                }
                break;
            case 1:
                {
                    _descriptionNameText.key = keyDia;
                    _descriptionCountTMP.text = string.Format("X {0:N0}", 50);
                }
                break;
            case 2:
                {
                    _descriptionNameText.key = keyMoney01;
                    _descriptionCountTMP.text = string.Format("X {0:N0}", 10000);
                }
                break;
            case 3:
                {
                    _descriptionNameText.key = keyNormalBox;
                    _descriptionCountTMP.text = string.Format("X {0}", 1);
                }
                break;
            case 4:
                {
                    _descriptionNameText.key = keyMoney02;
                    _descriptionCountTMP.text = string.Format("X {0:N0} ~\nX {1:N0}", 1000, 100000);
                }
                break;
            case 5:
                {
                    _descriptionNameText.key = keyMaterial;
                    _descriptionCountTMP.text = string.Format("X {0} ~ X {1}", 1, 5);
                }
                break;
        }
        _descriptionBoard.SetActive(true);
    }
    public void OnTouchCloseDescription()
    {
        _prevIdx = -1;
        _descriptionBoard.SetActive(false);
    }

    public override void OpenPopup()
    {
        _descriptionBoard.SetActive(false);
        base.OpenPopup();
    }

    public void OnTouchCloseButton()
    {
        if (_isRotated || !_closeButtonObj.activeSelf) return;

        ClosePopup();
    }
}
