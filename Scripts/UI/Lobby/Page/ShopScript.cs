using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyData;
using System;
using System.Collections;

public enum EShopProduct
{
    DIAMOND_01 = 0,
    DIAMOND_02,
    DIAMOND_03,
    DIAMOND_04,
    DIAMOND_05,
    DIAMOND_06,
    PACKAGE_01,
    PACKAGE_02,
    PACKAGE_03,
    MONEY_01,
    MONEY_02,
    MONEY_03,
    NORMAL_BOX,
    ELITE_BOX,
    ELITE_TEN_BOX,
    BOAT_BOX,
    WEAPON_BOX,
    DEFENSE_BOX,
    CAPTAIN_BOX,
    SAILOR_BOX,
    ENGINE_BOX,
    ENERGY_01,
    ENERGY_02,
    SEASONPASS,
    NONE,
}

public enum EPayType
{
    NONE = 0,
    MONEY,
    DIA,
    AD,
}

public class ShopScript : PageBase
{

    InvenScript _invenScript;
    DataManager _dataManager;
    [SerializeField] RectTransform _contentTrans;

    [Header("Random Box")]
    [SerializeField] Sprite[] _boxSripte; // closeBox 0 노멀, 1 엘리트 / OpenBox 2 노멀 3 엘리트
    Dictionary<ERewardType, int> _waitBoxDictionary = new Dictionary<ERewardType, int>();

    [Header("AD")]
    [SerializeField] ShopProduct[] _adTargetProductArr;

    [Header("Package")]
    //[SerializeField] PackageBoxPopup _packageBoxPopup;
    [SerializeField] GameObject _packageListObj; // 패키지가 없으면 꺼주기
    [SerializeField] GameObject[] _packageArrObj;

    [Header("Daily Item")]
    [SerializeField] OneDayShopItem[] _oneDayItem;
    [SerializeField] GameObject _freeItemAlram;

    [Header("AD Reset Time")]
    [SerializeField] TextMeshProUGUI _chestNormalTimeText;
    [SerializeField] TextMeshProUGUI _chestEliteTimeText;

    [Header("Popup")]
    ShopBoxPopup _shopBoxPopup;
    ChoiceAskPopup _buyQuestionPopup;

    [Header("Products")]
    [SerializeField] ShopProduct[] _shopProductArr;

    const float messageDelayTime = 1.5f;
    const string message_notEnoughDia = "다이아가 부족합니다";
    const string message_successPurchasing = "구매가 성공적으로 완료되었습니다";
    const string eventPurchaseFormat01 = "Purchase_{0}_{1}";
    const string eventPurchaseFormat02 = "Purchase_{0}";

    #region Init
    public override void Init(LobbyUIManager uiM)
    {
        base.Init(uiM);
        _invenScript = uiManager.GetInvenPage;
        userData = UserData.GetInstance;
        _dataManager = DataManager.GetInstance;
        _shopBoxPopup = uiManager.GetPopup<ShopBoxPopup>(PopupType.BOXOPEN);
        _buyQuestionPopup = uiManager.GetPopup<ChoiceAskPopup>(PopupType.ASK);
        IAPManager.GetInstance.InitUnityIAP();

        foreach (var product in _shopProductArr)
        {
            product.InitProduct(this);
        }
    }

    /// <summary>
    /// 일일 상품 갱신
    /// </summary>
    /// <param name="oneDay"></param>
    public void RewardInit(bool oneDay)
    {
        if (oneDay)
        {
            // 일일 상품 초기화
            userData.ResetShopDailyReceiveState();
            for (int i = 0; i < _oneDayItem.Length; ++i)
            {
                _oneDayItem[i].ResetItem();
                _oneDayItem[i].Init(i);
            }

            for (int i = 0; i < _oneDayItem.Length; ++i)
            {
                if (i == 0)
                {
                    userData.SetShopDailyOneKey(0, _oneDayItem[0].SelectRandomProduct(true));
                    continue;
                }
                userData.SetShopDailyTwoKey(i, _oneDayItem[i].MaterialNumberCount());
                userData.SetShopDailyOneKey(i, _oneDayItem[i].SelectRandomProduct(false));
            }
        }
        else
        {
            // 이미 초기화 된 환경
            // 일일 상품
            for (int i = 0; i < _oneDayItem.Length; i++)
            {
                _oneDayItem[i].Init(i);
                int oneKey = userData.GetShopDailyOneKey(i);
                int twoKey = 0;
                if (i > 0)
                {
                    twoKey = userData.GetShopDailyTwoKey(i);
                }
                _oneDayItem[i].SetProduct((ERewardType)oneKey, twoKey);
                // 한번 받은 상태면 블러처리
                if (userData.GetShopDailyReceivedState(i) == 1)
                {
                    _oneDayItem[i].SetBlockedButton();
                }
            }
        }

        if (userData.GetShopDailyReceivedState(0) == 0)
        {
            SetBadge(true);
        }
        else
        {
            SetBadge(false);
        }
    }
    #endregion Init

