using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Text.RegularExpressions;

public class LoginManager : MonoBehaviour
{
    [SerializeField] GameObject _touchObj; // 화면을 터치해주세요
    [SerializeField] GameObject _loginPageObj; // 구글 및 게스트버튼창 활성화
    [SerializeField] GameObject[] _loadingObj; // 로그인 끝나면 로비로 가기위해
    [SerializeField] GameObject _debugObj;
    [SerializeField] TextMeshProUGUI _versionText;

    [Header("Language")]
    [SerializeField] LanguageSelector[] _languageSelectorArr;
    [SerializeField] GameObject _languageObj;
    bool _onInit = false;

    //public const string LoginPage = "LoginPage";

    BackendFederationAuth _backend;

    [SerializeField] TMP_Dropdown _languageTopDown;
    string[] _languageList = new string[] { "English", "Korean" }; // { "English", "Korean", "Chinese", "Thai", "Japanese", "French", "Spanish", "German", "Russian" };
    [SerializeField] Sprite[] _languageSpriteList = null;
    bool _isInitialized = false;

    const string _versionTextFormat = "Ver. {0}";

    private void Start()
    {
        GameManager.GetInstance._nowScene = EScene.E_LOGIN;
        _versionText.text = string.Format(_versionTextFormat, Application.version);
        UserData u = UserData.GetInstance;

        if (AudioController.IsPlaying("00_WaterWorld_LoginPage") == false)
            SoundManager.GetInstance.PlayAudioBackgroundSound("00_WaterWorld_LoginPage");

        SetDropdownOptionsExample();
        SetValue(GetIndexLang(LocalizeText._configSetting._systemLanguage.ToString()));

        _touchObj.SetActive(true);
        InitLanguage();
        if (u.GetOptionData._optionType[(int)EOptionType.LOGINPAGE] == 1)
        {
            _touchObj.SetActive(false);
            BackendFederationAuth backend = GameObject.Find("BackendManager").GetComponent<BackendFederationAuth>();
#if UNITY_EDITOR
            backend.CustomSignUp();
#elif UNITY_ANDROID
            backend.GPGSLogin();
#endif
        }
        else if (u.GetOptionData._optionType[(int)EOptionType.LOGINPAGE] == 2)
        {
            _touchObj.SetActive(false);
            GuestFuntion();
        }
    }

    /// <summary>
    /// 터치해서 버튼창 활성화
    /// </summary>
    public void TouchLoginPageOn()
    {
        _touchObj.SetActive(false);
        _loginPageObj.SetActive(true);
        _languageObj.SetActive(true);

        if (!_onInit)
        {
            _onInit = true;
            BackendFederationAuth backend = GameObject.Find("BackendManager").GetComponent<BackendFederationAuth>();
            _loginPageObj.transform.Find("BG").transform.Find("Google").GetComponent<Button>().onClick.AddListener(backend.GPGSLogin);
        
            
        }
    }

    /// <summary>
    /// 바깥을 터치하면 다시 이미지 화면으로
    /// </summary>
    public void TouchBlurPageOff()
    {
        _touchObj.SetActive(true);
        _loginPageObj.SetActive(false);
        _languageObj.SetActive(false);
    }

    public void GoogleLoginOn()
    {
        for (int i = 0; i < _loadingObj.Length; ++i) { _loadingObj[i].SetActive(true); }
        _loginPageObj.SetActive(false);
        StartCoroutine(StartLobby());
    }

    public void NotGoogleLogin()
    {
        MessageHandler.GetInstance.ShowMessage("구글 로그인 실패", 1f);
        UserData.GetInstance.OptionTypeSetting((int)EOptionType.LOGINPAGE, 0);
        _touchObj.SetActive(true);
        _loginPageObj.SetActive(false);
        _languageObj.SetActive(false);
    }

    public void GuestFuntion()
    {
        if (PlayerPrefs.GetString("GuestNickName") == "")
        {
            PlayerPrefs.SetString("GuestNickName", string.Format("guest_{0}", UnityEngine.Random.Range(0, 99)));
            SoundManager.GetInstance.PlayAudioEffectSound("UI_Button_Touch");
        }
        GameManager.GetInstance.GetNickName = PlayerPrefs.GetString("GuestNickName");
        _loginPageObj.SetActive(false);
        StartCoroutine(StartLobby());
        UserData.GetInstance.OptionTypeSetting((int)EOptionType.LOGINPAGE, 2);
    }
    IEnumerator StartLobby()
    {
        if (_languageObj != null) { _languageObj.SetActive(false); }
        SoundManager.GetInstance.DestroyEffectSoundObj();
        LocalizeText.DeleteChangeLocalize();
        yield return YieldInstructionCache.WaitForSeconds(1.25f);
        cSceneManager.GetInstance.ChangeScene("Lobby", () => { });
    }

