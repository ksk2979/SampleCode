using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class OptionManager : MonoBehaviour
{
    [SerializeField] GameObject _root;
    PlayerController _playerController;

    bool _isOpen = false;
    float _prevTimeScale = 1f;

    public void Init()
    {
        if (_root != null) { _root.SetActive(false); }
        _playerController = CharacterManager.GetInstance.GetPlayer;
    }

    private void Update()
    {
        if (_playerController != null)
        {
            // 플레이어의 인벤토리가 활성화가 안되어 있다면
            if (!_playerController.InventoryCheck)
            {
                if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame) //escapeKey 에디터에서 하면 마우스가 밖으로 나가져서 빌드할때 교체
                {
                    if (_isOpen) { Close(); }
                    else { Open(); }
                }
            }
        }
    }

    public void Open()
    {
        if (_isOpen) { return; }
        _isOpen = true;

        _prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        if (_root != null) { _root.SetActive(true); }

        UIManager.GetInstance.GetCursorManager.SetMode(ECursorMode.E_Option);
    }

    public void Close()
    {
        if (!_isOpen) { return; }
        _isOpen = false;

        Time.timeScale = _prevTimeScale;

        if (_root != null) { _root.SetActive(false); }

        UIManager.GetInstance.GetCursorManager.SetMode(ECursorMode.E_GamePlay);
    }

    public bool IsOpen { get { return _isOpen; } }
}
