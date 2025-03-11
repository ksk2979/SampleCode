using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialIcon : MonoBehaviour
{
    [SerializeField] Image _iconImage;
    [SerializeField] TextMeshProUGUI _countTMP;
    [SerializeField] TextMeshProUGUI _bonusTMP;

    /// <summary>
    /// 이미지와 수량 둘다 필요한 경우
    /// </summary>
    public void SetIconInfo(Sprite iconSprite, int count)
    {
        _iconImage.sprite = iconSprite;
        _countTMP.text = count.ToString();
    }

    /// <summary>
    /// 수량만 필요한 경우
    /// </summary>
    public void SetIconInfo(int count)
    {
        _countTMP.text = count.ToString();
    }

    /// <summary>
    /// 수량표기에 컬러를 변경하는 경우
    /// </summary>
    public void SetIconInfo(int count, Color color)
    {
        SetIconInfo(count);
        _countTMP.color = color;
    }

    /// <summary>
    /// 보너스 수량 
    /// </summary>
    public void SetBonusInfo(int bonus)
    {
        _bonusTMP.text = string.Format("+{0}", bonus.ToString());
        _bonusTMP.gameObject.SetActive(true);
    }

    /// <summary>
    /// 수량 미표기(- 처리)
    /// </summary>
    public void SetIconInfo(string none)
    {
        _countTMP.text = none;
    }
}
