using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;
using System.Linq;
using System;
using TMPro;

public static class StandardFuncData
{
    public static int DoSelectRandomValue(List<int> list)
    {
        int maxValue = 0;
        for (int i = 0; i < list.Count; i++)
            maxValue += list[i];

        var randomTankOrColleague = StandardFuncData.RandomRange(0, maxValue);
        int sumValue = 0;
        int selectIdx = 0;
        for (int i = 0; i < list.Count; i++)
        {
            sumValue += list[i];
            if (randomTankOrColleague < sumValue)
            {
                selectIdx = i;
                break;
            }
        }
        return selectIdx;
    }
    private const string STR_TIME = "{0:00}:{1:00}:{2:00}";
    internal static string GetStrTime(int m_duration)
    {
        var hour = m_duration / (60 * 60);
        var minute = (m_duration - (hour * (60 * 60))) / 60;
        var second = m_duration - (hour * (60 * 60)) - (minute * 60);
        return string.Format(STR_TIME, hour, minute, second);

        //var timeSpan = new TimeSpan(0, 0, m_duration);
        //return GetStrTime(timeSpan);
    }
    //internal static string GetStrTime(int m_duration)
    //{
    //    var timeSpan = new TimeSpan(0, 0, m_duration);
    //    return GetStrTime(timeSpan);
    //}
    internal static string GetStrTime(TimeSpan ts)
    {
        return string.Format(STR_TIME, ts.Hours, ts.Minutes, ts.Seconds);
    }

    public static int BoatMaxId()
    {
        var lData = DataManager.GetInstance.GetList<BoatData>(DataManager.KEY_BOAT);
        return lData[lData.Count - 1].id1;
    }
    public static int CaptainMaxId()
    {
        var lData = DataManager.GetInstance.GetList<CaptainData>(DataManager.KEY_CAPTAIN);
        return lData[lData.Count - 1].id1;
    }
    public static int DefenseMaxId()
    {
        var lData = DataManager.GetInstance.GetList<DefenseData>(DataManager.KEY_DEFENSE);
        return lData[lData.Count - 1].id1;
    }
    public static int EngineMaxId()
    {
        var lData = DataManager.GetInstance.GetList<EngineData>(DataManager.KEY_ENGINE);
        return lData[lData.Count - 1].id1;
    }
    public static int SailorMaxId()
    {
        var lData = DataManager.GetInstance.GetList<SailorData>(DataManager.KEY_SAILOR);
        return lData[lData.Count - 1].id1;
    }
    public static int WeaponMaxId()
    {
        var lData = DataManager.GetInstance.GetList<WeaponData>(DataManager.KEY_WEAPON);
        return lData[lData.Count - 1].id1;
    }

    /// <summary>
    /// 리스트에서 보트를 찾는다
    /// </summary>
    public static BoatData FindBoatData(int id1, int id2)
    {
        var lData = DataManager.GetInstance.GetList<BoatData>(DataManager.KEY_BOAT);
        var list = lData.Where(x => x.id1 == id1 && x.id2 == id2).ToList();
        if (list == null || list.Count <= 0)
            return null;
        return list[0];
    }
    /// <summary>
    /// 리스트에서 해당 캡틴을 찾는다
    /// </summary>
    public static CaptainData FindCaptainData(int id1, int id2)
    {
        var lData = DataManager.GetInstance.GetList<CaptainData>(DataManager.KEY_CAPTAIN);
        var list = lData.Where(x => x.id1 == id1 && x.id2 == id2).ToList();
        if (list == null || list.Count <= 0)
            return null;
        return list[0];
    }
    /// <summary>
    /// 리스트에서 해당 방어를 찾는다
    /// </summary>
    public static DefenseData FindDefenseData(int id1, int id2)
    {
        var lData = DataManager.GetInstance.GetList<DefenseData>(DataManager.KEY_DEFENSE);
        var list = lData.Where(x => x.id1 == id1 && x.id2 == id2).ToList();
        if (list == null || list.Count <= 0)
            return null;
        return list[0];
    }
    /// <summary>
    /// 리스트에서 해당 엔진을 찾는다
    /// </summary>
    public static EngineData FindEngineData(int id1, int id2)
    {
        var lData = DataManager.GetInstance.GetList<EngineData>(DataManager.KEY_ENGINE);
        var list = lData.Where(x => x.id1 == id1 && x.id2 == id2).ToList();
        if (list == null || list.Count <= 0)
            return null;
        return list[0];
    }
    /// <summary>
    /// 리스트에서 해당 선원을 찾는다
    /// </summary>
    public static SailorData FindSailorData(int id1, int id2)
    {
        var lData = DataManager.GetInstance.GetList<SailorData>(DataManager.KEY_SAILOR);
        var list = lData.Where(x => x.id1 == id1 && x.id2 == id2).ToList();
        if (list == null || list.Count <= 0)
            return null;
        return list[0];
    }
    /// <summary>
    /// 리스트에서 해당 무기를 찾는다
    /// </summary>
    public static WeaponData FindWeaponData(int id1, int id2)
    {
        var lData = DataManager.GetInstance.GetList<WeaponData>(DataManager.KEY_WEAPON);
        var list = lData.Where(x => x.id1 == id1 && x.id2 == id2).ToList();
        if (list == null || list.Count <= 0)
            return null;
        return list[0];
    }

