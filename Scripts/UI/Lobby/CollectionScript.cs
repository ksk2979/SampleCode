using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionScript : MonoBehaviour
{
    CollectionPopup _collectionPopup;
    RectTransform _rectTrans;

    [SerializeField] Image _iconImage;
    [SerializeField] GameObject _noticeObj; // 알림 오브젝트
    [SerializeField] Image _gaugeImg;
    [SerializeField] TextMeshProUGUI _countText;
    [SerializeField] GameObject _rewardBtn; // 보상 버튼
    Button _receiveButton;
    int _count = 0;
    int _maxCount = 0;

    const string prevName = "Collection";
    public void CountInit(CollectionPopup popup, int nId, int count, int max)
    {
        _rectTrans = GetComponent<RectTransform>(); 

        _collectionPopup = popup;
        _count = count;
        _maxCount = max;

        string resName = prevName + string.Format("{0:D2}", nId);
        _iconImage.sprite = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format("ItemIcon/{0}", resName));
        _receiveButton = _rewardBtn.GetComponent<Button>();

        if (_count >= _maxCount)
        {
            _collectionPopup.SetNotice(true);
            _noticeObj.SetActive(true);
            _receiveButton.interactable = true;
        }
        else
        {
            _noticeObj.SetActive(false);
            _receiveButton.interactable = false;
        }
        CountUpdate();

        _receiveButton.onClick.AddListener(() => _collectionPopup.OnTouchRewardButton(nId));
    }
    public int CountAdd()
    {
        _count++;
        if (_count >= _maxCount)
        {
            _collectionPopup.SetNotice(true);
            _noticeObj.SetActive(true);
            _receiveButton.interactable = true;
        }
        CountUpdate();
        return _count;
    }

    public bool CountSub()
    {
        _count -= _maxCount;
        if (_count < 0) { _count = 0; } // 이게 될리가 없지만 혹시 모른다
        if (_count >= _maxCount) { return false; } // false 계속 보상이 나간다
        else { return true; } // true 보상이 정지
    }

    public void CountUpdate()
    {
        _gaugeImg.fillAmount = (float)_count / (float)_maxCount;
        _countText.text = string.Format("{0} / {1}", _count, _maxCount);
    }

    public int Count()
    {
        return _count;
    }

    public void ContinueCount()
    {
        CountUpdate();
        _noticeObj.SetActive(true);
        _receiveButton.interactable = true;
    }
    public void ResetScript()
    {
        CountUpdate();
        _noticeObj.SetActive(false);
        _receiveButton.interactable = false;
    }
    public bool GetNoticeActive { get { return _noticeObj.activeSelf; } }
    public GameObject GetRewardButtonObj => _rewardBtn;
    public RectTransform RectTrans => _rectTrans;
    public bool CanReceive => _count >= _maxCount;
}