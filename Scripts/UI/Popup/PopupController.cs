using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PopupType
{
    NONE,
    PAUSE,
    OPTION,
    RESULT,
    REVIVAL,
    ABILITY,
    ITEM,
    ENERGY,
    DAILYREWARD,
    COLLECTION,
    ASK,
    BOXOPEN,
    SYNTHESIS,
    RETRY,
    QUEST,
    ROULETTE,
    REWARDRESULT,
    ACTIVESKILL,
    EXPLOSION,
    POTENTIAL,
}

/// <summary>
/// Stack을 사용하여 백버튼(Escape) 입력시 Stack 최상단에 있는 팝업부터 종료
/// PopupBase에 BackClosable로 Stack 삽입여부 체크
/// </summary>
public class PopupController : MonoBehaviour
{
    [SerializeField] PopupBase[] _popupArr;
    [SerializeField] List<PopupType> _openedPopupList = new List<PopupType>();
    Stack<PopupBase> _popupStack = new Stack<PopupBase>();
    Dictionary<PopupType, PopupBase> _popupDictionary = new Dictionary<PopupType, PopupBase>();

    public void Init(Action initCallback = null)
    {
        // 팝업 등록
        if (_popupArr.Length > 0)
        {
            for (int i = 0; i < _popupArr.Length; i++)
            {
                AddPopup(_popupArr[i].GetPopupType, _popupArr[i]);
                _popupArr[i].gameObject.SetActive(false);
            }
        }
        initCallback?.Invoke();
    }
    /// <summary>
    /// 팝업사전에 팝업 등록하는 기능
    /// </summary>
    /// <param name="pType">팝업 타입</param>
    /// <param name="pBase">팝업 베이스</param>
    void AddPopup(PopupType pType, PopupBase pBase)
    {
        if (pType == PopupType.NONE) return;
        if (!_popupDictionary.ContainsKey(pType))
        {
            _popupDictionary.Add(pType, pBase);
        }
    }

    /// <summary>
    /// 팝업사전에서 팝업을 가져오는 기능
    /// 불러오는 위치에서 해당 형식으로 변환해서 사용(as ConfirmPopup 등)
    /// </summary>
    /// <param name="popupType">팝업 타입</param>
    /// <returns></returns>
    PopupBase GetPopup(PopupType popupType)
    {
        if (_popupDictionary.ContainsKey(popupType))
        {
            return _popupDictionary[popupType];
        }
        return null;
    }
    public T GetPopup<T>(PopupType type)
    {
        var popup = GetPopup(type);
        if (popup != null)
        {
            return (T)Convert.ChangeType(popup, typeof(T));
        }
        else
        {
            return default(T);
        }
    }
    /// <summary>
    /// 모든 팝업을 종료 시키는 기능
    /// </summary>
    public void CloseAllPopup()
    {
        foreach (var popup in _popupDictionary)
        {
            popup.Value.ClosePopup();
        }
    }

    #region Stack
    /// <summary>
    /// Stack에 등록
    /// </summary>
    /// <param name="popup"></param>
    public void PushPopup(PopupBase popup)
    {
        if (IsContainsPopup(popup)) { return; }
        _popupStack.Push(popup);
    }

    /// <summary>
    /// Stack에서 제거
    /// </summary>
    /// <param name="popup"></param>
    public bool RemovePopup()
    {
        PopupBase popup = null;
        if (_popupStack.TryPop(out popup))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Stack에 포함되어있는지 확인
    /// </summary>
    /// <param name="popup"></param>
    /// <returns></returns>
    bool IsContainsPopup(PopupBase popup)
    {
        return _popupStack.Contains(popup);
    }

    /// <summary>
    /// 맨 위에 있는 팝업 조회
    /// </summary>
    /// <returns></returns>
    public PopupBase PeekPopupStack()
    {
        PopupBase popup = null;
        if (_popupStack.TryPeek(out popup))
        {
            return popup;
        }
        else
        {
            return null;
        }
    }
    #endregion Stack

    #region Open Check
    
    /// <summary>
    /// 팝업이 열려있는 경우에 리스트 등록 (타입 등록)
    /// </summary>
    public void AddOpenedPopup(PopupType type) => _openedPopupList.Add(type);

    /// <summary>
    /// 팝업을 닫을 경우 리스트에서 제거
    /// </summary>
    public void RemoveOpendPopup(PopupType type) => _openedPopupList.Remove(type);

    /// <summary>
    /// 팝업이 열려있는지 확인
    /// </summary>
    /// <returns></returns>
    public bool IsExistOpenedPopup() => _openedPopupList.Count > 0;
    #endregion Open Check
}
