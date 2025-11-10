using UnityEditor.VersionControl;
using UnityEngine;

// 가해지는 직장상자 메인 빌런

public class MainNpcController : NpcController
{
    [SerializeField] float _stressGauge = 0f;
    UIManager _uiManager;

    private void Start()
    {
        OnStart();
    }

    public override void OnStart()
    {
        base.OnStart();
        _uiManager = UIManager.GetInstance;
        if (_uiCheck) { _uiManager.StressGaugeUpdate(_stressGauge); }
    }

    bool _uiCheck { get { if (_uiManager != null) { return true; } else { return false; } } }
}
