using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using MyData;

public class BackendFederationAuth : MonoBehaviour
{
    [SerializeField]
    FederationType _federationType;
    public FederationType GetFederationType { get { return _federationType; } }
    string _facebookToken;

    [SerializeField] BackendGameInfo _gameInfo;
    string _nickName;
    bool _nickNameCreate = false;

    // 로그인 방식 하나 더 추가할 것은 로컬로 하다가 구글 버튼을 눌러서 로컬->서버로 데이터를 넘기고 서버 데이터로 다시 로그인하기
    // 만약 서버끼리(구글, 페북) 바꾼다고 하면 이 부분은 그냥 새로운 계정을 하나 만들어 지는걸로
    // 만약에 저 구글의 정보를 페북으로 넘기고 싶어요 하면 문의해주면 가능

    #region 에디터
    // 에디터 서버 테스트 코드
    bool _testSingUp = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (!_testSingUp) { CustomSignUp(); }
        }
    }
    public void CustomSignUp()
    {
        string id = "editor"; // 원하는 아이디
        string password = "admin"; // 원하는 비밀번호
    
        var bro = Backend.BMember.CustomSignUp(id, password);
        if (bro.IsSuccess())
        {
            Debug.Log("회원가입 성공!");
            _testSingUp = true;
    
            TestEditorLogin(id, password);
            _gameInfo.ServerDataInit();
            StartCoroutine(HandleLoginCommonSteps(true));
        }
        else
        {
            // 회원가입되어있다면?
            if (bro.GetStatusCode() == "409")
            {
                _testSingUp = true;
                TestEditorLogin(id, password);
                StartCoroutine(HandleLoginCommonSteps(false));
            }
            else { Debug.LogError($"bro.GetStatusCode: {bro.GetStatusCode()}"); }
        }
    }
    public void TestEditorLogin(string id, string password)
    {
        BackendReturnObject login = Backend.BMember.CustomLogin(id, password);
        if (login.IsSuccess())
        {
            //Debug.Log("로그인에 성공했습니다");
        }
        else
        {
            //Debug.LogError("로그인 실패!");
            Debug.LogError(login); // 뒤끝의 리턴케이스를 로그로 보여줍니다.
        }
    }
    #endregion

    /// <summary>
    /// 구글 버튼을 누르면 호출 되는 함수
    /// </summary>
    public void GPGSLogin()
    {
        Debug.Log("구글초기화 시도");
        GoogleLoginInit();
        Debug.Log("구글초기화 완료");
        _federationType = FederationType.Google;
    
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            Debug.Log("구글 로그인 시도");
            Social.localUser.Authenticate(success =>
            {
                if (!success)
                {
                    //MessageHandler.ShowMessage("GOOGLE LOGIN FAIL", GameManager._popupTime);
                    Debug.Log("구글 로그인 실패");
                    GameObject.Find("LoginManager").GetComponent<LoginManager>().NotGoogleLogin();
                    return;
                }
    
                // 로그인 성공
                // 회원가입할때 가입 여부
                if (!CheckUserAuthenticate())
                {
                    // 서버에 필요한 데이터 동기 저장 해야함
                    GoogleGPGSFederation();
                    _gameInfo.ServerDataInit();
                    StartCoroutine(HandleLoginCommonSteps(true));
                }
                else
                {
                    GoogleGPGSFederation();
                    StartCoroutine(HandleLoginCommonSteps(false));
                }
            });
        }
    }
    
    /// <summary>
    /// 로그인 및 회원가입 시 공통으로 필요한 단계를 처리
    /// </summary>
    /// <param name="isInitialLogin">처음 로그인하는 경우 true, 아니면 false</param>
    IEnumerator HandleLoginCommonSteps(bool isInitialLogin)
    {
        UserData.GetInstance.GetServer = true;
        UserData user = UserData.GetInstance;
        // 공통 단계: 버전 체크 및 데이터 초기화
        // 버전 체크 및 Indata 초기화
        _gameInfo.PlayerVersionUpdateTableLoad();
        yield return YieldInstructionCache.WaitForFixedUpdate;

        if (isInitialLogin)
        {
            // 서버에서 회원가입이 완료되면 로컬 데이터를 초기화 시켜주는걸로 하자
            // 로컬 데이터 삭제
            user.ServerInitLocalDataDelete();
            yield return YieldInstructionCache.WaitForFixedUpdate;
            user.ServerInitLocalDataSave();

            user.SetVersionServerLogin(1);
            GameManager.GetInstance.GetSaveListAdd(EUserSaveType.VERSIONDATA, true);
        }
        else
        {
            // 버전 체크 및 Indata 초기화
            _gameInfo.PlayerVersionLoad();
            yield return YieldInstructionCache.WaitForFixedUpdate;

            // 버전 업데이트 확인
            _gameInfo.VersionUpdate(user.GetVersionData._version);

            ServerGetUserInfo();
        }

        // 닉네임 생성 절차
        if (!_nickNameCreate)
        {
            GameObject.Find("LoginManager").GetComponent<LoginManager>().NickNameCreate();
            while (!_nickNameCreate)
            {
                yield return YieldInstructionCache.WaitForFixedUpdate;
                if (_nickNameCreate) { break; }
            }
            Debug.Log("닉네임 생성");
        }
        
        // 서버에서 들고오는 정보는 맨 처음 깔고 회원가입이 되어있는 상태면 체크를 한다
        if (!isInitialLogin && user.GetVersionData._serverLogin == 0)
        {
            _gameInfo.PlayerInfoLoad();
            Debug.Log("서버 데이터 활성화");
            // 전체적으로 로컬에 저장해야겠네
            user.SetVersionServerLogin(1); // 로그아웃기능이 있으면 0으로 바꾼다
            user.ServerLocalAllDataSave();
        }
        else { Debug.Log("로컬로 시작"); }
        user.BackendServerInit();

        // 로그인 완료되어 게임 씬으로 넘어감
        yield return YieldInstructionCache.WaitForFixedUpdate;
        user.OptionTypeSetting((int)EOptionType.LOGINPAGE, 1);
        GameObject.Find("LoginManager").GetComponent<LoginManager>().GoogleLoginOn();
    }

    /// <summary>
    ///  구글 초기화
    /// </summary>
    void GoogleLoginInit()
    {
        // GPGS 플러그인 설정
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
            .Builder()
            .RequestServerAuthCode(false)
            .RequestEmail() // 이메일 권한을 얻고 싶지 않다면 해당 줄(RequestEmail)을 지워주세요. 이메일 요청
            .RequestIdToken() // 토큰 요청
            .Build();
        //커스텀 된 정보로 GPGS 초기화
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true; // 디버그 로그를 보고 싶지 않다면 false로 바꿔주세요.
                                                  //GPGS 시작.
        PlayGamesPlatform.Activate();
    }
    
    /// <summary>
    /// 회원가입 혹은 로그인
    /// </summary>
    public void GoogleGPGSFederation()
    {
        BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetAutoTokens(), FederationType.Google, "gpgs로 만든 계정");
    
        if (BRO.IsSuccess())
        {
            //MessageHandler.ShowMessage("GOOGLE LOGIN SUCCESS", GameManager._popupTime);
            //Debug.Log("구글 회원가입 로그인 성공");
            //UserData.GetInstance.AutoLoginLocalSave(2);
            //_loginMessageHandler.ShowMessage("google login success", 1f, new Color(255f / 255f, 255f / 255f, 235f / 255f, 1f));
        }
        else
        {
            FaildShowMessage(BRO);
        }
    }
    
    /// <summary>
    /// 이미 가입된 상태인지 확인
    /// </summary>
    /// <returns></returns>
    public bool CheckUserAuthenticate()
    {
        BackendReturnObject BRO = Backend.BMember.CheckUserInBackend(GetAutoTokens(), _federationType);
        if (BRO.GetStatusCode() == "200")
        {
            Debug.Log("가입 중인 계정입니다");
    
            // 해당 계정 정보
            Debug.Log(BRO.GetReturnValue());
            return true;
        }
        else
        {
            Debug.Log("가입된 계정이 아닙니다.");
            return false;
        }
    }
    
    /// <summary>
    /// 오토로 구글 토큰 받아오기
    /// </summary>
    /// <returns></returns>
    private string GetAutoTokens()
    {
        if (_federationType == FederationType.Google)
        {
            if (PlayGamesPlatform.Instance.localUser.authenticated)
            {
                // 유저 토큰 받기 첫번째 방법
                string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
                // 두번째 방법
                //string _IDtoken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
                return _IDtoken;
            }
            else
            {
                Debug.Log("접속되어 있지 않습니다. 다시 시도 중입니다.");
                GPGSLogin();
                return null;
            }
        }
        else if (_federationType == FederationType.Facebook)
        {
            return _facebookToken;
        }
    
        return null;
    }
    
    public void GoogleSignout()
    {
        Backend.BMember.Logout();
        ((PlayGamesPlatform)Social.Active).SignOut();
        Debug.Log("로그아웃 완료");
    }

    // 유저 정보 불러오기 (닉네임)
    public void ServerGetUserInfo()
    {
        var bro = Backend.BMember.GetUserInfo();
        if (!bro.IsSuccess())
        {
            Debug.LogError("에러가 발생했습니다 : " + bro.ToString());
            return;
        }
        LitJson.JsonData userInfoJson = bro.GetReturnValuetoJSON()["row"];

        //UserInfo userInfo = new UserInfo();
        //userInfo.gamerId = userInfoJson["gamerId"].ToString(); // 유저의 gamer_id
        //userInfo.countryCode = userInfoJson["countryCode"].ToString(); // 국가 코드를 설정하지 않은 경우 null
        //userInfo.nickname = userInfoJson["nickname"].ToString(); // 닉네임을 설정하지 않은 경우 null
        //userInfo.inDate = userInfoJson["inDate"].ToString(); // 유저의 inDate
        //userInfo.emailForFindPassword = userInfoJson["emailForFindPassword"]?.ToString(); // 커스텀 계정 id, pw 찾기 용 이메일. 등록하지 않으면 null
        //userInfo.subscriptionType = userInfoJson["subscriptionType"].ToString(); // 커스텀, 페더레이션 타입
        //userInfo.federationId = userInfoJson["federationId"]?.ToString(); // 구글, 애플, 페이스북 페더레이션 ID. 커스텀 계정은 null
        //Debug.Log(userInfo.ToString());
        if (userInfoJson["nickname"] == null)
        {
            //Debug.Log("닉네임 없다");
            _nickName = "";
        }
        else
        {
            _nickName = userInfoJson["nickname"].ToString();
        }
        if (_nickName == "") { _nickNameCreate = false; }
        else { GameManager.GetInstance.GetNickName = _nickName; _nickNameCreate = true; }
    }
    public void NickNameCreate(string nickname)
    {
        Backend.BMember.CreateNickname(nickname);
    }
    public bool NickNameDuplicationCheck(string nickname)
    {
        BackendReturnObject bro = Backend.BMember.CheckNicknameDuplication(nickname);
        if (bro.IsSuccess())
        {
            return true;
        }
        return false;
    }

    #region 에러 코드 모음
    void FaildShowMessage(BackendReturnObject BRO)
    {
        switch (BRO.GetStatusCode())
        {
            case "200":
                // 이미 회원 가입
                Debug.Log("Become a member");
                //_messageHandler.ShowMessage("Become a member", 1f, new Color(67f / 255f, 67f / 255f, 79f / 255f, 1)); 
                break;

            case "403":
                // 차단된 사용자
                Debug.Log("I am a blocked user. Reason for blocking: " + BRO.GetErrorCode());
                //_messageHandler.ShowMessage("I am a blocked user. Reason for blocking: " + BRO.GetErrorCode(), 1f, new Color(67f / 255f, 67f / 255f, 79f / 255f, 1));
                break;

            default:
                // 서버 공통 에러
                Debug.Log("Server Error");
                break;
        }
    }

    public void ShowErrorUI(BackendReturnObject backendReturn)
    {
        int statusCode = int.Parse(backendReturn.GetStatusCode());

        switch (statusCode)
        {
            case 401:
                Debug.Log("ID 또는 비밀번호가 틀렸습니다");
                break;

            case 403:
                // 콘솔창에 입력한 차단 사유가 GetErrorCode()로 전달된다.
                Debug.Log(backendReturn.GetErrorCode());
                break;

            case 404:
                Debug.Log("game not found, game을(를) 찾을 수 없습니다.");
                break;

            case 408:
                // 타임 아웃 오류(서버에서 응답이 없거나, 네트워크 등이 끊기는 경우)
                Debug.Log(backendReturn.GetMessage());
                break;

            case 409:
                Debug.Log("중복된 ID 입니다");
                break;

            case 410:
                Debug.Log("bad refreshToken, 잘못된 refreshToken 입니다");
                break;

            case 429:
                // 데이터베이스 할당량을 초과한 경우
                // 데이터 베이스 할당량 업데이트 중인 경우
                Debug.Log(backendReturn.GetMessage());
                break;

            case 503:
                // 서버가 정상적으로 작동하지 않는 경우
                Debug.Log(backendReturn.GetMessage());
                break;

            case 504:
                // 타임아웃 오류 (서버에서 응답이 늦거나, 네트워크 등이 끊겨 있는 경우)
                Debug.Log(backendReturn.GetMessage());
                break;
        }
    }
    #endregion

    public bool GetNickNameCreate { get { return _nickNameCreate; } set { _nickNameCreate = value; } }
}