    #region Add
    /// <summary>
    /// 지정된 아이템(장비)를 등록하는 메서드
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="id"></param>
    public void AddItem(EItemList itemType, int id)
    {
        CreateItem(itemType, id);
        userData.UnitTypeSave(itemType);
    }

    /// <summary>
    /// 재화 구매(다이아, 돈, 행동력)
    /// </summary>
    /// <param name="rType"></param>
    /// <param name="rValue"></param>
    public void AddCurrency(EPropertyType rType, int rValue, bool needPopup = true)
    {
        userData.AddCurrency(rType, rValue);
        uiManager.UpdateUserInfo();
        userData.SaveUserData();
        FirebaseManager.GetInstance.LogEvent(string.Format(eventPurchaseFormat01, rType.ToString(), rValue));
        //MessageHandler.Getinstance.ShowMessage(message_successPurchasing, messageDelayTime);

        if (needPopup)
        {
            var popup = uiManager.GetPopup<RewardResultPopup>(PopupType.REWARDRESULT);
            popup.ResetPopup();
            popup.SetPopup("구매 완료", (ERewardType)rType, rValue);
            popup.AddParticle((RParticleType)rType);
            uiManager.OpenRewardResultPopup(0f);
        }
        else
        {
            uiManager.ShowRewardEffect((RParticleType)rType, 1.0f);
        }
    }
    #endregion Add

    #region Product
    /// <summary>
    /// 상품 구매
    /// </summary>
    /// <param name="id">상품 id</param>
    public void BuyShopItem(EShopProduct productType)
    {
        //Debug.Log("BuyShopItem");
        if (productType == EShopProduct.NONE) return;

        var productData = FindShopProductData(productType);
        if (productData == null)
        {
            Debug.LogError("상품 데이터가 없습니다.");
            return;
        }
        var data = FindShopProductData(productType);
        if (data == null) return;

        // 다이아는 현금결제로 구매
        if (productType <= EShopProduct.DIAMOND_06)
        {
            AddCurrency(EPropertyType.DIAMOND, productData.rewardValue);
        }
        else
        {
            // 다이아로 구매하는 경우
            // 1. 다이아 체크 후 소모
            // 2. 상품 지급
            // 3. 메세지 출력
            if (userData.GetCurrency(EPropertyType.DIAMOND) < data.price)
            {
                // 다이아가 부족한 경우
                MessageHandler.GetInstance.ShowMessage(message_notEnoughDia, messageDelayTime);
                return;
            }
            else
            {
                userData.SubCurrency(EPropertyType.DIAMOND, data.price);
                userData.SaveUserData();
                uiManager.UpdateUserInfo();
            }

            string fbFormat1 = "";
            string rValueString = "";
            if (productType <= EShopProduct.PACKAGE_03)
            {
                // 패키지 구매
                switch (productType)
                {
                    case EShopProduct.PACKAGE_01:
                        PackageOne();
                        break;
                    case EShopProduct.PACKAGE_02:
                        PackageTwo();
                        break;
                    case EShopProduct.PACKAGE_03:
                        PackageThree();
                        break;
                }
                fbFormat1 = eventPurchaseFormat02;
                rValueString = productType.ToString();
            }
            else if (productType <= EShopProduct.MONEY_03)
            {
                // 골드(인게임 재화)구매 : 다이아 소모
                AddCurrency(EPropertyType.MONEY, productData.rewardValue);
            }
            else if (productType <= EShopProduct.NORMAL_BOX)
            {
                // 노멀 상자 구매
                CreateNormalBox();
                fbFormat1 = eventPurchaseFormat02;
                rValueString = productType.ToString();
            }
            else if (productType <= EShopProduct.ELITE_BOX)
            {
                // 엘리트 상자 구매
                CreateEliteBox();
                fbFormat1 = eventPurchaseFormat02;
                rValueString = productType.ToString();
            }
            else if (productType <= EShopProduct.ELITE_TEN_BOX)
            {
                // 엘리트 10개 상자 구매
                CreateEliteBox(data.rewardValue);
                fbFormat1 = eventPurchaseFormat02;
                rValueString = productType.ToString();
            }
            else if (productType <= EShopProduct.ENERGY_02)
            {
                // 행동력 다이아로 구매
                AddCurrency(EPropertyType.ACTIONENERGY, data.rewardValue);
                uiManager.GetPopup<EnergyPopup>(PopupType.ENERGY).ClosePopup();
            }
            else if (productType <= EShopProduct.SEASONPASS)
            {
                // 시즌 패스 구매
                if (userData.IsVipActivated) return;        // 이미 구매함

                // VIP 구매 처리
                userData.SetVIPState(true);
                uiManager.GetSeasonPassPanel.SetPurchaseState(true);

                userData.SaveSeasonPassData();
                uiManager.GetSeasonPassPanel.CheckVipUIs();
            }

            // Firebase 로그
            if (fbFormat1.Length > 0)
            {
                FirebaseManager.GetInstance.LogEvent(string.Format(fbFormat1, rValueString));
            }
        }
    }

