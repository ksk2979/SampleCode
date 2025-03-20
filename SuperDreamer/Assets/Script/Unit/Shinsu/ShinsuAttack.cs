using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShinsuAttack : MonoBehaviour
{
    float _damage = 1500f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(CommonStaticKey.TAG_ENEMY))
        {
            Enemy enemyHealth = collision.GetComponent<Enemy>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeToDamage(_damage);
            }
        }
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
