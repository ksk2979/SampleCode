using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ������ ��Ʈ�� ������Ʈ�� �����ؼ� �ð� ��ŭ ǥ���� ������� ���

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
        string str = damage.ToString();// GameDirector.GetSuffix(damage); ���� ǥ���ߴ� �Լ�
        text.text = str;
        Invoke("DestroyObject", destroyTime);
    }

    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0)); // �ؽ�Ʈ ��ġ

        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed); // �ؽ�Ʈ ���İ�
        text.color = alpha;
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
