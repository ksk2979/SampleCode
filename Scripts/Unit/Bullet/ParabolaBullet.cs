using MyData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaBullet : MonoBehaviour
{
    #region !!==Property
    private Transform _trans
    {
        get
        {
            if (trans == null)
                trans = GetComponent<Transform>();
            return trans;
        }
    }
    #endregion

    #region !!==Public Field
    public int bodyRotateSpeed = 10;
    public float height;
    public float duration;
    public float minDuration = 0.5f;
    public float minHeight = 3f;
    public Transform body;
    public string _hitEfx;
    #endregion

    #region !!==Private Field
    private Transform trans;
    private float _flowTime = 0;
    private bool trigger = false;
    private Vector3 startPos, endPos = Vector3.zero;
    private float _destoryTime;
    private WeaponData data;
    private float correctDuration = 0;
    private float correctHeight = 0;
    private PlayerStats _playerStats;
    #endregion

    #region !!== Public method
    public void SetData(Vector3 startPos, Vector3 endPos, WeaponData data, PlayerStats _playerStats)
    {
        Reset();
        this._playerStats = _playerStats;
        this.data = data;
        this.startPos = startPos;
        //endPos를 거리에 반영해야함
        float targetDistance = Vector3.Distance(startPos, endPos);
        this.endPos = startPos + _trans.forward * targetDistance;

        //var halfPoint = (Vector3.Lerp(startPos, endPos, 0.5f) + Vector3.up * height);
        //_trans.forward = (halfPoint - startPos).normalized;
        _trans.position = this.startPos;
        correctDuration = Mathf.Clamp(duration * Vector3.Distance(startPos, endPos) / this.data.shootingRange, minDuration, duration);
        correctHeight = Mathf.Clamp(height * Vector3.Distance(startPos, endPos) / this.data.shootingRange, minHeight, height);
        TriggerrOn();
    }

    public void Destroy()
    {
        // 마지막 이팩트 생성 구간
        var bullet = SimplePool.Spawn(CommonStaticDatas.RES_EFFECT, _hitEfx, _trans.position, Quaternion.identity);
        bullet.GetComponent<Explosion>().InitData(_playerStats, _playerStats.GetTDD1());
        Reset();
        SimplePool.Despawn(gameObject);
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
    #endregion


    #region !!== Monobehaviour callback
    private void Update()
    {
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
        if (trigger == false)
            return;

        _trans.position = MathParabola.Parabola(startPos, endPos + (Vector3.down * 0.5f), correctHeight, _flowTime / correctDuration);

        var halfPoint = (Vector3.Lerp(startPos, endPos, 0.5f) + Vector3.up * correctHeight);
        if (_flowTime < correctDuration * 0.5f)
            body.rotation = Quaternion.Slerp(body.rotation, Quaternion.LookRotation(halfPoint - startPos), Time.deltaTime * bodyRotateSpeed);
        else
            body.rotation = Quaternion.Slerp(body.rotation, Quaternion.LookRotation(endPos - halfPoint), Time.deltaTime * bodyRotateSpeed);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (CommonFuncUnit.IsTargetCompareTag(other.gameObject, new string[] { CommonStaticDatas.TAG_ENEMY }))
    //    {
    //        other.GetComponent<Interactable>().TakeToDamage(_playerStats.GetTDD1());
    //        return;
    //    }
    //}
    #endregion


}
