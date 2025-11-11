using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] List<InteractablePickup> _itemList;

    private void Awake()
    {
        for (int i = 0; i < _itemList.Count; ++i)
        {
            _itemList[i].Init();
        }
    }
}
