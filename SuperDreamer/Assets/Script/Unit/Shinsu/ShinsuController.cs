using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShinsuController : MonoBehaviour
{
    public LayerMask _objectLayer;
    public LayerMask _tileLayer;
    Camera _mainCamera;
    bool _isDragging = false;
    Vector3 _targetPosition;
    [SerializeField] ShinsuAttack _attack;
    [SerializeField] Animator _anim;
    AnimEventSender _animEventSender;
    bool _attackCheck = false;

    public void OnStart()
    {
        _mainCamera = Camera.main;
        _targetPosition = transform.position;
        if (_anim != null)
        { 
            _animEventSender = _anim.GetComponent<AnimEventSender>();
            _animEventSender.AddEvent("AttackOn", AttackOn);
            _animEventSender.AddEvent("AttackOff", AttackOff);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckObjectClicked(); 
        }
        if (Input.GetMouseButtonUp(0))
        {
            ApplyMove();
        }

        if (_isDragging)
        {
            StoreTilePosition();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_attackCheck) { return; }
        if (collision.CompareTag(CommonStaticKey.TAG_ENEMY))
        {
            _attackCheck = true;
            _anim.SetTrigger(CommonStaticKey.ANIMPARAM_ATTACK);
        }
    }
    public void AttackOn()
    {
        _attack.SetActive(true);
    }
    public void AttackOff()
    {
        _attackCheck = false;
        _attack.SetActive(false);
    }

    void CheckObjectClicked()
    {
        Vector3 mousePos = Input.mousePosition;
        float zDepth = Mathf.Abs(_mainCamera.transform.position.z);

        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zDepth));
        Collider2D hit = Physics2D.OverlapPoint(worldPos, _objectLayer);

        if (hit != null)
        {
            _isDragging = true;
        }
    }
    void StoreTilePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        float zDepth = Mathf.Abs(_mainCamera.transform.position.z);

        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zDepth));
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos, _tileLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag(CommonStaticKey.TAG_TILE))
            {
                _targetPosition = hit.transform.position;
                return;
            }
        }
    }
    void ApplyMove()
    {
        if (_isDragging)
        {
            transform.position = _targetPosition;
            _isDragging = false;
        }
    }
}
