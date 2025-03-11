using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemStatus : MonoBehaviour
{
    ItemStatusSupporter _statusSupporter;
    [SerializeField] TextMeshProUGUI[] _statusTitleArr;
    [SerializeField] TextMeshProUGUI[] _statusValueArr;

    public void Init(ItemStatusSupporter statusSupporter)
    {
        _statusSupporter = statusSupporter;

        for(int i = 0; i < _statusTitleArr.Length; i++)
        {
            _statusTitleArr[i].gameObject.SetActive(true);
            _statusValueArr[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 스테이터스 업데이트
    /// </summary>
    /// <param name="target"></param>
    public void UpdateStatusText(UnitIcon target, bool isNext = false)
    {
        _statusSupporter.Init(target);
        SetStatus(_statusSupporter.FindStatusTitles(), _statusSupporter.FindStatusValues(isNext));
    }

    /// <summary>
    /// 장비 스텟 정보 세팅
    /// </summary>
    /// <param name="titleArr">항목 배열</param>
    /// <param name="valueArr">값 배열</param>
    void SetStatus(string[] titleArr, float[] valueArr)
    {
        for (int i = 0; i < _statusTitleArr.Length; i++)
        {
            if (titleArr.Length > i && titleArr[i] != string.Empty)
            {
                _statusTitleArr[i].text = string.Format("{0} :", titleArr[i]);
                _statusTitleArr[i].gameObject.SetActive(true);

                _statusValueArr[i].text = valueArr[i].ToString();
                _statusValueArr[i].gameObject.SetActive(true);
            }
            else
            {

                _statusValueArr[i].gameObject.SetActive(false);
                _statusTitleArr[i].gameObject.SetActive(false);
            }
        }
    }
}
