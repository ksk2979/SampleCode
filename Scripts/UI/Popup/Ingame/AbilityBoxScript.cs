using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBoxScript : MonoBehaviour
{
    Collider _collider;

    public void Init()
    {
        if (_collider == null) { _collider = this.GetComponent<Collider>(); }
        this.gameObject.SetActive(true);
        _collider.enabled = false;
        StartCoroutine(DelayColliderEnabled());
    }

    // ���߿��� �ִϸ��̼����� ó�� �� ����?
    IEnumerator DelayColliderEnabled()
    {
        yield return YieldInstructionCache.WaitForSeconds(2f);
        _collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(CommonStaticDatas.TAG_PLAYER))
        {
            this.gameObject.SetActive(false);
            _collider.enabled = false;
            InGameUIManager.GetInstance.AddOneAbility();
        }
        if (other.CompareTag(CommonStaticDatas.TAG_UNTAGGED))
        {
            Debug.Log("Ư�� ��Ȳ���� �����Ƽ Ȯ��");
            if (other.GetComponent<Player>() != null)
            {
                Debug.Log("Ư�� ��Ȳ���� �����Ƽ ����");
                this.gameObject.SetActive(false);
                _collider.enabled = false;
                InGameUIManager.GetInstance.AddOneAbility();
            }
        }
    }
}