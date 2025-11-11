using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : SceneStaticObj<UIManager>
{
    GameTimeManager _gameTimeManager;
    DayCycleManager _dayCycleManager;
    [SerializeField] TextMeshProUGUI[] _timeTextArr; // 0 day, 1 hour, 2 minute
    [SerializeField] Image _stressGaugeImg;
    [SerializeField] InventoryUI _inventoryUI;
    [SerializeField] AimScript _aimScript;
    [SerializeField] OptionManager _optionManager;
    [SerializeField] CursorManager _cursorManager;

    private void Awake()
    {
        _gameTimeManager = this.GetComponent<GameTimeManager>();
        _gameTimeManager.Init();
        _gameTimeManager.SetScaleByMapping(30f, 8f);
        _dayCycleManager = this.GetComponent<DayCycleManager>();
        _dayCycleManager.Init(_gameTimeManager);

        _gameTimeManager.OnMinuteChanged += UpdateUI;
        _gameTimeManager.OnHourChanged += UpdateUI;
        _gameTimeManager.OnDayChanged += UpdateUI;
        _inventoryUI.Init();
        _optionManager.Init();
        _cursorManager.Init();
    }

    private void OnDisable()
    {
        if (_gameTimeManager != null)
        {
            _gameTimeManager.OnMinuteChanged -= UpdateUI;
            _gameTimeManager.OnHourChanged -= UpdateUI;
            _gameTimeManager.OnDayChanged -= UpdateUI;
        }
    }

    void UpdateUI(DateTime t)
    {
        if (_timeTextArr == null || _timeTextArr.Length < 3) return;

        _timeTextArr[0].text = t.Day.ToString("00"); // day
        _timeTextArr[1].text = t.Hour.ToString("00");
        _timeTextArr[2].text = t.Minute.ToString("00");
    }

    public void StressGaugeUpdate(float gauge)
    {
        _stressGaugeImg.fillAmount = gauge;
    }

    public DayCycleManager GetDayCycleManager => _dayCycleManager;
    public GameTimeManager GetGameTimeManager => _gameTimeManager;
    public InventoryUI GetInventoryUI => _inventoryUI;
    public AimScript GetAim => _aimScript;
    public OptionManager GetOptionManager => _optionManager;
    public CursorManager GetCursorManager => _cursorManager;
}
