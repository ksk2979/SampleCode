using UnityEngine;

[System.Serializable]
public struct ItemStack
{
    public ItemData _item;
    public int _count;
}

public class Inventory : MonoBehaviour
{
    [SerializeField] ItemStack[] _slots = new ItemStack[6];

    public ItemStack[] GetSnapshot() => _slots;

    public bool TryAdd(ItemData item, int amount, out int leftover)
    {
        leftover = amount;
        if (item == null || amount <= 0) return false;

        // 같은 아이템 스택 채우기
        if (item._stackable)
        {
            for (int i = 0; i < _slots.Length && leftover > 0; i++)
            {
                if (_slots[i]._item == item && _slots[i]._count < item._maxStack)
                {
                    int canPut = Mathf.Min(item._maxStack - _slots[i]._count, leftover);
                    _slots[i]._count += canPut;
                    leftover -= canPut;
                }
            }
        }

        // 빈 칸에 넣기
        for (int i = 0; i < _slots.Length && leftover > 0; i++)
        {
            if (_slots[i]._item == null)
            {
                int put = item._stackable ? Mathf.Min(item._maxStack, leftover) : 1;
                _slots[i]._item = item;
                _slots[i]._count = put;
                leftover -= put;
            }
        }

        return leftover < amount;
    }

    // 지정 슬롯에서 지정 수량 제거. 수량이 0이면 아이템을 null로 만든다.
    public bool RemoveAt(int slotIndex, int amount)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Length) return false;
        if (_slots[slotIndex]._item == null || amount <= 0) return false;
        if (_slots[slotIndex]._count < amount) return false;

        _slots[slotIndex]._count -= amount;
        if (_slots[slotIndex]._count <= 0)
        {
            _slots[slotIndex]._item = null;
            _slots[slotIndex]._count = 0;
        }
        return true;
    }

    // preferSlot이 비었거나 같은 아이템 스택 가능하면 그 슬롯부터 시도하고, 남으면 일반 TryAdd로 분산한다.
    public bool TryAddPreferSlot(int preferSlot, ItemData item, int amount)
    {
        if (item == null || amount <= 0) return false;
        int leftover = amount;

        if (preferSlot >= 0 && preferSlot < _slots.Length)
        {
            var s = _slots[preferSlot];
            if (s._item == null)
            {
                int put = item._stackable ? Mathf.Min(item._maxStack, leftover) : 1;
                _slots[preferSlot]._item = item;
                _slots[preferSlot]._count = put;
                leftover -= put;
            }
            else if (s._item == item && item._stackable && s._count < item._maxStack)
            {
                int canPut = Mathf.Min(item._maxStack - s._count, leftover);
                _slots[preferSlot]._count += canPut;
                leftover -= canPut;
            }
        }

        if (leftover > 0)
        {
            int after;
            TryAdd(item, leftover, out after);
            leftover = after;
        }

        return leftover == 0;
    }

    public void ClearAll()
    {
        for (int i = 0; i < _slots.Length; ++i)
        {
            _slots[i]._item = null;
            _slots[i]._count = 0;
        }
    }
}
