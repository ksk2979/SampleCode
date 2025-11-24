using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using TMPro.EditorUtilities;
using System.Reflection;
using UnityEditor;

// 물건은 사라지지않고 물건이 변경되거나 상호작용 되는 것들
public class InteractionObjectScript : MonoBehaviour, IInteractable, IDayResettable
{
    [SerializeField] ItemData _item;
    [SerializeField] int _amount = 1;
    [SerializeField] bool _resetOnNewDay = true;
    [SerializeField] ItemInteractionData _interactionData;

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
        if (inventory == null) { return; }

        var slots = inventory.GetSnapshot();
        int currentSlot = player.CurrentSlot;
        if (currentSlot < 0 || currentSlot >= slots.Length) { return; }

        var heldItem = slots[currentSlot]._item;
        if (heldItem == null) { return; }

        InteractionResultType resultType;
        ItemData resultObj;
        ItemData resultHeld;

        if (!_interactionData.TryGetInteractionResult(_item, heldItem, out resultObj, out resultHeld, out resultType))
        {
            return;
        }

        if (heldItem._type == ItemType.Consumable)
        {
            inventory.RemoveAt(currentSlot, 1);
        }   

        if (resultType == InteractionResultType.WorldChange)
        {
            _item = resultObj;
            SpawnObject();
        }
        else if (resultType == InteractionResultType.WorldDestroy)
        {
            gameObject.SetActive(false);
        }
        else if (resultType == InteractionResultType.WorldChange_PlayerDelete)
        {
            inventory.RemoveAt(currentSlot, 1);

            _item = resultObj;
            SpawnObject();
        }
        else if (resultType == InteractionResultType.PlayerChange)
        {
            inventory.TryAddPreferSlot(currentSlot, resultHeld, 1);
        }
        else if (resultType == InteractionResultType.WorldDestroy_PlayerChange)
        {
            gameObject.SetActive(false);

            inventory.TryAddPreferSlot(currentSlot, resultHeld, 1);
        }
        else if (resultType == InteractionResultType.BothChange)
        {
            _item = resultObj;
            SpawnObject();

            inventory.TryAddPreferSlot(currentSlot, resultHeld, 1);
        }
    }

    public string GetInteractPrompt()
    {
        return $"{_item._itemName}을\n 확인";
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
