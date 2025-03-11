using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageRotater : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    float _speed = 20f;
    bool _downCheck = false;
    float _xAngle = 0f;
    Transform _targetTrans;
    [SerializeField] InvenScript _inven;

    public void Init(Transform targetTrans)
    {
        _targetTrans = targetTrans;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _inven.OnTouchCategoryButton(1);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _xAngle = eventData.delta.x * Time.deltaTime * _speed;

        _targetTrans.Rotate(0, -_xAngle, 0, Space.World);
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
