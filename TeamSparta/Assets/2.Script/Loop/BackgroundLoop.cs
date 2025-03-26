using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    [SerializeField] Transform[] _background;
    [SerializeField] float _speed = 2f;
    [SerializeField] float _resetPositionX = -40f; // -37.9

    float _backgroundWidth = 0f;
    int _backgroundLength = 0;
    bool _stop = false;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _backgroundLength = _background.Length;
        _backgroundWidth = Mathf.Abs(_background[1].position.x - _background[0].position.x);
    }

    private void Update()
    {
        if (_stop) { return; }
        for (int i = 0; i < _backgroundLength; ++i)
        {
            Transform bg = _background[i];
            bg.position += Vector3.left * _speed * Time.deltaTime;
            if (bg.position.x <= _resetPositionX)
            {
                float x = GetRigthmostX() + _backgroundWidth;
                bg.position = new Vector3(x, bg.position.y, bg.position.z);
            }
        }
    }

    float GetRigthmostX()
    {
        float maxX = float.MinValue;
        for (int i = 0; i < _background.Length; ++i)
        {
            float x = _background[i].position.x;
            if (x > maxX)
            {
                maxX = x;
            }
        }
        return maxX;
    }

    public void AnimaController(bool active)
    {
        _stop = active;
    }
}
