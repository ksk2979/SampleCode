using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyStructData;
using System;
using MyData;

public class TutorialManager : SceneStaticObj<TutorialManager>
{
    [Header("마스크 관련")]
    [SerializeField] GameObject _tutorialObj;
    [SerializeField] RectTransform _tutorialMaskTrans;
    [SerializeField] RectTransform _effectTrans;
    [SerializeField] Button _tutoBtn;

    [Header("텍스트")]
    [SerializeField] TypingText _typingText; // 실제 내용
    [SerializeField] GameObject _tutoBlurObj; // 텍스트 오브젝트
    int _typingCount = 0;

    [Header("임의 테이블")]
    [SerializeField] string[] _messageText;
    [SerializeField] RectTransform[] _targetRect;
    UnitIcon _teamSelectBtn;
    UnitIcon _weaponBtn;
    UnitIcon _boatWeaponBtn;
    [SerializeField] RectTransform[] _boatWeaponRect; // 합성할때 보트 안에 있는 무기 뺴야함
    [SerializeField] RectTransform[] _fusionBtnRect;

    [SerializeField] int _tutorialCount = 0;
    [SerializeField] int _tutorialTextCount = 0;
    int _turorialTransCount = 0;
    bool _notNext = false;

    [Header("필요 함수")]
    [SerializeField] LobbyUIManager _lobbyManager;
    [SerializeField] RectTransform _invenContent;
    [SerializeField] GameObject[] _rewardIconObj;

    [Header("2번째 인게임 오브젝트")]
    [SerializeField] GameObject[] _ingameObjArr;

    bool _rewardCheck = false;
    bool _endReward = false;

    public void Init()
    {
        _notNext = false;
        UserData ud = UserData.GetInstance;

        if (ud.TutorialCheck() == 8) { ud.SaveTutorial(9); } // 합성은 8로 끝나고 다시 들오거나 하면 9로 변경

        // 첫 시작
        if (ud.TutorialCheck() == 0)
        {
            SetTutorialState(0, 0, 0);
            TypingAction();
        }
        // 혹시 보트 넣고 난 후에 그냥 종료 했을때를 대비해서
        else if (ud.TutorialCheck() == 1)
        {
            SetTutorialState(7, 4, 2);
            TypingAction();
        }
        // 인 게임
        else if (ud.TutorialCheck() == 2)
        {
            if (cSceneManager.GetInstance.GetSceneName("Stage_Tutorial"))
            {
                SetTutorialState(9, 5, 3);
                TypingAction();
                // 인게임은 머 없어서 바로 인벤토리 시작으로 넘겨준다
                ud.SaveTutorial(3);
                PlayerPrefs.DeleteAll();
            }
        }
        // 인벤토리 시작
        else if (ud.TutorialCheck() == 3)
        {
            SetTutorialState(12, 8, 3);
            TypingAction();
        }
        else if (ud.TutorialCheck() == 4)
        {
            SetTutorialState(24, 13, 8);
            TypingAction();
        }
        else if (ud.TutorialCheck() == 5)
        {
            // 보상으로 얻은 보트를 눌러서 원하는 곳에 서포터 배치를 할 수 있습니다
            SetTutorialState(25, 15, 9);
        }
        // 두번째 인 게임
        else if (ud.TutorialCheck() == 6)
        {
            if (cSceneManager.GetInstance.GetSceneName("Stage_Tutorial"))
            {
                SetTutorialState(32, 17, 0);
                _ingameObjArr[0].SetActive(false); // 조이스틱
                _ingameObjArr[1].SetActive(true); // 보스
                //TypingAction();
                // 인게임은 머 없어서 바로 인벤토리 합성으로 넘겨준다
                ud.SaveTutorial(7);
                PlayerPrefs.DeleteAll();
            }
        }
        // 로비로 오면
        else if (ud.TutorialCheck() == 7)
        {
            // 배치를 한 상태에서 튕기거나 그냥 나와서 다시 들온거니깐 그냥 게임 끝난걸로 치고 장비 지급해준다 (보트1)
            _notNext = true;
            // 34: 보상 먼저 줘야됨 (보트) 19: 이번엔 합성을 해보겠습니다 10: 인벤토리 들어가는 버튼
            SetTutorialState(36, 19, 12);
            //_lobbyManager.GetShopPage.AddItem(EItemList.BOAT, 1);
            // 보트 데이터만 따로 넣어주는 방식으로 전환
            var userData = UserData.GetInstance;
            var boatData = DataManager.GetInstance.GetData(MyData.DataManager.KEY_BOAT, 1, 1) as BoatData;
            int ingerenceID = userData.CreateUnitIngerenceID(1, boatData.grade);
            userData.BoatCoalesceneceAdd(ingerenceID, 1);
            userData.UnitTypeAdd(EItemList.BOAT, 1);
            userData.UnitTypeSave(EItemList.BOAT);
            _rewardIconObj[0].SetActive(true);
            // 보트를 받으면 자동으로 34번이 실행이 됨
            // 35번에는 합성 페이지로 가게 한다
            ud.SaveTutorial(8); // 보트 얻었으니 바로 세이브
        }
        // 테스트로 넣음
        //else if (ud.TutorialCheck() == 8)
        //{
        //    _tutorialCount = 34; // 보상 먼저 줘야됨 (보트)
        //    _tutorialTextCount = 19; // 이번엔 합성을 해보겠습니다
        //    _turorialTransCount = 10; // 인벤토리 들어가는 버튼
        //    TypingAction();
        //    for (int i = 0; i < ud.GetSelectUnitData._boatSPId.Count; ++i) { ud.GetSelectUnitData._boatSPId[i] = 0; } // 초기화
        //    GameManager.GetInstance.GetSaveListAdd(EUserSaveType.SELECTDATA, true);
        //}
        // 보트 합성 하던지 말던지 모든 작업이 끝나는걸로
        // 튜토 끝나서 오브젝트 꺼놓는 것 (보통 마지막 튜토일때 세이브 번호 적어놓음)
        else if (ud.TutorialCheck() == 9 || ud.TutorialCheck() == 101)
        {
            this.gameObject.SetActive(false);
        }
        // 도감은 100부터 시작
        else if (ud.TutorialCheck() == 100)
        {
            SetTutorialState(100, 24, 13);
            // 유니크를 잡으셨군요!
            TypingAction();
        }
    }

