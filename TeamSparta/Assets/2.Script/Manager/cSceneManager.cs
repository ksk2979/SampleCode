using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneIndex
{
    Init = 0,
    LoginPage = 1,
    Loading = 2,
    Loading_Lobby = 3,
    Lobby = 4,
    Game = 5,
}

public class cSceneManager : Singleton<cSceneManager>
{
    public SceneIndex GetCurrenctSceneIndex()
    {
        if(SceneManager.GetActiveScene().buildIndex < (int)SceneIndex.Game)
        {
            return (SceneIndex)SceneManager.GetActiveScene().buildIndex;
        }
        else
        {
            return SceneIndex.Game;
        }
    }
    public void ChangeScene(int sceneIndex,
        System.Action callBack,
        float delay = 0.0f,
        LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        var scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
        this.ChangeScene(scenePath, callBack, delay, loadSceneMode);
    }

    public void ChangeScene(string sceneName,
        System.Action callBack,
        float delay = 0.0f,
        LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        StartCoroutine(this.DoChangeScene(sceneName, callBack, delay, loadSceneMode));
    }

    // 씬 전환
    private IEnumerator DoChangeScene(string sceneName,
        System.Action callBack,
        float delay,
        LoadSceneMode loadSceneMode)
    {
        AsyncOperation asyncOper = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOper.isDone)
        {
            yield return null;
        }

        callBack();
    }

    public IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation asyncOper = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOper.isDone)
        {
            yield return null;
        }
    }

    // 현재 씬의 이름이 같을 경우 true
    public bool GetSceneName(string sceneName)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == sceneName) { return true; }
        else { return false; }
    }



}