    /// <summary>
    /// 박스 구매 전용
    /// </summary>
    /// <param name="productType"></param>
    /// <param name="itemType"></param>
    public void BuyShopItem(EShopProduct productType, int price, bool isElite = false)
    {
        // 보석 소모
        if (userData.GetCurrency(EPropertyType.DIAMOND) < price)
        {
            MessageHandler.GetInstance.ShowMessage(message_notEnoughDia, messageDelayTime);
            return;
        }
        else
        {
            userData.SubCurrency(EPropertyType.DIAMOND, price);
            userData.SaveUserData();
            uiManager.UpdateUserInfo();
        }

        string fbFormat1 = "";
        string rValueString = "";
        fbFormat1 = eventPurchaseFormat02;

        if (isElite)
        {
            rValueString = productType.ToString() + "_Elite";
            CreateEliteBox(1, GetSpecificItemType(productType));
        }
        else
        {
            rValueString = productType.ToString() + "_Normal";
            CreateNormalBox(GetSpecificItemType(productType));
        }

        // Firebase 로그
        if (fbFormat1.Length > 0)
        {
            FirebaseManager.GetInstance.LogEvent(string.Format(fbFormat1, rValueString));
        }
    }

    EItemList GetSpecificItemType(EShopProduct product)
    {
        return (EItemList)((int)product - (int)EShopProduct.BOAT_BOX);
    }

    /// <summary>
    /// 상품 구매(id로 찾기)
    /// </summary>
    /// <param name="id"></param>
    public void BuyShopItem(int id) => BuyShopItem((EShopProduct)id);


    /// <summary>
    /// 재화 지불 없는 아이템 지급(복수 보상)
    /// </summary>
    public void ProvideNotPurchasingAllReward(EPayType payType, ERewardType[] rTypeArr, int[] rValueArr, string rewardText, bool timeCheck)
    {
        for (int i = 0; i < rTypeArr.Length; i++)
        {
            ProvideNotPurchasingReward(payType, rTypeArr[i], rValueArr[i], rewardText, timeCheck, true);
        }
        uiManager.OpenRewardResultPopup(0.2f);
    }

