using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneObj : MonoBehaviour
{
    DroneManager _droneManager;
    UnitDamageData tdd;

    GameObject _obj;
    Transform _trans;

    [SerializeField] Animator _animator;
    AnimEnvetSender _animEnvetSender;

    bool _life = false;
    float _lifeTime = 0f;

    EnemyController _target;
    float _droneSpeed = 5f;
    float _attackTime = 0f;

    Queue<DroneBullet> _bullet;
    [SerializeField] GameObject _bulletPrefab;

    public void Init(UnitDamageData data, DroneManager manager)
    {
        _bullet = new Queue<DroneBullet>();
        _droneManager = manager;
        tdd = data;
        if (_obj == null) { _obj = this.gameObject; _obj.SetActive(false); }
        if (_trans == null) { _trans = this.transform; }
        if (_animator != null)
        {
            _animEnvetSender = _animator.GetComponent<AnimEnvetSender>();
            _animEnvetSender.AddEvent("EndAnima", EndAnima);
        }
    }

    // ����� ��ȯ���ش�
    public void StartAbility()
    {
        if (_obj != null) { _obj.SetActive(true); }
        // ������ �������� ��ȯ�Ѵ�
        if (_trans != null)
        { 
            _trans.position = _droneManager.GetPlayerTrans.position;
            _trans.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360), 0f));
            Vector3 offset = _trans.forward * 5f;
            _trans.position -= offset;
            _trans.rotation = Quaternion.identity;
        }
        // ��� �ִ� �ð��� üũ�Ѵ�
        _life = true;
        _lifeTime = 0f;
    }
    public void EndAbility()
    {
        _life = false;
        _lifeTime = 0f;
        _attackTime = 0f;
        _animator.SetTrigger(CommonStaticDatas.ANIMPARAM_CLOSEHATCH);
        _droneManager.OnReset();
    }
    public void EndAnima()
    {
        if (_obj != null) { _obj.SetActive(false); }
    }

    private void Update()
    {
        if (!_life) { return; }

        if (_target == null)
        {
            Transform target = StandardFuncUnit.CheckTraceTarget(_trans, tdd.targetTag, 20f, tdd.layerMask);
            if (target != null)
            {
                _target = target.GetComponent<EnemyController>();
            }
        }
        else
        {
            if (_target.IsDie()) { _target = null; return; }

            // �Ÿ��� 10 ���� ������ ������ ����
            float dis = Vector3.Distance(_trans.position, _target._trans.position);
            if (dis > 10f)
            {
                _trans.position = Vector3.MoveTowards(_trans.position, _target._trans.position, _droneSpeed * Time.deltaTime);
            }
            _attackTime += Time.deltaTime;
            // �ð����� ������ �߻��Ѵ� 
            if (_attackTime > 1f)
            {
                _attackTime = 0f;
                CreateBullet();
                //Debug.Log("Attack");
            }
        }
        

        _lifeTime += Time.deltaTime;
        if (_lifeTime > 30f)
        {
            EndAbility();
        }
    }

    void CreateBullet()
    {
        if (_bullet.Count != 0)
        {
            // ť���� �Ѿ��� ������ ����
            _bullet.Dequeue().StartAbility(_trans.position, _target._trans.position);
        }
        else
        {
            DroneBullet newBullet = Instantiate(_bulletPrefab, _trans.position, Quaternion.identity).GetComponent<DroneBullet>(); ;
            newBullet.Init(tdd, this);
            newBullet.StartAbility(_trans.position, _target._trans.position);
        }
    }
    public void EnqueueBullet(DroneBullet bullet)
    {
        _bullet.Enqueue(bullet);
    }
}
