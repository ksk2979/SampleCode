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
    /// �̹����� ���� �Ѵ� �ʿ��� ���
    /// </summary>
    public void SetIconInfo(Sprite iconSprite, int count)
    {
        _iconImage.sprite = iconSprite;
        _countTMP.text = count.ToString();
    }

    /// <summary>
    /// ������ �ʿ��� ���
    /// </summary>
    public void SetIconInfo(int count)
    {
        _countTMP.text = count.ToString();
    }

    /// <summary>
    /// ����ǥ�⿡ �÷��� �����ϴ� ���
    /// </summary>
    public void SetIconInfo(int count, Color color)
    {
        SetIconInfo(count);
        _countTMP.color = color;
    }

    /// <summary>
    /// ���ʽ� ���� 
    /// </summary>
    public void SetBonusInfo(int bonus)
    {
        _bonusTMP.text = string.Format("+{0}", bonus.ToString());
        _bonusTMP.gameObject.SetActive(true);
    }

    /// <summary>
    /// ���� ��ǥ��(- ó��)
    /// </summary>
    public void SetIconInfo(string none)
    {
        _countTMP.text = none;
    }
}
