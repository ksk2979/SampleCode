using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : MonoBehaviour
{
    double _damage = 0f;
    public void Init(double damage)
    {
        _damage = damage;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(CommonStaticKey.TAG_PLAYER))
        {
            collision.GetComponent<Player>().TakeToDamage(_damage);
            gameObject.SetActive(false);
        }
    }
    public void ResetAttack()
    {
        gameObject.SetActive(false);
    }
}
