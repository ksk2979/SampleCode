using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class OneDayShopItem : MonoBehaviour
{
    [SerializeField] ShopScript _shop;
    [SerializeField] Button _buyBtn;
    [SerializeField] TextLocalizeSetter _nameText;
    [SerializeField] TextMeshProUGUI _nameNumberText;
    [SerializeField] Image _iconImage;
    [SerializeField] GameObject _itemBlur;

    [Header("Price")]
    [SerializeField] GameObject _priceSet;
    [SerializeField] Image _priceIcon;
    [SerializeField] TextMeshProUGUI _priceText;

    [Header("AD")]
    [SerializeField] GameObject _adTextSet;

    int _itemIdx = 0;
    [SerializeField] EPayType _payType = EPayType.NONE;
    int _price;

    ERewardType _productType = ERewardType.None;
    int _productCount = 0;

    const string spritePathFormat = "ItemIcon/Inven{0}"; // 스프라이트 경로

    Dictionary<ERewardType, int> _freeItemDictionary = new Dictionary<ERewardType, int>()
    {
        {ERewardType.Money, 10000},
        {ERewardType.Diamond, 30},
        {ERewardType.NormalBox, 1}
    };

    Dictionary<string, string> _textStringDictionary = new Dictionary<string, string>()
    {
        {"Free", "무료"},
        {ERewardType.Diamond.ToString(), "보석"},
        {ERewardType.Money.ToString(), "돈"},
        {ERewardType.Copper.ToString(), "구리"},
        {ERewardType.Zinc.ToString(), "아연"},
        {ERewardType.Aluminum.ToString(), "알루미늄"},
        {ERewardType.Steel.ToString(), "철"},
        {ERewardType.Gold.ToString(), "주석"},
        {ERewardType.Oil.ToString(), "티타늄"},
        {ERewardType.NormalBox.ToString(), "기본 상자"}
    };

    public void Init(int idx)
    {
        _itemIdx = idx;
    }

    public int MaterialNumberCount()
    {
        int rand = Random.Range(0, 100);
        InitMaterialNumber(rand);
        return rand;
    }
    void InitMaterialNumber(int rand)
    {
        if (rand >= 0 && rand <= 9)
        {
            _price = 100;
            _productCount = 80;
        }
        else if (rand >= 10 && rand <= 29)
        {
            _price = 80;
            _productCount = 50;
        }
        else if (rand >= 30 && rand <= 60)
        {
            _price = 50;
            _productCount = 30;
        }
        else
        {
            _price = 20;
            _productCount = 10;
        }
    }

    /// <summary>
    /// 랜덤으로 정해진 상품번호를 돌려주는 메서드
    /// </summary>
    /// <param name="isFree">무료 상품 여부</param>
    /// <returns></returns>
    public int SelectRandomProduct(bool isFree)
    {
        if (isFree)
        {
            int rand = Random.Range(1, 100);
            ERewardType rType = ERewardType.None;
            int rValue = 0;
            if (rand <= 33)
            {
                rType = ERewardType.Money;
                rValue = 10000;
            }
            else if (rand <= 66)
            {
                rType = ERewardType.Diamond;
                rValue = 30;
            }
            else
            {
                rType = ERewardType.NormalBox;
                rValue = 1;
            }
            SetFreeProduct(rType, rValue);
            return (int)_productType;
        }
        else
        {
            _productType = (ERewardType)Random.Range(3, 9);
            SetPaidProduct(_productType);
            return (int)_productType;
        }
    }

    /// <summary>
    /// 기존에 초기화 되어있는 상품의 정보를 세팅
    /// </summary>
    /// <param name="type"></param>
    /// <param name="twoKey"></param>
    public void SetProduct(ERewardType type, int twoKey)
    {
        _productType = type;
        if (type >= ERewardType.Copper && type <= ERewardType.Oil)
        {
            InitMaterialNumber(twoKey);
            SetPaidProduct(type);
        }
        else
        {
            SetFreeProduct(type, _freeItemDictionary[type]);
        }
    }

    /// <summary>
    /// 무료 상품 설정
    /// </summary>
    /// <param name="rType"></param>
    /// <param name="rValue"></param>
    void SetFreeProduct(ERewardType rType, int rValue)
    {
        _payType = EPayType.NONE;
        _productType = rType;
        _price = 0;
        _productCount = rValue;
        _priceText.GetComponent<TextLocalizeSetter>().key = _textStringDictionary["Free"];
        _nameText.key = _textStringDictionary[rType.ToString()];
        _nameNumberText.text = string.Format("{0}", _productCount);
        _iconImage.sprite = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(spritePathFormat, rType.ToString()));
        _priceIcon.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// 결제 상품 정보(UI) 설정
    /// </summary>
    /// <param name="type"></param>
    void SetPaidProduct(ERewardType type)
    {
        _payType = EPayType.AD;
        if (_payType == EPayType.AD)
        {
            _priceSet.SetActive(false);
            _adTextSet.SetActive(true);
        }
        else
        {
            _priceText.text = _price.ToString();
            _priceSet.SetActive(true);
            _adTextSet.SetActive(false);
        }
        _productType = type;
        _nameText.key = _textStringDictionary[type.ToString()];
        _nameNumberText.text = string.Format("{0}", _productCount);
        _iconImage.sprite = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(spritePathFormat, type.ToString()));
    }

    /// <summary>
    /// 구매 처리 (UI)
    /// </summary>
    public void SetBlockedButton()
    {
        _buyBtn.interactable = false;
        _itemBlur.SetActive(true);
    }

    public void OnTouchPurchaseButton()
    {
        LobbyUIManager.GetInstance.GetPopup<RewardResultPopup>(PopupType.REWARDRESULT).ResetPopup();
        // 무료 상품
        if (_payType == EPayType.NONE)
        {
            _shop.CheckDailyProductQuest();
            _shop.BuyShopDailyProduct(0);
            _shop.SetBadge(false);
        }
        else
        {
            if (_payType == EPayType.AD)
            {
                ADManager.GetInstance.ShowRewardedVideo(()=>
                {
                    _shop.BuyShopDailyProduct(_itemIdx, true);
                    LobbyUIManager.GetInstance.OpenRewardResultPopup(0.2f);
                },
                () => {
                    MessageHandler.GetInstance.ShowMessage("광고가 준비중입니다 잠시 후 다시 시도해주세요", 1.5f);
                });
                // 리롤 기능 들어오면 그때 별도 처리
            }
        }
    }

    public void ResetItem()
    {
        _productType = ERewardType.None;
        _price = 0;
        _productCount = 0;
        _buyBtn.interactable = true;
        _itemBlur.SetActive(false);
        _priceIcon.transform.gameObject.SetActive(true);
        _priceSet.SetActive(true);
        if (_adTextSet != null)
        {
            _adTextSet.SetActive(true);
        }
    }

    public ERewardType GetProductType => _productType;
    public EPayType GetPayType => _payType;
    public int GetPrice => _price;
    public int GetProcutCount => _productCount;
}
