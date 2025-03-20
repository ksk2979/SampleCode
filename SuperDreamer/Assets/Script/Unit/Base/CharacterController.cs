using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    protected Transform _trans;
    [SerializeField] protected Animator _anim;
    protected AnimEventSender _animEventSender;
    protected bool _attack = false;
    protected bool _isDie = false;
    [SerializeField] protected UnitType _type = UnitType.NONE;
    protected Collider2D _collider2D;

    public virtual void OnStart()
    {
        if (_trans == null) { _trans = this.transform; }
        if (_anim != null) { _animEventSender = _anim.GetComponent<AnimEventSender>(); }
        if (_collider2D == null) { _collider2D = this.GetComponent<Collider2D>(); }
    }

    public Transform GetTrans { get { return _trans; } }
    public bool Attack { get { return _attack; } }
    public bool IsDie { get { return _isDie; } }
}
