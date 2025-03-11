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

    // 나중에는 애니메이션으로 처리 할 수도?
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
            Debug.Log("특정 상황에서 어빌리티 확인");
            if (other.GetComponent<Player>() != null)
            {
                Debug.Log("특정 상황에서 어빌리티 먹음");
                this.gameObject.SetActive(false);
                _collider.enabled = false;
                InGameUIManager.GetInstance.AddOneAbility();
            }
        }
    }
}