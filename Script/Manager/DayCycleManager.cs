using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DayCycleManager : MonoBehaviour
{
    [Header("Referencec")]
    GameTimeManager _gameTimeManager;
    [SerializeField] FadeInOutScript _dayTransitionFade;
    [SerializeField] TextMeshProUGUI _dayTransitionTitle;
    [SerializeField] TextMeshProUGUI _dayTransitionSubtitle;
    [SerializeField] GameObject _gameEndingRoot;
    [SerializeField] TextMeshProUGUI _gameEndingText;

    [Header("Day Setting")]
    [SerializeField] int _totalDays = 7;
    [SerializeField] int _dayStartHour = 9;
    [SerializeField] int _dayStartMinute = 0;
    [SerializeField] int _dayStartSecond = 0;
    [SerializeField] int _dayEndHour = 18;
    [SerializeField] int _dayEndMinute = 0;

    [Header("Transition Timings (seconds)")]
    [SerializeField] float _fadeInWait = 1f;
    [SerializeField] float _betweenDayWait = 1.5f;
    [SerializeField] float _fadeOutWait = 1f;

    [Header("Engding Text")]
    [SerializeField] string _endingMessage = "7일이 모두 지나 게임이 종료되었습니다";

    readonly List<IDayResettable> _dayResettables = new List<IDayResettable>();
    int _dayCompleted;
    int _currentDayNumber = 1;
    bool _isTransitioning;
    float _cachedTimeScale = 1f;


    public void Init(GameTimeManager timeManager)
    {
        _gameTimeManager = timeManager;
        if (_gameTimeManager != null)
        {
            _currentDayNumber = Mathf.Clamp(_gameTimeManager.GetGameTime().Day, 1, _totalDays);
        }
        else
        {
            _currentDayNumber = Mathf.Clamp(_currentDayNumber, 1, _totalDays);
        }

        _dayCompleted = Mathf.Clamp(_currentDayNumber - 1, 0, _totalDays);

        if (_gameTimeManager != null)
        {
            _gameTimeManager.OnMinuteChanged += HandleMinuteChanged;
        }

        RefreshTransitionTexts();
    }

    private void OnDisable()
    {
        if (_gameTimeManager != null)
        {
            _gameTimeManager.OnMinuteChanged -= HandleMinuteChanged;
        }

        if (_isTransitioning)
        {
            Time.timeScale = _cachedTimeScale;
            _isTransitioning = false;
        }
    }
    
    public void RegisterResettable(IDayResettable resettable)
    {
        if (resettable == null) return;
        if (_dayResettables.Contains(resettable)) return;
        _dayResettables.Add(resettable);
    }

    public void UnregisterResettable(IDayResettable resettable)
    {
        if (resettable == null) return;
        _dayResettables.Remove(resettable);
    }

    public int CurrentDay => _currentDayNumber;
    public int TotalDay => _totalDays;
    public int DaysRemaining => Mathf.Max(0, _totalDays - _dayCompleted);

    void HandleMinuteChanged(DateTime currentTime)
    {
        //Debug.Log($"{currentTime.Hour}, {currentTime.Minute}, {currentTime.Second}");
        if (_isTransitioning) { return; }
        if (_gameTimeManager != null && _gameTimeManager.IsPaused()) { return; }

        if (IsEndOfDay(currentTime))
        {
            StartCoroutine(DayTransitionRoutine());
        }
    }
    bool IsEndOfDay(DateTime time)
    {
        if (time.Hour > _dayEndHour) { return true; }
        if (time.Hour < _dayEndHour) { return false; }
        return time.Minute >= _dayEndMinute;
    }

    IEnumerator DayTransitionRoutine()
    {
        _isTransitioning = true;
        _cachedTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        if (_gameTimeManager != null)
        {
            _gameTimeManager.Pause(true);
        }

        ShowDayTransitionUI();

        if (_dayTransitionFade != null)
        {
            _dayTransitionFade.FadeIn();
        }

        if (_fadeInWait > 0f)
        {
            yield return YieldInstructionCache.RealTimeWaitForSeconds(_fadeInWait);
        }

        bool finishedAllDays = CompleteDayAndPrepareNext();

        if (finishedAllDays) { yield break; }
        if (_betweenDayWait > 0f)
        {
            yield return YieldInstructionCache.RealTimeWaitForSeconds(_betweenDayWait);
        }

        if (_dayTransitionFade != null)
        {
            _dayTransitionFade.FadeOut();
        }

        if (_fadeOutWait > 0f)
        {
            yield return YieldInstructionCache.RealTimeWaitForSeconds(_fadeOutWait);
        }

        if (_gameTimeManager != null)
        {
            _gameTimeManager.Pause(false);
        }

        Time.timeScale = _cachedTimeScale;
        _isTransitioning = false;
    }

    void ShowDayTransitionUI()
    {
        if (_dayTransitionTitle != null)
        {
            _dayTransitionTitle.text = string.Format("{0}일차 종료", _currentDayNumber);
        }

        UpdateRemainingDaysText(DaysRemaining - 1);
    }

    bool CompleteDayAndPrepareNext()
    {
        _dayCompleted = Mathf.Clamp(_dayCompleted + 1, 0, _totalDays);
        int daysRemaining = Mathf.Max(0, _totalDays - _dayCompleted);

        UpdateRemainingDaysText(daysRemaining);

        if (_dayCompleted >= _totalDays)
        {
            ShowGameEndingUI(daysRemaining);
            return true;
        }

        _currentDayNumber = Mathf.Min(_currentDayNumber + 1, _totalDays);

        if (_gameTimeManager != null)
        {
            DateTime nextDay = _gameTimeManager.GetGameTime().Date.AddDays(1)
                .AddHours(_dayStartHour)
                .AddMinutes(_dayStartMinute)
                .AddSeconds(_dayStartSecond);

            _gameTimeManager.SetGameTime(nextDay);
        }

        if (_dayTransitionTitle != null)
        {
            _dayTransitionTitle.text = string.Format("{0}일차 시작", _currentDayNumber);
        }

        ResetDayObject();

        return false;
    }

    void ResetDayObject()
    {
        for (int i = _dayResettables.Count - 1; i >= 0; i--)
        {
            if (_dayResettables[i] == null)
            {
                _dayResettables.RemoveAt(i);
                continue;
            }

            _dayResettables[i].ResetForNewDay();
        }
    }

    void ShowGameEndingUI(int daysremaining)
    {
        if (_gameEndingRoot != null)
        {
            _gameEndingRoot.SetActive(true);
        }

        if (_gameEndingText!= null)
        {
            string message = string.IsNullOrEmpty(_endingMessage) ? "모든 날이 종료되었습니다" : _endingMessage;

            _gameEndingText.text = message;
        }
    }

    void UpdateRemainingDaysText(int daysremaining)
    {
        if (_dayTransitionSubtitle == null) { return; }
        daysremaining = Mathf.Max(0, daysremaining);
        _dayTransitionSubtitle.text = string.Format("남은 기간: {0}일", daysremaining);
    }

    void RefreshTransitionTexts()
    {
        UpdateRemainingDaysText(DaysRemaining);
        if (_dayTransitionTitle != null)
        {
            _dayTransitionTitle.text = string.Format("{0}일차", _currentDayNumber);
        }
    }
}
