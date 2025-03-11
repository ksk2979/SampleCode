using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhasingAttack : NormalMeeleAttack
{
    private void OnTriggerEnter(Collider other)
    {
        if (tdd != null)
        {
            if (other.CompareTag(tdd.targetTag[0]))
            {
                other.GetComponent<Interactable>().TakeToDamage(tdd);
            }
        }
    }
}
