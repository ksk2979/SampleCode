using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class QuestItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _questTitleTMP;
    [SerializeField] TextLocalizeSetter _questContext;
    [SerializeField] TextMeshProUGUI _questProgressRate;
    [SerializeField] Image _progressBar;
    [SerializeField] Button _receiptButton;
    [SerializeField] GameObject _blockImageObj;
    [SerializeField] GameObject _alramObj;

    [Header("Info")]
    ERewardType _rewardType;
    int _rewardCount;
    [SerializeField] Image _rewardIcon;
    [SerializeField] TextMeshProUGUI _rewardCountTMP;
    const string rewardCountFormat = "X {0}";

    System.Action _buttonAction;
    public void SetQuestItem(List<string> textList, Sprite rewardSprite, ERewardType rewardType, int rewardCount,  float rate, bool isReceived)
    {
        _questTitleTMP.text = textList[0];
        _questContext.key = textList[1];
        _questProgressRate.text = textList[2];

        _rewardType = rewardType;
        _rewardIcon.sprite = rewardSprite;
        _rewardCountTMP.text = string.Format(rewardCountFormat, rewardCount);

        _progressBar.fillAmount = rate;
        if (_progressBar.fillAmount >= 1.0f && !isReceived)
        {
            _receiptButton.interactable = true;
            _alramObj.SetActive(true);
        }
        else
        {
            _receiptButton.interactable = false;
            _alramObj.SetActive(false);
        }

        _blockImageObj.SetActive(isReceived);
    }

    public void SetButton(System.Action buttonAction)
    {
        _buttonAction = null;
        _receiptButton.onClick.RemoveAllListeners();

        _buttonAction = buttonAction;
        _receiptButton.onClick.AddListener(()=> 
        { 
            _receiptButton.interactable = false;
            _blockImageObj.SetActive(true);
            _alramObj.SetActive(false);
            buttonAction?.Invoke();
        });
    }

    public void ClearQuest()
    {
        _receiptButton.interactable = false;
        _alramObj.SetActive(false);
        _blockImageObj.SetActive(true);
        _buttonAction?.Invoke();
    }

    // 수령 가능 상태 확인(알림 기능에 사용)
    public bool IsReceivable => _receiptButton.interactable;
    public float Rate => _progressBar.fillAmount;
    public ERewardType QuestRewardType => _rewardType;
    public int QuestRewardCount => _rewardCount;
}
