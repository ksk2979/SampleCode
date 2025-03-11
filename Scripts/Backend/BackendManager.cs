using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;

public enum ELoginType
{
    NONE = 0,
    GUEST,
    GOOGLE,
    FACEBOOK,
}

public class BackendManager : Singleton<BackendManager>
{
    //[SerializeField] string _googleHash;
    [SerializeField] string _serverStatus;
    [SerializeField] bool _oneDay = false; // 하루가 지났는가?
    [SerializeField] bool _nextOneDay = false; // 게임도중 하루가 지났을떄

    public BackendGameInfo _gameInfo;
    public BackendFederationAuth _federation;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        BackendServerInit();
    }
    /// <summary>
    /// 백엔드 서버 초기화
    /// </summary>
    void BackendServerInit()
    {
        var bro = Backend.Initialize();
        if (bro.IsSuccess())
        {
            //Debug.Log("초기화 성공!");
            //_googleHash = Backend.Utils.GetGoogleHash();
            //Debug.Log("해쉬키 : " + _googleHash);
        }
        else { Debug.Log("초기화 실패"); }
    }

    #region -------Property-------
    public bool NextOneDay
    {
        get { return _nextOneDay; }
        set { _nextOneDay = value; }
    }
    public string ServerStatus
    {
        get { return _serverStatus; }
        set { _serverStatus = value; }
    }

    #endregion
}