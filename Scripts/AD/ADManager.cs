using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using UnityEngine;

public class ADManager : Singleton<ADManager>
{
    public IronSourceMediationSettings mediationSettings;
    // 보상형 광고에서 사용자 광고 데이터 수집을 위한 고유 ID
    // IronSource.Agent.setUserId ("UserId"); 의 형태로 사용자 ID를 설정할 수 있음
    public static string uniqueUserId = "demoUserUnity";

    public delegate void ADEventDelegate();
    public event ADEventDelegate OnRewardedEventListener;
    public event ADEventDelegate OnFailureEventListener;


    #region PlacementKey
    public static string PLACEMENTKEY_REVIVAL = "Revival";
    public static string PLACEMENTKEY_ABILITY = "BonusAbility";
    public static string PLACEMENTKEY_STAGEREWARD = "BonusReward";
    #endregion PlacementKey

    // 자사 광고
    [SerializeField] bool _playOwnAD = false;
    public VideoController videoController;
    float _playRate = 1.1f;                 // 광고 나올 확률

    // Use this for initialization
    void Start()
    {
#if UNITY_ANDROID
        string appKey = mediationSettings.AndroidAppKey;
        //string appKey = "85460dcd";
#elif UNITY_IPHONE
        string appKey = mediationSettings.IOSAppKey;
#else
        string appKey = "unexpected_platform";
#endif
        Debug.Log("unity-script: MyAppStart Start called : " + appKey);

        //Dynamic config example (클라이언트에서 콜백 날릴것인지 안할건지 선택)
        //IronSourceConfig.Instance.setClientSideCallbacks(true);

        string id = IronSource.Agent.getAdvertiserId();
        Debug.Log("unity-script: IronSource.Agent.getAdvertiserId : " + id);

        // 미디에이션 통합이 완료되었는지 확인가능 (테스트 과정에서만 사용) -> 디버그에서 확인가능
        Debug.Log("unity-script: IronSource.Agent.validateIntegration");
        IronSource.Agent.validateIntegration();             
        Debug.Log("unity-script: unity version" + IronSource.unityVersion());

        // Init AD
        InitRewardedVideo();        // 보상형 광고
        //InitBanner();               // 배너 광고
        //InitInterstitial();         // 전면 광고    

        InitRegalationSetting();    // 법률 동의
        InitWaterFallSetting();     // 폭포수 모델
        InitSegments();             // 세그먼트 설정

        //InitTesting();              // 테스트

        // SDK init
        // 광고 단위 초기화 : 리워드, 전면, 배너 (개별적으로 초기화 가능)
        IronSource.Agent.init(appKey);
        //IronSource.Agent.init (appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.OFFERWALL, IronSourceAdUnits.BANNER);
        //IronSource.Agent.initISDemandOnly (appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);

        //LoadInterstitial();
        //LoadBanner();

        videoController.Init();
    }

    /// <summary>
    /// 법률 관련 동의 -> Google Admob의 UMP 사용하여 동의 받음
    /// Google CMP를 지원 하지 않는 광고는 아래의 코드로 수동으로 보내 줘야 함
    /// </summary>
    void InitRegalationSetting()
    {
        IronSource.Agent.setConsent(true); // EU - GDPR 동의 여부
        IronSource.Agent.setMetaData("do_not_sell", "false");   // US - CPRA 동의(true : 판매 거부 / false : 판매 동의)
    }

    /// <summary>
    /// 사용자 정보 설정(현재 나이만 전송, 15세로 고정(유해광고 제외 목적))
    /// </summary>
    void InitSegments()
    {
        IronSourceSegment segment = new IronSourceSegment();
        segment.age = 15;
        IronSource.Agent.setSegment(segment);
    }

