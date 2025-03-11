using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;
using TMPro;

public class Text3dLocalizeSetter : MonoBehaviour
{
    private TextMeshPro textMesh;
    private TextMeshPro _textMesh
    {
        get
        {
            if (textMesh == null)
                textMesh = GetComponent<TextMeshPro>();
            return textMesh;
        }
    }
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
        }
    }
    [SerializeField]
    private SystemLanguage _language = SystemLanguage.Unknown;
    public string _strKey;
    private void OnEnable()
    {
        SystemLanguage language = this.language == SystemLanguage.Unknown ? LocalizeText._configSetting._systemLanguage : this.language;
        _textMesh.font = LocalizeText.Get(language);
        _textMesh.SetText(LocalizeText.Get(_strKey));
    }
}