    /// <summary>
    /// 재화 지불 없는 아이템 지급
    /// </summary>
    public bool ProvideNotPurchasingReward(EPayType payType, ERewardType rType,
        int rValue, string resultText, bool timeCheck, bool addPopupContent = false)
    {
        string eventLog = "";
        bool isProvided = false;
        bool isConsumable = true;

        if (rType >= ERewardType.Money && rType <= ERewardType.ActionEnergy)
        {
            isProvided = true;
        }
        else if (rType >= ERewardType.Copper && rType <= ERewardType.Oil)
        {
            if (payType == EPayType.AD)
            {
                eventLog = string.Format("AD_{0}_{1}", rType.ToString(), rValue);
            }
            isProvided = true;
        }
        else if (rType >= ERewardType.NormalBox && rType <= ERewardType.EliteBox)
        {
            isConsumable = false;
            isProvided = true;
            switch (rType)
            {
                case ERewardType.NormalBox:
                    {
                        CreateNormalBox();
                    }
                    break;
                case ERewardType.EliteBox:
                    {
                        CreateEliteBox();
                    }
                    break;
            }
        }
        // 소모성 재화 지급
        if (isConsumable)
        {
            userData.AddCurrency(rType, rValue);
        }
        if (payType == EPayType.AD)
        {
            // 광고 시청후 지급 보상
            eventLog = string.Format("AD_{0}", rType.ToString());
            switch (rType)
            {
                case ERewardType.Money:
                    {
                        if (timeCheck)
                        {
                            uiManager.ResetCheckTime(ETimeCheckType.AD_MONEY);
                            SetAdRewardSet(3, false);
                        }
                        isProvided = true;
                    }
                    break;
                case ERewardType.Diamond:
                    {
                        if (timeCheck)
                        {
                            uiManager.ResetCheckTime(ETimeCheckType.AD_DIA);
                            SetAdRewardSet(2, false);
                        }
                        isProvided = true;
                    }
                    break;
                case ERewardType.ActionEnergy:
                    {
                        if (timeCheck)
                        {
                            uiManager.ResetCheckTime(ETimeCheckType.AD_ACTIONENERGY);
                        }
                        isProvided = true;
                    }
                    break;
                case ERewardType.NormalBox:
                    {
                        if (timeCheck)
                        {
                            uiManager.ResetCheckTime(ETimeCheckType.AD_NORMALBOX);        // 1일
                            _adTargetProductArr[0].SetPriceState(false);
                        }
                        isProvided = true;
                    }
                    break;
                case ERewardType.EliteBox:
                    {
                        if (timeCheck)
                        {
                            uiManager.ResetCheckTime(ETimeCheckType.AD_ELITEBOX);         // 7일
                            _adTargetProductArr[1].SetPriceState(false);
                        }
                        isProvided = true;
                    }
                    break;
            }
        }

        uiManager.ActionEnergyAddUpdate();
        uiManager.UpdateUserInfo();
        userData.SaveUserData();

        // 팝업 여부 
        // 룰렛 / 일괄 보상(퀘스트, 시즌패스)
        if (addPopupContent)
        {
            // Result Popup
            if (rType >= ERewardType.Money && rType < ERewardType.NormalBox)
            {
                var popup = uiManager.GetPopup<RewardResultPopup>(PopupType.REWARDRESULT);
                popup.SetPopup(resultText, rType, rValue);
                if (rType >= ERewardType.Money && rType <= ERewardType.ActionEnergy)
                {
                    popup.AddParticle((RParticleType)rType);
                }
            }
        }
        else
        {
            if (rType >= ERewardType.Money && rType <= ERewardType.ActionEnergy)
            {
                // no Popup
                uiManager.ShowRewardEffect((RParticleType)rType, 0);
            }
        }

        // Firebase LogEvent
        if (eventLog.Length > 0)
        {
            FirebaseManager.GetInstance.LogEvent(eventLog);
        }
        return isProvided;
    }

    /// <summary>
    /// 일일상품 구매
    /// </summary>
    /// <param name="num"></param>
    public void BuyShopDailyProduct(int num, bool showPopup = false)
    {
        var product = _oneDayItem[num];
        bool check = ProvideNotPurchasingReward(product.GetPayType, product.GetProductType, product.GetProcutCount, "일일 상품", false, showPopup);
        if (check)
        {
            _oneDayItem[num].SetBlockedButton();
            Debug.Log("OneDay Count : " + (userData.GetShopDailyReceivedState(num) + 1));
            userData.SetShopOneDayReceivedState(num, userData.GetShopDailyReceivedState(num) + 1);
        }
    }
    #endregion Product

