using MyData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TextLocalizeSetter : MonoBehaviour
{
    /// <summary>
    /// The text
    /// </summary>
    //[SerializeField]
    //public UnityEngine.UI.Text text;
    private TextMeshProUGUI text;

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    /// <value>The icon.</value>
    public string key
    {
        get => this._key;
        set
        {
            this._key = value;
            this.Repaint();
        }
    }
    [SerializeField]
    private string _key;

    /// <summary>
    /// Gets or sets the language.
    /// </summary>
    /// <value>The language.</value>
    public SystemLanguage language
    {
        get => this._language;
        set
        {
            this._language = value;
            this.Repaint();
        }
    }
    [SerializeField]
    private SystemLanguage _language = SystemLanguage.Unknown;

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    private void Awake()
    {
        LocalizeText.changeLocalizeDel += ChangeLocalize;
        if (this.text == null) this.text = this.GetComponent<TextMeshProUGUI>();
        this.Repaint();
    }

    private void ChangeLocalize(SystemLanguage lang)
    {
        this.Repaint();
    }

    /// <summary>
    /// Called when [validate].
    /// </summary>
    private void OnValidate()
    {
        this.Repaint();
    }

    /// <summary>
    /// Repaints this instance.
    /// </summary>
    public void Repaint()
    {
        if (this.text == null) 
            return;
        if (this.text.text == "") { return; }
        SystemLanguage language = this.language == SystemLanguage.Unknown ? LocalizeText._configSetting._systemLanguage : this.language;
        this.text.font = LocalizeText.Get(language);
        if (string.IsNullOrEmpty(key))
            return;
        this.text.text = LocalizeText.Get(key, language);
    }
}
