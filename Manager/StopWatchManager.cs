using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;

public class StopWatchManager
{
    private Stopwatch _stopwatch;
    private bool _isRunning;

    public StopWatchManager()
    {
        _stopwatch = new Stopwatch();
        _isRunning = false;
    }

    public void StartStopwatch()
    {
        _stopwatch.Start();
        _isRunning = true;
    }

    public void StopStopwatch()
    {
        _stopwatch.Stop();
        _isRunning = false;
    }
    public void ResetStopwatch(TextMeshProUGUI timeText)
    {
        _stopwatch.Reset();
        UpdateStopwatchText(timeText);
    }

    public void UpdateStopwatchText(TextMeshProUGUI timeText)
    {
        timeText.text = StopwatchTextReturn();
    }
    public string StopwatchTextReturn()
    {
        return string.Format("{0:00}:{1:00}:{2:00}",
            _stopwatch.Elapsed.Minutes,
            _stopwatch.Elapsed.Seconds,
            _stopwatch.Elapsed.Milliseconds / 10);
    }

    public bool IsRunning => _isRunning;
}
