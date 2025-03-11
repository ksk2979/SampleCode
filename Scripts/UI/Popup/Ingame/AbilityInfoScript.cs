using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityInfoScript : MonoBehaviour
{
    [SerializeField] Image _imgIcon;
    [SerializeField] TextMeshProUGUI _textCount;

    int _count;

    public void Setting(Sprite sprite)
    {
        _imgIcon.sprite = sprite;
        _count = 1;
        _textCount.text = string.Format("{0}", _count);
    }
    public void CountUp()
    {
        _count++;
        _textCount.text = string.Format("{0}", _count);
    }
}
