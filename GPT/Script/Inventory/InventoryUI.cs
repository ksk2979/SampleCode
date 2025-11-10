using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Action OnInventoryChanged;

    [SerializeField] Inventory _inventory;
    [SerializeField] HetKeyScript[] _hetKeySlot;
    [SerializeField] Sprite _emptyIcon;

    [SerializeField] Canvas _canvas;
    [SerializeField] Transform _instantiateTarget; // 미리보기 이미지 생성 타겟
    [SerializeField] Image _draggingIconPrefab; // 드래그 시 표시용 프리팹
    [SerializeField] GameObject _inventoryUIRoot; // 인벤 활성화시 배경 가림용

    Image _draggingIcon; // 드래그 중 아이콘 인스턴스
    int _draggingFrom = -1; // 드래그 시작 슬롯 인덱스

    [Header("Crafting UI")]
    [SerializeField] Button _craftButton;                     // 조합 버튼
    [SerializeField] TextMeshProUGUI _craftButtonLabel;       // 버튼 라벨 텍스트
    [SerializeField] CraftingRecipeData _craftingData;
    [SerializeField] FadeInOutScript _warningText;

    bool _isCraftMode = false;

    public void Init()
    {
        _inventory = CharacterManager.GetInstance.GetPlayer.GetComponent<Inventory>();

        for (int i = 0; i < _hetKeySlot.Length; i++)
        {
            _hetKeySlot[i].Index = i;

            var handler = _hetKeySlot[i]._icon.GetComponent<SlotClickHandler>();
            if (handler == null)
                handler = _hetKeySlot[i]._icon.gameObject.AddComponent<SlotClickHandler>();

            handler.SlotIndex = i;
            handler.Parent = this;
        }

        if (_craftButton != null)
            _craftButton.onClick.AddListener(ToggleCraftMode);

        UpdateCraftButtonLabel();

        Refresh();
    }

    void Update()
    {
        // 드래그 중이면 마우스 따라다니게
        if (_draggingIcon != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                Input.mousePosition,
                _canvas.worldCamera,
                out Vector2 pos);
            _draggingIcon.rectTransform.anchoredPosition = pos;

            // 마우스 버튼 놓을 때 드롭 처리
            if (Input.GetMouseButtonUp(0))
                TryDropToSlotUnderMouse();
        }
    }

    void ToggleCraftMode()
    {
        _isCraftMode = !_isCraftMode;
        UpdateCraftButtonLabel();
    }

    void UpdateCraftButtonLabel()
    {
        if (_craftButtonLabel != null)
            _craftButtonLabel.text = _isCraftMode ? "조합 중" : "조합 시작";
    }

    public void Refresh()
    {
        var slots = _inventory.GetSnapshot();
        for (int i = 0; i < _hetKeySlot.Length; i++)
        {
            var ui = _hetKeySlot[i];
            if (i < slots.Length && slots[i]._item != null)
            {
                ui._icon.sprite = slots[i]._item._icon != null ? slots[i]._item._icon : _emptyIcon;
                ui._icon.enabled = true;
                ui._countText.text = slots[i]._item._stackable ? slots[i]._count.ToString() : "";
            }
            else
            {
                ui._icon.sprite = _emptyIcon;
                ui._icon.enabled = _emptyIcon != null;
                ui._countText.text = "";
            }
        }
    }
    public void OnSlotPointerDown(int index)
    {
        var slots = _inventory.GetSnapshot();
        var slot = slots[index];

        //Debug.Log($"PointerDown - 슬롯 {index} 클릭됨");

        if (slot._item == null) return;

        if (_draggingIcon == null)
        {
            _draggingFrom = index;
            _draggingIcon = Instantiate(_draggingIconPrefab, _instantiateTarget);
            _draggingIcon.gameObject.SetActive(true);
            _draggingIcon.sprite = slot._item._icon;
            _draggingIcon.SetNativeSize();
            _draggingIcon.raycastTarget = false;
            //Debug.Log($"드래그 시작 - 슬롯 {index} ({slot._item._itemName})");
        }
    }

    public void OnSlotPointerUp(int index)
    {
        // 마우스 떼면 드롭 처리
        TryDropToSlotUnderMouse();
    }

    void TryDropToSlotUnderMouse()
    {
        if (_draggingIcon == null) return;
    
        //Debug.Log($"TryDropToSlotUnderMouse - 드래그 중이던 슬롯: {_draggingFrom}");
    
        // 커서 아래 UI 탐색
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);
    
        int targetSlot = -1;
        foreach (var r in results)
        {
            for (int i = 0; i < _hetKeySlot.Length; i++)
            {
                if (r.gameObject == _hetKeySlot[i]._icon.gameObject)
                {
                    targetSlot = i;
                    break;
                }
            }
        }
    
        if (targetSlot == -1)
        {
            //Debug.Log("드롭 실패 - 슬롯 UI를 찾지 못함");
            _draggingFrom = -1;
            Destroy(_draggingIcon.gameObject);
            _draggingIcon = null;
            return;
        }

        if (_isCraftMode)
        {
            bool crafted = TryCraft(_draggingFrom, targetSlot);
            // 조합 성공 또는 실패 모두 드래그는 종료
            _draggingFrom = -1;
            Destroy(_draggingIcon.gameObject);
            _draggingIcon = null;

            if (crafted)
            {
                Refresh();
                OnInventoryChanged?.Invoke();
            }
            return;
        }
        else
        {
            // 기존 스왑 동작
            SwapSlots(_draggingFrom, targetSlot);
            _draggingFrom = -1;
            Destroy(_draggingIcon.gameObject);
            _draggingIcon = null;

            Refresh();
            OnInventoryChanged?.Invoke();
        }

        //Debug.Log($"드롭 감지 - 슬롯 {targetSlot} ({_hetKeySlot[targetSlot]._icon.gameObject.name})로 드롭됨");

        // 아이템 스왑
        //SwapSlots(_draggingFrom, targetSlot);
        //_draggingFrom = -1;
        //Destroy(_draggingIcon.gameObject);
        //_draggingIcon = null;
        //Refresh();
        //
        //OnInventoryChanged?.Invoke();
    }

    void SwapSlots(int from, int to)
    {
        var slots = _inventory.GetSnapshot();
        (slots[to], slots[from]) = (slots[from], slots[to]);
    }
    // 조합 시도하기
    bool TryCraft(int from, int to)
    {
        if (_craftingData == null || _inventory == null) { return false; }
        if (from == to) { return false; }

        var slots = _inventory.GetSnapshot();
        if (from < 0 || from >= slots.Length) { return false; }
        if (to < 0 || to >= slots.Length) { return false; }

        var a = slots[from];
        var b = slots[to];
        if (a._item == null || b._item == null) { return false;}

        if (!_craftingData.TryFindResult(a._item, b._item, out ItemData result, out int resultCount))
        {
            //Debug.Log($"매칭 실패: {a._item._id} + {b._item._id}");
            _warningText.FadeOut(1f);
            return false;
        }

        // 같은 슬롯끼리 조합은 위에서 배제. 같은 아이템을 두 개 소모해야 하는 규칙이라면 수량 체크 필요
        if (a._item == b._item)
        {
            if (a._count + b._count < 2) { return false; }
            if (a._count <= 0 || b._count <= 0) { return false; }
        }
        else
        {
            if (a._count <= 0 || b._count <= 0) { return false; }
        }

        // 각 1개씩 제거
        bool ra = _inventory.RemoveAt(from, 1);
        bool rb = _inventory.RemoveAt(to, 1);
        if (!ra || !rb) { return false; }

        // 결과를 to 슬롯에 우선 배치, 남으면 일반 Add
        bool placed = _inventory.TryAddPreferSlot(to, result, resultCount);
        return placed;
    }

    public void InventoryRootActive(bool active)
    {
        // 조합중이였을 경우 풀어주기
        if (_isCraftMode) { ToggleCraftMode(); }
        _inventoryUIRoot.SetActive(active);
    }
}
