using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardResultItem : MonoBehaviour
{
    [SerializeField] Image _iconImage;
    [SerializeField] TextMeshProUGUI _context;

    ERewardType rType = ERewardType.None;
    int _rCount = 0;
    const string countTextFormat = "X {0}";

    public void SetItem(ERewardType type, Sprite iconSprite, int count)
    {
        if (rType == ERewardType.None)
        {
            rType = type;
        }
        _iconImage.sprite = iconSprite;
        ChangeItemValue(count);
    }

    public void ChangeItemValue(int count)
    {
        _rCount = count;
        _context.text = string.Format(countTextFormat, _rCount);
    }

    public void ResetItem()
    {
        rType = ERewardType.None;
        _iconImage.sprite = null;
        _context.text = string.Empty;
        _rCount = 0;
    }

    public ERewardType RewardType => rType;
    public int RewardCount => _rCount;
}
