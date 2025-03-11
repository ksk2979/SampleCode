using MyData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using System;

public class ShopProduct : MonoBehaviour
{
    [SerializeField] EShopProduct _productType;

    ShopScript _shopScript;
    ShopProductData _productData;

    [Header("Info")]
    [SerializeField] TextLocalizeSetter _nameText;
    [SerializeField] GameObject _priceIconObj;
    [SerializeField] TextMeshProUGUI _priceText;
    [SerializeField] Button _buyButton;

    [Header("Option")]
    [SerializeField] GameObject _adButtonsSet;
    [SerializeField] Button _adButton;
    [SerializeField] TextMeshProUGUI _priceText02;
    [SerializeField] Button _buyButton02;

    bool _checkAllowButton = false;
    const float _allowTime = 0.5f;
    float _tmptime = 0;

    int _specificBox = 16;
    int _normalPrice = 0;
    int _elitePrice = 0;

    public void InitProduct(ShopScript shopScript)
    {
        _shopScript = shopScript;
        _productData = _shopScript.FindShopProductData(_productType);

        if (_productData != null)
        {
            if(_productData.nId <= 6)
            {
                _nameText.key = string.Format("DIAMOND {0}", _productData.rewardValue);
            }
            else
            {
                _nameText.key = _productData.productKey;
            }
            
            if(_productData.nId >= _specificBox && _productData.nId <= _specificBox + (int)EItemList.ENGINE)
            {
                var normalData = _shopScript.FindShopProductData(EShopProduct.NORMAL_BOX);
                var eliteData = _shopScript.FindShopProductData(EShopProduct.ELITE_BOX);

                _normalPrice = Mathf.RoundToInt(normalData.price * 1.5f);
                _elitePrice = Mathf.RoundToInt(eliteData.price * 1.5f);
                _priceText.text = _normalPrice.ToString("N0");
                _priceText02.text = _elitePrice.ToString("N0");
            }
            else
            {
                _priceText.text = _productData.price.ToString("N0");
            }
        }
    }

    private void OnEnable()
    {
        _buyButton.onClick.AddListener(OnTouchBuyButton);
        if(_buyButton02 != null)
        {
            _buyButton02.onClick.AddListener(OnTouchBuyButton02);
        }
        if (_adButton != null)
        {
            _adButton.onClick.AddListener(OnTouchADButton);
        }
    }

    void BuyButton(EShopProduct productType, int btnIdx = 0)
    {
        if (_checkAllowButton)
        {
            return;
        }
        else
        {
            StartButtonCheck();
        }

        if (productType <= EShopProduct.DIAMOND_06)
        {
            _shopScript.InAppCheck(_shopScript.FindShopProductData(productType).productKey);
        }
        else if (productType >= EShopProduct.NORMAL_BOX || productType <= EShopProduct.ENGINE_BOX)
        {
            var popup = _shopScript.GetBuyQuestionPopup();
            if(productType <= EShopProduct.ELITE_TEN_BOX)
            {
                popup.OnAgreeEventListener += () => _shopScript.BuyShopItem(productType);
                popup.SetContext(_shopScript.GetBuyAskText(productType));
            }
            else
            {
                if(btnIdx == 0)
                {
                    popup.OnAgreeEventListener += () => _shopScript.BuyShopItem(productType, _normalPrice);
                    popup.SetContext(_shopScript.GetBuyAskText(EShopProduct.NORMAL_BOX));
                }
                else
                {
                    popup.OnAgreeEventListener += () => _shopScript.BuyShopItem(productType, _elitePrice, true);
                    popup.SetContext(_shopScript.GetBuyAskText(EShopProduct.ELITE_BOX));
                }
            }
            popup.OpenPopup();
        }
        else
        {
            _shopScript.BuyShopItem(productType);
        }
    }

    #region Button
    public void OnTouchBuyButton()
    {
        BuyButton(_productType);
    }

    public void OnTouchBuyButton02()
    {
        BuyButton(_productType, 1);
    }

    public void OnTouchADButton()
    {
        if (_checkAllowButton)
        {
            return;
        }
        else
        {
            StartButtonCheck();
        }
        ADManager.GetInstance.ShowRewardedVideo((ERewardType)_productData.rewardType);
    }

    public void StartButtonCheck()
    {
        _checkAllowButton = true;
        StartCoroutine(CheckAllowButton());
    }

    IEnumerator CheckAllowButton()
    {
        while (_checkAllowButton)
        {
            _tmptime += 0.02f;
            if (_tmptime >= _allowTime)
            {
                _checkAllowButton = false;
                _tmptime = 0;
            }
            yield return YieldInstructionCache.RealTimeWaitForSeconds(0.02f);
        }
    }
    #endregion Button

    #region AD
    public void SetPriceState(bool activateAD)
    {
        if(_adButtonsSet != null)
        {
            if(_priceIconObj != null)
            {
                _priceIconObj.SetActive(!activateAD);
                _priceText.gameObject.SetActive(!activateAD);
            }
            _adButtonsSet.SetActive(activateAD);
            if (_adButton != null)
            {
                _adButton.interactable = activateAD;
            }
        }
    }
    #endregion AD
}
