using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : CharacterController
{
    Player _player;

    [Header("Shotgun Settings")]
    [SerializeField] float _attackRange = 10f;
    [SerializeField] Transform _shotgunTransform;
    [SerializeField] LayerMask _enemyLayer;

    float _delayScanTime = 0f;
    [SerializeField] float _attackSpeed = 1f;

    [SerializeField] Transform _gunMuzzle;
    [SerializeField] int _pelletCount = 5;
    [SerializeField] float _spreadAngle = 10f;

    public override void OnStart()
    {
        base.OnStart();
        if (_player == null) { _player = GetComponent<Player>(); }
    }

    private void Update()
    {
        if (IsDie) { return; }

        _delayScanTime += Time.deltaTime;
        if (_delayScanTime > _attackSpeed)
        {
            _delayScanTime = 0f;
            Transform target = GetNearestEnemyInRange();
            if (target != null)
            {
                AimAt(target);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _delayScanTime = _attackSpeed;
        }
    }
    public void DoDie()
    {
        if (_isDie) { return; }
        _isDie = true;
        if (_player != null) { _player.GetPlayerStats.OnHpbar(false); }
        if (_anim != null) { _anim.SetTrigger(CommonStaticKey.ANIMPARAM_DIE); _anim.SetBool(CommonStaticKey.ANIMPARAM_ISDIE, true); }
        gameObject.SetActive(false); // 임의로 없어짐
    }

    void AimAt(Transform target)
    {
        // 샷건을 움직인 후 탄환을 발사
        Vector2 dir = target.position - _shotgunTransform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _shotgunTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        FireGun();
    }
    float _shotDamage = 1500f;
    void FireGun()
    {
        float baseAngle = _shotgunTransform.eulerAngles.z;
        // 테스트로 직선 체크를 위해
        if (_pelletCount == 1)
        {
            float rad = baseAngle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject bullet = SimplePool.Spawn(CommonStaticKey.RESOURCES_BULLET, "Bullet");
            bullet.transform.position = _gunMuzzle.position;
            bullet.transform.rotation = Quaternion.identity;
            bullet.GetComponent<BulletScript>().Init(dir, _shotDamage);
        }
        else
        {
            float angleStep = _spreadAngle / (_pelletCount - 1);
            float startAngle = -_spreadAngle / 2f;

            for (int i = 0; i < _pelletCount; ++i)
            {
                float angle = baseAngle + startAngle + (angleStep * i);
                float rad = angle * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

                GameObject bullet = SimplePool.Spawn(CommonStaticKey.RESOURCES_BULLET, "Bullet");
                bullet.transform.position = _gunMuzzle.position;
                bullet.transform.rotation = Quaternion.identity;
                bullet.GetComponent<BulletScript>().Init(dir, _shotDamage / _pelletCount);
            }
        }
    }

    // 가장 가까운 적을 찾아줌 이후 작업 시 전역으로 빼서 통괄적으로 사용
    Transform GetNearestEnemyInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attackRange, _enemyLayer);

        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.transform;
            }
        }

        return closest;
    }
}
