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
/// Stack�� ����Ͽ� ���ư(Escape) �Է½� Stack �ֻ�ܿ� �ִ� �˾����� ����
/// PopupBase�� BackClosable�� Stack ���Կ��� üũ
/// </summary>
public class PopupController : MonoBehaviour
{
    [SerializeField] PopupBase[] _popupArr;
    [SerializeField] List<PopupType> _openedPopupList = new List<PopupType>();
    Stack<PopupBase> _popupStack = new Stack<PopupBase>();
    Dictionary<PopupType, PopupBase> _popupDictionary = new Dictionary<PopupType, PopupBase>();

    public void Init(Action initCallback = null)
    {
        // �˾� ���
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
    /// �˾������� �˾� ����ϴ� ���
    /// </summary>
    /// <param name="pType">�˾� Ÿ��</param>
    /// <param name="pBase">�˾� ���̽�</param>
    void AddPopup(PopupType pType, PopupBase pBase)
    {
        if (pType == PopupType.NONE) return;
        if (!_popupDictionary.ContainsKey(pType))
        {
            _popupDictionary.Add(pType, pBase);
        }
    }

    /// <summary>
    /// �˾��������� �˾��� �������� ���
    /// �ҷ����� ��ġ���� �ش� �������� ��ȯ�ؼ� ���(as ConfirmPopup ��)
    /// </summary>
    /// <param name="popupType">�˾� Ÿ��</param>
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
    /// ��� �˾��� ���� ��Ű�� ���
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
    /// Stack�� ���
    /// </summary>
    /// <param name="popup"></param>
    public void PushPopup(PopupBase popup)
    {
        if (IsContainsPopup(popup)) { return; }
        _popupStack.Push(popup);
    }

    /// <summary>
    /// Stack���� ����
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
    /// Stack�� ���ԵǾ��ִ��� Ȯ��
    /// </summary>
    /// <param name="popup"></param>
    /// <returns></returns>
    bool IsContainsPopup(PopupBase popup)
    {
        return _popupStack.Contains(popup);
    }

    /// <summary>
    /// �� ���� �ִ� �˾� ��ȸ
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
    /// �˾��� �����ִ� ��쿡 ����Ʈ ��� (Ÿ�� ���)
    /// </summary>
    public void AddOpenedPopup(PopupType type) => _openedPopupList.Add(type);

    /// <summary>
    /// �˾��� ���� ��� ����Ʈ���� ����
    /// </summary>
    public void RemoveOpendPopup(PopupType type) => _openedPopupList.Remove(type);

    /// <summary>
    /// �˾��� �����ִ��� Ȯ��
    /// </summary>
    /// <returns></returns>
    public bool IsExistOpenedPopup() => _openedPopupList.Count > 0;
    #endregion Open Check
}
