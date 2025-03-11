using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UserData : Singleton<UserData>
{
    #region Daily Quest
    /// <summary>
    /// 일일퀘스트 초기화
    /// </summary>
    public void ResetDailyQuest()
    {

        for (int i = 0; i < (int)EDailyQuest.NONE; i++)
        {
            // AllClear 7회 제외 초기화
            if (i < (int)EDailyQuest.SEVENCLEAR)
            {
                _stQuestData.dailyQuestList[i] = 0;
                _stQuestData.dailyReceivedList[i] = false;
            }
            else
            {
                // AllClear 7회 초기화 (7회 달성 이후 수령 했다면)
                if(i == (int)EDailyQuest.SEVENCLEAR && _stQuestData.dailyReceivedList[i])
                {
                    _stQuestData.dailyQuestList[i] = 0;
                    _stQuestData.dailyReceivedList[i] = false;
                }
            }
        }
        // 로그인
        _stQuestData.dailyQuestList[0] = 1;
        SaveQuestData();
    }

    /// <summary>
    /// 일일 퀘스트 기록 (외부에서 세이브 실행 필요)
    /// </summary>
    /// <param name="questType">퀘스트 타입</param>
    /// <param name="value">퀘스트 수치</param>
    public void SetDailyQuest(EDailyQuest questType, int value)
    {
        if (questType == EDailyQuest.NONE) return;
        _stQuestData.dailyQuestList[(int)questType] = value;
    }

    /// <summary>
    /// 일일 퀘스트 수령 상황 등록
    /// </summary>
    /// <param name="questType">퀘스트 타입</param>
    /// <param name="state">보상 수령</param>
    public void SetDailyQuestState(EDailyQuest questType, bool state)
    {
        if (questType == EDailyQuest.NONE) return;
        _stQuestData.dailyReceivedList[(int)questType] = state;
        SaveQuestData(true);
    }

    /// <summary>
    /// 일일 퀘스트 진행 상황 로드
    /// </summary>
    /// <returns></returns>
    public List<int> GetDailyQuest() => _stQuestData.dailyQuestList;

    /// <summary>
    /// 일일 퀘스트 수령 상황 로드
    /// </summary>
    /// <returns></returns>
    public List<bool> GetDailyQuestState() => _stQuestData.dailyReceivedList;

    #endregion Daily Quest

    #region Main Quest
    /// <summary>
    /// 메인 퀘스트 기록
    /// </summary>
    /// <param name="stage">해당 스테이지 넘버</param>
    /// <param name="value">클리어 한 웨이브</param>
    public void SetMainQuest(int stage, int value)
    {
        if (stage <= 0 || stage >= 99) return;
        int num = stage - 1;

        // 해당 스테이지 첫 플레이 기록
        if (_stQuestData.mainQuestList.Count < stage)
        {
            _stQuestData.mainQuestList.Add(value);
        }
        else
        {
            // 해당 스테이지 다회차 플레이 기록
            _stQuestData.mainQuestList[num] = value;
        }
        SaveQuestData();
    }

    /// <summary>
    /// 메인 퀘스트 기록 조회
    /// </summary>
    /// <param name="stage">클리어 한 웨이브</param>
    /// <returns></returns>
    public int GetMainQuest(int stage)
    {
        if (stage <= 0 || stage >= 99) return -1;
        int num = stage - 1;

        // 해당 스테이지 플레이 기록 없음
        if(_stQuestData.mainQuestList.Count < stage)
        {
            SetMainQuest(stage, 0);
        }
        return _stQuestData.mainQuestList[num];
    }

    /// <summary>
    /// 메인 퀘스트 수령 기록 메서드
    /// </summary>
    /// <param name="stage">스테이지</param>
    /// <param name="idx">해당 인덱스</param>
    /// <param name="state">상태</param>
    public void SetMainQuestState(int stage, EMainQuest questType, bool state)
    {
        if (stage <= 0) return;
        int num = (stage - 1) * 3;       // 조회 시작할 첫 인덱스 확인

        int idx = (int)questType - (int)EMainQuest.WAVE3;
        // 퀘스트 수령기록이 없는 경우 생성
        if (_stQuestData.mainReceivedList.Count - 1 < num)
        {
            for (int i = 0; i < 3; i++)
            {
                _stQuestData.mainReceivedList.Add(false);
            }
        }
        _stQuestData.mainReceivedList[num + idx] = state;
        SaveQuestData();
    }

    /// <summary>
    /// 메인 퀘스트 수령 기록 반환
    /// </summary>
    /// <param name="stage">스테이지</param>
    /// <returns></returns>
    public List<bool> GetMainQuestState(int stage)
    {
        if (stage <= 0) return null;
        int num = (stage - 1) * 3;       // 조회 시작할 첫 인덱스 확인
        List<bool> stateList = new List<bool>();

        // 퀘스트 수령기록이 없는 경우
        if (_stQuestData.mainReceivedList.Count - 1 < num)
        {
            SetMainQuestState(stage, EMainQuest.WAVE3, false);
        }

        for (int j = num; j < num + 3; j++)
        {
            stateList.Add(_stQuestData.mainReceivedList[j]);
        }
        return stateList;
    }
    #endregion Main Quest
}