    /// <summary>
    /// 폭포수 모델 설정(보류)
    /// </summary>
    void InitWaterFallSetting()
    {
/*        // Build the WaterfallConfiguration and add data to constrain or control a waterfall
        WaterfallConfiguration config = WaterfallConfiguration.Builder()
                      .SetCeiling(120)    // 상한가
                      .SetFloor(50)      // 하한가
                    .Build();
        // set a configuration for an ad unit
        IronSource.Agent.SetWaterfallConfiguration(config, AdFormat.RewardedVideo);*/

    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    #region RewardVideo
    /// <summary>
    /// 보상형 광고 초기화 메서드
    /// </summary>
    void InitRewardedVideo()
    {
        // Add Rewarded Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
    }

    /// <summary>
    /// 보상형 광고 호출 메서드
    /// </summary>
    /// <param name="rewardCallback">보상 지급 Callback</param>
    /// <param name="isOwnAD">자사 광고 여부(추후 추가, 임시적으로 일괄 false 처리)</param>
    public void ShowRewardedVideo(System.Action rewardCallback, System.Action failCallback, string placeName = "")
    {
#if UNITY_EDITOR
        if(_playOwnAD)
        {
            PlayVideoAD(rewardCallback, failCallback);
        }
        else
        {
            CheckUserData(rewardCallback);
            rewardCallback = null;
        }
#elif UNITY_ANDROID
        float random = UnityEngine.Random.Range(0f, 1f);
        if (random <= _playRate)
        {
            // 자사 광고 실행
            PlayVideoAD(rewardCallback, failCallback);
        }
        else
        {
            OnFailureEventListener = null;
            OnFailureEventListener += () =>
            {
                failCallback?.Invoke();
            };

            // 광고 유효성 체크
            if (IronSource.Agent.isRewardedVideoAvailable())
            {            
                // 보상 이벤트 등록
                OnRewardedEventListener = null;
                OnRewardedEventListener += () => {
                    OnFailureEventListener = null;
                    CheckUserData(rewardCallback); 
                };
                Debug.Log("Play ShowRewardedVideo");
                if(placeName.Length >= 1)
                {
                    // 특정 위치의 광고 재생
                    IronSource.Agent.showRewardedVideo(placeName);
                }
                else
                {
                    // 일반적인 광고 재생
                    IronSource.Agent.showRewardedVideo();
                }
            }
        }
#endif
    }

    public void ShowRewardedVideo(ERewardType rewardType, string placeName = "")
    {
        System.Action successCallback = () =>
        {
            ShopScript shop = LobbyUIManager.GetInstance.GetShopPage;
            shop.ProvideNotPurchasingReward(EPayType.AD, rewardType, GetFixedRewardValue(rewardType), "광고 보상", true, false);
            if (rewardType > ERewardType.ActionEnergy && rewardType < ERewardType.NormalBox)
            {
                LobbyUIManager.GetInstance.OpenRewardResultPopup(0.2f);
            }
        };

        System.Action failCallback = () => 
        {
            MessageHandler.GetInstance.ShowMessage("광고가 준비중입니다 잠시 후 다시 시도해주세요", 1.5f);
        };
#if UNITY_EDITOR
        if(_playOwnAD)
        {
            PlayVideoAD(successCallback, failCallback);
        }
        else
        {
            CheckUserData(successCallback);
            successCallback = null;
        }
#elif UNITY_ANDROID
        float random = UnityEngine.Random.Range(0f, 1f);
        if (random <= _playRate)
        {
            // 자사 광고 실행
            PlayVideoAD(successCallback, failCallback);
        }
        else
        {
            OnFailureEventListener = null;
            OnFailureEventListener += () =>
            {
                failCallback?.Invoke();
            };

            // 광고 유효성 체크
            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                // 보상 이벤트 등록
                OnRewardedEventListener = null;
                OnRewardedEventListener += () => CheckUserData(()=>
                {
                    OnFailureEventListener = null;
                    successCallback?.Invoke();
                });
                Debug.Log("Play ShowRewardedVideo");
                if (placeName.Length >= 1)
                {
                    // 특정 위치의 광고 재생
                    IronSource.Agent.showRewardedVideo(placeName);
                }
                else
                {
                    // 일반적인 광고 재생
                    IronSource.Agent.showRewardedVideo();
                }
            }
        }
#endif
    }

    // RewardVideo Events
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("RewardedVideoOnAdOpenedEvent");
        OnFailureEventListener = null;
    }

    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("RewardedVideoOnAdClosedEvent");
    }

    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
        Debug.Log("RewardedVideoOnAdAvailable");
    }

    void RewardedVideoOnAdUnavailable()
    {
        Debug.LogError("RewardedVideo is not available");
        OnFailureEventListener?.Invoke();
        OnFailureEventListener = null;
    }

    void RewardedVideoOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        Debug.LogError("RewardedVideo show fail");
        OnFailureEventListener?.Invoke();
        OnFailureEventListener = null;
    }

    /// <summary>
    /// 광고 시청 완료에 따른 보상 지급 이벤트
    /// 광고 호출시 Callback 메서드를 받아 미리 입력 시켜 둠      -> UI 처리를 여기서 할지 Close 이벤트에서 할지 고민 필요
    /// </summary>
    /// <param name="ironSourcePlacement"></param>
    /// <param name="adInfo"></param>
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
    {
        Debug.Log("RewardedVideoOnAdRewardedEvent");
        OnRewardedEventListener?.Invoke();
        OnRewardedEventListener = null;
    }

    void RewardedVideoOnAdClickedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
    {
        Debug.Log("RewardedVideoOnAdClickedEvent");
    }

    void PlayVideoAD(System.Action rewardCallback, System.Action failCallback)
    {
        Debug.Log("PlayVideoAD");
        videoController.OnVideoEventListener += () =>
        {
            CheckUserData(rewardCallback);
            rewardCallback = null;
        };
        videoController.PlayVideo();
/*        if (videoController.IsPrepared)
        {
            videoController.OnVideoEventListener += () =>
            {
                CheckUserData(rewardCallback);
                rewardCallback = null;
            };
            videoController.PlayVideo();
        }
        else
        {
            Debug.Log("Fail Video");
            failCallback.Invoke();
            failCallback = null;
        }*/
    }
