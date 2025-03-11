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
    [Header("����ũ ����")]
    [SerializeField] GameObject _tutorialObj;
    [SerializeField] RectTransform _tutorialMaskTrans;
    [SerializeField] RectTransform _effectTrans;
    [SerializeField] Button _tutoBtn;

    [Header("�ؽ�Ʈ")]
    [SerializeField] TypingText _typingText; // ���� ����
    [SerializeField] GameObject _tutoBlurObj; // �ؽ�Ʈ ������Ʈ
    int _typingCount = 0;

    [Header("���� ���̺�")]
    [SerializeField] string[] _messageText;
    [SerializeField] RectTransform[] _targetRect;
    UnitIcon _teamSelectBtn;
    UnitIcon _weaponBtn;
    UnitIcon _boatWeaponBtn;
    [SerializeField] RectTransform[] _boatWeaponRect; // �ռ��Ҷ� ��Ʈ �ȿ� �ִ� ���� ������
    [SerializeField] RectTransform[] _fusionBtnRect;

    [SerializeField] int _tutorialCount = 0;
    [SerializeField] int _tutorialTextCount = 0;
    int _turorialTransCount = 0;
    bool _notNext = false;

    [Header("�ʿ� �Լ�")]
    [SerializeField] LobbyUIManager _lobbyManager;
    [SerializeField] RectTransform _invenContent;
    [SerializeField] GameObject[] _rewardIconObj;

    [Header("2��° �ΰ��� ������Ʈ")]
    [SerializeField] GameObject[] _ingameObjArr;

    bool _rewardCheck = false;
    bool _endReward = false;

    public void Init()
    {
        _notNext = false;
        UserData ud = UserData.GetInstance;

        if (ud.TutorialCheck() == 8) { ud.SaveTutorial(9); } // �ռ��� 8�� ������ �ٽ� ����ų� �ϸ� 9�� ����

        // ù ����
        if (ud.TutorialCheck() == 0)
        {
            SetTutorialState(0, 0, 0);
            TypingAction();
        }
        // Ȥ�� ��Ʈ �ְ� �� �Ŀ� �׳� ���� �������� ����ؼ�
        else if (ud.TutorialCheck() == 1)
        {
            SetTutorialState(7, 4, 2);
            TypingAction();
        }
        // �� ����
        else if (ud.TutorialCheck() == 2)
        {
            if (cSceneManager.GetInstance.GetSceneName("Stage_Tutorial"))
            {
                SetTutorialState(9, 5, 3);
                TypingAction();
                // �ΰ����� �� ��� �ٷ� �κ��丮 �������� �Ѱ��ش�
                ud.SaveTutorial(3);
                PlayerPrefs.DeleteAll();
            }
        }
        // �κ��丮 ����
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
            // �������� ���� ��Ʈ�� ������ ���ϴ� ���� ������ ��ġ�� �� �� �ֽ��ϴ�
            SetTutorialState(25, 15, 9);
        }
        // �ι�° �� ����
        else if (ud.TutorialCheck() == 6)
        {
            if (cSceneManager.GetInstance.GetSceneName("Stage_Tutorial"))
            {
                SetTutorialState(32, 17, 0);
                _ingameObjArr[0].SetActive(false); // ���̽�ƽ
                _ingameObjArr[1].SetActive(true); // ����
                //TypingAction();
                // �ΰ����� �� ��� �ٷ� �κ��丮 �ռ����� �Ѱ��ش�
                ud.SaveTutorial(7);
                PlayerPrefs.DeleteAll();
            }
        }
        // �κ�� ����
        else if (ud.TutorialCheck() == 7)
        {
            // ��ġ�� �� ���¿��� ƨ��ų� �׳� ���ͼ� �ٽ� ��°Ŵϱ� �׳� ���� �����ɷ� ġ�� ��� �������ش� (��Ʈ1)
            _notNext = true;
            // 34: ���� ���� ��ߵ� (��Ʈ) 19: �̹��� �ռ��� �غ��ڽ��ϴ� 10: �κ��丮 ���� ��ư
            SetTutorialState(36, 19, 12);
            //_lobbyManager.GetShopPage.AddItem(EItemList.BOAT, 1);
            // ��Ʈ �����͸� ���� �־��ִ� ������� ��ȯ
            var userData = UserData.GetInstance;
            var boatData = DataManager.GetInstance.GetData(MyData.DataManager.KEY_BOAT, 1, 1) as BoatData;
            int ingerenceID = userData.CreateUnitIngerenceID(1, boatData.grade);
            userData.BoatCoalesceneceAdd(ingerenceID, 1);
            userData.UnitTypeAdd(EItemList.BOAT, 1);
            userData.UnitTypeSave(EItemList.BOAT);
            _rewardIconObj[0].SetActive(true);
            // ��Ʈ�� ������ �ڵ����� 34���� ������ ��
            // 35������ �ռ� �������� ���� �Ѵ�
            ud.SaveTutorial(8); // ��Ʈ ������� �ٷ� ���̺�
        }
        // �׽�Ʈ�� ����
        //else if (ud.TutorialCheck() == 8)
        //{
        //    _tutorialCount = 34; // ���� ���� ��ߵ� (��Ʈ)
        //    _tutorialTextCount = 19; // �̹��� �ռ��� �غ��ڽ��ϴ�
        //    _turorialTransCount = 10; // �κ��丮 ���� ��ư
        //    TypingAction();
        //    for (int i = 0; i < ud.GetSelectUnitData._boatSPId.Count; ++i) { ud.GetSelectUnitData._boatSPId[i] = 0; } // �ʱ�ȭ
        //    GameManager.GetInstance.GetSaveListAdd(EUserSaveType.SELECTDATA, true);
        //}
        // ��Ʈ �ռ� �ϴ��� ������ ��� �۾��� �����°ɷ�
        // Ʃ�� ������ ������Ʈ ������ �� (���� ������ Ʃ���϶� ���̺� ��ȣ �������)
        else if (ud.TutorialCheck() == 9 || ud.TutorialCheck() == 101)
        {
            this.gameObject.SetActive(false);
        }
        // ������ 100���� ����
        else if (ud.TutorialCheck() == 100)
        {
            SetTutorialState(100, 24, 13);
            // ����ũ�� �����̱���!
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
    // ���� ����
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
            // �÷��̸� ���� ���� ������ �ľ�â�� ����ݴϴ�.
            TypingAction();
        }
        else if (_tutorialCount.Equals(2))
        {
            // ���� ���� ��ư Ȱ��ȭ
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            BtnInit(_lobbyManager.ReadyForStart);
        }
        else if (_tutorialCount.Equals(3))
        {
            //���Ϳ���� 1���� ���� ��� 4���� ����Ʈ ��� ������ �˴ϴ�.
            // ���⼭ �����ؾ��� ��Ʈ �����´�
            _teamSelectBtn = _lobbyManager.GetReadyPage.GetBoatList[0];
            TypingAction();
        }
        else if (_tutorialCount.Equals(4))
        {
            // ���� �ִ� ��Ʈ�� ������ ��� ���� �ڸ��� �����ݴϴ�.
            TypingAction();
        }
        else if (_tutorialCount.Equals(5))
        {
            // ��Ʈ�� ����
            MaskOpen(_teamSelectBtn.GetTrans);
            BtnInit(_teamSelectBtn.BoatPositionSelectFunction);
        }
        else if (_tutorialCount.Equals(6))
        {
            // ���� �ڸ� ����
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            _lobbyManager.GetReadyPage.SetBoatListBlockerState(false);
            BtnInit(() => { _lobbyManager.GetReadyPage.GetUnitPosition(0).PositionBtn(); });
        }
        else if (_tutorialCount.Equals(7))
        {
            // ���� �ڸ��� �������� �÷��̸� ���� ������ �����մϴ�.
            TypingAction();
            // ���⼭ ���̺� �ѹ�~
            UserData.GetInstance.SaveTutorial(1);
        }
        else if (_tutorialCount.Equals(8))
        {
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            _notNext = true;
            BtnInit(GameTutorialStartBtn);
            // ���⼭ ���̺� �ѹ�~
            UserData.GetInstance.SaveTutorial(2);
        }
        // �ΰ��� ����
        else if (_tutorialCount.Equals(10))
        {
            // ������ ���̺긶�� �����Ƽ ������ �� �� ������ ���� ���� ������ �� �� �ְ� �����ݴϴ�.
            TypingAction();
        }
        else if (_tutorialCount.Equals(11))
        {
            // ���� ������ �����ɴϴ�. ����� ���ϴ�.
            TypingAction();
            _notNext = true;
        }
        // �κ��丮 ����
        else if (_tutorialCount.Equals(13))
        {
            // �κ��丮�� ������
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            BtnInit(() => { _lobbyManager.ChangePage(PageType.INVEN); });
        }
        else if (_tutorialCount.Equals(14))
        {
            // ���� ���� ���׷��̵带 ���ؼ� ���� �Ǿ��ִ� ���⸦ ���� ��ȭ�� �غ��ڽ��ϴ�.
            TypingAction();
            // ���⼭ ���� ���� ����
            _lobbyManager.GetInvenPage.OnTouchCategoryButton((int)EItemList.WEAPON + 1);
            _weaponBtn = _lobbyManager.GetInvenPage.GetInven(EItemList.WEAPON)[0];
        }
        else if (_tutorialCount.Equals(15))
        {
            // ���� ��� ������ Ŭ�� ����Ʈ ����
            MaskOpen(_weaponBtn.GetTrans);
            BtnInit(_weaponBtn.SelectInvenIcon);
        }
        else if (_tutorialCount.Equals(16))
        {
            // ���� ����� ����â�Դϴ�.
            TypingAction();
        }
        else if (_tutorialCount.Equals(17))
        {
            // ������ �ִ� ���� ��Ḧ ���ؼ� ��� ��ȭ, ���� �������� �غ��ڽ��ϴ�.
            TypingAction();
        }
        else if (_tutorialCount.Equals(18))
        {
            // ��� ����
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;

            BtnInit(_weaponBtn.IconEditor);
            // ���� ��� ���� ����
            //BtnInit(_lobbyManager.GetItemEditorPage.OpenPage);
            //var popup = _lobbyManager.GetPopupController.GetPopup(PopupType.ITEMEDIT) as ItemEditPopup;
            //popup.OpenPopup();
            //BtnInit(popup.OnTouchUpgradeButton);
        }
        else if (_tutorialCount.Equals(19))
        {
            // ��� ���׷��̵� â Ȱ��ȭ
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;

            BtnInit(_lobbyManager.GetPopup<ItemPopup>(PopupType.ITEM).UpgradeItem);
        }
        else if (_tutorialCount.Equals(20))
        {
            // ��� ���׷��̵� ��ư
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;

            BtnInit(_lobbyManager.GetItemEditorPage.OnTouchUpgrade);
        }
        else if (_tutorialCount.Equals(21))
        {
            // ��� ���׷��̵� â Ŭ����
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;

            BtnInit(_lobbyManager.GetItemEditorPage.ClosePage);
        }
        else if (_tutorialCount.Equals(22))
        {
            // ������ �Ǹ� �κ� ȭ�鿡�� Ȯ���� �����մϴ�.
            TypingAction();
            _notNext = true; //�ӽ� (��Ʈ ���� ��)
            // ���⼭ ���̺� �ѹ�~
            UserData.GetInstance.SaveTutorial(4);
        }
        else if (_tutorialCount.Equals(24))
        {
            // �ٽ� Ȩ�� ���� �������� ȭ������ ���ּ���
            TypingAction();
        }
        else if (_tutorialCount.Equals(25))
        {
            // Ȩ ��ư
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            BtnInit(() => { _lobbyManager.ChangePage(PageType.STAGE); });
        }
        else if (_tutorialCount.Equals(26))
        {
            // ���� ���� ��ư
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            BtnInit(_lobbyManager.ReadyForStart);
        }
        else if (_tutorialCount.Equals(27))
        {
            // ��: �������� ���� ��Ʈ�� ������ ���ϴ� ���� ������ ��ġ�� �� �� �ֽ��ϴ�
            TypingAction();
            //_notNext = true;
            // ���⼭ ���̺� �ѹ�~
            UserData.GetInstance.SaveTutorial(5);
        }
        else if (_tutorialCount.Equals(28))
        {
            // ��Ʈ�� ����
            _teamSelectBtn = _lobbyManager.GetReadyPage.GetBoatList[0];
            MaskOpen(_teamSelectBtn.GetTrans);
            BtnInit(_teamSelectBtn.BoatPositionSelectFunction);
        }
        else if (_tutorialCount.Equals(29))
        {
            // ���� �ڸ� ����
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            _lobbyManager.GetReadyPage.SetBoatListBlockerState(false);
            BtnInit(() => { _lobbyManager.GetReadyPage.GetUnitPosition(1).PositionBtn(); });
        }
        else if (_tutorialCount.Equals(30))
        {
            // ��: ���� �ڸ��� �������� �ٽ� �÷��̸� ���� ������ �����մϴ�.
            TypingAction();
            // ���⼭ ���̺� �ѹ�~
            UserData.GetInstance.SaveTutorial(6);
        }
        else if (_tutorialCount.Equals(31))
        {
            // ���� ���� ��ư Ȱ��ȭ
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            _notNext = true;
            BtnInit(GameTutorialStartBtn);
        }
        else if (_tutorialCount.Equals(33))
        {
            // 32���� �װ� ���ڿ� ��� ������
            // 33�� ��Ȱ�ý��ۿ� ���� ���� �ϰ�
            // 34�� ��Ȱ ��ư�� �־��ش�
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
            // �װ� �ٽ� ��Ƴ��� �κ�� �� �ڿ� ��Ʈ ���� ���� ���� �� ���ϴ� �ؽ�Ʈ (�ռ��ؾ��ϴ� Ʃ��) ��: �̹��� �ռ��� �غ��ڽ��ϴ� ���� ��ư ���;���
            // �κ��丮�� ������
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            BtnInit(() => { _lobbyManager.ChangePage(PageType.INVEN); });
        }
        else if (_tutorialCount.Equals(38))
        {
            // ��: �ռ��� �ϱ� ���ؼ��� ����Ǿ� �ִ°��� �����ؾ��մϴ�
            TypingAction();
            _boatWeaponBtn = _lobbyManager.GetInvenPage.GetInven(EItemList.BOAT)[1];
        }
        else if (_tutorialCount.Equals(39))
        {
            // ��Ʈ Ŭ��
            MaskOpen(_boatWeaponBtn.GetTrans);
            BtnInit(_boatWeaponBtn.IconEditor);
        }
        else if (_tutorialCount.Equals(40))
        {
            // ���� ��ư Ŭ��
            UnitIcon u = _lobbyManager.GetInvenPage.GetInven(EItemList.WEAPON)[0];
            MaskOpen(_boatWeaponRect[0]);
            BtnInit(u.SelectInvenIcon);
        }
        else if (_tutorialCount.Equals(41))
        {
            StartCoroutine(DelayedAction(() =>
            {
                // ���� ��ư Ŭ��
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
            // �ռ� ��ư Ŭ��
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
            // ��: �ռ� ������ �Դϴ�
            TypingAction();
        }
        else if (_tutorialCount.Equals(44))
        {
            // ��: ���� ����� ��Ʈ 3���� �ռ��� ������
            _notNext = true;
            TypingAction();
        }

        // ����
        else if (_tutorialCount.Equals(101))
        {
            // ���� ��ư Ȱ��ȭ
            MaskOpen(_targetRect[_turorialTransCount]);
            _turorialTransCount++;
            BtnInit(_lobbyManager.GetPopup<CollectionPopup>(PopupType.COLLECTION).OpenPopup);
        }
        else if (_tutorialCount.Equals(102))
        {
            // ���������� �Դϴ�. ����ũ�� ��� �Ǹ� �ش� ����ũ ��ġ�� ������ ���� ��ġ �̻��� �Ǹ� ������ ȹ�� �� �� �ֽ��ϴ�.
            TypingAction();
        }
        else if (_tutorialCount.Equals(103))
        {
            // ���� ������� ������ ȹ�� �غ��ڽ��ϴ�.
            TypingAction();
        }
        else if (_tutorialCount.Equals(104))
        {
            // ���� ��ư Ȱ��ȭ
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
        if (userData.ChooseBoatTemp[(int)EUnitPosition.MIDDLE] == null) { MessageHandler.GetInstance.ShowMessage("���κ�Ʈ�� �����ϴ�", 1.5f); return; }

        userData.UnitInfo.Clear();
        var data = userData.GetSelectData;
        for (int i = 0; i < data._boatSPId.Count; ++i) { data._boatSPId[i] = 0; } // �ʱ�ȭ
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

        // ���̺�
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
    /// ���� ���� Ʃ�丮�� Ȯ��(���� ���� ���� �� �����, ���� ��� ���� �� ���� �ʿ�)
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