    #region Create
    /// <summary>
    /// 아이템 생성
    /// </summary>
    /// <param name="itemList">타입</param>
    /// <param name="id"></param>
    /// <returns></returns>
    public UnitIcon CreateItem(EItemList itemList, int id)
    {
        UnitIcon unit = null;
        if (itemList == EItemList.BOAT)
        {
            var data = DataManager.GetInstance.GetData(MyData.DataManager.KEY_BOAT, id, 1) as BoatData;

            int ingerenceID = userData.CreateUnitIngerenceID(id, data.grade);
            // 오브젝트 생성 - 생성이 되면서 _all 변수에도 추가됨
            userData.BoatCoalesceneceAdd(ingerenceID, id);
            unit = _invenScript.CreateItemIcon(id, 1, itemList, _invenScript.GetInven(itemList).Count, ingerenceID);
            // 출전 아이콘 생성
            uiManager.GetReadyPage.CreateBoatIcon();
        }
        else
        {
            unit = _invenScript.CreateItemIcon(id, 1, itemList, _invenScript.GetInven(itemList).Count);
        }
        userData.UnitTypeAdd(itemList, id);
        userData.UnitPotentialAdd(itemList, unit.GetPotentialString);
        uiManager.UpdateUserInfo();
        return unit;
    }

    /// <summary>
    /// 일반 등급 상자 생성
    /// </summary>
    void CreateNormalBox(EItemList itemType = EItemList.NONE)
    {
        UnitIcon unit = CreateBoxItem(ERewardType.NormalBox, itemType);
        if (unit == null)
        {
            Debug.Log("unit is NULL");
        }
        userData.UnitTypeSave(unit.GetItemType);
        _shopBoxPopup.SetBoxImages(_boxSripte[0], _boxSripte[3]);
        _shopBoxPopup.OpenOneBox(unit);
    }

    /// <summary>
    /// 엘리트 등급 상자 생성
    /// </summary>
    /// <param name="count">수량</param>
    void CreateEliteBox(int count = 1, EItemList itemType = EItemList.NONE)
    {
        if (count <= 1)
        {
            // 박스 1
            UnitIcon unit = CreateBoxItem(ERewardType.EliteBox, itemType);
            userData.UnitTypeSave(unit.GetItemType);
            _shopBoxPopup.SetBoxImages(_boxSripte[1], _boxSripte[4]);
            _shopBoxPopup.OpenOneBox(unit);
        }
        else
        {
            // 박스 10
            UnitIcon[] unitIcon = new UnitIcon[count];
            for (int i = 0; i < count; ++i)
            {
                unitIcon[i] = CreateBoxItem(ERewardType.EliteBox);
            }
            List<EItemList> list = new List<EItemList>();
            bool iContinue = false;
            for (int i = 0; i < unitIcon.Length; ++i)
            {
                for (int j = 0; j < list.Count; ++j)
                {
                    if (list[j] == unitIcon[i].GetItemType)
                    {
                        iContinue = true;
                        break;
                    }
                }
                if (iContinue) { iContinue = false; continue; }

                list.Add(unitIcon[i].GetItemType);
            }
            userData.UnitAllTypeSave(GetItemTypeList(unitIcon));
            _shopBoxPopup.SetBoxImages(_boxSripte[2], _boxSripte[5]);
            _shopBoxPopup.OpenTenBoxes(unitIcon);
        }
    }

