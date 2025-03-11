using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TestGameData
{
    public string _boatDataID;
    public string _boatDataLevel;
    public string _boatDamage;
    public string _abilityCheck;
    public string _enemyID;
    public string _enemyCount;
    public string _time;

    public void ResetData()
    {
        _boatDataID = "";
        _boatDataLevel = "";
        _boatDamage = "";
        _abilityCheck = "";
        _enemyID = "";
        _enemyCount = "";
        _time = "";
    }
}

public class CSVFileManager
{
    private List<TestGameData> gameDataList = new List<TestGameData>();

    // 예제: 게임 데이터 추가
    public void AddGameData(TestGameData data)
    {
        gameDataList.Add(data);
    }

    // CSV 파일로 내보내기
    public void ExportToCSV(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // 헤더 추가
            writer.WriteLine("BoatDataID,BoatDataLevel,BoatDamage,AbilityCheck,EnemyID,EnemyCount,Time");

            // 데이터 추가
            foreach (var data in gameDataList)
            {
                writer.WriteLine($"'{data._boatDataID},'{data._boatDataLevel},'{data._boatDamage},'{data._abilityCheck},'{data._enemyID},'{data._enemyCount},'{data._time}");
            }
        }
    }

    public void ExportFile()
    {
        string filePath;
#if UNITY_EDITOR
        filePath = Path.Combine(Application.dataPath, "GameDataTest.csv");
#else
        filePath = Path.Combine(Application.persistentDataPath, "GameDataTest.csv");
#endif

        ExportToCSV(filePath);

        Debug.Log("Data exported to " + filePath);

#if UNITY_ANDROID
        OpenFile(filePath);
#endif
    }

    private void OpenFile(string filePath)
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", intentClass.GetStatic<string>("ACTION_SEND"));
        intent.Call<AndroidJavaObject>("setType", "text/csv");
        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject fileUri = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + filePath);
        intent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), fileUri);

        AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intent, "Share File");
        chooser.Call<AndroidJavaObject>("setFlags", intentClass.GetStatic<int>("FLAG_ACTIVITY_NEW_TASK"));
        currentActivity.Call("startActivity", chooser);
#endif
    }
}
