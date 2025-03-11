using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Video;
using LightShaft.Scripts;

public class VideoController : MonoBehaviour
{
    public delegate void VideoEventDelegate();
    public event VideoEventDelegate OnVideoEventListener;
    public event VideoEventDelegate OnFailEventListener;

    [Header("Components")]
    [SerializeField] Camera _videoCamera;
    [SerializeField] Canvas _videoCanvas;
    [SerializeField] VideoPlayer _videoPlayer;
    [SerializeField] YoutubePlayer _youtubePlayer;

    [Header("Video")]
    [SerializeField] List<string> _videoIDList = new List<string>();
    [SerializeField] List<string> _urlList = new List<string>();
    [SerializeField] List<string> _gameNameList = new List<string>();
    [SerializeField] List<Sprite> _waitSpriteList = new List<Sprite>();
    [SerializeField] YoutubeProgressBar _videoProgress;
    [SerializeField] StandbyScreen _standbyScreen;
    [SerializeField] GameObject _blurObject;


    //InvidiousVideoPlayer _invidiousVideoPlayer;
    
    Image _linkedImage;
    Button _linkedButton;

    [SerializeField] bool _isPrepared = false;
    bool _isPaused = false;
    int videoIndex = 0;
    string _url;
    const string andStoreWebFormat = "https://play.google.com/store/apps/details?id=com.{0}";
    const string andStoreAppFormat = "market://details?id=com.{0}";
    
    public void Init()
    {
        //_invidiousVideoPlayer = GetComponent<InvidiousVideoPlayer>();
        _linkedImage = GetComponent<Image>();
        _linkedButton = GetComponent<Button>();

        //_videoPlayer.enabled = false;
        SetLinkButtonState(false);
        _blurObject.SetActive(false);
        _standbyScreen.SetScreenState(false);

        _videoCanvas.worldCamera = _videoCamera;
        _videoCanvas.sortingLayerName = "UI";
        _youtubePlayer.mainCamera = _videoCamera;

        //_youtubePlayer.autoPlayOnEnable = true;
        //_youtubePlayer.PreLoadVideo(_videoIDList[0]);
        //_youtubePlayer.gameObject.SetActive(false);
        PrepareVideo();
    }

    /// <summary>
    /// 영상 준비
    /// </summary>
    public void PrepareVideo()
    {
        Prepare();
    }

    /// <summary>
    /// 영상 실행
    /// </summary>
    public void PlayVideo()
    {
        _isPrepared = false;
        _videoCanvas.enabled = true;
        SoundManager.GetInstance.BackgroundMute = true;
        var cam = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
        var camScript = cam.GetComponent<UniversalAdditionalCameraData>();

        if (camScript.renderType == CameraRenderType.Base)
        {
            //_videoPlayer.targetCamera = cam;
            camScript.cameraStack.Add(_videoCamera);
        }
        else
        {
            //_videoPlayer.targetCamera = cam;
            var camMain = Camera.main;
            camMain.GetComponent<UniversalAdditionalCameraData>().cameraStack.Add(_videoCamera);
        }

        if(GameManager.GetInstance._nowScene == EScene.E_LOBBY)
        {
            LobbyUIManager.GetInstance.GetVideoBlocker.SetActive(true);
        }
        else
        {
            InGameUIManager.GetInstance.GetVideoBlocker.SetActive(true);
        }

        //_invidiousVideoPlayer.VideoPlayer.prepareCompleted -= CompletePrepareVideo;
        _videoPlayer.loopPointReached += CompleteShowVideo;
        _videoProgress.SetProgressState(true);
        SetLinkButtonState(true);
        _blurObject.SetActive(true);
        //Time.timeScale = 0;
        _youtubePlayer.Play(_videoIDList[0]);
        //_youtubePlayer.gameObject.SetActive(true);
        //_videoPlayer.Play();
    }

    void ErrorReceived(VideoPlayer source, string message)
    {
        OnFailEventListener?.Invoke();
        OnFailEventListener = null;
    }

    void ClockResyncOccurred(VideoPlayer source, double seconds)
    {
        OnFailEventListener?.Invoke();
        OnFailEventListener = null;
    }

    async void Prepare()
    {
/*        Debug.Log("Prepare Video");
        if(_videoPlayer == null || _invidiousVideoPlayer == null)
        {
            _invidiousVideoPlayer = GetComponent<InvidiousVideoPlayer>();
            _videoPlayer = _invidiousVideoPlayer.VideoPlayer;
        }
        _videoPlayer.enabled = true;
        _invidiousVideoPlayer.VideoId = _videoIDList[RandomVideoIndex()];
        _invidiousVideoPlayer.VideoPlayer.prepareCompleted += CompletePrepareVideo;
        _invidiousVideoPlayer.VideoPlayer.loopPointReached -= CompleteShowVideo;
        _invidiousVideoPlayer.VideoPlayer.errorReceived += LoadingErrorOccured;
        await _invidiousVideoPlayer.PrepareVideoAsync();*/
        SetUrl();
    }

    /// <summary>
    /// 실행할 자사 광고 영상 랜덤 인덱스
    /// </summary>
    /// <returns></returns>
    int RandomVideoIndex()
    {
        int randId = Random.Range(0, _videoIDList.Count);
        Debug.Log("Video ID : " + randId);
        videoIndex = randId;
        return randId;
    }

    /// <summary>
    /// 영상 로딩 완료
    /// </summary>
    /// <param name="source"></param>
    public void CompletePrepareVideo(VideoPlayer source)
    {
        Debug.Log("Video Ready");
        _isPrepared = true;
        transform.parent.transform.gameObject.SetActive(true);
    }

    /// <summary>
    /// 영상 송출 완료
    /// </summary>
    /// <param name="source"></param>
    public void CompleteShowVideo(VideoPlayer source)
    {
        Debug.Log("Video End");
        // 보상 지급
        _standbyScreen.OnRewardEventListener += () =>
        {
            OnVideoEventListener?.Invoke();
            _videoCanvas.enabled = false;
            OnVideoEventListener = null;
        };
        //_videoPlayer.enabled = false;
        _blurObject.SetActive(false);
        _videoProgress.SetProgressState(false);
        SetLinkButtonState(false);

        _standbyScreen.InitScreen(_waitSpriteList[videoIndex], _gameNameList[videoIndex]);
        _standbyScreen.SetScreenState(true);

        _youtubePlayer.StopVideo();
        //_youtubePlayer.PreLoadVideo(_videoIDList[0]);
        Prepare();
        _videoPlayer.loopPointReached -= CompleteShowVideo;
    }

    public void LoadingErrorOccured(VideoPlayer source, string msg = "")
    {
        Debug.Log("광고 로딩 실패");
        OnFailEventListener?.Invoke();
        OnFailEventListener = null;
    }

    public void SetLinkButtonState(bool state)
    {
        _linkedImage.enabled = state;
        _linkedButton.enabled = state;
    }

    void SetUrl()
    {
        _url = string.Empty;
#if UNITY_EDITOR
        _url = string.Format(andStoreWebFormat, _urlList[videoIndex]);
#elif UNITY_ANDROID
        _url = string.Format(andStoreAppFormat, _urlList[videoIndex]);
#else
        _url = string.Format(andStoreWebFormat, _urlList[videoIndex]);
#endif
    }

    public void OnTouchLinkStore()
    {
        if(_url.Length > 0)
        {
            Debug.Log(_url);
            Application.OpenURL(_url);
        }
    }
    public bool IsPrepared => _isPrepared;

    public VideoPlayer VideoADPlayer => _videoPlayer;
}