    /// <summary>
    /// 보트 레벨 정보 무시하고 1렙짜리 개수별로 리스트를 리턴한다
    /// </summary>
    /// <returns></returns>
    public static List<BoatData> GetEachOtherBotaData()
    {
        var ist = new List<BoatData>();
        for (int i = 1; i <= BoatMaxId(); i++)
            ist.Add(FindBoatData(i, 1));
        return ist;
    }
    public static List<CaptainData> GetEachOtherCaptainData()
    {
        var ist = new List<CaptainData>();
        for (int i = 1; i <= CaptainMaxId(); i++)
            ist.Add(FindCaptainData(i, 1));
        return ist;
    }
    public static List<DefenseData> GetEachOtherDefenseData()
    {
        var ist = new List<DefenseData>();
        for (int i = 1; i <= DefenseMaxId(); i++)
            ist.Add(FindDefenseData(i, 1));
        return ist;
    }
    public static List<EngineData> GetEachOtherEngineData()
    {
        var ist = new List<EngineData>();
        for (int i = 1; i <= EngineMaxId(); i++)
            ist.Add(FindEngineData(i, 1));
        return ist;
    }
    public static List<SailorData> GetEachOtherSailorData()
    {
        var ist = new List<SailorData>();
        for (int i = 1; i <= SailorMaxId(); i++)
            ist.Add(FindSailorData(i, 1));
        return ist;
    }
    public static List<WeaponData> GetEachOtherWeaponData()
    {
        var ist = new List<WeaponData>();
        for (int i = 1; i <= WeaponMaxId(); i++)
            ist.Add(FindWeaponData(i, 1));
        return ist;
    }

    public static DateTime ToLocalTime(DateTime dt)
    {
        DateTime iKnowThisIsUtc = dt;
        DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
            iKnowThisIsUtc,
            DateTimeKind.Utc);
        return runtimeKnowsThisIsUtc.ToLocalTime();
    }

    static public int RandomRange(int min, int max)
    {
        if (min == max) return min;
        return UnityEngine.Random.Range(min, max + 1);
    }

    public static string Get2inputSplash(int v1, int v2)
    {
        if (v1 < v2)
            return CommonStaticDatas.STR_PROGRESS_2INPUT_SLASH_EMPTY;
        else if (v1 > v2)
            return CommonStaticDatas.STR_PROGRESS_2INPUT_SLASH_ENOUGH;
        else
            return CommonStaticDatas.STR_PROGRESS_2INPUT_SLASH;
    }


    public static int GetDispatchNotisCate(int id)
    {
        return (id + 3);
    }

    /*
    ss = 7
    s = 6
    a = 5
    b = 4
    c = 3
    d = 2
    e = 1
    */
    public static string GradeCheck(int grade, bool isShortType = false)
    {
        string gradeText = string.Empty;
        if (grade.Equals(1)) { gradeText = "Class E"; } // 회색
        else if (grade.Equals(2)) { gradeText = "Class D"; } // 파랑
        else if (grade.Equals(3)) { gradeText = "Class C"; } // 초록
        else if (grade.Equals(4)) { gradeText = "Class B"; } // 초록 발광
        else if (grade.Equals(5)) { gradeText = "Class A"; } // 보라
        else if (grade.Equals(6)) { gradeText = "Class A+"; } // 보라 발광
        else if (grade.Equals(7)) { gradeText = "Class S"; } // 노랑
        else if (grade.Equals(8)) { gradeText = "Class S+"; } // 노랑 발광
        else if (grade.Equals(9)) { gradeText = "Class SS"; } // 붉은
        else if (grade.Equals(10)) { gradeText = "Class SS+"; } // 붉은 발광
        else if (grade.Equals(11)) { gradeText = "Class SSS"; } // ?
        else if (grade.Equals(12)) { gradeText = "Class SSS+"; } // ?
        else { gradeText = "Class SS++"; }

        if(isShortType)
        {
            return gradeText.Split(" ")[1];
        }
        else
        {
            return gradeText;
        }
    }

    public static T GetRandomItem<T>(List<T> list)
    {
        if (list.Count == 0)
        {
            throw new System.IndexOutOfRangeException("GetRandomitem empty list");
        }
        int index = UnityEngine.Random.Range(0, list.Count);
        return list[index];
    }
    // 아이템 업그레이드 비율 함수 
    public static float ItemUpgradeCalculateMultiplier(EItemList type) // 재료
    {
        if (type == EItemList.BOAT) { return 1.05f; }
        else { return 1.025f; }
    }
    public static float ItemUpgradeMoneyCalculateMultiplier(int grade) // 돈
    {
        if (grade == 1) { return 1.05f; }
        else if (grade == 2) { return 1.075f; }
        else if (grade == 3) { return 1.1f; }
        else if (grade == 4) { return 1.125f; }
        else if (grade == 5) { return 1.15f; }
        else if (grade == 6) { return 1.175f; }
        else if (grade == 7) { return 1.2f; }
        else { return 1.2f; }
    }
}