#endregion RewardVideo

    #region Interstitial
    /// <summary>
    /// 전면 광고 초기화
    /// </summary>
    void InitInterstitial()
    {
        // Add Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
    }

    /// <summary>
    /// 전면 광고 로드 메서드
    /// 광고 실행 하기전에 로드 필수
    /// </summary>
    void LoadInterstitial()
    {
        IronSource.Agent.loadInterstitial();
    }

    /// <summary>
    /// 전면 광고 실행 메서드
    /// </summary>
    public void ShowInterstitial(bool isOwnAD)
    {
        if (isOwnAD)
        {
            // 자사 전면 광고(별도의 클래스에서 관리)
        }
        else
        {
            if (IronSource.Agent.isInterstitialReady())
            {
                IronSource.Agent.showInterstitial();
            }
        }
    }

    // Interstitial Events

    /// <summary>
    /// 전면 광고 준비 되었을 때 호출되는 이벤트
    /// </summary>
    /// <param name="adInfo"></param>
    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdReadyEvent With AdInfo " + adInfo);
    }

    void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
    {
        Debug.Log("unity-script: I got InterstitialOnAdLoadFailed With Error " + ironSourceError);
    }

    void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdOpenedEvent With AdInfo " + adInfo);
    }

    void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClickedEvent With AdInfo " + adInfo);
    }

    void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdShowSucceededEvent With AdInfo " + adInfo);
    }

    void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdShowFailedEvent With Error " + ironSourceError + " And AdInfo " + adInfo);
    }

    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        LoadInterstitial();
    }
    #endregion Interstitial

    #region Banner
    /// <summary>
    /// 배너 이벤트 초기화 메서드
    /// </summary>
    void InitBanner()
    {
        // Add Banner Events
        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;
    }

    /// <summary>
    /// 배너 호출 메서드
    /// </summary>
    public void LoadBanner()
    {
        // 배너 크기, 배너 위치 설정
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }

    /// <summary>
    /// 배너 삭제 메서드
    /// </summary>
    public void DestroyBanner()
    {
        IronSource.Agent.destroyBanner();
    }

    /// <summary>
    /// 배너 숨기는 메서드
    /// </summary>
    public void HideBanner()
    {
        IronSource.Agent.hideBanner();
    }

    /// <summary>
    /// 배너 출력 처리(이미 로드 된 배너)
    /// </summary>
    public void DisplayBanner()
    {
        IronSource.Agent.displayBanner();
    }

    //Banner Events
    void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdLoadedEvent With AdInfo " + adInfo);
    }

    void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
    {
        Debug.Log("unity-script: I got BannerOnAdLoadFailedEvent With Error " + ironSourceError);
    }

    void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdClickedEvent With AdInfo " + adInfo);
    }

    void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdScreenPresentedEvent With AdInfo " + adInfo);
    }

    void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdScreenDismissedEvent With AdInfo " + adInfo);
    }

    void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdLeftApplicationEvent With AdInfo " + adInfo);
    }
    #endregion Banner

    #region Data
    void CheckUserData(System.Action successCallBack)
    {
        UserData userData = UserData.GetInstance;
        var list = userData.GetDailyQuest();
        if (list[(int)EDailyQuest.WATCHAD] <= 0)
        {
            userData.SetDailyQuest(EDailyQuest.WATCHAD, 1);
            userData.SaveQuestData();
        }

        if (GameManager.GetInstance._nowScene == EScene.E_LOBBY)
        {
            LobbyUIManager.GetInstance.GetPopup<QuestPopup>(PopupType.QUEST).CheckDailyQuestSet();
        }
        StartCoroutine(DelayGetReward(successCallBack));
    }

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
        if (productType != EShopProduct.NONE)
        {
            var data = LobbyUIManager.GetInstance.GetShopPage.FindShopProductData(productType);
            if (data != null)
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

    IEnumerator DelayGetReward(Action successCallback)
    {
        yield return YieldInstructionCache.RealTimeWaitForSeconds(0.5f);
        successCallback?.Invoke();
        yield return YieldInstructionCache.RealTimeWaitForSeconds(0.5f);
    }
    #endregion Data

    #region Debug

    // Test Suite

    /// <summary>
    /// 광고 테스트를 위한 초기화
    /// </summary>
    void InitTesting()
    {
        IronSource.Agent.setMetaData("is_test_suite", "enable");
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
    }

    /// <summary>
    /// 초기화 성공시 테스트 도구 활성화(실제 광고 재생시 삭제 필요)
    /// </summary>
    void SdkInitializationCompletedEvent()
    {
        //Launch test suite
        IronSource.Agent.launchTestSuite();
    }
    #endregion Debug
}
