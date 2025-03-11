using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UserData
{
    /// <summary>
    /// 시즌 보상 수령 여부 체크
    /// </summary>
    /// <param name="level">패스 레벨</param>
    /// <param name="isVIP">VIP 여부</param>
    /// <returns></returns>
    public bool GetSeasonState(int level, bool isVIP = false)
    {
        if (isVIP)
        {
            return _stSeasonPassData._vipRewardStateList[level - 1];
        }
        else
        {
            if (_stSeasonPassData._level < level || level <= 0)
            {
                return false;
            }
            return _stSeasonPassData._normalRewardStateList[level - 1];
        }
    }

    /// <summary>
    /// 시즌 보상 수령 여부 설정
    /// </summary>
    /// <param name="level">패스 레벨</param>
    /// <param name="state">수령 상태</param>
    /// <param name="isVIP">VIP 여부</param>
    public void SetSeasonState(int level, bool state, bool isVIP = false)
    {
        if (isVIP)
        {
            _stSeasonPassData._vipRewardStateList[level - 1] = state;
        }
        else
        {
            _stSeasonPassData._normalRewardStateList[level - 1] = state;
        }

    }

    /// <summary>
    /// 시즌패스 레벨업
    /// </summary>
    public void LevelUpSeasonPass(int addLevel, int seasonMaxLevel = 15)
    {
        _stSeasonPassData._level += addLevel;
        if (_stSeasonPassData._level > seasonMaxLevel)
        {
            _stSeasonPassData._level = seasonMaxLevel;
        }
    }

    /// <summary>
    /// 해당 레벨의 항목이 유효한지 체크(F : 미수령 / T : 수령)
    /// </summary>
    /// <param name="itemLevel">항목 레벨</param>
    /// <param name="isVip">VIP 여부</param>
    /// <returns></returns>
    public bool CheckPassItemValidation(int itemLevel, bool isVip = false)
    {
        if (isVip)
        {
            return _stSeasonPassData._vipRewardStateList[itemLevel - 1];
        }
        else
        {
            if (GetSeasonLevel < itemLevel)
            {
                return false;
            }
            return _stSeasonPassData._normalRewardStateList[itemLevel - 1];
        }
    }

    /// <summary>
    /// VIP 상태 전환
    /// </summary>
    /// <param name="state">상태</param>
    public void SetVIPState(bool state)
    {
        if (_stSeasonPassData._isVipActivated) return;

        _stSeasonPassData._isVipActivated = state;
    }

    /// <summary>
    /// 시즌패스 갱신(시즌 변경에 따른 정보 변경)
    /// </summary>
    /// <param name="season">시즌 넘버</param>
    public void SetSeasonInfo(int season)
    {
        _stSeasonPassData.Init();
        _stSeasonPassData._currentSeason = season;
        _stSeasonPassData._isActivated = true;
    }

    /// <summary>
    /// 시즌패스 초기화
    /// </summary>
    public void ResetSeasonPass()
    {
        _stSeasonPassData.Init();
    }

    public int GetCurrentSeason => _stSeasonPassData._currentSeason;    // 현재 시즌
    public int GetSeasonLevel => _stSeasonPassData._level;  // 현재 레벨

    // 다음 레벨
    public int GetNextPassLevel(int seasonMaxLevel = 15)
    {
        if (_stSeasonPassData._level >= seasonMaxLevel)
        {
            return 0;
        }
        else
        {
            return _stSeasonPassData._level + 1;
        }
    }
    // 현재 달성된 정도(킬수)
    public int GetCurrentProgress => _stSeasonPassData._progressCount;

    /// <summary>
    /// 진행 상황 정보 변경
    /// </summary>
    /// <param name="count">수량</param>
    public void SetCurrentProgress(int count)
    {
        _stSeasonPassData._progressCount = count;
    }

    // vip 활성화 여부
    public bool IsVipActivated => _stSeasonPassData._isVipActivated;
    // 시즌패스 활성화 여부
    public bool IsPassActivated => _stSeasonPassData._isActivated;
}
