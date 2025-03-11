using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UserData
{
    /// <summary>
    /// ���� ���� ���� ���� üũ
    /// </summary>
    /// <param name="level">�н� ����</param>
    /// <param name="isVIP">VIP ����</param>
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
    /// ���� ���� ���� ���� ����
    /// </summary>
    /// <param name="level">�н� ����</param>
    /// <param name="state">���� ����</param>
    /// <param name="isVIP">VIP ����</param>
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
    /// �����н� ������
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
    /// �ش� ������ �׸��� ��ȿ���� üũ(F : �̼��� / T : ����)
    /// </summary>
    /// <param name="itemLevel">�׸� ����</param>
    /// <param name="isVip">VIP ����</param>
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
    /// VIP ���� ��ȯ
    /// </summary>
    /// <param name="state">����</param>
    public void SetVIPState(bool state)
    {
        if (_stSeasonPassData._isVipActivated) return;

        _stSeasonPassData._isVipActivated = state;
    }

    /// <summary>
    /// �����н� ����(���� ���濡 ���� ���� ����)
    /// </summary>
    /// <param name="season">���� �ѹ�</param>
    public void SetSeasonInfo(int season)
    {
        _stSeasonPassData.Init();
        _stSeasonPassData._currentSeason = season;
        _stSeasonPassData._isActivated = true;
    }

    /// <summary>
    /// �����н� �ʱ�ȭ
    /// </summary>
    public void ResetSeasonPass()
    {
        _stSeasonPassData.Init();
    }

    public int GetCurrentSeason => _stSeasonPassData._currentSeason;    // ���� ����
    public int GetSeasonLevel => _stSeasonPassData._level;  // ���� ����

    // ���� ����
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
    // ���� �޼��� ����(ų��)
    public int GetCurrentProgress => _stSeasonPassData._progressCount;

    /// <summary>
    /// ���� ��Ȳ ���� ����
    /// </summary>
    /// <param name="count">����</param>
    public void SetCurrentProgress(int count)
    {
        _stSeasonPassData._progressCount = count;
    }

    // vip Ȱ��ȭ ����
    public bool IsVipActivated => _stSeasonPassData._isVipActivated;
    // �����н� Ȱ��ȭ ����
    public bool IsPassActivated => _stSeasonPassData._isActivated;
}
