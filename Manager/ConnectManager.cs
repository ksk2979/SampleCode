using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System;

public class ConnectManager : Singleton<ConnectManager>
{
    [Header("Network")]
    string _googleUrl = "https://www.google.com";
    DateTime _currentTime;
    bool _isConnected = false;
    float _checkTime = 1.0f;
    const float _normalCheckTime = 1.0f;

    [Header("Quit")]
    [SerializeField] GameObject _waitConnectBlocker;
    [SerializeField] TextMeshProUGUI _waitConnectText;
    bool _isQuitCheck = false;
    const float _quitTime = 30f;
    float _tmpQuitTime = 0f;

    #region Getter
    public DateTime CurrentTime => _currentTime;
    public bool IsConnected => _isConnected;
    public float CheckTime => _checkTime;
    #endregion Getter

    private void Start()
    {
        _checkTime = _normalCheckTime;
        _waitConnectBlocker.SetActive(false);
        StartConnectCheck();
    }

    public void StartConnectCheck()
    {
        StartCoroutine(CheckNetworkState());
    }

    private void FixedUpdate()
    {

    }

    #region Checker
    /// <summary>
    /// 주기적으로 네트워크를 체크
    /// </summary>
    IEnumerator CheckNetworkState()
    {
        while (true)
        {
            CheckConnect();
            StartCoroutine(WebCheck());
            //Debug.Log("Connect Check Cycle : " + _checkTime);
            if (_isQuitCheck)
            {
                //Debug.Log("FixedUpdate");
                if (CheckConnect())
                {
                    StartConnectCheck();
                    _isQuitCheck = false;
                }
                _tmpQuitTime += _checkTime;
                //Debug.Log("remainTime : " + (_quitTime - _tmpQuitTime));
                _waitConnectText.text = string.Format("{0}sec", (int)(_quitTime - _tmpQuitTime));
                if (_tmpQuitTime >= _quitTime)
                {
                    //Debug.Log("Application.Quit");
                    Application.Quit();
                }
            }
            yield return YieldInstructionCache.RealTimeWaitForSeconds(_checkTime);
        }
    }

    public bool CheckConnect()
    {
        // 인터넷에 연결되지 않은 상태
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _isConnected = false;
            _isQuitCheck = true;
            //Debug.Log("CheckConnect : " + _isConnected);
            if (!_waitConnectBlocker.activeSelf)
            {
                _waitConnectBlocker.SetActive(true);
                //_checkTime = Time.fixedDeltaTime;
                _checkTime = 0.02f;
                //Debug.Log("CheckTime : " + _checkTime);
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                Time.timeScale = 0f;
            }
        }
        // 인터넷에 연결된 상태
        else
        {
            _isConnected = true;
            _isQuitCheck = false;
            //Debug.Log("CheckConnect : " + _isConnected);
            if (_waitConnectBlocker.activeSelf)
            {
                _waitConnectBlocker.SetActive(false);
                _checkTime = _normalCheckTime;
                Time.fixedDeltaTime = 0.02f;
                Time.timeScale = 1f;
                _tmpQuitTime = 0f;
            }
        }
        return _isConnected;
    }
    #endregion Checker

    public void TimeCheck()
    {
        StartCoroutine(WebCheck());
    }

    IEnumerator WebCheck()
    {
        UnityWebRequest request = new UnityWebRequest();
        using (request = UnityWebRequest.Get(_googleUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string date = request.GetResponseHeader("date"); //이곳에서 반송된 데이터에 시간 데이터가 존재
                //Debug.Log("받아온 시간" + date); // GMT로 받아온다.
                _currentTime = DateTime.Parse(date).ToLocalTime(); // ToLocalTime() 메소드로 한국시간으로 변환시켜 준다.
                //Debug.Log("한국시간으로변환" + _currentTime);
                //Debug.Log(string.Format("Web Check Result : {0}", request.result));
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }
}