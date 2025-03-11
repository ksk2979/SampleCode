using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyData;

public class CollectionPopup : PopupBase
{
    [SerializeField] Transform _collectionParent;
    [SerializeField] GameObject _collectionBoxPrefab;
    [SerializeField] List<CollectionScript> _collectionList;
    UserData _userData;
    LobbyUIManager _uiManager;

    bool _isWorking = false;
    public void Init(LobbyUIManager uiManager)
    {
        _uiManager = uiManager;
        _userData = UserData.GetInstance;
        List<int> list = _userData.GetCollectionData._collectionList;
        var dataList = DataManager.GetInstance.GetList<CollectionData>(DataManager.KEY_COLLECTION);
        for(int i = 0; i < dataList.Count; i++)
        {
            GameObject collectionObj = Instantiate(_collectionBoxPrefab, _collectionParent);
            CollectionScript collectionBox = collectionObj.GetComponent<CollectionScript>();
            collectionBox.CountInit(this, dataList[i].nId, list[i], dataList[i].objective);
            _collectionList.Add(collectionBox);
        }
        NoticeCheck();
        SortCollectionList();
    }

    /// <summary>
    /// 알림 체크
    /// </summary>
    public void NoticeCheck()
    {
        for (int i = 0; i < _collectionList.Count; ++i)
        {
            if (_collectionList[i].GetNoticeActive)
            {
                // 바깥에도 알림설정 활성화
                SetNotice(true);
                return;
            }
        }
        // 다 체크했는데 없으면 비활성화
        SetNotice(false);
    }

    /// <summary>
    /// 알림 체크(외부에서 사용)
    /// </summary>
    /// <param name="state"></param>
    public void SetNotice(bool state)
    {
        _uiManager.SetBadge(BadgeType.Collection, state);
    }
    
    /// <summary>
    /// 수령 가능한 항목 상위로 정렬
    /// </summary>
    public void SortCollectionList()
    {
        // 수령가능/불가능 분류
        List<CollectionScript> receiveableList = new List<CollectionScript>();
        List<CollectionScript> nonReceiveableList = new List<CollectionScript>();
        for(int i = 0; i < _collectionList.Count; i++)
        {
            if(_collectionList[i].CanReceive)
            {
                receiveableList.Add(_collectionList[i]);
            }
            else
            {
                nonReceiveableList.Add(_collectionList[i]);
            }
        }

        // Rect위치를 재정렬
        int idx = 0;
        for(int j = 0; j < receiveableList.Count; j++)
        {
            receiveableList[j].RectTrans.SetSiblingIndex(idx);
            idx++;
        }

        for(int k = 0; k < nonReceiveableList.Count; k++)
        {
            nonReceiveableList[k].RectTrans.SetSiblingIndex(idx);
        }
    }

    /// <summary>
    /// 보상 받는 버튼
    /// </summary>
    /// <param name="nId">데이터 ID</param>
    public void OnTouchRewardButton(int nId)
    {
        if (_isWorking) return;
        _isWorking = true;
        CollectionData colData = DataManager.GetInstance.FindData(DataManager.KEY_COLLECTION, nId) as CollectionData;
        int arr = nId - 1;

        if (colData == null || arr < 0) return;

        bool endReceive = _collectionList[arr].CountSub();
        if (endReceive)
        {
            SetNotice(false);
            _collectionList[arr].ResetScript();
        }
        else
        {
            SetNotice(true);
            _collectionList[arr].ContinueCount();
        }
        var shop = _uiManager.GetShopPage;
        shop.ProvideNotPurchasingReward(EPayType.NONE, (ERewardType)colData.rewardType01, colData.rewardValue01, "수집 보상", false);
        _userData.CollectionUnitSave(arr, _collectionList[arr].Count());
        SortCollectionList();
        _isWorking = false;
    }

    public List<CollectionScript> GetCollectionList => _collectionList;
}