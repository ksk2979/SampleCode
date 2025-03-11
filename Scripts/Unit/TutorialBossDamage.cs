using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBossDamage : MonoBehaviour
{
    [SerializeField] bool _onMove = false;
    [SerializeField] float _speed = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(CommonStaticDatas.TAG_PLAYER))
        {
            other.GetComponent<Player>().TakeToDamage(999999999);
        }
    }

    // 움직임도 같이 한다
    private void Update()
    {
        if (_onMove)
        {
            this.transform.position += Vector3.right * (Time.deltaTime * _speed);
            if (this.transform.position.x > 50f) { this.gameObject.SetActive(false); }
        }
    }
}
