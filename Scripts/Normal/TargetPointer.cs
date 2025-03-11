using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPointer : MonoBehaviour
{
    public bool _isTargetOn
    {
        get { return _target != null; }
    }

    private MeshRenderer _meshRenderer = null;
    private Transform _trans = null;
    private Transform _target = null;

    // Start is called before the first frame update
    void Start()
    {
        _trans = transform;
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void LateUpdate()
    {
        if (_target == null)
            return;
        _trans.position = _target.position;
    }
    /// <summary>
    /// 타겟 및에 pontier둠
    /// </summary>
    /// <param name="target"></param>
    public void OnTarget(Transform target)
    {
        _meshRenderer.enabled = true;
        _target = target.GetComponent<EnemyController>().SetDieCall(OffTarget);
    }

    public void OffTarget()
    {
        _meshRenderer.enabled = false;
        _target = null;
    }


}
