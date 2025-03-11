using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLine : Effect
{
    public AnimationCurve animationCurveHeight;
    public float duration = 1f;


    private LineRenderer lineRenderer;
    private LineRenderer _lineRenderer
    {
        get
        {
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
                lineRenderer.startWidth = defaultStartWidht;
                lineRenderer.endWidth = defaultEndWidht;
            }
            return lineRenderer;
        }
    }
    private CapsuleCollider capsuleCollider;
    private CapsuleCollider _capsuleCollider
    {
        get
        {
            if (capsuleCollider == null)
            {
                capsuleCollider = GetComponentInChildren<CapsuleCollider>();
                capsuleCollider.radius = defaultColliderRadius;
            }
            return capsuleCollider;
        }
    }
    private TriggerEnterAttack triggerEnter;
    private TriggerEnterAttack _triggerEnter
    {
        get
        {
            if (triggerEnter == null)
                triggerEnter = GetComponentInChildren<TriggerEnterAttack>();
            return triggerEnter;
        }
    }
    private float onTime = 0;
    private bool bOn = false;

    public float defaultStartWidht = 0.1f;
    public float defaultEndWidht = 0.1f;
    public float defaultColliderRadius = 0.1f;

    private Transform target;
    private Transform me;


    public void OnFireData(Vector3 from, Vector3 to, CharacterStats data, UnitDamageData tdd)
    {
        _triggerEnter.InitData(data, tdd);
        _lineRenderer.SetPosition(0, from);
        _lineRenderer.SetPosition(1, to);

        _capsuleCollider.transform.position = from;
        _capsuleCollider.transform.up = (to - from).normalized;
        _capsuleCollider.height = (to - from).magnitude;
        _capsuleCollider.center = Vector3.up * _capsuleCollider.height * 0.5f;
        onTime = Time.realtimeSinceStartup;
        bOn = true;
    }

    public void TestFire(Transform me, Transform target)
    {
        this.me = me;
        this.target = target;
        TestFire();
    }

    [ContextMenu("TestFire")]
    public void TestFire()
    {
        Vector3 from = me.position;
        Vector3 to = target.position;
        _lineRenderer.SetPosition(0, from);
        _lineRenderer.SetPosition(1, to);
        _capsuleCollider.transform.position = from;
        _capsuleCollider.transform.up = (to - from).normalized;
        _capsuleCollider.height = (to - from).magnitude;
        _capsuleCollider.center = Vector3.up * _capsuleCollider.height * 0.5f;

        onTime = Time.realtimeSinceStartup;
        bOn = true;
    }

    void Update()
    {
        if (bOn == false)
            return;

        var flowTime = Time.realtimeSinceStartup - onTime;
        var curveValue = animationCurveHeight.Evaluate(flowTime);

        lineRenderer.startWidth = defaultStartWidht * curveValue;
        lineRenderer.endWidth = defaultEndWidht * curveValue;
        capsuleCollider.radius = defaultColliderRadius * curveValue;

        if (duration <= flowTime)
        {
            bOn = false;
            DestroyThis();
        }
    }
}
