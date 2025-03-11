using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSelector : MonoBehaviour
{
    [Header("Components")]
    LoginManager _loginManager;
    [SerializeField] SystemLanguage _languageType;
    [SerializeField] Color[] _buttonColorArr;       // 0 : true, 1 : false
    [SerializeField] Button _langButton;
    [SerializeField] Image _buttonImage;
    [SerializeField] GameObject _checkObj;

    public void Init(LoginManager loginM)
    {
        _loginManager = loginM;
        _buttonImage.color = _buttonColorArr[0];
        _checkObj.SetActive(false);

        _langButton.onClick.AddListener(OnTouchChangeLanguage);
    }

    /// <summary>
    /// ��ư ���� ��Ȱ��ȭ(UI)
    /// </summary>
    public void SetDisableButton()
    {
        _buttonImage.color = _buttonColorArr[1];
        _checkObj.SetActive(false);
    }

    /// <summary>
    /// ��� ��ư ��ġ ���
    /// </summary>
    public void OnTouchChangeLanguage()
    {
        _loginManager.DisableAllLangButton();
        _buttonImage.color = _buttonColorArr[0];
        _checkObj.SetActive(true);
        LocalizeText.SetCinfigLang(_languageType);
        PlayerPrefs.SetString(CommonStaticDatas.STR_KEY_LOCALLANGUAGE, _languageType.ToString());
    }

    public SystemLanguage LanguageType => _languageType;
}
