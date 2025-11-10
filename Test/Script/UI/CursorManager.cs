using UnityEngine;

public enum ECursorMode { E_GamePlay, E_Inventory, E_Option }
public class CursorManager : MonoBehaviour
{
    ECursorMode _current = ECursorMode.E_GamePlay;

    public void Init()
    {
        Apply();
    }

    public void SetMode(ECursorMode mode)
    {
        _current = mode;
        Apply();
    }
    public void Apply()
    {
        switch (_current)
        {
            case ECursorMode.E_GamePlay:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case ECursorMode.E_Inventory:
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                break;
            case ECursorMode.E_Option:
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                break;
        }
    }

    //private void OnApplicationFocus(bool focus)
    //{
    //    if (focus)
    //    {
    //        Apply();
    //    }
    //}
}