    /// <summary>
    /// 아이템 정보 생성
    /// </summary>
    /// <param name="rType">랜덤 박스 타입</param>
    /// <returns></returns>
    UnitIcon CreateBoxItem(ERewardType rType, EItemList itemType = EItemList.NONE)
    {
        EItemList type;
        if (itemType == EItemList.NONE)
        {
            type = (EItemList)UnityEngine.Random.Range(0, (int)EItemList.MATERIAL);
        }
        else
        {
            type = itemType;
        }
        int percent = UnityEngine.Random.Range(0, 10000);
        int[] unit;

        if (rType == ERewardType.NormalBox)
        {
            unit = new int[6];
            if (percent < 9344) { unit[0] = 1; unit[1] = 12; unit[2] = 23; unit[3] = 34; unit[4] = 45; unit[5] = 56; } // E
            else if (percent < 9844) { unit[0] = 2; unit[1] = 13; unit[2] = 24; unit[3] = 35; unit[4] = 46; unit[5] = 57; } // D
            else if (percent < 9944) { unit[0] = 3; unit[1] = 14; unit[2] = 25; unit[3] = 36; unit[4] = 47; unit[5] = 58; } // C
            else if (percent < 9994) { unit[0] = 4; unit[1] = 15; unit[2] = 26; unit[3] = 37; unit[4] = 48; unit[5] = 59; } // B
            else if (percent < 9997) { unit[0] = 5; unit[1] = 16; unit[2] = 27; unit[3] = 38; unit[4] = 49; unit[5] = 60; } // A
            else if (percent < 9999) { unit[0] = 6; unit[1] = 17; unit[2] = 28; unit[3] = 39; unit[4] = 50; unit[5] = 61; } // A+
            else { unit[0] = 7; unit[1] = 18; unit[2] = 29; unit[3] = 40; unit[4] = 51; unit[5] = 62; } // S
        }
        else
        {
            unit = new int[6];

            if (percent < 8100) { unit[0] = 3; unit[1] = 14; unit[2] = 25; unit[3] = 36; unit[4] = 47; unit[5] = 58; } // C
            else if (percent < 9100) { unit[0] = 4; unit[1] = 15; unit[2] = 26; unit[3] = 37; unit[4] = 48; unit[5] = 59; } // B
            else if (percent < 9600) { unit[0] = 5; unit[1] = 16; unit[2] = 27; unit[3] = 38; unit[4] = 49; unit[5] = 60; } // A
            else if (percent < 9900) { unit[0] = 6; unit[1] = 17; unit[2] = 28; unit[3] = 39; unit[4] = 50; unit[5] = 61; } // A+
            else { unit[0] = 7; unit[1] = 18; unit[2] = 29; unit[3] = 40; unit[4] = 51; unit[5] = 62; } // S
        }
        return CreateItem(type, unit[UnityEngine.Random.Range(0, unit.Length)]);
    }
    #endregion Create

    #region Package
    // 무기 10개 / 무기 10개 나머지 배분 4 / 무기 50개 나머지 10개
    public void PackageOne()
    {
        int[] matarial = userData.GetAllMaterials();
        matarial[(int)(EInvenType.Diamond)] += 200;
        matarial[(int)(EInvenType.Zinc)] += 10;
        userData.MaterialSave(matarial);
        uiManager.UpdateUserInfo();

        // 랜덤 장비 주기
        UnitIcon[] unitIcon = new UnitIcon[1];
        for (int i = 0; i < 1; ++i)
        {
            int rand = UnityEngine.Random.Range(0, (int)EItemList.MATERIAL);
            EItemList type = EItemList.WEAPON;
            int[] unit;
            unit = new int[3]; // 현재 있는 무기리스트를 뽑는다
            unit[0] = 3; unit[1] = 14; unit[2] = 25;

            rand = UnityEngine.Random.Range(0, unit.Length);
            unitIcon[i] = CreateItem(type, unit[rand]);
            userData.UnitTypeSave(type);
        }

        //_packageBoxPopup.TenBoxOpen(10, 0, 0, 0, 0, 0, 200, unitIcon);

        userData.PackageSave(0);
        _packageArrObj[0].SetActive(false);
        if (userData.PackageAllCheck()) { _packageListObj.SetActive(false); }
    }
    public void PackageTwo()
    {
        int[] matarial = userData.GetAllMaterials();
        matarial[(int)(EInvenType.Diamond)] += 500;
        matarial[(int)(EInvenType.Zinc)] += 10;
        matarial[(int)(EInvenType.Copper)] += 4;
        matarial[(int)(EInvenType.Aluminum)] += 4;
        matarial[(int)(EInvenType.Steel)] += 4;
        matarial[(int)(EInvenType.Gold)] += 4;
        matarial[(int)(EInvenType.Oil)] += 4;
        userData.MaterialSave(matarial);
        uiManager.UpdateUserInfo();

        // 랜덤 장비 주기
        UnitIcon[] unitIcon = new UnitIcon[1];
        for (int i = 0; i < 1; ++i)
        {
            int rand = UnityEngine.Random.Range(0, (int)EItemList.MATERIAL);
            EItemList type = EItemList.WEAPON;
            int[] unit;
            unit = new int[3]; // 현재 있는 무기리스트를 뽑는다
            unit[0] = 4; unit[1] = 15; unit[2] = 26;

            rand = UnityEngine.Random.Range(0, unit.Length);
            unitIcon[i] = CreateItem(type, unit[rand]);
            userData.UnitTypeSave(type);
        }

        //_packageBoxPopup.TenBoxOpen(10, 4, 4, 4, 4, 4, 500, unitIcon);

        userData.PackageSave(1);
        _packageArrObj[1].SetActive(false);
        if (userData.PackageAllCheck()) { _packageListObj.SetActive(false); }
    }

