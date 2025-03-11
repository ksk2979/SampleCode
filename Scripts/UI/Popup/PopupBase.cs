using System.Collections;
using System.Collections.Generic;
//using TMPro.EditorUtilities;
using UnityEngine;

public class PopupBase : MonoBehaviour
{
    public delegate void PopupEventDelegate();
    public event PopupEventDelegate OnOpenEventListener;
    public event PopupEventDelegate OnCloseEventListener;

    [SerializeField] protected PopupController popupController;
    [SerializeField] bool _backClosable = false;
    [SerializeField] PopupType _popupType;
    protected RectTransform rectTrans;

    protected bool _buttonTouched = false;
    public bool BackClosable
    {
        get => _backClosable;
        set => _backClosable = value;
    }
    public bool IsOpened => gameObject.activeSelf;

    public PopupType GetPopupType => _popupType;

    public virtual void OpenPopup()
    {
        if (rectTrans == null) rectTrans = transform.GetComponent<RectTransform>();
        if (popupController == null) popupController = transform.parent.GetComponent<PopupController>();
        rectTrans.SetAsLastSibling();
        PushPopup();
        OnOpenEventListener?.Invoke();
        OnOpenEventListener = null;
        popupController.AddOpenedPopup(_popupType);
        gameObject.SetActive(true);
    }

    public virtual void ClosePopup()
    {
        if (popupController.PeekPopupStack() == this)
        {
            popupController.RemovePopup();
        }
        OnCloseEventListener?.Invoke();
        OnCloseEventListener = null;
        /*        if (GameManager.GetInstance._nowScene == EScene.E_LOBBY)
                {
                    LobbyUIManager.GetInstance.GetRegisteredAction()?.Invoke();
                }
                else if (GameManager.GetInstance._nowScene == EScene.E_GAME)
                {

                }*/
        popupController.RemoveOpendPopup(_popupType);
        gameObject.SetActive(false);
    }

    void PushPopup()
    {
        popupController.PushPopup(this);
        if (_backClosable) { popupController.PushPopup(this); }
    }
}
