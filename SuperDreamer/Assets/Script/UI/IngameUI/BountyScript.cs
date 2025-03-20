using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics.CodeAnalysis;

public class BountyScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _timeText;
    float _time = 60f;
    bool _on = false;
    
    public void TimeUpdate()
    {
        if (_on) { return; }
        _time -= Time.deltaTime;
        if (_timeText != null) { _timeText.text = _time.ToString("F0"); }
        if (_time < 0f)
        {
            _time = 60f;
            _on = true;
        }
    }

    public void BountyBtn(int index)
    {
        if (_on)
        {
            _on = false;
            // 현상금 적 생성
            StageManager.GetInstance.BountyCreate(index);
        }
        else { MessageHandler.Getinstance.ShowMessage("Not Ready", 2f); }
    }
}