    public void PackageThree()
    {
        int[] matarial = userData.GetAllMaterials();
        matarial[(int)(EInvenType.Diamond)] += 1000;
        matarial[(int)(EInvenType.Zinc)] += 50;
        matarial[(int)(EInvenType.Copper)] += 10;
        matarial[(int)(EInvenType.Aluminum)] += 10;
        matarial[(int)(EInvenType.Steel)] += 10;
        matarial[(int)(EInvenType.Gold)] += 10;
        matarial[(int)(EInvenType.Oil)] += 10;
        userData.MaterialSave(matarial);
        uiManager.UpdateUserInfo();

        // 랜덤 장비 주기
        UnitIcon[] unitIcon = new UnitIcon[3];

        int rand = UnityEngine.Random.Range(0, (int)EItemList.MATERIAL);
        EItemList type = EItemList.WEAPON;
        int[] unit;
        unit = new int[3]; // 현재 있는 무기리스트를 뽑는다
        unit[0] = 4; unit[1] = 15; unit[2] = 26;

        rand = UnityEngine.Random.Range(0, unit.Length);
        unitIcon[0] = CreateItem(type, unit[rand]);

        for (int i = 1; i < 3; ++i)
        {
            rand = UnityEngine.Random.Range(0, (int)EItemList.MATERIAL);
            type = (EItemList)rand;
            if (type == EItemList.WEAPON)
            {
                type = NotWeaponTypeChange();
            }
            // 정한 타입으로 데이터를 생성한다
            unit = new int[6];
            unit[0] = 4; unit[1] = 15; unit[2] = 26; unit[3] = 37; unit[4] = 48; unit[5] = 59;

            rand = UnityEngine.Random.Range(0, unit.Length);
            unitIcon[i] = CreateItem(type, unit[rand]);
        }

        userData.UnitAllTypeSave(GetItemTypeList(unitIcon));
        //_packageBoxPopup.TenBoxOpen(50, 10, 10, 10, 10, 10, 1000, unitIcon);

        userData.PackageSave(2);
        _packageArrObj[2].SetActive(false);
        if (userData.PackageAllCheck()) { _packageListObj.SetActive(false); }
    }
    #endregion Package

    public void SetBadge(bool state)
    {
        uiManager.SetBadge(BadgeType.Shop, state);
        _freeItemAlram.SetActive(state);
    }

    /// <summary>
    /// 아이템 타입리스트로 반환
    /// </summary>
    /// <param name="unitIconArr">아이콘 배열</param>
    /// <returns></returns>
    List<EItemList> GetItemTypeList(UnitIcon[] unitIconArr)
    {
        List<EItemList> list = new List<EItemList>();
        bool iContinue = false;
        for (int i = 0; i < unitIconArr.Length; ++i)
        {
            for (int j = 0; j < list.Count; ++j)
            {
                if (list[j] == unitIconArr[i].GetItemType)
                {
                    iContinue = true;
                    break;
                }
            }
            if (iContinue) { iContinue = false; continue; }

            list.Add(unitIconArr[i].GetItemType);
        }
        return list;
    }

    EItemList NotWeaponTypeChange()
    {
        int rand = UnityEngine.Random.Range(0, 5);
        if (rand == 0) { return EItemList.BOAT; }
        else if (rand == 1) { return EItemList.DEFENSE; }
        else if (rand == 2) { return EItemList.CAPTAIN; }
        else if (rand == 3) { return EItemList.SAILOR; }
        else { return EItemList.ENGINE; }
    }

