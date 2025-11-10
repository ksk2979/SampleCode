using System;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    [Header("Time Scale")]
    [Tooltip("현실 1초당 게임이 흐를 초 수. (기본: 16 = 현실1초 -> 게임16초)")]
    [SerializeField] float _gameSecondsPerRealSecond = 16f;

    [Header("Start Time (Game)")]
    [SerializeField] int _startYear = 2025; // 년
    [SerializeField] int _startMonth = 10; // 월
    [SerializeField] int _startDay = 1; // 일
    [SerializeField] int _startHour = 8; // 시
    [SerializeField] int _startMinute = 0; // 분
    [SerializeField] int _startSecond = 0; // 초

    [Header("Options")]
    [SerializeField] bool _useUnscaledTime = true;
    [SerializeField] bool _startPaused = false;

    DateTime _gameTime;
    bool _paused;
    int _lastMinute;
    int _lastHour;
    int _lastDay;

    public event Action<DateTime> OnMinuteChanged;
    public event Action<DateTime> OnHourChanged;
    public event Action<DateTime> OnDayChanged;

    public void Init()
    {
        _gameTime = new DateTime(_startYear, _startMonth, _startDay, _startHour, _startMinute, _startSecond);
        _lastMinute = _gameTime.Minute;
        _lastHour = _gameTime.Hour;
        _lastDay = _gameTime.Day;
        _paused = _startPaused;
    }

    void Update()
    {
        if (_paused) return;

        float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        double addSec = dt * _gameSecondsPerRealSecond;

        _gameTime = _gameTime.AddSeconds(addSec);

        if (_gameTime.Minute != _lastMinute)
        {
            _lastMinute = _gameTime.Minute;
            OnMinuteChanged?.Invoke(_gameTime);
        }
        if (_gameTime.Hour != _lastHour)
        {
            _lastHour = _gameTime.Hour;
            OnHourChanged?.Invoke(_gameTime);
        }
        if (_gameTime.Day != _lastDay)
        {
            _lastDay = _gameTime.Day;
            OnDayChanged?.Invoke(_gameTime);
        }
    }

    // 현실 X분 = 게임 Y시간 으로 직관 설정
    public void SetScaleByMapping(float realMinutes, float gameHours)
    {
        if (realMinutes <= 0f) realMinutes = 0.0001f;
        _gameSecondsPerRealSecond = (gameHours * 3600f) / (realMinutes * 60f);
    }

    // 직접 배속 설정(현실1초당 게임초)
    public void SetScale(float gameSecondsPerRealSecond)
    {
        _gameSecondsPerRealSecond = Mathf.Max(0f, gameSecondsPerRealSecond);
    }

    public float GetScale() => _gameSecondsPerRealSecond;

    public void Pause(bool pause) => _paused = pause;
    public bool IsPaused() => _paused;

    public DateTime GetGameTime() => _gameTime;

    public void SetGameTime(DateTime newTime)
    {
        _gameTime = newTime;
        _lastMinute = _gameTime.Minute;
        _lastHour = _gameTime.Hour;
        _lastDay = _gameTime.Day;
        OnMinuteChanged?.Invoke(_gameTime);
        OnHourChanged?.Invoke(_gameTime);
        OnDayChanged?.Invoke(_gameTime);
    }

    public string GetTimeHHMM()
    {
        return _gameTime.ToString("HH:mm");
    }

    public string GetDateYYYYMMDD_HHMM()
    {
        return _gameTime.ToString("yyyy-MM-dd HH:mm");
    }
}
