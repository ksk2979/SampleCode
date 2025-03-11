using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RParticleType
{
    Money = 0,
    Diamond = 1,
    Energy = 2,
}

public class LobbyParticle : MonoBehaviour
{
    LobbyUIManager _uiManager;

    [SerializeField] GameObject[] _rewardParticleArr;
    List<List<GameObject>> _particlePool;

    public void Init(LobbyUIManager uiManager)
    {
        _particlePool = new List<List<GameObject>>();
        for (int i = 0; i < _rewardParticleArr.Length; i++)
        {
            _particlePool.Add(new List<GameObject>());
        }
        _uiManager = uiManager;
    }

    public void ShowParticle(RParticleType pType)
    {
        GameObject particleObj = GetParticleObject(pType);
        if (particleObj == null)
        {
            particleObj = Instantiate(_rewardParticleArr[(int)pType]);
            _particlePool[(int)pType].Add(particleObj);
        }
        StartCoroutine(DelayedPlay(particleObj, pType));
    }

    GameObject GetParticleObject(RParticleType type)
    {
        List<GameObject> targetList = _particlePool[(int)type];
        if(targetList.Count <= 0)
        {
            return null;
        }

        for(int i =0; i < targetList.Count; i++)
        {
            if(!targetList[i].activeSelf)
            {
                return targetList[i];
            }
        }
        return null;
    }

    IEnumerator DelayedPlay(GameObject obj, RParticleType pType)
    {
        LobbyUIManager uiManager = LobbyUIManager.GetInstance;
        var topUIRect = uiManager.GetUserInterface.GetRectTrans;
        var popupRect = uiManager.GetPopupController.gameObject.GetComponent<RectTransform>(); ;
        topUIRect.SetSiblingIndex(topUIRect.GetSiblingIndex() + 1);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition3D = new Vector3(0f, 0f, 0f);
        rect.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>().position
            = _uiManager.GetUserInterface.GetInterfaceIcon()[(int)pType].position;
        obj.SetActive(true);
        yield return YieldInstructionCache.WaitForSeconds(2.0f);
        topUIRect.SetSiblingIndex(topUIRect.GetSiblingIndex() - 1);
        obj.SetActive(false);
    }
}