    #region AD
    /// <summary>
    /// 광고 버튼 OFF 처리 0 : 노말상자 / 1 : 엘리트상자 / 2 : 다이아 / 3 : 골드
    /// </summary>
    public void SetAdRewardSet(int idx, bool state) => _adTargetProductArr[idx].SetPriceState(state);
    #endregion AD

    #region ShortCut
    public void DiamondPageBtn()
    {
        IAPManager.GetInstance.CheckISInitialized();
        uiManager.ChangePage(PageType.SHOP);
        _contentTrans.anchoredPosition = new Vector3(0f, 1500f);
    }
    public void GoldPageBtn()
    {
        IAPManager.GetInstance.CheckISInitialized();
        uiManager.ChangePage(PageType.SHOP);
        _contentTrans.anchoredPosition = new Vector3(0f, 1725f);
    }

    public override void OpenPage()
    {
        IAPManager.GetInstance.CheckISInitialized();
        _contentTrans.anchoredPosition = new Vector3(0f, 0f);
        base.OpenPage();
    }
    #endregion ShortCut

    /// <summary>
    /// 일일 퀘스트 : 무료 상품 구매 체크
    /// </summary>
    public void CheckDailyProductQuest()
    {
        var list = userData.GetDailyQuest();
        if (list[(int)EDailyQuest.BUYREWARD] <= 0)
        {
            userData.SetDailyQuest(EDailyQuest.BUYREWARD, 1);
            uiManager.GetPopup<QuestPopup>(PopupType.QUEST).CheckDailyQuestSet();
            userData.SaveQuestData();
        }
    }

    /// <summary>
    /// 상품 데이터를 찾아주는 메서드
    /// </summary>
    /// <param name="shopProduct">상품 타입</param>
    /// <returns></returns>
    public ShopProductData FindShopProductData(EShopProduct shopProduct)
    {
        var dataList = _dataManager.GetList<ShopProductData>(DataManager.KEY_SHOPPRODUCT);
        if (dataList.Count <= (int)shopProduct) return null;

        if (dataList[(int)shopProduct].nId - 1 == (int)shopProduct)
        {
            return dataList[(int)shopProduct];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 구매 확인 텍스트 
    /// </summary>
    /// <param name="shopProduct">상품 타입</param>
    /// <returns></returns>
    public string GetBuyAskText(EShopProduct shopProduct)
    {
        if (shopProduct == EShopProduct.NORMAL_BOX)
        {
            return LocalizeText.Get("기본 상자를 구매하시겠습니까?");
        }
        else if (shopProduct == EShopProduct.ELITE_BOX)
        {
            return LocalizeText.Get("고급 상자를 구매하시겠습니까?");
        }
        else if (shopProduct == EShopProduct.ELITE_TEN_BOX)
        {
            return LocalizeText.Get("10회 고급 상자를 구매하시겠습니까?");
        }
        else if (shopProduct <= EShopProduct.DIAMOND_06)
        {
            return LocalizeText.Get("다이아를 구매하시겠습니까?");
        }
        else if (shopProduct >= EShopProduct.MONEY_01 && shopProduct <= EShopProduct.MONEY_03)
        {
            return LocalizeText.Get("골드를 구매하시겠습니까?");
        }
        else if (shopProduct == EShopProduct.ENERGY_02)
        {
            return LocalizeText.Get("행동력을 구매하시겠습니까?");
        }
        else if (shopProduct == EShopProduct.SEASONPASS)
        {
            return LocalizeText.Get("시즌 패스를 구매하시겠습니까?");
        }
        return "";
        // 패키지는 이후 추가
    }

    public TextMeshProUGUI GetChestNormalTimeText() { return _chestNormalTimeText; }
    public TextMeshProUGUI GetChestEliteTimeText() { return _chestEliteTimeText; }
    public void InAppCheck(string id) => IAPManager.GetInstance.Purchase(id);
    public void NotInAppMessage() => MessageHandler.GetInstance.ShowMessage("상품 결제 취소", 1.5f);
    public void ShowMessageNotEnoughDia() => MessageHandler.GetInstance.ShowMessage("다이아가 부족합니다.", 1.5f);
    public ChoiceAskPopup GetBuyQuestionPopup() => _buyQuestionPopup;
}
