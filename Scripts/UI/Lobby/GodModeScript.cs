using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GodModeScript : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] TMP_InputField _dmgMultipleInputField;
    [SerializeField] TMP_InputField _hpMultipleInputField;
    [SerializeField] TextMeshProUGUI _curDmgTMP;
    [SerializeField] TextMeshProUGUI _curHpTMP;

    private void Start()
    {
        gameObject.SetActive(false);
        SetCurrentStatsText();
    }

    void SetCurrentStatsText()
    {
        var gm = GameManager.GetInstance;
        _curDmgTMP.text = string.Format("DMG : {0}x", gm.dmgMultiple);
        _curHpTMP.text = string.Format("HP : {0}x", gm.hpMultiple);
    }

    public void OnTouchSetDmgButton()
    {
        string text = _dmgMultipleInputField.text;
        if (string.IsNullOrEmpty(text))
            return;
        if (float.TryParse(text, out float multiple))
        {
            if(multiple > 0f)
            {
                GameManager.GetInstance.dmgMultiple = multiple;
                SetCurrentStatsText();
            }
        }
    }

    public void OnTouchSetHpButton()
    {
        string text = _hpMultipleInputField.text;
        if (string.IsNullOrEmpty(text))
            return;
        if (float.TryParse(text, out float multiple))
        {
            if(multiple > 0f)
            {
                GameManager.GetInstance.hpMultiple = multiple;
                SetCurrentStatsText();
            }
        }
    }
}
