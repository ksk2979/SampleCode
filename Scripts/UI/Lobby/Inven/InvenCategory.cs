using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InvenCategory : MonoBehaviour
{
    [SerializeField] EItemList _itemListType;
    [SerializeField] string _titleText;
    [SerializeField] TextLocalizeSetter _titleTextSetter;
    [SerializeField] RectTransform _iconListRectTrans;
    RectTransform _categoryRect;
    GridLayoutGroup _invenGridLayOut;
    InvenScript _inven;

    // 데이터 관리
    [SerializeField] List<UnitIcon> _iconList;
    Dictionary<int, int> _itemDataIndexDic;
    UnitIcon _latestIcon = null;
    const int _lineMax = 4;

    // Materials
    [SerializeField] List<GameObject> _materialObjList;
    [SerializeField] List<TextMeshProUGUI> _materialAmountTMPList;

    public void Init(InvenScript inven)
    {
        _inven = inven;
        _titleTextSetter.key = _titleText;
        _iconList = new List<UnitIcon>();
        _itemDataIndexDic = new Dictionary<int, int>();
        _categoryRect = GetComponent<RectTransform>();
        _invenGridLayOut = _iconListRectTrans.GetComponent<GridLayoutGroup>();

        if(_itemListType == EItemList.MATERIAL)
        {
            InitMaterial();
        }
    }

    #region Items
    /// <summary>
    /// 카테고리에 아이템 추가
    /// </summary>
    /// <param name="unit">아이템</param>
    /// <param name="iconObj">오브젝트</param>
    public void AddInvenIcon(UnitIcon unit, GameObject iconObj)
    {
        if (unit == null || _itemListType == EItemList.NONE || 
            _itemListType == EItemList.MATERIAL)
        {
            return;
        }

        iconObj.transform.SetParent(_iconListRectTrans);
        iconObj.transform.localPosition = Vector3.zero;
        iconObj.transform.localScale = new Vector3(1, 1, 1);

        if (_itemListType == EItemList.BOAT)
        {
            iconObj.GetComponent<Button>().onClick.AddListener(() => unit.IconEditor());
        }
        else
        {
            iconObj.GetComponent<Button>().onClick.AddListener(unit.SelectInvenIcon);
        }
        _iconList.Add(unit);
        _latestIcon = unit;
        SortInven();
    }

    /// <summary>
    /// 카테고리의 정렬 기능(재료 제외)
    /// </summary>
    public void SortInven()
    {
        if (_itemListType == EItemList.NONE || 
            _itemListType == EItemList.MATERIAL)
        {
            return;
        }

        _iconList.Sort((icon1, icon2) => icon1.CompareTo(icon2));
        for (int i = 0; i < _iconList.Count; i++)
        {
            Transform parent = _iconList[i].transform.parent.transform;
            ObjHierarchyChange.SetObjMove(parent, _iconList[i].gameObject, i);
            _iconList[i].ChangeHierarchyIndex(i);
            AddItemDataDictionary(i, _iconList[i].GetDataIndex);
        }
    }

    /// <summary>
    /// 아이템 삭제 기능
    /// </summary>
    /// <param name="idx">위치</param>
    public void DestroyItem(int idx)
    {
        UnitIcon target = _iconList[idx];
        _iconList.RemoveAt(idx);
        Destroy(target.gameObject);
    }

    public void DestoryItem()
    {
        for (int i = 0; i < _iconList.Count; i++)
        {
            if (_iconList[i].GetDataIndex <= -1)
            {
                Destroy(_iconList[i].gameObject);
                _iconList[i] = null;
            }
        }

        int idx = 0;
        while(true)
        {
            if (idx >= _iconList.Count || _iconList.Count <= 0)
            {
                break;
            }

            if (_iconList[idx] == null)
            {
                _iconList.RemoveAt(idx);
                idx--;
            }
            idx++;
        }
    }
    #endregion Items

    #region Material
    void InitMaterial()
    {
        int maxCount = _iconListRectTrans.childCount;
        for(int i = 0; i < maxCount; i++)
        {
            _materialObjList.Add(_iconListRectTrans.GetChild(i).gameObject);
            _materialAmountTMPList.Add(_materialObjList[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>());
            _materialObjList[i].SetActive(false);
        }
    }

    /// <summary>
    /// 재료 정보 갱신
    /// </summary>
    /// <param name="materialArr">정보</param>
    public void RefreshMaterial(List<int> matCountList)
    {
        for(int i = 0; i < _materialObjList.Count; i++)
        {
            if(matCountList[i] == 0)
            {
                _materialObjList[i].SetActive(false);
                continue;
            }

            _materialObjList[i].SetActive(true);
            _materialAmountTMPList[i].text = matCountList[i].ToString();
        }
    }
    #endregion Material

    #region Common
    /// <summary>
    /// Category Rect의 Y 길이 반환
    /// </summary>
    /// <returns></returns>
    public int GetLength()
    {
        if (gameObject.activeSelf)
        {
            // length : 나중에는 지역변수로 변경
            int length = 0;
            length = Mathf.RoundToInt(Mathf.Abs(_iconListRectTrans.localPosition.y));
            length += _invenGridLayOut.padding.top;

            int lines = 0;

            lines = GetItemCount() / _lineMax;
            if (GetItemCount() % _lineMax > 0)
            {
                lines++;
            }

            if (lines > 0)
            {
                length += (lines + 1) * Mathf.RoundToInt(_invenGridLayOut.cellSize.y);
                length += (lines - 1) * Mathf.RoundToInt(_invenGridLayOut.spacing.y);
            }
            
            return length;
        }
        else
        {
            return 0;
        }
    }

    public void SetPosition(int posY)
    {
        Vector2 anchoredPosition = new Vector2(_categoryRect.anchoredPosition.x, posY);
        _categoryRect.anchoredPosition = anchoredPosition;
    }

    /// <summary>
    /// 활성화된 아이템 / 재료 갯수만 반환
    /// </summary>
    /// <returns></returns>
    public int GetItemCount()
    {
        int count = 0;
        if(_itemListType != EItemList.MATERIAL)
        {
            foreach (var item in _iconList)
            {
                if (item.gameObject.activeSelf)
                {
                    count++;
                }
            }
        }
        else
        {
            foreach(var mat in _materialObjList)
            {
                if(mat.gameObject.activeSelf)
                {
                    count++;
                }
            }
        }
        return count;
    }
    #endregion Common

    #region Data
    /// <summary>
    /// 실제 데이터 순서를 파악하기 위한 등록
    /// </summary>
    /// <param name="index">UI상 순서</param>
    /// <param name="address">실 데이터 위치</param>
    void AddItemDataDictionary(int index, int address)
    {
        if (!_itemDataIndexDic.ContainsKey(index))
        {
            _itemDataIndexDic.Add(index, address);
            return;
        }
        _itemDataIndexDic[index] = address;
    }
    /// <summary>
    /// 카테고리에 저장된 아이템들의 데이터 인덱스를 바꿔주는 메서드
    /// </summary>
    /// <param name="origin">원본 값</param>
    /// <param name="changed">변경 값</param>
    public void ChangeItemDataIndex(int origin, int changed)
    {
        if (_itemDataIndexDic.ContainsValue(origin))
        {
            var icon = FindItemByDataIndex(origin);
            if (icon != null)
            {
                icon.ChangeDataIndex(changed);
            }
        }
    }

    /// <summary>
    /// 아이템을 데이터 인덱스로 찾아주는 메서드
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public UnitIcon FindItemByDataIndex(int idx)
    {
        for (int n = 0; n < _itemDataIndexDic.Count; n++)
        {
            if (_itemDataIndexDic[n] == idx)
            {
                if (_iconList.Count <= n || _iconList[n] == null)
                {
                    return null;
                }
                else
                {
                    return _iconList[n];
                }
            }
        }
        return null;
    }
    #endregion Data

    public string GetTitle => _titleText;
    public void SetCategoryState(bool state) => gameObject.SetActive(state);
    public EItemList ItemListType => _itemListType;
    public List<UnitIcon> GetList => _iconList;
    public UnitIcon LastestIcon => _latestIcon;
    public bool IsActivated => gameObject.activeSelf;
    public int AllItemCount => _iconList.Count;         // 실제 아이템 갯수 얻고자 한 경우
    public bool IsEmpty => GetItemCount() <= 0;         // UI 활성화 된 아이템 갯수 여부 체크
}
