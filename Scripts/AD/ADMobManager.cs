using UnityEngine;
using System.Collections;
using System;
using MyStructData;
/*
앱 ID = ca-app-pub-3689962790424883~8027349667
테스트 = ca-app-pub-3940256099942544~3347511713

ca-app-pub-3689962790424883/8153761496 NormalBox
 */

public enum EADType
{
    MONEY = 0,
    DIA,
    ENERGY,
    NORMALBOX,
    ELITEBOX,
    DAILYPRODUCT,
    ROULETTE,
    REVIVAL,
    STAGEREWARD,
    NONE,
}

public class ADMobManager : Singleton<ADMobManager>
{
    //RewardedAd[] _adReward;
    string adUnitId;
    public bool _test = false;

    bool[] _adCheck;

    ShopScript _shop;

    string[] _adKey =
    {
        "ca-app-pub-3940256099942544/5224354917",       // Test01
        "ca-app-pub-3940256099942544/5224354917",       // Test02
        "ca-app-pub-3940256099942544/5224354917",       // Test03
        "ca-app-pub-3940256099942544/5224354917",       // Test04
        "ca-app-pub-3940256099942544/5224354917",       // Test05
        "ca-app-pub-3940256099942544/5224354917",       // Test06
        "ca-app-pub-3940256099942544/5224354917",       // Test07
        "ca-app-pub-3940256099942544/5224354917",       // Test08
        "ca-app-pub-3940256099942544/5224354917",       // Test09
/*        "ca-app-pub-3689962790424883/1967292054",       // Money
        "ca-app-pub-3689962790424883/3471945419",       // Diamond
        "ca-app-pub-3689962790424883/6729298873",       // Energy
        "ca-app-pub-3689962790424883/8153761496",       // NormalBox
        "ca-app-pub-3689962790424883/2278618287",       // EliteBox
        "ca-app-pub-3689962790424883/7957456346",       // ShopDailyProduct
        "ca-app-pub-3689962790424883/2775883055",       // Roulette
        "ca-app-pub-3689962790424883/7556139276",       // Revival
        "ca-app-pub-3689962790424883/9960695357",       // StageReward*/
    };

    public void Start()
    {
/*        _adReward = new RewardedAd[(int)EADType.NONE];
        _adCheck = new bool[(int)EADType.NONE];
        for (int i = 0; i < _adCheck.Length; ++i) { _adCheck[i] = false; }
        // 모바일 광고 SDK를 초기화함.
        MobileAds.Initialize(initStatus => { });
        StartCoroutine(DelayCreateLoadAD());*/
    }
    IEnumerator DelayCreateLoadAD()
    {
        for(int i = 0; i < (int)EADType.NONE; i++)
        {
            yield return YieldInstructionCache.WaitForSeconds(1f);
            ADRewardSetting((EADType)i);
        }
    }

    void ADRewardSetting(EADType type)
    {
#if UNITY_ANDROID
        if (_test)
        {
            adUnitId = "ca-app-pub-3940256099942544/5224354917";
        }
        else
        {
            adUnitId = _adKey[(int)type];
        }
#else
        adUnitId = "unexpected_platform";
#endif

       /* if (_adReward[(int)type] != null)
        {
            _adReward[(int)type].Destroy();
            _adReward[(int)type] = null;
        }

        AdRequest request = new AdRequest();
        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load an ad " + "with error : " + error);
                return;
            }

            _adReward[(int)type] = ad;
        });*/
    }

    IEnumerator DelayGetReward(EADType adType, ERewardType rewardType)
    {
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        _shop.ProvideNotPurchasingReward(EPayType.AD, rewardType, GetFixedRewardValue(rewardType), "광고 보상", true, true);
        if(rewardType >= ERewardType.Money && rewardType < ERewardType.NormalBox)
        {
            LobbyUIManager.GetInstance.OpenRewardResultPopup(0.2f);
        }
        //_adReward[(int)adType] = null;
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        ADRewardSetting(adType);
    }

    IEnumerator DelayGetReward(EADType adType, Action successCallback)
    {
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        successCallback?.Invoke();
        //_adReward[(int)adType] = null;
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        ADRewardSetting(adType);
    }

