using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BadgeType
{
    Collection = 0,
    DailyQuest,
    MainQuest,
    MainQuest_L,
    MainQuest_R,
    Roulette,
    Shop,
    Inven,
    SeasonPass,
    None,
}

public class BadgeController : MonoBehaviour
{
    [SerializeField] BadgeType[] _badgeTypeArr;
    [SerializeField] GameObject[] _badgeObjArr;

    Dictionary<BadgeType, GameObject> _badgeIconDictionary;

    public void Init()
    {
        _badgeIconDictionary = new Dictionary<BadgeType, GameObject>();

        for(int i = 0; i < _badgeTypeArr.Length; i++)
        {
            if (!_badgeIconDictionary.ContainsKey(_badgeTypeArr[i]))
            {
                _badgeIconDictionary.Add(_badgeTypeArr[i], _badgeObjArr[i]);
            }
            else
            {
                Debug.LogError(string.Format("Registered Badge Icon : {0}", _badgeTypeArr[i]));
            }
        }
        ResetBadge();
    }

    /// <summary>
    /// �˸� ���� ���� ����
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isOn"></param>
    public void SetBadge(BadgeType type, bool isOn)
    {
        if(_badgeIconDictionary.ContainsKey(type))
        {
            _badgeIconDictionary[type].SetActive(isOn);
        }
    }

    /// <summary>
    /// �˸� ���� �ʱ�ȭ
    /// </summary>
    public void ResetBadge()
    {
        foreach(var badge in _badgeIconDictionary)
        {
            badge.Value.SetActive(false);
        }
    }
}
