using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(StartLobby());
    }

    IEnumerator StartLobby()
    {
        yield return YieldInstructionCache.WaitForSeconds(1.25f);
        cSceneManager.GetInstance.ChangeScene("Lobby", () => { });
    }
}
