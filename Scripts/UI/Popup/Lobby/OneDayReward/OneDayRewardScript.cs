using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OneDayRewardScript : MonoBehaviour
{
    [SerializeField] Image _iconImg;
    [SerializeField] Image _iconBoxImg;
    [SerializeField] GameObject _blurObj;
    [SerializeField] TextMeshProUGUI _contextTMP;

    Color _blurTextColor = new Color(150 / 255f, 150 / 255f, 150 / 255f, 1f);

    public void Init(Sprite iconSprite, Sprite iconBoxSprite, string context)
    {
        _iconImg.sprite = iconSprite;
        _iconBoxImg.sprite = iconBoxSprite;
        _contextTMP.text = context;
    }

    public void SetBlur(bool blurState)
    {
        if(blurState)
        {
            _blurObj.SetActive(true);
            _contextTMP.color = _blurTextColor;
        }
        else
        {
            _blurObj.SetActive(false);
            _contextTMP.color = Color.white;
        }
    }
}
