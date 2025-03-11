using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PageType
{
    STAGE = 0,
    SHOP = 1,
    INVEN = 2,
    READY = 3,
    ITEMEDITOR = 4,
}

public class PageBase : MonoBehaviour
{
    protected UserData userData;
    protected LobbyUIManager uiManager;

    public virtual void OpenPage()
    {
        gameObject.SetActive(true);
    }

    public virtual void ClosePage()
    {
        gameObject.SetActive(false);
    }

    public virtual void Init(LobbyUIManager uiM)
    {
        uiManager = uiM;
        userData = UserData.GetInstance;
    }
}
