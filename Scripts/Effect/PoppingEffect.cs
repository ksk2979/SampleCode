using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Rigidbody를 가지며, 그냥 생성되자마자 대포처럼 펑 쏨
* 화면을 일정 벗어나면 스스로 파괴 (파괴에서 오브젝트 풀링으로 교체)
*/

public class PoppingEffect : MonoBehaviour
{
    RectTransform _rt;

    private void OnEnable()
    {
        Vector2 dir;
        if (this.GetComponent<RectTransform>() != null)
        {
            if (_rt == null) { _rt = this.GetComponent<RectTransform>(); }
            dir = new Vector2(Random.Range(-300f, 300f), Random.Range(300f, 500f));
            //dir = dir.normalized;
        }
        else
        {
            dir = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0.5f, 1.0f));
            dir = dir.normalized;
        }

        if (_rt != null)
        {
            GetComponent<Rigidbody2D>().AddForce(dir * Random.Range(50f, 120f)); //100f, 300f
        }
        else
        {
            GetComponent<Rigidbody2D>().AddForce(dir * Random.Range(70f, 200f));
        }
    }

    void Update()
    {
        if (_rt != null)
        {
            if (_rt.anchoredPosition.y < -1080f)
            {
                this.gameObject.SetActive(false);
                //Destroy(gameObject);
            }
        }
        else if (transform.position.y < -5.0f)
        {
            this.gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
}