    void SetTutorialState(int tutorialCount, int tutorialTextCount, int turorialTransCount)
    {
        _tutorialCount = tutorialCount;
        _tutorialTextCount = tutorialTextCount;
        _turorialTransCount = turorialTransCount;
    }

    public void TypingAction()
    {
        if (_typingCount == 0)
        {
            _tutoBlurObj.SetActive(true);
            StartCoroutine(_typingText.Typing(LocalizeText.Get(_messageText[_tutorialTextCount]), 0.05f));
            _typingCount++;
        }
        else if (_typingCount == 1)
        {
            if (_typingText.NowTyping)
            {
                _typingText.TypingSkip = true;
                _typingCount++;
            }
            else { TypingNext(); }
        }
        else if (_typingCount == 2)
        {
            TypingNext();
        }
    }
    // 다음 내용
    void TypingNext()
    {
        _tutoBlurObj.SetActive(false);
        _typingCount = 0;
        _tutorialCount++;
        _tutorialTextCount++;
        NextTutorial();
    }

    public void NextTutorial()
    {
        if (_notNext)
        {
            if (_tutorialCount.Equals(23))
            {
                _lobbyManager.GetShopPage.AddItem(EItemList.BOAT, 1);
                _rewardIconObj[0].SetActive(true);
            }
            else if (_tutorialCount.Equals(46))
            {
                _lobbyManager.GetPopup<SynthesisPopup>(PopupType.SYNTHESIS).ClosePopup();
                _lobbyManager.GetShopPage.AddItem(EItemList.BOAT, 3);
                _rewardIconObj[1].SetActive(true);
                _notNext = true;
                _rewardCheck = true;
                _tutorialCount++;
            }
            else if (_tutorialCount.Equals(47))
            {
                _rewardCheck = false;
                _endReward = true;
                _lobbyManager.GetShopPage.AddItem(EItemList.WEAPON, 3);
                _rewardIconObj[2].SetActive(true);
            }
            return;
        }

        if (_tutorialCount.Equals(1))
        {
            // 플레이를 눌러 게임 시작의 파업창을 띄워줍니다.
            TypingAction();
        }
        else if (_tutorialCount.Equals(2))
        {
            // 게임 선택 버튼 활성화
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            BtnInit(_lobbyManager.ReadyForStart);
        }
        else if (_tutorialCount.Equals(3))
        {
            //워터월드는 1개의 메인 배와 4개의 서포트 배로 구성이 됩니다.
            // 여기서 선택해야할 보트 가져온다
            _teamSelectBtn = _lobbyManager.GetReadyPage.GetBoatList[0];
            TypingAction();
        }
        else if (_tutorialCount.Equals(4))
        {
            // 현재 있는 보트를 선택해 가운데 메인 자리에 놓아줍니다.
            TypingAction();
        }
        else if (_tutorialCount.Equals(5))
        {
            // 보트를 선택
            MaskOpen(_teamSelectBtn.GetTrans);
            BtnInit(_teamSelectBtn.BoatPositionSelectFunction);
        }
        else if (_tutorialCount.Equals(6))
        {
            // 메인 자리 선택
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            _lobbyManager.GetReadyPage.SetBoatListBlockerState(false);
            BtnInit(() => { _lobbyManager.GetReadyPage.GetUnitPosition(0).PositionBtn(); });
        }
        else if (_tutorialCount.Equals(7))
        {
            // 메인 자리에 놓았으면 플레이를 눌러 게임을 시작합니다.
            TypingAction();
            // 여기서 세이브 한번~
            UserData.GetInstance.SaveTutorial(1);
        }
        else if (_tutorialCount.Equals(8))
        {
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            _notNext = true;
            BtnInit(GameTutorialStartBtn);
            // 여기서 세이브 한번~
            UserData.GetInstance.SaveTutorial(2);
        }
        // 인게임 시작
        else if (_tutorialCount.Equals(10))
        {
            // 정해진 웨이브마다 어빌리티 선택을 할 수 있으며 더욱 강한 공격을 할 수 있게 도와줍니다.
            TypingAction();
        }
        else if (_tutorialCount.Equals(11))
        {
            // 이제 적들이 몰려옵니다. 행운을 빕니다.
            TypingAction();
            _notNext = true;
        }
        // 인벤토리 시작
        else if (_tutorialCount.Equals(13))
        {
            // 인벤토리로 가야함
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            BtnInit(() => { _lobbyManager.ChangePage(PageType.INVEN); });
        }
        else if (_tutorialCount.Equals(14))
        {
            // 무기 성능 업그레이드를 위해서 지급 되어있는 무기를 눌러 강화를 해보겠습니다.
            TypingAction();
            // 여기서 무기 정보 들어간다
            _lobbyManager.GetInvenPage.OnTouchCategoryButton((int)EItemList.WEAPON + 1);
            _weaponBtn = _lobbyManager.GetInvenPage.GetInven(EItemList.WEAPON)[0];
        }
        else if (_tutorialCount.Equals(15))
        {
            // 무기 장비 아이콘 클릭 이펙트 생성
            MaskOpen(_weaponBtn.GetTrans);
            BtnInit(_weaponBtn.SelectInvenIcon);
        }
        else if (_tutorialCount.Equals(16))
        {
            // 현재 장비의 정보창입니다.
            TypingAction();
        }
        else if (_tutorialCount.Equals(17))
        {
            // 가지고 있는 돈과 재료를 통해서 장비를 강화, 이후 장착까지 해보겠습니다.
            TypingAction();
        }
        else if (_tutorialCount.Equals(18))
        {
            // 장비 장착
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;

            BtnInit(_weaponBtn.IconEditor);
            // 원래 들어 갔던 정보
            //BtnInit(_lobbyManager.GetItemEditorPage.OpenPage);
            //var popup = _lobbyManager.GetPopupController.GetPopup(PopupType.ITEMEDIT) as ItemEditPopup;
            //popup.OpenPopup();
            //BtnInit(popup.OnTouchUpgradeButton);
        }
        else if (_tutorialCount.Equals(19))
        {
            // 장비 업그레이드 창 활성화
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;

            BtnInit(_lobbyManager.GetPopup<ItemPopup>(PopupType.ITEM).UpgradeItem);
        }
        else if (_tutorialCount.Equals(20))
        {
            // 장비 업그레이드 버튼
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;

            BtnInit(_lobbyManager.GetItemEditorPage.OnTouchUpgrade);
        }
        else if (_tutorialCount.Equals(21))
        {
            // 장비 업그레이드 창 클로즈
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;

            BtnInit(_lobbyManager.GetItemEditorPage.ClosePage);
        }
        else if (_tutorialCount.Equals(22))
        {
            // 장착이 되면 인벤 화면에서 확인이 가능합니다.
            TypingAction();
            _notNext = true; //임시 (보트 보상 전)
            // 여기서 세이브 한번~
            UserData.GetInstance.SaveTutorial(4);
        }
        else if (_tutorialCount.Equals(24))
        {
            // 다시 홈을 눌러 스테이지 화면으로 가주세요
            TypingAction();
        }
        else if (_tutorialCount.Equals(25))
        {
            // 홈 버튼
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            BtnInit(() => { _lobbyManager.ChangePage(PageType.STAGE); });
        }
        else if (_tutorialCount.Equals(26))
        {
            // 게임 시작 버튼
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            BtnInit(_lobbyManager.ReadyForStart);
        }
        else if (_tutorialCount.Equals(27))
        {
            // 텍: 보상으로 얻은 보트를 눌러서 원하는 곳에 서포터 배치를 할 수 있습니다
            TypingAction();
            //_notNext = true;
            // 여기서 세이브 한번~
            UserData.GetInstance.SaveTutorial(5);
        }
        else if (_tutorialCount.Equals(28))
        {
            // 보트를 선택
            _teamSelectBtn = _lobbyManager.GetReadyPage.GetBoatList[0];
            MaskOpen(_teamSelectBtn.GetTrans);
            BtnInit(_teamSelectBtn.BoatPositionSelectFunction);
        }
        else if (_tutorialCount.Equals(29))
        {
            // 서브 자리 선택
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            _lobbyManager.GetReadyPage.SetBoatListBlockerState(false);
            BtnInit(() => { _lobbyManager.GetReadyPage.GetUnitPosition(1).PositionBtn(); });
        }
        else if (_tutorialCount.Equals(30))
        {
            // 텍: 서브 자리에 놓았으면 다시 플레이를 눌러 게임을 시작합니다.
            TypingAction();
            // 여기서 세이브 한번~
            UserData.GetInstance.SaveTutorial(6);
        }
        else if (_tutorialCount.Equals(31))
        {
            // 게임 시작 버튼 활성화
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            _notNext = true;
            BtnInit(GameTutorialStartBtn);
        }
        else if (_tutorialCount.Equals(33))
        {
            // 32에는 죽고 난뒤에 대사 나오고
            // 33은 부활시스템에 대한 설명 하고
            // 34는 부활 버튼을 넣어준다
            TypingAction();
        }
        else if (_tutorialCount.Equals(34))
        {
            _notNext = true;
            _ingameObjArr[0].SetActive(true);
            _ingameObjArr[2].SetActive(true);
            RevivalPopup popup = _ingameObjArr[2].GetComponent<RevivalPopup>();
            InGameUIManager ui = GameObject.Find("InGameCanvas").GetComponent<InGameUIManager>();
            popup.OnDiaEventListener += ui.RetryOk;
            popup.OpenPopup();
        }
        else if (_tutorialCount.Equals(37))
        {
            // 죽고 다시 살아나서 로비로 온 뒤에 보트 보상 지급 받은 후 말하는 텍스트 (합성해야하는 튜토) 텍: 이번엔 합성을 해보겠습니다 이후 버튼 나와야함
            // 인벤토리로 가야함
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            BtnInit(() => { _lobbyManager.ChangePage(PageType.INVEN); });
        }
        else if (_tutorialCount.Equals(38))
        {
            // 텍: 합성을 하기 위해서는 착용되어 있는것을 해제해야합니다
            TypingAction();
            _boatWeaponBtn = _lobbyManager.GetInvenPage.GetInven(EItemList.BOAT)[1];
        }
        else if (_tutorialCount.Equals(39))
        {
            // 보트 클릭
            MaskOpen(_boatWeaponBtn.GetTrans);
            BtnInit(_boatWeaponBtn.IconEditor);
        }
        else if (_tutorialCount.Equals(40))
        {
            // 무기 버튼 클릭
            UnitIcon u = _lobbyManager.GetInvenPage.GetInven(EItemList.WEAPON)[0];
            MaskOpen(_boatWeaponRect[0]);
            BtnInit(u.SelectInvenIcon);
        }
        else if (_tutorialCount.Equals(41))
        {
            StartCoroutine(DelayedAction(() =>
            {
                // 해제 버튼 클릭
                UnitIcon u = _lobbyManager.GetInvenPage.GetInven(EItemList.WEAPON)[0];
                MaskOpen(_boatWeaponRect[1]);
                BtnInit(() =>
                {
                    u.IconEditor();
                    _lobbyManager.GetPopup<ItemPopup>(PopupType.ITEM).ClosePopup();
                });
            }));
        }
        else if (_tutorialCount.Equals(42))
        {
            // 합성 버튼 클릭
            MaskOpen(_fusionBtnRect[0]);
            BtnInit(() =>
            {
                SynthesisPopup popup = _lobbyManager.GetPopup<SynthesisPopup>(PopupType.SYNTHESIS);
                popup.OpenPopup();
                popup.OnTouchCategoryButton(1);
                popup.SetSearchButtonState(false);
            });
        }
        else if (_tutorialCount.Equals(43))
        {
            // 텍: 합성 페이지 입니다
            TypingAction();
        }
        else if (_tutorialCount.Equals(44))
        {
            // 텍: 같은 등급의 보트 3개를 합성해 보세요
            _notNext = true;
            TypingAction();
        }

        // 도감
        else if (_tutorialCount.Equals(101))
        {
            // 도감 버튼 활성화
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            BtnInit(_lobbyManager.GetPopup<CollectionPopup>(PopupType.COLLECTION).OpenPopup);
        }
        else if (_tutorialCount.Equals(102))
        {
            // 도감페이지 입니다. 유니크를 잡게 되면 해당 유니크 수치가 오르고 일정 수치 이상이 되면 보상을 획득 할 수 있습니다.
            TypingAction();
        }
        else if (_tutorialCount.Equals(103))
        {
            // 현재 맛보기로 보상을 획득 해보겠습니다.
            TypingAction();
        }
        else if (_tutorialCount.Equals(104))
        {
            // 보상 버튼 활성화
            CollectionPopup popup = _lobbyManager.GetPopup<CollectionPopup>(PopupType.COLLECTION);
            MaskOpen(popup.GetCollectionList[0].GetRewardButtonObj.GetComponent<RectTransform>());
            _turorialTransCount++;
            BtnInit(() =>
            {
                popup.OnTouchRewardButton(1);
                LobbyUIManager.GetInstance.CheckRewardTimes((int)ETimeCheckType.ONEDAY);
            });
            _notNext = true;
            UserData.GetInstance.SaveTutorial(101);
            LobbyUIManager.GetInstance.SetTutorialState(CheckTutorial());
        }
    }

