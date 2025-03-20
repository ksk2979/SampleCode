using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBarScript : MonoBehaviour
{
    Transform _target; // 적 캐릭터의 Transform
    RectTransform _myRect;
    public Image _healthBarFill; // 체력바 (UI)
    public TextMeshProUGUI _hpText;
    public Vector3 _offset = new Vector3(0, 1.5f, 0); // 체력바 오프셋

    Camera _mainCamera;

    bool _onLife = false;
    float _lifeTime = 0f;

    public void Init(Transform target)
    {
        if (_mainCamera == null) { _mainCamera = Camera.main; }
        if (transform.parent != UIManager.GetInstance.GetHpbarTrans)
        {
            transform.SetParent(UIManager.GetInstance.GetHpbarTrans);
            transform.localScale = Vector3.one;
        }
        if (_myRect == null) { _myRect = GetComponent<RectTransform>(); }
        _target = target;
    }

    void Update()
    {
        if (_target != null)
        {
            TargetUpdate();
        }
        if (_onLife)
        {
            _lifeTime -= Time.deltaTime;
            if (_lifeTime < 0f) { gameObject.SetActive(false); }
        }
    }

    public void UpdateHealthBar(float healthPercent, string text = "")
    {
        if (_hpText != null) { _hpText.text = text; }
        _healthBarFill.fillAmount = healthPercent; // 체력 비율 조정 (0~1)
    }
    public void OnLife()
    {
        _onLife = true;
    }
    public void BossLifeOff()
    {
        _onLife = false;
        gameObject.SetActive(true);
    }
    public void LifeTime()
    {
        if (!_onLife) { return; }
        gameObject.SetActive(true);
        _lifeTime = 2f;
    }
    public void TargetUpdate()
    {
        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(_target.position + _offset);
        screenPosition.z = 0;
        _myRect.anchoredPosition3D = screenPosition;
    }
}
