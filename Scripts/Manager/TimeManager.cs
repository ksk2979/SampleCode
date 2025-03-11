using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManager : Singleton<TimeManager>
{
    int _year;
    int _month;
    int _day;
    int _hour;
    int _minute;
    int _second;

    bool _timeStopWatchPause = false;
    int _watchSecond = 0;
    int _watchMinute = 0;
    string _watchText;

    public int TimeYear { get { return _year; } }
    public int TimeMonth { get { return _month; } }
    public int TimeDay { get { return _day; } }
    public int TimeHour { get { return _hour; } }
    public int TimeMinute { get { return _minute; } }
    public int TimeSecond { get { return _second; } }
    public int WatchSecond { get { return _watchSecond; } }
    public int WatchMinute { get { return _watchMinute; } }

    protected override void Awake()
    {
        base.Awake();
    }

    IEnumerator TimeUpdate()
    {
        yield return YieldInstructionCache.WaitForSeconds(1f);

        _second++;
        if (_second > 59)
        {
            _second = 0;
            _minute++;
            if (_minute > 59)
            {
                _minute = 0;
                _hour++;
                if (_hour > 23)
                {
                    _day++;
                    _hour = 0;
                    yield break;
                }
            }
        }

        StartCoroutine(TimeUpdate());
    }

    void DayUpdate()
    {
        switch (_month)
        {
            case 1:
                if (_day > 31)
                {
                    _month++;
                    _day = 1;
                }
                break;
            case 2:
                if (FebruaryFourYear())
                {
                    if (_day > 29)
                    {
                        _month++;
                        _day = 1;
                    }
                }
                else
                {
                    if (_day > 28)
                    {
                        _month++;
                        _day = 1;
                    }
                }
                break;
            case 3:
                if (_day > 31)
                {
                    _month++;
                    _day = 1;
                }
                break;
            case 4:
                if (_day > 30)
                {
                    _month++;
                    _day = 1;
                }
                break;
            case 5:
                if (_day > 31)
                {
                    _month++;
                    _day = 1;
                }
                break;
            case 6:
                if (_day > 30)
                {
                    _month++;
                    _day = 1;
                }
                break;
            case 7:
                if (_day > 31)
                {
                    _month++;
                    _day = 1;
                }
                break;
            case 8:
                if (_day > 31)
                {
                    _month++;
                    _day = 1;
                }
                break;
            case 9:
                if (_day > 30)
                {
                    _month++;
                    _day = 1;
                }
                break;
            case 10:
                if (_day > 31)
                {
                    _month++;
                    _day = 1;
                }
                break;
            case 11:
                if (_day > 30)
                {
                    _month++;
                    _day = 1;
                }
                break;
            case 12:
                if (_day > 31)
                {
                    _year++;
                    _month++;
                    _day = 1;
                }
                break;
        }
    }

    public bool FebruaryFourYear()
    {
        if (_year == 2024 || _year == 2028 || _year == 2032 || _year == 2036 || _year == 2040 || _year == 2044 || _year == 2048 || _year == 2052 || _year == 2056 || _year == 2060 || _year == 2064 || _year == 2068 || _year == 2072
                    || _year == 2076 || _year == 2080 || _year == 2084 || _year == 2088 || _year == 2092 || _year == 2096 || _year == 2100)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int DayFuntion(int month)
    {
        if (month == 0) { month = 12; }
        switch (month)
        {
            case 1:
                return 31;
            case 2:
                if (FebruaryFourYear()) { return 29; }
                else { return 28; }
            case 3:
                return 31;
            case 4:
                return 30;
            case 5:
                return 31;
            case 6:
                return 30;
            case 7:
                return 31;
            case 8:
                return 31;
            case 9:
                return 30;
            case 10:
                return 31;
            case 11:
                return 30;
            case 12:
                return 31;
            default:
                return 0;
        }
    }

    public void DayTimeCompare()
    {
        // 시작 시간 저장
        DateTime startTime = DateTime.Now; // 현재 시간을 저장
        PlayerPrefs.SetString("stTime_1", startTime.ToString()); // 유니티 저장으로 string 저장

        // 현재 시간과 시작 시간 비교
        string timeStr = PlayerPrefs.GetString("stTime_1");
        DateTime startT = Convert.ToDateTime(timeStr);

        DateTime currentTime = DateTime.Now;
        TimeSpan timeDir = currentTime - startT;

        if (timeDir.Days == 0)
        {
            // 아직 하루가 안지남
        }
        else if (timeDir.Days > 0)
        {
            // 하루가 지남 (여기서 days는 24시간이 지나야 1일이 지난걸로 인식을 하기 때문에 현실에서 하루가 지났다고 해도 실제로는 12시간밖에 안지났다고 생각한다)
        }

        //timeDir.Hours 시간보상으로 줄때 사용 1시간 간격으로 보상을 줄때 사용하면될듯
    }

    // 오늘 하루 시간에 00:00 을 맞춰주는 함수
    public DateTime DayFirstTime()
    {
        DateTime startTime = DateTime.Now; // 현재 시간을 저장
        string saveTime = startTime.ToString("yyyyMMddHHmm");
        string[] saveTimeArr = { "", "", "" };
        saveTimeArr[0] = saveTime.Substring(0, 4);
        saveTimeArr[1] = saveTime.Substring(4, 2);
        saveTimeArr[2] = saveTime.Substring(6, 2);
        return Convert.ToDateTime(string.Format("{0}/{1}/{2} 00:00", saveTimeArr[0], saveTimeArr[1], saveTimeArr[2]));
    }

    bool _stopWatchReset = false;
    public void TimeStopWatchStart()
    {
        _stopWatchReset = false;
        _timeStopWatchPause = false;
        _watchSecond = 0;
        _watchMinute = 0;
        StartCoroutine(StopWatchUpdate());
    }
    IEnumerator StopWatchUpdate()
    {
        yield return YieldInstructionCache.WaitForSeconds(1f);

        while (_timeStopWatchPause)
        {
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }

        if (_stopWatchReset) { yield break; }

        _watchSecond++;
        if (_watchSecond > 59)
        {
            _watchSecond = 0;
            _watchMinute++;
        }

        StartCoroutine(StopWatchUpdate());
    }
    public void ResetStopWatch()
    {
        _watchSecond = 0;
        _watchMinute = 0;
        _stopWatchReset = true;
    }
    public string StopWatchText()
    {
        //_watchText = "";

        _watchText = string.Format("{0}:{1}", _watchMinute.ToString("00"), _watchSecond.ToString("00"));

        return _watchText;
    }
    public void TimeStopWatchPause()
    {
        _timeStopWatchPause = _timeStopWatchPause ? false : true;
    }
    public void TestStopWatchAdd()
    {
        _watchSecond = 54;
    }
    public bool GetTimeStopWatchPause { get { return _timeStopWatchPause; } }
    /*
    다르게 표현
    _time += Time.deltaTime;
    _timeText.text = ((int)_time / 3600).toString();
    _timeText1.text = ((int)_time / 60 % 60).toString();
    _timeText2.text = ((int)_time / % 60).toString();
     */
}
