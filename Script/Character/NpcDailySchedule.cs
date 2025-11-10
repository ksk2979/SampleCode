using System;
using System.Collections.Generic;
using UnityEngine;

public class NpcDailySchedule : MonoBehaviour
{
    GameTimeManager _gameTimeManager;
    NpcController _npc;

    [Header("Schedule (Daily Repeat)")]
    [SerializeField] List<Entry> _entries = new List<Entry>();

    [Header("Options")]
    [Tooltip("시작할 때 현재 시각 이후 항목만 실행 (이전 시각들은 지나간 것으로 간주)")]
    [SerializeField] bool _skipPastOnStart = false;

    [Tooltip("같은 분(예: 09:00)에 여러 항목이 있으면 모두 실행할지 여부")]
    [SerializeField] bool _allowMultipleAtSameMinute = true;

    [Tooltip("에디터에서 자동 정렬(시->분)")]
    [SerializeField] bool _autoSortInEditor = true;

    [Serializable]
    public class Entry
    {
        [Header("When")]
        public string _note;    // 메모
        public TimeOfDay _time; // HH:MM
        //[Tooltip("조건 태그(선택). 비어있으면 항상 실행")]
        //public string _conditionTag;

        [Header("What (Phases)")]
        [Tooltip("시작, 중간, 종료 구간별 애니/이동 설정")]
        public EntryAnimSet _animSet;

        [NonSerialized] public int _seqIndex;
    }
    [Serializable]
    public struct EntryAnimSet
    {
        [Header("Start Phase (시작 구간)")]
        [Tooltip("시작 애니메이션 명령들")]
        public List<AnimCommand> _startAnims;

        [Tooltip("시작 구간에서 이동할 위치들")]
        public List<Vector3> _startTargets;

        [Header("Middle Phase (중간 구간)")]
        [Tooltip("중간 애니메이션 명령들")]
        public List<AnimCommand> _middleAnims;

        [Tooltip("중간 구간에서 이동할 위치들")]
        public List<Vector3> _middleTargets;

        [Header("End Phase (끝 구간)")]
        [Tooltip("엔딩 애니메이션 명령들")]
        public List<AnimCommand> _endAnims;

        [Tooltip("엔딩 구간에서 이동할 위치들")]
        public List<Vector3> _endTargets;
    }
    [Serializable]
    public struct AnimCommand
    {
        public enum Kind { Trigger, Bool, TriggerAndBool }
        public Kind _kind;

        [Tooltip("Trigger/Bool 파라미터 이름")]
        public string _triggerName;
        public string _boolName;

        [Tooltip("이전 애니 실행 후 대기 시간(초)")]
        public float _delayAfter;
    }

    // 오늘 이미 처리한 (Hour, Minute) 캐시 (중복 방지용)
    HashSet<int> _firedToday = new HashSet<int>();
    int _firedDay = -1;

    public void Init()
    {
        _gameTimeManager = UIManager.GetInstance.GetGameTimeManager;
        _npc = this.GetComponent<NpcController>();
        if (_autoSortInEditor) { SortEntries(); }

        if (_gameTimeManager == null || _npc == null)
        {
            Debug.Log($"[{name}] NpcDailySchedule: 참조가 비어있음");
            return;
        }

        _gameTimeManager.OnMinuteChanged += OnMinuteChanged;

        // 시작 시 Day 캐시 초기화를 한다
        var t = _gameTimeManager.GetGameTime();
        _firedDay = t.Day;
        _firedToday.Clear();

        // 옵션 항목 시작 시각이 이미 지난 항목은 스킵한다
        if (_skipPastOnStart)
        {
            for (int i = 0; i < _entries.Count; ++i)
            {
                var e = _entries[i];
                if (e == null) continue;

                if (IsPast(t, e._time))
                {
                    MarkFired(e._time);
                }
            }
        }

        EvaluateMinute(t);
    }

    void OnDisable()
    {
        if (_gameTimeManager != null)
            _gameTimeManager.OnMinuteChanged -= OnMinuteChanged;
    }

    void OnValidate()
    {
        if (_autoSortInEditor) { SortEntries(); }
    }

    void SortEntries()
    {
        _entries.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;
            if (a._time._hour != b._time._hour) return a._time._hour.CompareTo(b._time._hour);
            return a._time._minute.CompareTo(b._time._minute);
        });
    }

    void OnMinuteChanged(DateTime t)
    {
        // 날짜가 바뀌면 캐시 리셋(매일 반복)
        if (_firedDay != t.Day)
        {
            _firedDay = t.Day;
            _firedToday.Clear();
        }

        EvaluateMinute(t);
    }

    // 현재 분을 검사하여 항목 실행
    void EvaluateMinute(DateTime t)
    {
        for (int i = 0; i < _entries.Count; i++)
        {
            var e = _entries[i];
            if (e == null) continue;

            if (e._time.EqualsHM(t))
            {
                if (!_allowMultipleAtSameMinute && WasFired(e._time)) continue;
                if (WasFired(e._time)) continue;

                // 조건 체크 하는것 근데 지금 하는게 없어서 일단 보류
                //if (!PassCondition(e._conditionTag)) { MarkFired(e._time); continue; }

                //_npc.RunDailyRoutine(e._animSet);

                MarkFired(e._time);
            }
        }
    }
    bool PassCondition(string conditionTag)
    {
        if (string.IsNullOrEmpty(conditionTag)) return true;

        return true;
    }

    bool WasFired(TimeOfDay tod)
    {
        return _firedToday.Contains(tod._hour * 60 + tod._minute);
    }

    void MarkFired(TimeOfDay tod)
    {
        _firedToday.Add(tod._hour * 60 + tod._minute);
    }

    // t 시각보다 항목 시각이 과거인가?
    bool IsPast(DateTime t, TimeOfDay tod)
    {
        if (tod._hour < t.Hour) return true;
        if (tod._hour > t.Hour) return false;
        return tod._minute < t.Minute;
    }
}
