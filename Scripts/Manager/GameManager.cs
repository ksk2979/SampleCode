using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : Singleton<GameManager>
{
    #region 이 게임에 필요한 정보
    string _nickName;

    public GameObject[] _prefabs;

    // 스테이지 관련
    public EStageDifficulty _eStageDifficulty;
    public EScene _nowScene;
    [SerializeField] int _nowStageNumber;

    // 광고 관련 
    bool _adCheck = false;
    // 종료
    bool _checkExit = false;

    // 디버그 관련
    [SerializeField] GameObject _repoterObj;

    [Header("테스트")]
    [SerializeField] bool _testEditor = false;
    [SerializeField] bool _production = false;
    public bool TestEditor => _testEditor;
    public bool Production { get { return _production; } set { _production = value; } }
    #endregion

    // 저장 데이터 관련
    bool _save;
    float _saveTime = 0f;
    float _saveTimeMax = 5f;
    List<EUserSaveType> _saveTypeList;

    // 게임 -> 메인 전환시 상태변화 체크
    bool _isFirstCleared = false;
    int _prevStage = -1;
    int _prevLevel = 1;
    public float dmgMultiple = 1f;            // 디버그용 조정 수치
    public float hpMultiple = 1f;             // 디버그용 조정 수치
    private void Start()
    {
        _saveTypeList = new List<EUserSaveType>();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
        _prevLevel = UserData.GetInstance.GetUserLevel;
        if (_repoterObj != null) { _repoterObj.SetActive(false); }
    }

    private void Update()
    {
        if (_save)
        {
            _saveTime += Time.deltaTime;
            if (_saveTime > _saveTimeMax)
            {
                _save = false;
                _saveTime = 0f;
                for (int i = 0; i < _saveTypeList.Count; ++i)
                {
                    UserData.GetInstance.LocalSaveFuntion(_saveTypeList[i]);
                }
                //Debug.Log("자동 세이브");
                _saveTypeList.Clear();
            }
        }
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    if (UserData.GetInstance.GetServer) { UserData.GetInstance.ServerSave(); }
        //}
    }

    bool _paused = false;
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            _paused = true;
            Time.timeScale = 0;
        }
        else
        {
            if (_paused)
            {
                _paused = false;
                //todo : 내려놓은 어플리케이션을 다시 올리는 순간에 처리할 행동들 
                if (!_adCheck)
                {
                    if (CheckPauseState()) return;
                    Time.timeScale = 1;
                }
            }
        }
    }

    bool CheckPauseState()
    {
        if(_nowScene == EScene.E_GAME)
        {
            if(InGameUIManager.GetInstance.GetPopup<PausePopup>(PopupType.PAUSE).IsOpened ||
                InGameUIManager.GetInstance.GetPopup<AbilityPopup>(PopupType.ABILITY).IsOpened)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public void ReporterObjActive()
    {
        _repoterObj.SetActive(_repoterObj.activeSelf == true ? false : true);
    }

    public void GetSaveListAdd(EUserSaveType type, bool fastSave = false)
    {
        for (int i = 0; i < _saveTypeList.Count; ++i)
        {
            if (_saveTypeList[i] == type) { return; }
        }
        if (!_save) { _save = true; }
        _saveTypeList.Add(type);
        if (fastSave) { GetFastSave(); }
    }
    public void GetServerSaveListAdd(EUserServerSaveType type)
    {
        UserData.GetInstance.LocalServerSaveFuntion(type);
    }
    void GetFastSave()
    {
        if (!_save) { return; }
        _saveTime = _saveTimeMax;
    }

    public int GetNowStageNumber { get { return _nowStageNumber; } set { _nowStageNumber = value; } }
    public string GetNickName { get { return _nickName; } set { _nickName = value; } }
    public bool GetADCheck { get { return _adCheck; } set { _adCheck = value; } }
    public bool GetExitCheck { get { return _checkExit; } set { _checkExit = value; } }
    public bool GetFirstCleared { get { return _isFirstCleared; } set { _isFirstCleared = value; } }
    public int GetPrevLevel { get { return _prevLevel; } set { _prevLevel = value; } }
    public int GetPrevStage { get { return _prevStage; } set { _prevStage = value; } }  
}