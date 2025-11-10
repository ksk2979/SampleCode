//using UnityEngine;
//using MyData;
//using TMPro;
//using System;
//
//public static class LocalizeText
//{
//    public delegate void ChangeLocalize(SystemLanguage data);
//    public static ChangeLocalize changeLocalizeDel;
//
//    private static Language data;
//    public static Language _langData
//    {
//        get
//        {
//            if (data == null)
//                data = Resources.Load<Language>("Data/Language");
//            return data;
//        }
//    }
//
//    private static ConfigSetting configSetting;
//    public static ConfigSetting _configSetting
//    {
//        get
//        {
//            if (configSetting == null)
//            {
//                configSetting = Resources.Load<ConfigSetting>("Data/ConfigSetting");
//                configSetting.systemLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), PlayerPrefs.GetString(CommonStaticKey.STR_KEY_LOCALLANGUAGE, Application.systemLanguage.ToString/()) );
//            }
//            return configSetting;
//        }
//    }
//
//    public static TMP_FontAsset Get(SystemLanguage systemLanguage)
//    {
//        if (systemLanguage == SystemLanguage.English)
//        {
//            return _langData.m_englishFont;
//        }
//        else if (systemLanguage == SystemLanguage.Korean)
//        {
//            return _langData.m_koreaFont;
//        }
//        else if (systemLanguage == SystemLanguage.Japanese)
//        {
//            return _langData.m_japanFont;
//        }
//        else if (systemLanguage == SystemLanguage.Thai)
//        {
//            return _langData.m_thaiFont;
//        }
//        else if (systemLanguage == SystemLanguage.Chinese)
//        {
//            return _langData.m_chinaFont;
//        }
//        else if (systemLanguage == SystemLanguage.French)
//        {
//            return _langData.m_frenchFont;
//        }
//        else if (systemLanguage == SystemLanguage.Spanish)
//        {
//            return _langData.m_spanishFont;
//        }
//        else if (systemLanguage == SystemLanguage.German)
//        {
//            return _langData.m_germanFont;
//        }
//        else if (systemLanguage == SystemLanguage.Russian)
//        {
//            return _langData.m_russianFont;
//        }
//        else
//        {
//            return _langData.m_englishFont;
//        }
//    }
//
//    public static string Get(string key, SystemLanguage systemLanguage)
//    {
//        if (DataManager.GetInstance == null)
//            return key + "!";
//        var data = DataManager.GetInstance.FindData<LanguageData>(DataManager.KEY_LANGUAGE, key);
//        if (data == null)
//            return key + "!";
//
//        if (systemLanguage == SystemLanguage.English)
//        {
//            return data.English;
//        }
//        if (systemLanguage == SystemLanguage.Korean)
//        {
//            return data.Korean;
//        }
//        //if (systemLanguage == SystemLanguage.Chinese)
//        //{
//        //    return data.Chinese;
//        //}
//        //if (systemLanguage == SystemLanguage.Thai)
//        //{
//        //    return data.Thai;
//        //}
//        //if (systemLanguage == SystemLanguage.Japanese)
//        //{
//        //    return data.Japanese;
//        //}
//        //if (systemLanguage == SystemLanguage.French)
//        //{
//        //    return data.French;
//        //}
//        //if (systemLanguage == SystemLanguage.Spanish)
//        //{
//        //    return data.Spanish;
//        //}
//        //if (systemLanguage == SystemLanguage.German)
//        //{
//        //    return data.German;
//        //}
//        //if (systemLanguage == SystemLanguage.Russian)
//        //{
//        //    return data.Russian;
//        //}
//        else
//        {
//            return data.English;
//        }
//    }
//
//    internal static string Get(string key)
//    {
//        return Get(key, _configSetting._systemLanguage);
//    }
//
//    internal static void SetConfigLang(SystemLanguage value)
//    {
//        Debug.Log("SetConfigLang : " + value);
//        _configSetting._systemLanguage = value;
//        if (changeLocalizeDel != null)
//        {
//            changeLocalizeDel(_configSetting._systemLanguage);
//        }
//    }
//
//    internal static void DeleteChangeLocalize()
//    {
//        changeLocalizeDel = null;
//    }
//}