    void MaskOpen(RectTransform rect)
    {
        _tutorialObj.SetActive(true);
        _tutorialMaskTrans.position = rect.position;
        _effectTrans.position = rect.position;
        _tutoBtn.transform.position = rect.position;
        _tutorialMaskTrans.sizeDelta = new Vector2(rect.rect.width, rect.rect.height);
        _effectTrans.localScale = new Vector2(rect.rect.width, rect.rect.height);
        (_tutoBtn.transform as RectTransform).sizeDelta = new Vector2(rect.rect.width, rect.rect.height);
    }
    void MaskClose()
    {
        _tutorialObj.SetActive(false);
    }
    public void BtnInit(UnityEngine.Events.UnityAction call)
    {
        _tutoBtn.onClick.RemoveAllListeners();
        _tutoBtn.onClick.AddListener(call);
        _tutoBtn.onClick.AddListener(BtnSelect);
    }
    public void BtnSelect()
    {
        MaskClose();
        _tutorialCount++;
        NextTutorial();
    }
    public void ReStartTutorial()
    {
        if (_rewardCheck) { NextTutorial(); return; }
        if (_endReward)
        {
            _endReward = false;
            _lobbyManager.CheckRewardTimes((int)ETimeCheckType.ONEDAY);
            return;
        }
        _notNext = false;
        TypingAction();
    }
    public void GameTutorialStartBtn()
    {
        UserData userData = UserData.GetInstance;
        if (userData.ChooseBoatTemp[(int)EUnitPosition.MIDDLE] == null) { MessageHandler.GetInstance.ShowMessage("메인보트가 없습니다", 1.5f); return; }

        userData.UnitInfo.Clear();
        var data = userData.GetSelectData;
        for (int i = 0; i < data._boatSPId.Count; ++i) { data._boatSPId[i] = 0; } // 초기화
        for (int i = 0; i < userData.ChooseBoatTemp.Length; ++i)
        {
            if (userData.ChooseBoatTemp[i] != null)
            {
                _lobbyManager.GetStagePage.StartPlayerInfoSetting(i);
            }
            else
            {
                userData.UnitInfo.Add(null);
            }
        }

        // 세이브
        userData.SaveUnitSelect();
        FirebaseManager.GetInstance.LogEvent("Stage_Tutorial");
        _lobbyManager.LoadingStage("Stage_Tutorial");
    }

    IEnumerator DelayedAction(System.Action action)
    {
        yield return YieldInstructionCache.WaitForEndOfFrame;
        action?.Invoke();
    }

    /// <summary>
    /// 진행 중인 튜토리얼 확인(게임 강제 종료 시 재시작, 추후 기능 분할 및 변경 필요)
    /// </summary>
    public bool CheckTutorial()
    {
        UserData userData = UserData.GetInstance;
        int curNum = userData.TutorialCheck();
        if(curNum < 100)
        {
            if(curNum > 8)
            {
                return true;
            }
            else
            {
                return false;
            }   
        }
        else
        {
            if (curNum >= 101)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
