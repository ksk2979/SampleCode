using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    float _lifeTime = 3f;

    Vector2 _direction;
    Transform _trans;
    float _damage = 10f;

    public void Init(Vector2 dir, float damage)
    {
        _direction = dir.normalized;
        if (_trans == null) { _trans = transform; }
        _lifeTime = 3f;
        _damage = damage;
    }

    void Update()
    {
        _lifeTime -= Time.deltaTime;
        if (_lifeTime < 0f)
        {
            SimplePool.Despawn(this.gameObject);
            return;
        }
        _trans.Translate(_direction * _speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(CommonStaticKey.TAG_ENEMY))
        {
            // 임의로 여기서 치명타 확률을 줘서 치명타 대미지 보이게
            if (Random.Range(0, 100) > 70) { collision.GetComponent<Enemy>().TakeToDamage(_damage * 2); }
            else { collision.GetComponent<Enemy>().TakeToDamage(_damage); }
            SimplePool.Despawn(this.gameObject);
        }
    }
}
