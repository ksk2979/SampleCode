using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StageIcon : MonoBehaviour
{
    [SerializeField] Image _iconImage;
    [SerializeField] Button _iconButton;
    [SerializeField] GameObject _iconBlur;
    [SerializeField] TextMeshProUGUI _iconTMP;


    public void SetIcon(Sprite iconSprite, string iconText)
    {
        _iconImage.sprite = iconSprite;
        _iconBlur.GetComponent<Image>().sprite = iconSprite;
        _iconTMP.text = iconText;
    }

    /// <summary>
    /// 아이콘 버튼 기능 등록
    /// </summary>
    /// <param name="buttonAction"></param>
    public void SetButtonAction(Action buttonAction)
    {
        _iconButton.onClick.AddListener(() => buttonAction());
    }

    /// <summary>
    /// 블러 상태 전환
    /// </summary>
    /// <param name="state"></param>
    public void SetBlurState(bool state)
    {
        _iconBlur.SetActive(state);
    }
}