    //private void RegisterEventHandlers(RewardedAd ad)
    //{
    //    // Raised when the ad is estimated to have earned money.
    //    ad.OnAdPaid += (AdValue adValue) =>
    //    {
    //        Debug.Log(String.Format("Rewarded ad paid {0} {1}.", // 보상형 광고 유료
    //            adValue.Value,
    //            adValue.CurrencyCode));
    //    };
    //    // Raised when an impression is recorded for an ad.
    //    ad.OnAdImpressionRecorded += () =>
    //    {
    //        // 보상형 광고는 노출을 기록했습니다 2
    //    };
    //    // Raised when a click is recorded for an ad.
    //    ad.OnAdClicked += () =>
    //    {
    //        Debug.Log("보상형 광고가 클릭되었습니다");
    //    };
    //    // Raised when an ad opened full screen content.
    //    ad.OnAdFullScreenContentOpened += () =>
    //    {
    //        // 보상형 광고 전체 화면 콘텐츠가 열렸습니다 1
    //    };
    //    // Raised when the ad closed full screen content.
    //    ad.OnAdFullScreenContentClosed += () =>
    //    {
    //        // 보상형 광고 전체 화면 콘텐츠가 종료되었습니다 3
    //    };
    //    // Raised when the ad failed to open full screen content.
    //    ad.OnAdFullScreenContentFailed += (AdError error) =>
    //    {
    //        Debug.LogError("Rewarded ad failed to open full screen content " + // 다음 오류로 인해 보상형 광고가 전체 화면 콘텐츠를 열지 못했습니다
    //                       "with error : " + error);
    //    };
    //}

/*    public void ShowShopAds(EADType adType, ERewardType rewardType)
    {
        if(_shop == null) 
        { 
            _shop = LobbyUIManager.GetInstance.GetShopPage; 
        }
        if (_adReward[(int)adType] != null && _adReward[(int)adType].CanShowAd())
        {
            _adReward[(int)adType].Show((Reward reward) =>
            {
                _shop.CheckWatchADQuest();
                StartCoroutine(DelayGetReward(adType, rewardType));
            });
        }
        else
        {
            MessageHandler.GetInstance.ShowMessage("광고가 준비중입니다 잠시 후 다시 시도해주세요", 1.5f);
        }
    }*/

/*    public void ShowShopAds(EADType adType, Action successCallBack, Action failCallBack = null)
    {
        if (_shop == null)
        {
            _shop = LobbyUIManager.GetInstance.GetShopPage;
        }

        if (_adReward[(int)adType] != null && _adReward[(int)adType].CanShowAd())
        {
            _adReward[(int)adType].Show((Reward reward) =>
            {
                _shop.CheckWatchADQuest();
                StartCoroutine(DelayGetReward(adType, successCallBack));
            });
        }
        else
        {
            failCallBack?.Invoke();
            MessageHandler.GetInstance.ShowMessage("광고가 준비중입니다 잠시 후 다시 시도해주세요", 1.5f);
        }
    }*/

/*    public void ShowInGameAds(EADType adType, Action successCallBack, Action failCallBack = null)
    {
        if (_adReward[(int)adType] != null && _adReward[(int)adType].CanShowAd())
        {
            _adReward[(int)adType].Show((Reward reward) =>
            {
                UserData userData = UserData.GetInstance;
                var list = userData.GetDailyQuest();
                if (list[(int)EDailyQuest.WATCHAD] <= 0)
                {
                    userData.SetDailyQuest(EDailyQuest.WATCHAD, 1);
                    userData.SaveQuestData();
                }
                StartCoroutine(DelayGetReward(adType, successCallBack));
            });
        }
        else
        {
            failCallBack?.Invoke();
            MessageHandler.GetInstance.ShowMessage("광고가 준비중입니다 잠시 후 다시 시도해주세요", 1.5f);
        }
    }*/

    /// <summary>
    /// 고정적인 광고 보상값을 반환
    /// </summary>
    /// <param name="rType"></param>
    /// <returns></returns>
    int GetFixedRewardValue(ERewardType rType)
    {
        EShopProduct productType = EShopProduct.NONE;
        switch (rType)
        {
            case ERewardType.Money:
                productType = EShopProduct.MONEY_01;
                break;
            case ERewardType.Diamond:
                productType = EShopProduct.DIAMOND_01;
                break;
            case ERewardType.ActionEnergy:
                productType = EShopProduct.ENERGY_01;
                break;
            case ERewardType.NormalBox:
                productType = EShopProduct.NORMAL_BOX;
                break;
            case ERewardType.EliteBox:
                productType = EShopProduct.ELITE_BOX;
                break;
        }
        if(productType != EShopProduct.NONE)
        {
            var data = _shop.FindShopProductData(productType);
            if(data != null)
            {
                Debug.Log("ADReward : " + data.rewardValue);
                return data.rewardValue;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }

    //public RewardedAd[] GetRewardedAds() => _adReward;
}
