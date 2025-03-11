using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMoveAction : RootAction
{
    private float dis;
    private float speed;
    private float waitTime;
    public Transform TargetTr;
    public RectTransform _rtTarget;
    
    RectTransform _rt;
    Rigidbody2D _rigidbody;

    public bool _popping;

    void Start()
    {
        if (_popping)
        {
            PoppingStart();
        }
        else
        {
            GuidedMissileStart();
        }
    }
    void GuidedMissileStart()
    {
        // 3D ���� ���� �̻��� �ڵ�
        dis = Vector3.Distance(transform.position, TargetTr.position);
        //��ź������ �ʹݿ� ��ź�� ���������� �����ϱ�����
        //��ź�� ȸ���� ĳ������ġ���� ��ź�� ��ġ�� �������� �����ϴ�
        transform.rotation = Quaternion.LookRotation(transform.position - TargetTr.position);
        _popping = false;

        StartCoroutine(DiffusionMissile_Move_Operation());
    }

    void PoppingStart()
    {
        // �� ������ ȿ���� ����
        Vector2 dir;
        if (this.GetComponent<RectTransform>() != null)
        {
            _rt = this.GetComponent<RectTransform>();
            dir = new Vector2(Random.Range(-300f, 300f), Random.Range(300f, 500f));
            dir = dir.normalized;
        }
        else
        {
            dir = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0.5f, 1.0f));
            dir = dir.normalized;
        }

        if (_rt != null)
        {
            GetComponent<Rigidbody2D>().AddForce(dir * Random.Range(100f, 300f));
            StartCoroutine(RunAction(_rt.anchoredPosition, _rtTarget.position));
        }
        else
        {
            GetComponent<Rigidbody2D>().AddForce(dir * Random.Range(70f, 200f));
            StartCoroutine(RunAction(transform.position, TargetTr.position));
        }

        _rigidbody = GetComponent<Rigidbody2D>();
        _popping = true;
    }

    IEnumerator DiffusionMissile_Move_Operation()
    {
        if (TargetTr == null) yield break;

        // ���� ������Ʈ�� �����ߴ����� �ڷ�ƾ���� �ٲٱ�

        waitTime += Time.deltaTime;
        //1.5�� ���� õõ�� forward �������� �����մϴ�
        if (waitTime < 1.5f)
        {
            speed = Time.deltaTime;
            transform.Translate(transform.forward * speed, Space.World);
        }
        else
        {
            // 1.5�� ���� Ÿ�ٹ������� lerp��ġ�̵� �մϴ�

            speed += Time.deltaTime;
            float t = speed / dis;

            transform.position = Vector3.LerpUnclamped(transform.position, TargetTr.position, t);
        }

        // �������Ӹ��� Ÿ�ٹ������� ��ź�� �������ٲߴϴ�
        //Ÿ����ġ - ��ź��ġ = ��ź�� Ÿ�����׼��� ����
        Vector3 directionVec = TargetTr.position - transform.position;
        Quaternion qua = Quaternion.LookRotation(directionVec);
        transform.rotation = Quaternion.Slerp(transform.rotation, qua, Time.deltaTime * 2f);
    }

    public IEnumerator RunAction(Vector3 start, Vector3 end)
    {
        if (_frontDelayTime != 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(_frontDelayTime);
        }
        _rigidbody.bodyType = RigidbodyType2D.Static;

        if (_rt != null)
        {
            start = _rt.anchoredPosition;
        }
        else
        {
            start = this.transform.position;
        }
        

        for (float t = 0f; t < 1f; t += Time.deltaTime / _actionTime)
        {
            if (_rt != null)
            {
                _rt.anchoredPosition = Vector3.Lerp(start, end, ChangeSmoothTime(t));
            }
            else
            {
                this.transform.position = Vector3.Lerp(start, end, ChangeSmoothTime(t));
            }

            yield return null;
        }

        if (_rt != null)
        {
            _rt.anchoredPosition = end;
        }
        else
        {
            this.transform.position = end;
        }
        Destroy(gameObject);
    }
}
