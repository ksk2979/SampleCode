using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �˼��� ���ͼ� ���ι踦 ���ΰ� ���� �¾� ü���� �������� Ǯ���鼭 �״´�
// ���� ������ ���ȴ�
public class TentacleScript : MonoBehaviour
{
    AnimEnvetSender _animEventSender;
    [SerializeField] float _hp;
    bool _die = false;
    BossType10Controller _controller;
    BossType9Controller _controller9;
    public bool IsDie => _die;

    Collider _collider;
    [SerializeField] Animator _anima;

    public void Init(BossType10Controller controller)
    {
        _controller = controller;
        _animEventSender = _anima.GetComponent<AnimEnvetSender>();
        _animEventSender.AddEvent("CheckAttack", CheckAttack);
        _animEventSender.AddEvent("EndObj", EndObj);
        _collider = this.GetComponent<Collider>();
    }
    public void Init(BossType9Controller controller)
    {
        if (_controller9 == null) { _controller9 = controller; }
    }
    public void Resetting(float value)
    {
        _hp = value;
        _die = false;
        _collider.enabled = true;
    }
    public void CheckAttack()
    {
        _controller.OKDiePlayer();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (_die) { return; }
        var bullet = other.GetComponent<Shot>();
        if (bullet == null) { return; }

        bool isSameTag = false;
        var tags = bullet.GetTdd().targetTag;
        for (int i = 0; i < tags.Length; i++)
        {
            if (tags[i] == tag)
            {
                isSameTag = true;
                break;
            }
        }

        if (isSameTag == false)
            return;

        HitDamage(bullet.GetTdd().damage);
    }
    public void HitDamage(float damage)
    {
        if (_die) { return; }
        _hp -= damage;
        Debug.Log(string.Format("{0}: {1}", name, _hp));
        if (_controller != null)
        {
            if (_hp < 0) { _die = true; _collider.enabled = false; _anima.SetTrigger(CommonStaticDatas.ANIMPARAM_DIE_1); }
        }
        else if (_controller9 != null)
        {
            if (_hp < 0) { _die = true; _collider.enabled = false; TurtleAttack(); EndObj(); }
        }
    }
    public void EndObj()
    {
        this.gameObject.SetActive(false);
    }
    // ���� �׷α�
    public void TurtleAttack()
    {
        _controller9.Attack01Groggy();
    }
}
