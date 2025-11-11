using UnityEngine;
using System.Collections;

public class InteractablePickup : MonoBehaviour, IInteractable, IDayResettable
{
    [SerializeField] ItemData _item;
    [SerializeField] int _amount = 1;
    [SerializeField] bool _resetOnNewDay = true;

    DayCycleManager _dayCycleManager;

    int _initialAmount;
    Coroutine _registerRoutine;

    public void Init()
    {
        _initialAmount = _amount;
        _dayCycleManager = UIManager.GetInstance.GetDayCycleManager;

        if (_registerRoutine != null)
        {
            StopCoroutine(_registerRoutine);
            _registerRoutine = null;
        }

        if (_resetOnNewDay)
        {
            _registerRoutine = StartCoroutine(RegisterWhenReady());
        }
    }

    void OnDisable() 
    {
        if (_registerRoutine != null)
        {
            StopCoroutine(_registerRoutine);
            _registerRoutine = null;
        }
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
                // 남은 게 없으면 파괴
                //if (left <= 0) Destroy(gameObject);
                //else _amount = left; // 일부만 주웠으면 남은 양 업데이트

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
                    {
                        _amount = left;
                    }
                }

                    return true;
            }
        }
        return false;
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

    IEnumerator RegisterWhenReady()
    {
        while (_dayCycleManager == null)
        {
            yield return null;
        }

        _dayCycleManager.RegisterResettable(this);
        _registerRoutine = null;
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
