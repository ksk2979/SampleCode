using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 보스5의 곡사 공격할때 씀

public class BossParabolaAttack : BaseAttack
{
    private Transform _trans
    {
        get
        {
            if (trans == null)
                trans = GetComponent<Transform>();
            return trans;
        }
    }

    public int bodyRotateSpeed = 10;
    public float height;
    public float duration;
    public float minDuration = 0.5f;
    public float minHeight = 3f;
    public Transform body;
    public string _hitEfx;

    private Transform trans;
    private float _flowTime = 0;
    private bool trigger = false;
    private Vector3 startPos, endPos = Vector3.zero;
    private float _destoryTime;
    private float correctDuration = 0;
    private float correctHeight = 0;

    public void SetData(Vector3 startPos, Vector3 endPos)
    {
        Reset();
        this.startPos = startPos;
        this.endPos = endPos;
        var halfPoint = (Vector3.Lerp(startPos, endPos, 0.5f) + Vector3.up * height);
        _trans.forward = (halfPoint - startPos).normalized;
        _trans.position = this.startPos;
        correctDuration = Mathf.Clamp(duration * Vector3.Distance(startPos, endPos) / 10f, minDuration, duration); //10f <- this.data.shootingRange
        correctHeight = Mathf.Clamp(height * Vector3.Distance(startPos, endPos) / 10f, minHeight, height);
        TriggerrOn();
    }

    public void Destroy()
    {
        // 마지막 이팩트 생성 구간
        StartCoroutine(DelaySpawn());
        _notUpdate = true;
        //Reset();
        //SimplePool.Despawn(gameObject);
    }
    IEnumerator DelaySpawn()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.2f);

        var bullet = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _hitEfx, _trans.position, Quaternion.identity);
        BossRadiusUpAttack attack = bullet.GetComponent<BossRadiusUpAttack>();
        attack.InitData(stats, tdd);
        attack.DownSpeed = true;
        attack.DownTime = 0.3f;
    }

    [ContextMenu("TriggerrOn")]
    public void TriggerrOn()
    {
        trigger = true;
    }
    [ContextMenu("Reset")]
    public void Reset()
    {
        trigger = false;
        _flowTime = 0;
        _destoryTime = 0;
        _trans.position = startPos;
    }
    private void OnDisable()
    {
        _notUpdate = false;
    }
    bool _notUpdate = false;
    
    private void Update()
    {
        if (_notUpdate) { return; }
        if (trigger == false)
            return;
        _destoryTime += Time.deltaTime;
        _flowTime += Time.deltaTime;
        _flowTime = _flowTime % correctDuration;

        if (correctDuration < _destoryTime)
            Destroy();
    }
    void FixedUpdate()
    {
        if (_notUpdate) { return; }
        if (trigger == false)
            return;

        _trans.position = MathParabola.Parabola(startPos, endPos + (Vector3.down * 0.5f), correctHeight, _flowTime / correctDuration);

        var halfPoint = (Vector3.Lerp(startPos, endPos, 0.5f) + Vector3.up * correctHeight);
        if (_flowTime < correctDuration * 0.5f)
            body.rotation = Quaternion.Slerp(body.rotation, Quaternion.LookRotation(halfPoint - startPos), Time.deltaTime * bodyRotateSpeed);
        else
            body.rotation = Quaternion.Slerp(body.rotation, Quaternion.LookRotation(endPos - halfPoint), Time.deltaTime * bodyRotateSpeed);
    }
}
