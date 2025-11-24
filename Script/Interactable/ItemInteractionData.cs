using UnityEngine;
using System;
using System.Collections.Generic;

public enum InteractionResultType
{
    [Tooltip("대상 교체")]
    WorldChange,          // 월드 아이템만 교체
    [Tooltip("대상 제거")]
    WorldDestroy,         // 월드 오브젝트 사라짐
    [Tooltip("대상 교체 / 손 제거")]
    WorldChange_PlayerDelete, // 월드 교체 + 손 아이템 제거
    [Tooltip("손 교체")]
    PlayerChange,         // 플레이어 held 아이템 교체
    [Tooltip("대상 제거 / 손 교체")]
    WorldDestroy_PlayerChange, // 월드 소멸 + 손 아이템 교체
    [Tooltip("대상, 손 둘다 교체")]
    BothChange,           // 월드, 플레이어 둘 다 교체
    None,                 // 아무 변화 없음
}

[CreateAssetMenu(menuName = "Game/ItemInteractionData")]
public class ItemInteractionData : ScriptableObject
{
    [Serializable]
    public struct InteractionEntry
    {
        public ItemData _objectItem; // 상호작용 되는 월드 아이템
        public ItemData _heldItem; // 플레이어가 들고 있는 아이템
        [Header("상호작용 대상 아이템 교체")]
        public ItemData _resultObject; // 상호작용 후 월드 아이템이 바뀌는 경우
        [Header("플레이어 손 아이템 교체")]
        public ItemData _resultHeld; // 상호작용 후 플레이어 아이템이 바뀌는 경우
        [Header("어떤 타입으로 교체되는가")]
        public InteractionResultType _resultType;
    }

    [SerializeField] List<InteractionEntry> _entries = new List<InteractionEntry>();

    public bool TryGetInteractionResult(ItemData objectItem, ItemData heldItem, out ItemData resultObject, out ItemData resultHeld, out InteractionResultType type)
    {
        type = InteractionResultType.None;
        resultObject = null;
        resultHeld = null;
        foreach (var e in _entries)
        {
            if (e._objectItem == objectItem && e._heldItem == heldItem)
            {
                type = e._resultType;
                resultObject = e._resultObject;
                resultHeld = e._resultHeld;
                return true;
            }
        }

        return false;
    }
}