using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] TextMeshProUGUI _soulText;
    [SerializeField] TextMeshProUGUI _spellText;

    public void UpdateText(int arr, int count)
    {
        if (arr == 0) { _goldText.text = string.Format("G: {0}", count); }
        else if (arr == 1) { _soulText.text = string.Format("So: {0}", count); }
        else { _spellText.text = string.Format("Sp: {0}/25", count); }
    }
}
