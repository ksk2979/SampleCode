using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] List<InteractablePickup> _pickupItemList;
    [SerializeField] List<InteractionObjectScript> _interactionItemList;

    private void Start()
    {
        for (int i = 0; i < _pickupItemList.Count; ++i)
        {
            _pickupItemList[i].Init();
        }
        for (int i = 0; i < _interactionItemList.Count; ++i)
        {
            _interactionItemList[i].Init();
        }
    }
}
