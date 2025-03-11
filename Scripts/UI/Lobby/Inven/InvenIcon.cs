using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenIcon : MonoBehaviour
{
    InvenScript _invenScript;
    RectTransform _rectTrans;
    Button _plusButton;
    
    [SerializeField] EItemList _itemType;
    [SerializeField] UnitIcon _unitIcon;

    public void Init(InvenScript invenS)
    {
        _invenScript = invenS;
        _unitIcon.gameObject.SetActive(false);
        _rectTrans = transform.GetComponent<RectTransform>();
        if(_itemType != EItemList.BOAT)
        {
            _plusButton = transform.GetComponent<Button>();
            _plusButton.onClick.AddListener(OnTouchPlusButton);
        }
    }

    /// <summary>
    /// 버튼 상태 체크
    /// </summary>
    public void CheckButtonState()
    {
        if (_unitIcon == null || _itemType == EItemList.BOAT) return;

        if(_unitIcon.gameObject.activeSelf)
        {
            _plusButton.interactable = false;
        }
        else
        {
            _plusButton.interactable = true;
            _unitIcon.ChangePotential(new List<int>());
        }
    }

    /// <summary>
    /// 장착칸이 비어있을때 + 버튼 터치
    /// </summary>
    public void OnTouchPlusButton()
    {
        _invenScript.OnTouchCategoryButton((int)_itemType + 1);
    }
    public RectTransform GetRectTrans => _rectTrans;
    public UnitIcon GetUnitIcon => _unitIcon;
}
