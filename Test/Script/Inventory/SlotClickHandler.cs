using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotClickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int SlotIndex;
    InventoryUI _parent;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //Debug.Log($"PointerDown - 슬롯 {SlotIndex} 눌림");
            _parent.OnSlotPointerDown(SlotIndex);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //Debug.Log($"PointerUp - 슬롯 {SlotIndex}에서 손 뗌");
            _parent.OnSlotPointerUp(SlotIndex);
        }
    }

    public InventoryUI Parent { get { return _parent; } set { _parent = value; } }
}
