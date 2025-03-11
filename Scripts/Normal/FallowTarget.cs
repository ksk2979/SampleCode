using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallowTarget : MonoBehaviour
{
    private Transform _target;
    private Transform _trans;
    private void Start()
    {
        _trans = transform;
    }

    public void Init(Transform target)
    {
        this._target = target;
    }

    private void FixedUpdate()
    {
        _trans.position = _target.position + (Vector3.up * 2);
    }
}
