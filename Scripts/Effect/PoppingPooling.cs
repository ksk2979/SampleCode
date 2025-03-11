using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 펑 터지는 효과의 오브젝트 풀링 기법

public class PoppingPooling : MonoBehaviour
{
    public GameObject _poppingObj; // 필요한 오브젝트 가져옴
    [SerializeField] List<GameObject> _poppingList; // 사용한 오브젝트 담아두고 재사용 되는 리스트
    [SerializeField] List<Transform> _tfList; // sprite 오브젝트
    [SerializeField] List<RectTransform> _rtList; // UI 오브젝트
    [SerializeField] bool _transform = true;
    public void PoppingStart(Vector3 position)
    {
        if (_transform)
        {
            // 먼저 풀수 있는 오브젝트가 있는지 검사
            for (int i = 0; i < _poppingList.Count; ++i)
            {
                if (!_poppingList[i].activeSelf)
                {
                    _poppingList[i].SetActive(true);
                    _tfList[i].position = position;
                    return;
                }
            }
            // 더 열수 있는게 없어? 그럼 더 만들어 (최대치: 300개)
            if (_poppingList.Count < 300)
            {
                GameObject obj = Instantiate(_poppingObj, transform);
                obj.transform.position = position;
                obj.transform.parent = this.transform;
                _poppingList.Add(obj);
                _tfList.Add(obj.GetComponent<Transform>());
            }
        }
        else
        {
            // 먼저 풀수 있는 오브젝트가 있는지 검사
            for (int i = 0; i < _poppingList.Count; ++i)
            {
                if (!_poppingList[i].activeSelf)
                {
                    _poppingList[i].SetActive(true);
                    _rtList[i].anchoredPosition = position;
                    return;
                }
            }
            // 더 열수 있는게 없어? 그럼 더 만들어 (최대치: 300개)
            if (_poppingList.Count < 300)
            {
                GameObject obj = Instantiate(_poppingObj, transform);
                obj.GetComponent<RectTransform>().anchoredPosition = position;
                obj.transform.SetParent(this.transform);
                _poppingList.Add(obj);
                _rtList.Add(obj.GetComponent<RectTransform>());
            }
        }
    }
}
