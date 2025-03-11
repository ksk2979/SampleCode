using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassItem : MonoBehaviour
{
    bool _isReached = false;
    [SerializeField] Color[] _textColorArray;

    [Header("Content UI")]
    [SerializeField] GameObject _unfinishedBoard;
    [SerializeField] GameObject _finishedBoard;
    [SerializeField] Image _itemImage;
    [SerializeField] TextMeshProUGUI _itemValueTMP;
    [SerializeField] GameObject _claimButtonObj;

    public void SetContentState(bool isFinished)
    {
        if (isFinished)
        {
            _unfinishedBoard.SetActive(false);
            _finishedBoard.SetActive(true);
            //_itemValueTMP.color = _textColorArray[1];
            _claimButtonObj.SetActive(false);
        }
        else
        {
            _unfinishedBoard.SetActive(true);
            _finishedBoard.SetActive(false);
            //_itemValueTMP.color = _textColorArray[0];
            _claimButtonObj.SetActive(true);
            _claimButtonObj.GetComponent<Button>().interactable = _isReached;
        }
    }

    public void SetPassItem(Sprite iconSpirte, int itemValue, bool isReached, bool isFinished)
    {
        _isReached = isReached;
        if (iconSpirte != null)
            _itemImage.sprite = iconSpirte;
        _itemValueTMP.text = itemValue.ToString();
        SetContentState(isFinished);
    }

    public void EnableItem()
    {
        gameObject.SetActive(true);
    }

    public void DisableItem()
    {
        gameObject.SetActive(false);
    }

    public void OnTouchClaimButton()
    {
        if (!_isReached)
            return;
        SetContentState(true);
    }
}
