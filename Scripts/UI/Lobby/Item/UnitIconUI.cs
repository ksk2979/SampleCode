using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyData;

public class UnitIconUI : MonoBehaviour
{
    [Header("Components")]
    UnitIcon _unitIcon;
    InvenScript _inven;

    [SerializeField] Image _iconImage;
    [SerializeField] Image _baseImage;

    [Header("Level")]
    [SerializeField] TextMeshProUGUI _levelText;

    [Header("Grade")]
    [SerializeField] TextMeshProUGUI _gradeText;
    [SerializeField] Image _outLine;
    [SerializeField] Image _gradeOutLine;

    [Header("Images")]
    [SerializeField] Image _outLineImg;
    [SerializeField] Image _cornerDecoImg;
    [SerializeField] Image _lightImg;
    [SerializeField] Image _glowImg;
    [SerializeField] Image _gradeImg;

    const string levelTextFormat = "Lv {0}";
    const string ItemIconPathFormat = "ItemIcon/{0}";

    public void Init(UnitIcon icon)
    {
        _unitIcon = icon;
        _inven = LobbyUIManager.GetInstance.GetInvenPage;
    }

    #region UI

    /// <summary>
    /// Icon UI 통합 업데이트
    /// </summary>
    public void UpdateUISetting()
    {
        UpdateLevelText();
        UpdateGradeText();
        UpdateGradeOutLine();
        UpdateIconSprite();
    }

    /// <summary>
    /// 레벨 텍스트 업데이트
    /// </summary>
    public void UpdateLevelText(int level = 0)
    {
        if (_levelText == null) return;
        if (level > 0)
        {
            _levelText.text = string.Format(levelTextFormat, level);
        }
        else
        {
            _levelText.text = string.Format(levelTextFormat, _unitIcon.GetLevel);
        }
    }

    /// <summary>
    /// 포맷없이 그냥 레벨 텍스트에 넣어주는 메서드
    /// </summary>
    /// <param name="other"></param>
    public void UpdateLevelText(string other)
    {
        _levelText.text = other;
    }

    /// <summary>
    /// 등급 텍스트 업데이트
    /// </summary>
    /// <param name="grade"></param>
    public void UpdateGradeText(int grade = 0)
    {
        if (_gradeText == null) return;
        if (grade > 0)
        {
            _gradeText.text = StandardFuncData.GradeCheck(grade, true);
        }
        else
        {
            _gradeText.text = StandardFuncData.GradeCheck(_unitIcon.GetGrade, true);
        }
    }

    /// <summary>
    /// 등급 OutLine 업데이트
    /// </summary>
    public void UpdateGradeOutLine(int tGrade = 0)
    {
        int grade = 0;
        if(tGrade > 0)
        {
            grade = tGrade;
        }
        else
        {
            grade = _unitIcon.GetGrade;
        }
        var gradeData = DataManager.GetInstance.GetList<GradeData>(DataManager.KEY_GRADE);
        _outLineImg.color = ColorUtility.TryParseHtmlString(gradeData[grade - 1].outlineColor, out Color outLineColor) ? outLineColor : Color.white;
        _cornerDecoImg.color = ColorUtility.TryParseHtmlString(gradeData[grade - 1].cornerDecoColor, out Color cornerDecoColor) ? cornerDecoColor : Color.white;
        _lightImg.color = ColorUtility.TryParseHtmlString(gradeData[grade - 1].lightColor, out Color lightColor) ? lightColor : Color.white;
        _glowImg.color = ColorUtility.TryParseHtmlString(gradeData[grade - 1].glowColor, out Color glowColor) ? glowColor : Color.white;
        _gradeImg.color = ColorUtility.TryParseHtmlString(gradeData[grade - 1].gradeColor, out Color gradeColor) ? gradeColor : Color.white;
    }

    /// <summary>
    /// 아이템 이미지 업데이트
    /// </summary>
    public void UpdateIconSprite(Sprite iconSprite = null)
    {
        if (iconSprite == null)
        {
            Sprite loadedSprite = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(ItemIconPathFormat, _unitIcon.GetResName));
            if (loadedSprite != null)
            {
                _iconImage.sprite = loadedSprite;
            }
        }
        else
        {
            _iconImage.sprite = iconSprite;
        }
    }

    /// <summary>
    /// UI 정보 복사
    /// </summary>
    /// <param name="iconUI"></param>
    public void CopyUISetting(UnitIconUI iconUI)
    {
        //_baseImage.sprite = iconUI.GetBasicImage.sprite;
        //_baseImage.color = iconUI.GetBasicImage.color;
        _iconImage.sprite = iconUI.GetIconImage.sprite;

        _levelText.text =iconUI.GetLevelTMP.text;

        _gradeText.text = iconUI.GetGradeTMP.text;
        //_outLine.sprite = iconUI.GetOutLine.sprite;
        //_gradeOutLine.sprite = iconUI.GetGradeOutLine.sprite;
    }
    #endregion UI

    #region Color
    List<Color> GetColorList(int grade)
    {
        List<Color> colorList = new List<Color>();
        switch (grade)
        {
            case 1:
                {
                    //colorList.Add()
                }
                break;
            case 2:
                break;
            case 3:
                break;

            default:
                break;
        }
        return colorList;
    }
    #endregion Color

    #region Button

    #endregion Button
    public Image GetIconImage => _iconImage;
    public TextMeshProUGUI GetLevelTMP => _levelText;
    public TextMeshProUGUI GetGradeTMP => _gradeText;
}
