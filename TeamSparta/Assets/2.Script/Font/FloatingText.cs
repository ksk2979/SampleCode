using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

// ������ ��Ʈ�� ������Ʈ�� �����ؼ� �ð� ��ŭ ǥ���� ������� ���

public class FloatingText : MonoBehaviour
{
    [SerializeField] float _fadeDuration = 1.2f;
    [SerializeField] float _gravity = 9.8f;
    [SerializeField] float _initialForceMin = 1.5f;
    [SerializeField] float _initialForceMax = 3f;

    TextMeshProUGUI _text;
    Vector3 _velocity; // ���� �̵� �ӵ�
    float _timer;
    Transform _trans;

    public void Init(double damage, Transform parent)
    {
        if (_trans == null) { _trans = transform; }
        if (_trans.parent == null || _trans.parent != parent)
        {
            _trans.SetParent(parent);
            if (_text == null) { _text = GetComponent<TextMeshProUGUI>(); }
        }

        _trans.localPosition = Vector3.zero;
        _text.text = BigIntegerManager.ToCurrencyString(damage);

        _timer = 0f;

        // ���� �������� �ʱ� �ӵ� ���� (�ణ ���� ƨ���)
        float angle = Random.Range(60f, 120f); // ���� �߻簢 (���� �� ~ ���� ��)
        float force = Random.Range(_initialForceMin, _initialForceMax);
        float rad = angle * Mathf.Deg2Rad;
        _velocity = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * force;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        _velocity.y -= _gravity * Time.deltaTime;
        _trans.localPosition += _velocity * Time.deltaTime;

        float alpha = Mathf.Lerp(1f, 0f, _timer / _fadeDuration);
        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, alpha);

        if (_timer >= _fadeDuration)
        {
            SimplePool.Despawn(gameObject);
        }
    }
}

/*
 
[SerializeField] float _floatHeight = 1f;
    [SerializeField] float _bounceSpeed = 2f;
    [SerializeField] float _fadeDuration = 1f;
    TextMeshProUGUI _text;
    Vector3 _startPos;
    float _timer;

    public void Init(double damage, Transform parent)
    {
        if (transform.parent == null || transform.parent != parent)
        {
            transform.SetParent(parent);
            if (_text == null) { _text = GetComponent<TextMeshProUGUI>(); }
        }
        transform.localPosition = Vector3.zero;
        _text.text = BigIntegerManager.ToCurrencyString(damage);
        _startPos = transform.localPosition;
        _timer = 0f;
    }

    void Update()
    {
        _timer += Time.deltaTime;

        float bounce = Mathf.Sin(_timer * _bounceSpeed) * 0.2f;
        float rise = _floatHeight * (_timer / _fadeDuration);

        transform.localPosition = _startPos + new Vector3(0f, rise + bounce, 0f);

        // ���� ���̵�ƿ�
        float alpha = Mathf.Lerp(1f, 0f, _timer / _fadeDuration);
        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, alpha);

        if (_timer >= _fadeDuration)
            SimplePool.Despawn(gameObject);
    }

 */