    [SerializeField] GameObject _nicknameObj;
    //[SerializeField] InputField inputField;
    [SerializeField] TMP_InputField _inputField;
    public string[] lines;
    string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    public string GetPath()
    {
        string path = null;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                path = Application.persistentDataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(Application.persistentDataPath, "Resources/");
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                path = Application.persistentDataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(path, "Assets", "Resources/");
            case RuntimePlatform.WindowsEditor:
                path = Application.dataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(path, "Assets", "Resources/");
            default:
                path = Application.dataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(path, "Resources/");
        }
    }

    void InitNickName()
    {
        List<MyData.BadWordData> lineList = MyData.DataManager.GetInstance.GetList<MyData.BadWordData>(MyData.DataManager.KEY_BADWORD);
        lines = new string[lineList.Count];
        for (int i = 0; i < lines.Length; ++i)
        {
            lines[i] = lineList[i].badWord;
        }
    }

    public void ChangeNickName()
    {
        SoundManager.GetInstance.PlayAudioEffectSound("UI_Button_Touch");
        if (_backend == null) { _backend = GameObject.Find("BackendManager").GetComponent<BackendFederationAuth>(); }
        if (_inputField.text == "") { MessageHandler.GetInstance.ShowMessage("비어있습니다", 1f); return; }
        if (_inputField.text.Length < 2) { MessageHandler.GetInstance.ShowMessage("한글자는 안됩니다", 1f); return; }
        if (_inputField.text.Length > 8) { MessageHandler.GetInstance.ShowMessage("8글자 이상은 안됩니다", 1f); return; }

        for (int i = 0; i < lines.Length; i++)
        {
            if (_inputField.text.Contains(lines[i]))
            {
                MessageHandler.GetInstance.ShowMessage("비속어는 사용할 수 없습니다", 1f);
                return;
            }
        }

        if (!_backend.NickNameDuplicationCheck(_inputField.text))
        {
            MessageHandler.GetInstance.ShowMessage("다른 유저가 닉네임을 사용중입니다", 1f);
            return;
        }

        string Check = Regex.Replace(_inputField.text, @"[ ^0-9a-zA-Z가-힣 ]{1,10}", "", RegexOptions.Singleline);

        if (_inputField.text.Equals(Check) == false)
        {
            _nicknameObj.SetActive(false);

            GameManager.GetInstance.GetNickName = _inputField.text;
            _backend.NickNameCreate(_inputField.text);
            _backend.GetNickNameCreate = true;
            _languageObj.SetActive(false);
        }
        else
        {
            //Debug.Log("특수문자는 사용할 수 없습니다.");
            MessageHandler.GetInstance.ShowMessage("특수문자는 사용할 수 없습니다", 1f);
        }
    }

    public void NickNameCreate()
    {
        InitNickName();
        _inputField.characterLimit = 10;
        _nicknameObj.SetActive(true);
        _languageObj.SetActive(false);
    }


    public void SetValue(int value)
    {
        _languageTopDown.value = value;
    }
    private int GetIndexLang(string key)
    {
        for (int i = 0; i < _languageList.Length; i++)
        {
            if (_languageList[i] == key)
                return i;
        }
        return 0;
    }
    private void SetDropdownOptionsExample()// Dropdown 목록 생성
    {
        if (_isInitialized)
            return;
        _languageTopDown.options.Clear();
        for (int i = 0; i < _languageList.Length; i++)//1부터 10까지
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = _languageList[i];
            option.image = _languageSpriteList[i];
            _languageTopDown.options.Add(option);
        }
        _isInitialized = true;
    }
    public void OnValueChanged()
    {
        if (!_isInitialized)
            return;
        var lang = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), _languageList[_languageTopDown.value]);
        LocalizeText.SetCinfigLang(lang);
        PlayerPrefs.SetString(CommonStaticDatas.STR_KEY_LOCALLANGUAGE, lang.ToString());
    }

    #region Language
    void InitLanguage()
    {
        // 초기화
        for(int i = 0; i < _languageSelectorArr.Length; i++)
        {
            _languageSelectorArr[i].Init(this);
        }

        // 디바이스 언어를 디폴트로 
        for(int j = 0; j < _languageSelectorArr.Length; j++)
        {
            if (_languageSelectorArr[j].LanguageType == Application.systemLanguage)
            {
                _languageSelectorArr[j].OnTouchChangeLanguage();
            }
            else
            {
                _languageSelectorArr[j].SetDisableButton();
            }
        }
    }

    public void DisableAllLangButton()
    {
        // 초기화
        for (int i = 0; i < _languageSelectorArr.Length; i++)
        {
            _languageSelectorArr[i].SetDisableButton();
        }
    }
    #endregion Language

    public void DebugCheck()
    {
        GameManager.GetInstance.ReporterObjActive();
    }
}
