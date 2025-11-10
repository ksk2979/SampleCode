using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Check : MonoBehaviour
{
    [Range(1, 100)]
    public int _fFont_Size;
    [Range(0, 1)]
    public float _red, _green, _blue;

    float _deltaTime = 0.0f;

    GUIStyle _style = new GUIStyle();
    Rect _rect;
    Color _color;

    public bool _changeUpdate = false;

    private void Start()
    {
        _fFont_Size = _fFont_Size == 0 ? 50 : _fFont_Size;
        int w = Screen.width, h = Screen.height;
        _rect = new Rect(0, 0, w, h * 2 / 100);
        _color = new Color(_red, _green, _blue, 1.0f);
    }

    private void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        if (_changeUpdate)
        {
            int w = Screen.width, h = Screen.height;
            _rect = new Rect(0, 0, w, h * 2 / 100);
            _color = new Color(_red, _green, _blue, 1.0f);
        }
    }

    private void OnGUI()
    {
        int h = Screen.height;
        
        _style.alignment = TextAnchor.UpperLeft;
        _style.fontSize = h * 2 / _fFont_Size;
        _style.normal.textColor = _color;
        float msec = _deltaTime * 1000.0f;
        float fps = 1.0f / _deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0}.fps)", msec, fps);
        GUI.Label(_rect, text, _style);

    }
}
