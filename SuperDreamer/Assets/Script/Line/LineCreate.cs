using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCreate : MonoBehaviour
{
    [SerializeField] GameObject _lineObj;
    float _setY = 0f;

    public void Init()
    {
        Create();
    }

    void Create()
    {
        for (int i = 0; i < 6; ++i)
        {
            GameObject obj = GameObject.Instantiate(_lineObj);
            obj.transform.SetParent(this.transform);
            _setY -= 2.5f;
            obj.transform.localPosition = new Vector3(_lineObj.transform.localPosition.x, _lineObj.transform.localPosition.y + _setY, _lineObj.transform.localPosition.z);
            obj.transform.localScale = Vector3.one;
        }
    }

    public Transform GetLineObj { get { return _lineObj.transform; } }
}
