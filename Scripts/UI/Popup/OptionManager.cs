using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class OptionManager : PopupBase
{
    [SerializeField] Toggle _soundToggle;
    [SerializeField] Toggle _vibrationToggle;
    [SerializeField] Toggle _joystickToggle;
    [SerializeField] GameObject _debugObj;

    bool _option_SoundMute = false;
    bool _option_Vibration = false;
    bool _option_JoystickOff = false;

    [SerializeField] List<Image> _joystickObj;

    [SerializeField] TMP_Dropdown _languageTopDown;
    string[] _languageList = new string[] { "English", "Korean" }; // { "English", "Korean", "Chinese", "Thai", "Japanese", "French", "Spanish", "German", "Russian" };
    [SerializeField] Sprite[] _languageSpriteList = null;
    bool _isInitialized = false;

    public void Init()
    {
        SetDropdownOptionsExample();
        SetLanguageValue(GetIndexLang(LocalizeText._configSetting._systemLanguage.ToString()));

        UserData u = UserData.GetInstance;
        if (u.GetOptionData._optionType[(int)EOptionType.SOUNDMUTE] == 1)
        {
            _option_SoundMute = true;
            _soundToggle.isOn = true;
        }
        else { _option_SoundMute = false; }
        if (u.GetOptionData._optionType[(int)EOptionType.VIBRATION] == 1)
        {
            _option_Vibration = true;
            _vibrationToggle.isOn = true;
        }
        else { _option_Vibration = false; }
        if (u.GetOptionData._optionType[(int)EOptionType.JOYSTICK] == 1)
        {
            _option_JoystickOff = true;
            _joystickToggle.isOn = true;
            if (_joystickObj.Count != 0)
            {
                for (int i = 0; i < _joystickObj.Count; ++i)
                {
                    _joystickObj[i].color = new Color(_joystickObj[i].color.r, _joystickObj[i].color.g, _joystickObj[i].color.b, 0f);
                }
            }
        }
        else
        {
            _option_JoystickOff = false;
            if (_joystickObj.Count != 0)
            {
                _joystickObj[0].color = new Color(_joystickObj[0].color.r, _joystickObj[0].color.g, _joystickObj[0].color.b, 0.58f);
                for (int i = 1; i < _joystickObj.Count; ++i)
                {
                    _joystickObj[i].color = new Color(_joystickObj[i].color.r, _joystickObj[i].color.g, _joystickObj[i].color.b, 1f);
                }
            }
        }
    }

    #region Option Setting
    public void SoundMuteToggle()
    {
        if (_soundToggle.isOn)
        {
            _option_SoundMute = true;
            UserData.GetInstance.OptionTypeSetting((int)EOptionType.SOUNDMUTE, 1);
            AudioController audio = GameObject.Find("AudioController").GetComponent<AudioController>();
            audio.Volume = 0;
            //Debug.Log("사운드 오프");
        }
        else
        {
            _option_SoundMute = false;
            UserData.GetInstance.OptionTypeSetting((int)EOptionType.SOUNDMUTE, 0);
            AudioController audio = GameObject.Find("AudioController").GetComponent<AudioController>();
            audio.Volume = 1;
            //Debug.Log("사운드 온");
        }
    }

    public void VibrationToggle()
    {
        if (_vibrationToggle.isOn)
        {
            //Debug.Log("진동 온");
            _option_Vibration = true;
            //Vibration.Vibrate((long)200); // 500 0.5초 1000 1초
            UserData.GetInstance.OptionTypeSetting((int)EOptionType.VIBRATION, 1);
        }
        else
        {
            _option_Vibration = false;
            UserData.GetInstance.OptionTypeSetting((int)EOptionType.VIBRATION, 0);
            //Debug.Log("진동 오프");
        }
    }

    public void JoystickToggle()
    {
        if (_joystickToggle.isOn)
        {
            _option_JoystickOff = true;
            UserData.GetInstance.OptionTypeSetting((int)EOptionType.JOYSTICK, 1);
            if (_joystickObj.Count != 0)
            {
                for (int i = 0; i < _joystickObj.Count; ++i)
                {
                    _joystickObj[i].color = new Color(_joystickObj[i].color.r, _joystickObj[i].color.g, _joystickObj[i].color.b, 0f);
                }
            }
            //Debug.Log("조이스틱 온");
        }
        else
        {
            _option_JoystickOff = false;
            UserData.GetInstance.OptionTypeSetting((int)EOptionType.JOYSTICK, 0);
            if (_joystickObj.Count != 0)
            {
                for (int i = 0; i < _joystickObj.Count; ++i)
                {
                    _joystickObj[i].color = new Color(_joystickObj[i].color.r, _joystickObj[i].color.g, _joystickObj[i].color.b, 1f);
                }
            }
        }
    }

    public void SetLanguageValue(int value)
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
    #endregion Option Setting

    #region Debug
    public void DebugCheck()
    {
        GameManager.GetInstance.ReporterObjActive();
    }
    #endregion Debug

    public bool GetSoundMute => _option_SoundMute;
    public bool GetVibration => _option_Vibration;
    public bool GetJoystick => _option_JoystickOff;
}
