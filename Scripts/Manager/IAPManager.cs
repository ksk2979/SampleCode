using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System.Globalization;
using MyData;
using Mono.Cecil.Cil;

public class IAPManager : Singleton<IAPManager>, IDetailedStoreListener, IStoreListener
{
    IStoreController _IStoreController;
    IExtensionProvider _IExtensionProvider;

    public const string productIDConsumable = "consumable";
    public const string productIDNonConsumable = "nonconsumable";
    public const string productIDSubscription = "subscription";

    //Android ��ǰ ID
    private const string _Android_ConsumableId = "com.studio.app.consumable.android";
    private const string _Android_NonconsumableId = "com.studio.app.nonconsumable.android";
    private const string _Android_SubscriptionId = "com.studio.app.subscription.android";

    //iOS ��ǰ ID
    private const string _IOS_ConsumableId = "com.studio.app.consumable.ios";
    private const string _IOS_NonconsumableId = "com.studio.app.nonconsumable.ios";
    private const string _IOS_SubscriptionId = "com.studio.app.subscription.ios";

    bool _init = false;

    string debugPurchaseFormat = "ProcessPurchase: PASS. Product: '{0}";
    string debugFailFormat = "ProcessPurchase: FAIL. Unrecognized product: '{0}";
    string eventProductFormat = "InApp_{0}";
    protected override void Awake()
    {
        base.Awake();
    }

    public void InitUnityIAP()
    {
        if (_init) { return; }
        _init = true;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        var dataList = DataManager.GetInstance.GetList<ShopProductData>(DataManager.KEY_SHOPPRODUCT);
        
        for(int i = 0; i <= (int)EShopProduct.DIAMOND_06; i++)
        {
            Debug.Log("dataList[i].productKey : " + dataList[i].productKey);
            builder.AddProduct(dataList[i].productKey, ProductType.Consumable);
        }
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extension)
    {
        Debug.Log("����Ƽ IAP �ʱ�ȭ ����");
        _IStoreController = controller;
        _IExtensionProvider = extension;

        //validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
        //AppleTangle.Data(), Application.identifier);
        //
        //var products = _IStoreController.products.all;
        //foreach (var product in products)
        //{
        //    if (product.hasReceipt)
        //    {
        //        var result = validator.Validate(product.receipt);
        //
        //        foreach (IPurchaseReceipt productReceipt in result)
        //        {
        //            //�۽������ ��� GooglePlayReceipt�� AppleInAppPurchaseReceipt�� �ٲٸ� �˴ϴ�.
        //            GooglePlayReceipt googlePlayReceipt = productReceipt as GooglePlayReceipt;
        //            if (null != googlePlayReceipt)
        //            {
        //                Debug.Log($"Product ID : {googlePlayReceipt.productID}");
        //                Debug.Log($"Purchase date : {googlePlayReceipt.purchaseDate.ToLocalTime()}");
        //                Debug.Log($"Transaction ID : {googlePlayReceipt.transactionID}");
        //                Debug.Log($"Purchase token : {googlePlayReceipt.purchaseToken}");
        //            }
        //        }
        //    }
        //}
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"����Ƽ IAP �ʱ�ȭ ���� {error}");
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogWarning($"���� ���� - {product.definition.id}, {reason}");
        LobbyUIManager.GetInstance.GetShopPage.NotInAppMessage();
    }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log($"���� ���� - ID : {args.purchasedProduct.definition.id}");

        bool successPurchasing = false;
        var productList = DataManager.GetInstance.GetList<ShopProductData>(DataManager.KEY_SHOPPRODUCT);
        for(int i = 0; i < productList.Count; i++)
        {
            if(string.Equals(args.purchasedProduct.definition.id, productList[i].productKey, StringComparison.Ordinal))
            {
                successPurchasing = true;
                LobbyUIManager.GetInstance.GetShopPage.BuyShopItem(productList[i].nId - 1);
                break;
            }
        }
        if(successPurchasing)
        {
            Debug.Log(string.Format(debugPurchaseFormat, args.purchasedProduct.definition.id));
        }
        else
        {
            Debug.Log(string.Format(debugFailFormat, args.purchasedProduct.definition.id));
        }

        //else if (args.purchasedProduct.definition.id == ProductCharacterSkin)
        //{
        //    Debug.Log("�� �Ҹ�ǰ ���� �Ϸ�");
        //}
        //else if (args.purchasedProduct.definition.id == ProductSubscription)
        //{
        //    Debug.Log("���� ���� ���� �Ϸ�");
        //}

        return PurchaseProcessingResult.Complete;
    }

    // �����ϱ� ��ư �������� ���� ��
    public void CheckISInitialized()
    {
        InitUnityIAP();
    }
    // �����ϱ� ��ư �������� ���� ��
    public void Purchase(string productId)
    {
        if (!IsInitialized())
        {
            return;
        }

        Debug.Log("productID : " + productId);
        var product = _IStoreController.products.WithID(productId);
        if (product != null && product.availableToPurchase)
        {
            Debug.Log($"���� �õ� - {product.definition.id}");
            _IStoreController.InitiatePurchase(product);
        }
        else
        {
            Debug.Log($"���� �õ� �Ұ� - {productId}");
        }
    }
    bool IsInitialized()
    {
        return _IStoreController != null && _IExtensionProvider != null;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        throw new NotImplementedException();
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError(error);
        Debug.Log(message);
    }
}


