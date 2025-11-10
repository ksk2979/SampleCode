using UnityEngine;

public class InteractablePickup : MonoBehaviour
{
    [SerializeField] ItemData _item;
    [SerializeField] int _amount = 1;

    public bool TryPickup(Inventory to)
    {
        if (to == null || _item == null || _amount <= 0) return false;

        if (to.TryAdd(_item, _amount, out int left))
        {
            int picked = _amount - left;
            if (picked > 0)
            {
                // 남은 게 없으면 파괴
                if (left <= 0) Destroy(gameObject);
                else _amount = left; // 일부만 주웠으면 남은 양 업데이트
                return true;
            }
        }
        return false;
    }
}
