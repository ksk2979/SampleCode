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

    // ������ ����
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
    /// ī�װ��� ������ �߰�
    /// </summary>
    /// <param name="unit">������</param>
    /// <param name="iconObj">������Ʈ</param>
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
    /// ī�װ��� ���� ���(��� ����)
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
    /// ������ ���� ���
    /// </summary>
    /// <param name="idx">��ġ</param>
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
    /// ��� ���� ����
    /// </summary>
    /// <param name="materialArr">����</param>
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
    /// Category Rect�� Y ���� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int GetLength()
    {
        if (gameObject.activeSelf)
        {
            // length : ���߿��� ���������� ����
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
    /// Ȱ��ȭ�� ������ / ��� ������ ��ȯ
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
    /// ���� ������ ������ �ľ��ϱ� ���� ���
    /// </summary>
    /// <param name="index">UI�� ����</param>
    /// <param name="address">�� ������ ��ġ</param>
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
    /// ī�װ��� ����� �����۵��� ������ �ε����� �ٲ��ִ� �޼���
    /// </summary>
    /// <param name="origin">���� ��</param>
    /// <param name="changed">���� ��</param>
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
    /// �������� ������ �ε����� ã���ִ� �޼���
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
    public int AllItemCount => _iconList.Count;         // ���� ������ ���� ����� �� ���
    public bool IsEmpty => GetItemCount() <= 0;         // UI Ȱ��ȭ �� ������ ���� ���� üũ
}
