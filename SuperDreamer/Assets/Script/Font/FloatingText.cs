using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 데미지 폰트를 오브젝트로 생성해서 시간 만큼 표시후 사라지는 기능

public class FloatingText : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float alphaSpeed;
    float destroyTime;
    Text text;
    Color alpha;
    public int damage;

    void Start()
    {
        if (moveSpeed == 0 && alphaSpeed == 0)
        {
            moveSpeed = 2.0f;
            alphaSpeed = 2.0f;
        }
        destroyTime = 2.0f;

        text = GetComponent<Text>();
        alpha = text.color;
        string str = damage.ToString();// GameDirector.GetSuffix(damage); 단위 표현했던 함수
        text.text = str;
        Invoke("DestroyObject", destroyTime);
    }

    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0)); // 텍스트 위치

        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed); // 텍스트 알파값
        text.color = alpha;
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