//protected override void Awake()
//{
//    base.Awake();
//}
//private void Start()
//{
//    InitializePurchasing();
//}
//
//bool IsInitialized()
//{
//    return _IStoreController != null && _IExtensionProvider != null;
//}
//public void InitializePurchasing()
//{
//    var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
//
//    builder.AddProduct("diamond_80", ProductType.Consumable);
//    builder.AddProduct("diamond_500", ProductType.Consumable);
//    builder.AddProduct("diamond_1200", ProductType.Consumable);
//    builder.AddProduct("diamond_2500", ProductType.Consumable);
//    builder.AddProduct("diamond_6500", ProductType.Consumable);
//    builder.AddProduct("diamond_14000", ProductType.Consumable);
//
//    UnityPurchasing.Initialize(this, builder);
//}
//
//public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//{
//    this._IStoreController = controller;
//    this._IExtensionProvider = extensions;
//}
//
//public void OnInitializeFailed(InitializationFailureReason error)
//{
//    Debug.Log("Not Init Failed");
//}
//
//public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
//{
//    if (string.Equals(e.purchasedProduct.definition.id, "diamond_80", StringComparison.Ordinal))
//    {
//        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}", e.purchasedProduct.definition.id));
//    }
//    else if (string.Equals(e.purchasedProduct.definition.id, "diamond_500", StringComparison.Ordinal))
//    {
//        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}", e.purchasedProduct.definition.id));
//    }
//    else if (string.Equals(e.purchasedProduct.definition.id, "diamond_1200", StringComparison.Ordinal))
//    {
//        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}", e.purchasedProduct.definition.id));
//    }
//    else if (string.Equals(e.purchasedProduct.definition.id, "diamond_2500", StringComparison.Ordinal))
//    {
//        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}", e.purchasedProduct.definition.id));
//    }
//    else if (string.Equals(e.purchasedProduct.definition.id, "diamond_6500", StringComparison.Ordinal))
//    {
//        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}", e.purchasedProduct.definition.id));
//    }
//    else if (string.Equals(e.purchasedProduct.definition.id, "diamond_14000", StringComparison.Ordinal))
//    {
//        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}", e.purchasedProduct.definition.id));
//    }
//    else
//    {
//        Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}", e.purchasedProduct.definition.id));
//    }
//    return PurchaseProcessingResult.Complete;
//}
//
//// ���� ���� ����Ǵ°�
//public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
//{
//    Debug.Log($"{i.definition.id} Now Buy {p} Error");
//}
//
//public void OnPurchaseClicked(string productID)
//{
//    if (!IsInitialized())
//    {
//        Debug.Log("IAP�� �ʱ�ȭ ���� �ʾ���");
//        return;
//    }
//
//    _IStoreController.InitiatePurchase(productID);
//}
//
//public void RestorePurchases()
//{
//    if (!IsInitialized()) { return; }
//
//    // �۽��� ���� �۵�
//    if (Application.platform == RuntimePlatform.IPhonePlayer ||
//        Application.platform == RuntimePlatform.OSXPlayer)
//    {
//        Debug.Log("restorePurchases started ...");
//
//        // ����� ���� �ý��ۿ� ����
//        var apple = _IExtensionProvider.GetExtension<IAppleExtensions>();
//
//        //�񵿱��۾����� ����
//        // result�� ���ؼ� �񵿱� �۾��� �Ϸ�Ǹ� ����� �ݹ� �Լ� (����޼���� �����ص� ������) - > ���⼭ ������ �׸� �ǵ����µ� ����
//        apple.RestoreTransactions((result) =>
//        {
//            Debug.Log("RestorePurchases continuing: " + result + ". if no further messages, no purchases available to restore.");
//        });
//    }
//    // �ٸ� �÷������� �����Ѵٸ� Restore�� ���� ���� ����
//    else
//    {
//        Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
//    }
//}