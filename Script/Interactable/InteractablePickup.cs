using UnityEngine;
using System.Collections;

public class InteractablePickup : MonoBehaviour, IInteractable, IDayResettable
{
    [SerializeField] ItemData _item;
    [SerializeField] int _amount = 1;
    [SerializeField] bool _resetOnNewDay = true;

    DayCycleManager _dayCycleManager;

    int _initialAmount;

    GameObject _spawnObj; // 생성되는 오브젝트

    public void Init()
    {
        _initialAmount = _amount;
        _dayCycleManager = UIManager.GetInstance.GetDayCycleManager;

        if (_dayCycleManager != null)
        {
            _dayCycleManager.RegisterResettable(this);
        }

        SpawnObject();
    }

    void OnDestroy()
    {
        if (_dayCycleManager != null)
        {
            _dayCycleManager.UnregisterResettable(this);
        }
    }

    public bool TryPickup(Inventory to)
    {
        if (to == null || _item == null || _amount <= 0) return false;

        if (to.TryAdd(_item, _amount, out int left))
        {
            int picked = _amount - left;
            if (picked > 0)
            {
                if (left <= 0)
                {
                    _amount = 0;
                    if (_resetOnNewDay)
                    {
                        gameObject.SetActive(false);
                    }
                }
                else
                {
                    _amount = left;
                }

                    return true;
            }
        }
        return false;
    }

    void SpawnObject()
    {
        if (_spawnObj != null) { Destroy(_spawnObj); }

        if (_item != null && _item._holdPrefab != null)
        {
            _spawnObj = Instantiate(_item._holdPrefab, transform);
            _spawnObj.transform.localPosition = Vector3.zero;
            _spawnObj.transform.localRotation = Quaternion.identity;
        }
    }

    public void Interact(PlayerController player)
    {
        var inventory = player.GetComponent<Inventory>();
        if (inventory == null) { Debug.Log("플레이어 인벤토리 없음"); return; }

        if (TryPickup(inventory))
            Debug.Log($"{_item._itemName} 획득!");
        else
            Debug.Log("인벤토리 가득");
    }

    public string GetInteractPrompt()
    {
        return $"{_item._itemName}을\n습득한다";
    }

    public void ResetForNewDay()
    {
        if (!_resetOnNewDay) { return; }

        _amount = Mathf.Max(1, _initialAmount);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }
}
