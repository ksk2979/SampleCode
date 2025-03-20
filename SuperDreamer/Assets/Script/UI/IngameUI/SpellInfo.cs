using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] _infoText;

    public void OnInfo(double damage, float maxCoolTime)
    {
        gameObject.SetActive(true);
        _infoText[0].text = damage.ToString();
        _infoText[1].text = string.Format("{0}s", maxCoolTime.ToString());
    }
    public void CloseInfo()
    {
        gameObject.SetActive(false);
    }
}
