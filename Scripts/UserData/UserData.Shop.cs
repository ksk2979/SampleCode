using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UserData : Singleton<UserData>
{
    // 패키지 여부
    public void PackageSave(int num)
    {
        PackageCheck(num);
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.PACKAGEDATA, true);
        if (_server) { Debug.Log("서버_리워드타임세이브"); GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.UTILITYDATA); } // GameManager.GetInstance.GetServerSaveListAdd(EUserSaveType.REWARDTIMEDATA, 0, true);
    }

    public void PackageCheck(int num)
    {
        if (num == 0)
        {
            _stRewardData._package0 = 1;
        }
        else if (num == 1)
        {
            _stRewardData._package1 = 1;
        }
        else
        {
            _stRewardData._package2 = 1;
        }
    }

    public bool PackageAllCheck()
    {
        if (_stRewardData._package0 == 1 && _stRewardData._package1 == 1 && _stRewardData._package2 == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Daliy Shop Product Receive State
    public void ResetShopDailyReceiveState()
    {
        Debug.Log("ResetShopDailyReceiveState");
        for (int i = 0; i < _stDailyRewardData._shopReceiveCount.Length; ++i)
        {
            _stDailyRewardData._shopReceiveCount[i] = 0;
        }
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.DAILYREWARDDATA, true);
    }
    public int GetShopDailyReceivedState(int arr) => _stDailyRewardData._shopReceiveCount[arr];
    public void SetShopOneDayReceivedState(int arr, int value)
    {
        _stDailyRewardData._shopReceiveCount[arr] = value;
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.DAILYREWARDDATA, true);
    }

    // Daily Shop Product Data
    public void SetShopDailyOneKey(int arr, int value) => _stDailyRewardData._shopItemOneKey[arr] = value;
    public void SetShopDailyTwoKey(int arr, int value) => _stDailyRewardData._shopItemTwoKey[arr] = value;
    public void SetShopDailyReceiveCount(int arr, int value) => _stDailyRewardData._shopReceiveCount[arr] = value;

    public int GetShopDailyOneKey(int arr) => _stDailyRewardData._shopItemOneKey[arr];
    public int GetShopDailyTwoKey(int arr) => _stDailyRewardData._shopItemTwoKey[arr];
    public int GetShopDailyReceiveCount(int arr) => _stDailyRewardData._shopReceiveCount[arr];
    
    // Login Reward Data
    public void SetDailyLoginCount(int count) => _stDailyRewardData._loginRewardCount = count;
    public int GetDailyLoginCount() => _stDailyRewardData._loginRewardCount;

    // Roulette Data
    public void SetRouletteCount(int count) => _stDailyRewardData._rouletteCount = count;
    public int GetRouletteCount() => _stDailyRewardData._rouletteCount;
}
