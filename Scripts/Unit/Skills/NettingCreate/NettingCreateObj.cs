using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NettingCreateObj : MonoBehaviour
{
    Transform _trans;
    Vector3 _target;

    NettingCreateManager _nettingCreateManager;
    PlayerAbility _playerAbility;

    float _lifeTime = 3f;

    [SerializeField] Animator _anima;
    bool _onAnima = false;

    public void Init(PlayerAbility ability, NettingCreateManager manager)
    {
        _trans = this.transform;
        _playerAbility = ability;
        _nettingCreateManager = manager;
    }
    public void StartAbility(Transform target)
    {
        _trans.rotation = target.rotation;
        _trans.position = target.position;
        _trans.Translate(Vector3.back * 5f);

        _target = _trans.position;
        _trans.position = target.position;
        _trans.Translate(Vector3.back * 2.5f);
        _lifeTime = 3f;
        this.gameObject.SetActive(true);
    }

    public void StartAbility(Vector3 pos)
    {
        _trans.rotation = Quaternion.identity;
        _trans.position = pos;
        _trans.Translate(Vector3.back * 5f);

        _target = _trans.position;
        _trans.position = pos;
        _trans.Translate(Vector3.back * 2.5f);
        _lifeTime = 3f;
        _onAnima = false;
        this.gameObject.SetActive(true);
    }

    public void EndAbility()
    {
        this.gameObject.SetActive(false);
        _nettingCreateManager.NettingCreateObjEnqueue(this);
    }
    private void FixedUpdate()
    {
        _lifeTime -= Time.deltaTime;
        if (_lifeTime < 0f)
        {
            EndAbility();
            return;
        }
        _trans.position = Vector3.Lerp(_trans.position, _target, Time.deltaTime * 5f);
        if (!_onAnima)
        {
            if (Vector3.Distance(_trans.position, _target) < 0.1f)
            {
                _onAnima = true;
                if (_anima != null) { _anima.SetTrigger(CommonStaticDatas.ANIMPARAM_OPEN); }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(CommonStaticDatas.TAG_ENEMY))
        {
            var unit = other.GetComponent<Interactable>();
            if (unit._unit == UNIT.Boss) return;
            if (_playerAbility.GetShock)
            {
                if (unit.GetController()._curState != eCharacterStates.Shock) { unit.GetController().SetState(eCharacterStates.Shock); }
                else { unit.GetController().GetStatusEffects.ShockAttack(); }
            }
        }
    }
}
