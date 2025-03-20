using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellAttack : MonoBehaviour
{
    public double _damage;
    public float _speed = 1f;
    public float _lifeTimeMax = 3f;
    float _lifeTime = 0f;

    public void OnStart()
    {
        _lifeTime = _lifeTimeMax;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(CommonStaticKey.TAG_ENEMY))
        {
            Enemy enemyHealth = collision.GetComponent<Enemy>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeToDamage(_damage);
            }

            DestroyProjectile();
        }
    }
    void Update()
    {
        _lifeTime -= Time.deltaTime;
        if (_lifeTime < 0f) { DestroyProjectile(); }
        if (transform.position.y < -1.5f) { DestroyProjectile(); }
        transform.position += Vector3.down * _speed * Time.deltaTime;
    }

    void DestroyProjectile()
    {
        SimplePool.Despawn(gameObject);
    }
}
