using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckAnima : MonoBehaviour
{
    [SerializeField] Transform[] _tireTrans;
    [SerializeField] float _speed = 2f;
    bool _stop = false;

    private void Update()
    {
        if (_stop) { return; }
        for (int i = 0; i < _tireTrans.Length; ++i)
        {
            _tireTrans[i].Rotate(0f, 0f, -360f * (_speed * Time.deltaTime));
        }
    }

    public void AnimaController(bool active)
    {
        _stop = active;
    }
}
