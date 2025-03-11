using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using UnityEngine;

public class ADManager : Singleton<ADManager>
{
    public IronSourceMediationSettings mediationSettings;
    // ������ ������ ����� ���� ������ ������ ���� ���� ID
    // IronSource.Agent.setUserId ("UserId"); �� ���·� ����� ID�� ������ �� ����
    public static string uniqueUserId = "demoUserUnity";

    public delegate void ADEventDelegate();
    public event ADEventDelegate OnRewardedEventListener;
    public event ADEventDelegate OnFailureEventListener;


    #region PlacementKey
    public static string PLACEMENTKEY_REVIVAL = "Revival";
    public static string PLACEMENTKEY_ABILITY = "BonusAbility";
    public static string PLACEMENTKEY_STAGEREWARD = "BonusReward";
    #endregion PlacementKey

    // �ڻ� ����
    [SerializeField] bool _playOwnAD = false;
    public VideoController videoController;
    float _playRate = 1.1f;                 // ���� ���� Ȯ��

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

        //Dynamic config example (Ŭ���̾�Ʈ���� �ݹ� ���������� ���Ұ��� ����)
        //IronSourceConfig.Instance.setClientSideCallbacks(true);

        string id = IronSource.Agent.getAdvertiserId();
        Debug.Log("unity-script: IronSource.Agent.getAdvertiserId : " + id);

        // �̵��̼� ������ �Ϸ�Ǿ����� Ȯ�ΰ��� (�׽�Ʈ ���������� ���) -> ����׿��� Ȯ�ΰ���
        Debug.Log("unity-script: IronSource.Agent.validateIntegration");
        IronSource.Agent.validateIntegration();             
        Debug.Log("unity-script: unity version" + IronSource.unityVersion());

        // Init AD
        InitRewardedVideo();        // ������ ����
        //InitBanner();               // ��� ����
        //InitInterstitial();         // ���� ����    

        InitRegalationSetting();    // ���� ����
        InitWaterFallSetting();     // ������ ��
        InitSegments();             // ���׸�Ʈ ����

        //InitTesting();              // �׽�Ʈ

        // SDK init
        // ���� ���� �ʱ�ȭ : ������, ����, ��� (���������� �ʱ�ȭ ����)
        IronSource.Agent.init(appKey);
        //IronSource.Agent.init (appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.OFFERWALL, IronSourceAdUnits.BANNER);
        //IronSource.Agent.initISDemandOnly (appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);

        //LoadInterstitial();
        //LoadBanner();

