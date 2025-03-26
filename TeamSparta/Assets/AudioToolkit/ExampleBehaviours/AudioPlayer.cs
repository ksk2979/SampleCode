using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AudioPlayer : MonoBehaviour
{
    public enum EventType
    {
        Start,
        Awake,
        OnEnable,
        OnDisable,
        OnCollisionEnter,
        OnCollisionExit,
        OnDestroy,
        Funtion,
    }
    public EventType et = EventType.OnEnable;
    public string audioID;
    public bool addChild = false;
    public bool isFx = true;
    public float delayTime = 0f;
    SoundManager _soundManager;
    SoundManager GetSound()
    {
        if (_soundManager == null) { _soundManager = SoundManager.GetInstance; }
        return _soundManager;
    }
    private void Awake()
    {
        if (et == EventType.Awake)
            if (!string.IsNullOrEmpty(audioID))
                Play();
    }

    void Start()
    {
        if (et == EventType.Start)
            if (!string.IsNullOrEmpty(audioID))
                Play();
    }
    private void OnEnable()
    {
        if (et == EventType.OnEnable)
            if (!string.IsNullOrEmpty(audioID))
                Play();
    }

    private void OnDisable()
    {
        if (et == EventType.OnDisable)
            if (!string.IsNullOrEmpty(audioID))
                PlayDisable();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (et == EventType.OnCollisionEnter)
            if (!string.IsNullOrEmpty(audioID))
                Play();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (et == EventType.OnCollisionExit)
            if (!string.IsNullOrEmpty(audioID))
                Play();
    }

    private void OnDestroy()
    {
        if (et == EventType.OnDestroy)
            if (!string.IsNullOrEmpty(audioID))
                Play();
    }

    public void AudioPlayerFuntion()
    {
        if (et == EventType.Funtion)
            if (!string.IsNullOrEmpty(audioID))
                Play();
    }

    private void Play()
    {
        if (GetSound().AudioVolumeReturn() == 0) { return; }
        StartCoroutine(PlayCo());
    }
    private void PlayDisable()
    {
        if (isFx)
        {
            var obj = GetSound().PlayAudioEffectSound(audioID);
            if (addChild)
            {
                if (obj != null) { obj.SetParent(transform); }
            }
        }
    }

    private IEnumerator PlayCo()
    {
        //yield return null;
        //yield return wfsWaitDelay;

        yield return YieldInstructionCache.WaitForSeconds(delayTime);
        if (isFx == false)
        {
            var obj = GetSound().PlayAudioBackgroundSound(audioID); 
            if (addChild)
            {
                obj.SetParent(transform);
            }
        }
        else
        {
            var obj = GetSound().PlayAudioEffectSound(audioID);
            if (addChild)
            {
                if (obj != null) { obj.SetParent(transform); }
            }
        }
    }


}
