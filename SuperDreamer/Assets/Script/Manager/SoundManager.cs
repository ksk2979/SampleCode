using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundManager : Singleton<SoundManager>
{
    #region -------private Field-------
    private bool _backgroundMute = false;
    private bool _effectMute = false;

    private float _backgroundVolume = 1.0f;
    private float _effectVolume = 1.0f;

    private Sounds _background = null;
    private Dictionary<string, List<Sounds>> _effectList = null;

    private string _backgroundStr = null;
    private AudioObject _backgroundSound = null;
    private Dictionary<string, List<AudioObject>> _audioList = null;

    private float _backgroundSoundSave = 1.0f;
    private float _effectSoundSave = 1.0f;

    Queue<GameObject> _effectObjQueue;
    Transform _cameraTrans;
    AudioController _audioController;
    #endregion

    #region -------default Method-------
    protected override void Awake()
    {
        base.Awake();
        _effectList = new Dictionary<string, List<Sounds>>();

        GameObject obj = new GameObject();
        obj.AddComponent<Sounds>();
        obj.name = "BackgroundSound";
        obj.transform.parent = this.transform;
        _background = obj.GetComponent<Sounds>();

        _audioList = new Dictionary<string, List<AudioObject>>();
        _effectObjQueue = new Queue<GameObject>();
        _audioController = GameObject.Find("AudioController").GetComponent<AudioController>();
    }

    #endregion

    #region -------public Method-------
    public void PlayBackgroundSound(string filepath, bool blsLoop = true)
    {
        this.BackgroundVolume = _backgroundVolume;
        _background.PlaySound(filepath, blsLoop, false);
    }

    public void PlayEffectSound(string filepath, bool blsLoop = false,
        bool bls3d = false)
    {
        var sound = this.FindPlayableEffectSound(filepath);

        if (sound != null)
        {
            this.EffectVolume = _effectVolume;
            sound.PlaySound(filepath, blsLoop, bls3d);
        }
    }

    public void PlayEffectSound(string filepath, float volume, bool blsLoop = false,
        bool bls3d = false)
    {
        var sound = this.FindPlayableEffectSound(filepath);

        if (sound != null)
        {
            this.EffectVolume = _effectVolume;
            sound.PlaySound(filepath, blsLoop, bls3d);
            sound.Volume = volume;
        }
    }

    // 오디오 오브젝트 내보내기
    public Transform PlayAudioEffectSound(string audioID)
    {
        var audio = this.FindPlaySound(audioID);
        //Debug.Log(string.Format("{0}이 들어옴", audioID));
        if (audio == null) { return null; }
        return audio.transform;
    }
    public void DestroyEffectSoundObj()
    {
        while (_effectObjQueue.Count != 0)
        {
            Destroy(_effectObjQueue.Dequeue());
        }
        _audioList.Clear();
    }
    public Transform PlayAudioBackgroundSound(string audioID, Transform parentTrans = null)
    {
        if (_backgroundStr == null || _backgroundStr == "")
        {
            _backgroundStr = audioID;
        }
        else
        {
            if (_backgroundStr.Equals(audioID))
            {
                return _backgroundSound.transform;
            }
            else
            {
                DestroyBackgroundObj();
                _backgroundStr = audioID;
            }
        }

        if (_audioController.Volume == 0) { _audioController.Volume = 0.1f; } // 옵션에서 mute 사용자 일 경우
        if (parentTrans != null) { _backgroundSound = AudioController.PlayMusic(audioID, parentTrans); }
        else { _backgroundSound = AudioController.PlayMusic(audioID); }
        if (_audioController.Volume == 0.1f) { _audioController.Volume = 0; }

        return _backgroundSound.transform;
    }
    // 배경음 지우기
    public void DestroyBackgroundObj()
    {
        if (_backgroundSound != null)
        {
            _backgroundStr = null;
            Destroy(_backgroundSound.gameObject);
            _backgroundSound = null;
        }
    }

    // 오디오 컨트롤러 볼륨 컨트롤러
    public void AudioVolumeController(float value)
    {
        _audioController.Volume = value;
    }
    public float AudioVolumeReturn()
    {
        return _audioController.Volume;
    }

    #endregion

    #region -------private Method-------

    //효과음 리스트를 순회한다
    private void EnumerateEffectSoundList(System.Action<Sounds>
        callback)
    {
        //using System.Linq 추가해줄것.
        var keyList = _effectList.Keys.ToList();

        for (int i = 0; i < keyList.Count; ++i)
        {
            string key = keyList[i];
            var soundList = _effectList[key];
            for (int j = 0; j < soundList.Count; ++j)
            {
                var sound = soundList[j];
                callback(sound);
            }
        }
    }
    //재생 가능한 효과음을 탐색한다
    private Sounds FindPlayableEffectSound(string filepath)
    {
        if (!_effectList.ContainsKey(filepath))
        {
            var tempSoundList = new List<Sounds>();
            _effectList.Add(filepath, tempSoundList);
        }
        var soundList = _effectList[filepath];

        //최대 중첩 횟수를 벗어나지 않았을 경우
        if (soundList.Count < 10)
        {
            GameObject obj = new GameObject();
            obj.AddComponent<Sounds>();
            obj.name = "EffectSound";
            obj.transform.parent = this.transform;
            soundList.Add(obj.GetComponent<Sounds>());

            return obj.GetComponent<Sounds>();
        }
        else
        {
            for (int i = 0; i < soundList.Count; ++i)
            {
                var sound = soundList[i];

                if (!sound.IsPlayingSound) return sound;
            }
        }
        return null;
    }

    //재생 가능한 효과음을 탐색
    private AudioObject FindPlaySound(string audioID)
    {
        if (!_audioList.ContainsKey(audioID))
        {
            var tempSoundList = new List<AudioObject>();
            _audioList.Add(audioID, tempSoundList);
        }
        var soundList = _audioList[audioID];

        //최대 중첩 횟수를 벗어나지 않았을 경우
        if (soundList.Count < 20)
        {
            AudioObject audio = AudioController.Play(audioID);
            if (audio == null) { return null; }

            if (_cameraTrans == null) { _cameraTrans = Camera.main.transform; }
            audio.GetTrans.SetParent(_cameraTrans);

            audio.GetTrans.localPosition = Vector3.zero;
            //audio.GetTrans.position = _cameraManager.GetTrans.position;
            soundList.Add(audio);
            _effectObjQueue.Enqueue(audio.gameObject);
            return audio;
        }
        else
        {
            for (int i = 0; i < soundList.Count; ++i)
            {
                var sound = soundList[i];
                if (!sound.gameObject.activeSelf) { sound.gameObject.SetActive(true); }
                if (!sound.IsPlaying())
                {
                    //Debug.Log(string.Format("{0}이 들어옴", audioID));
                    //sound.GetTrans.position = _cameraManager.GetTrans.position;
                    sound.Play();
                    return sound;
                }
            }
        }
        return null;
    }

    #endregion

    #region -------Property-------
    // 이펙트 볼륨 조절
    public float EffectVolume
    {
        get { return _effectVolume; }
        set { _effectVolume = value; }
    }

    // 배경 볼퓸 조절
    public float BackgroundVolume
    {
        get { return _background.Volume; }
        set
        {
            _backgroundVolume = value;
            _background.Volume = value;
        }
    }

    // 이펙트 음소거
    public bool EffectMute
    {
        get { return _effectMute; }
        set { _effectMute = value; }
    }

    // 배경 음소거
    public bool BackgroundMute
    {
        get { if (_backgroundSound == null) { Debug.Log("사운드가 없음"); return false; } return _backgroundSound.primaryAudioSource.mute; } //  _backgroundMute
        set
        {
            if (_backgroundSound == null) { return; }
            _backgroundSound.primaryAudioSource.mute = value;
            //_backgroundMute = value;
            //_background.Mute = value;
        }
    }

    // 이펙트 사운드 저장
    public float EffectSoundSave
    {
        get { return _effectSoundSave; }
        set { _effectSoundSave = value; }
    }

    // 배경 사운드 저장
    public float BackgroundSoundSave
    {
        get { return _backgroundSoundSave; }
        set { _backgroundSoundSave = value; }
    }

    // 씬 교체될때 배경 바꾸어 주기 위해
    public string BackgroundSTR
    {
        get { return _backgroundStr; }
        set { _backgroundStr = value; }
    }
    #endregion
}