        videoController.Init();
    }

    /// <summary>
    /// ���� ���� ���� -> Google Admob�� UMP ����Ͽ� ���� ����
    /// Google CMP�� ���� ���� �ʴ� ����� �Ʒ��� �ڵ�� �������� ���� ��� ��
    /// </summary>
    void InitRegalationSetting()
    {
        IronSource.Agent.setConsent(true); // EU - GDPR ���� ����
        IronSource.Agent.setMetaData("do_not_sell", "false");   // US - CPRA ����(true : �Ǹ� �ź� / false : �Ǹ� ����)
    }

    /// <summary>
    /// ����� ���� ����(���� ���̸� ����, 15���� ����(���ر��� ���� ����))
    /// </summary>
    void InitSegments()
    {
        IronSourceSegment segment = new IronSourceSegment();
        segment.age = 15;
        IronSource.Agent.setSegment(segment);
    }

    /// <summary>
    /// ������ �� ����(����)
    /// </summary>
    void InitWaterFallSetting()
    {
/*        // Build the WaterfallConfiguration and add data to constrain or control a waterfall
        WaterfallConfiguration config = WaterfallConfiguration.Builder()
                      .SetCeiling(120)    // ���Ѱ�
                      .SetFloor(50)      // ���Ѱ�
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
    /// ������ ���� �ʱ�ȭ �޼���
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
    /// ������ ���� ȣ�� �޼���
    /// </summary>
    /// <param name="rewardCallback">���� ���� Callback</param>
    /// <param name="isOwnAD">�ڻ� ���� ����(���� �߰�, �ӽ������� �ϰ� false ó��)</param>
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
            // �ڻ� ���� ����
            PlayVideoAD(rewardCallback, failCallback);
        }
        else
        {
            OnFailureEventListener = null;
            OnFailureEventListener += () =>
            {
                failCallback?.Invoke();
            };

            // ���� ��ȿ�� üũ
            if (IronSource.Agent.isRewardedVideoAvailable())
            {            
                // ���� �̺�Ʈ ���
                OnRewardedEventListener = null;
                OnRewardedEventListener += () => {
                    OnFailureEventListener = null;
                    CheckUserData(rewardCallback); 
                };
                Debug.Log("Play ShowRewardedVideo");
                if(placeName.Length >= 1)
                {
                    // Ư�� ��ġ�� ���� ���
                    IronSource.Agent.showRewardedVideo(placeName);
                }
                else
                {
                    // �Ϲ����� ���� ���
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
            shop.ProvideNotPurchasingReward(EPayType.AD, rewardType, GetFixedRewardValue(rewardType), "���� ����", true, false);
            if (rewardType > ERewardType.ActionEnergy && rewardType < ERewardType.NormalBox)
            {
                LobbyUIManager.GetInstance.OpenRewardResultPopup(0.2f);
            }
        };

        System.Action failCallback = () => 
        {
            MessageHandler.GetInstance.ShowMessage("���� �غ����Դϴ� ��� �� �ٽ� �õ����ּ���", 1.5f);
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
            // �ڻ� ���� ����
            PlayVideoAD(successCallback, failCallback);
        }
        else
        {
            OnFailureEventListener = null;
            OnFailureEventListener += () =>
            {
                failCallback?.Invoke();
            };

            // ���� ��ȿ�� üũ
            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                // ���� �̺�Ʈ ���
                OnRewardedEventListener = null;
                OnRewardedEventListener += () => CheckUserData(()=>
                {
                    OnFailureEventListener = null;
                    successCallback?.Invoke();
                });
                Debug.Log("Play ShowRewardedVideo");
                if (placeName.Length >= 1)
                {
                    // Ư�� ��ġ�� ���� ���
                    IronSource.Agent.showRewardedVideo(placeName);
                }
                else
                {
                    // �Ϲ����� ���� ���
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
    /// ���� ��û �Ϸῡ ���� ���� ���� �̺�Ʈ
    /// ���� ȣ��� Callback �޼��带 �޾� �̸� �Է� ���� ��      -> UI ó���� ���⼭ ���� Close �̺�Ʈ���� ���� ��� �ʿ�
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
    /// ���� ���� �ʱ�ȭ
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
    /// ���� ���� �ε� �޼���
    /// ���� ���� �ϱ����� �ε� �ʼ�
    /// </summary>
    void LoadInterstitial()
    {
        IronSource.Agent.loadInterstitial();
    }

    /// <summary>
    /// ���� ���� ���� �޼���
    /// </summary>
    public void ShowInterstitial(bool isOwnAD)
    {
        if (isOwnAD)
        {
            // �ڻ� ���� ����(������ Ŭ�������� ����)
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
    /// ���� ���� �غ� �Ǿ��� �� ȣ��Ǵ� �̺�Ʈ
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
    /// ��� �̺�Ʈ �ʱ�ȭ �޼���
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
    /// ��� ȣ�� �޼���
    /// </summary>
    public void LoadBanner()
    {
        // ��� ũ��, ��� ��ġ ����
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }

    /// <summary>
    /// ��� ���� �޼���
    /// </summary>
    public void DestroyBanner()
    {
        IronSource.Agent.destroyBanner();
    }

    /// <summary>
    /// ��� ����� �޼���
    /// </summary>
    public void HideBanner()
    {
        IronSource.Agent.hideBanner();
    }

    /// <summary>
    /// ��� ��� ó��(�̹� �ε� �� ���)
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
    /// �������� ���� ������ ��ȯ
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
    /// ���� �׽�Ʈ�� ���� �ʱ�ȭ
    /// </summary>
    void InitTesting()
    {
        IronSource.Agent.setMetaData("is_test_suite", "enable");
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
    }

    /// <summary>
    /// �ʱ�ȭ ������ �׽�Ʈ ���� Ȱ��ȭ(���� ���� ����� ���� �ʿ�)
    /// </summary>
    void SdkInitializationCompletedEvent()
    {
        //Launch test suite
        IronSource.Agent.launchTestSuite();
    }
    #endregion Debug
}
