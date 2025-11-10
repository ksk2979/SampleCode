using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    private AudioSource _audioSource = null;

    // 사운드를 재생
    public void PlaySound(string filepath, bool isLoop, bool is3DSound)
    {
        _audioSource.clip = ResourceManager.GetInstance.GetAudioClipForKey(filepath);

        _audioSource.loop = isLoop;
        _audioSource.spatialBlend = is3DSound ? 1.0f : 0.0f;

        _audioSource.Play();
    }

    // 일시 정지
    public void PauseSound()
    {
        _audioSource.Pause();
    }

    // 사운드 끄기
    public void StopSound()
    {
        _audioSource.Stop();
    }

    // 볼륨 프로퍼티
    public float Volume
    {
        get { return _audioSource.volume; }
        set { _audioSource.volume = value; }
    }

    // 음소거 프로퍼티
    public bool Mute
    {
        get { return _audioSource.mute; }
        set { _audioSource.mute = value; }
    }

    // 현재 플레이 중인지 검사
    public bool IsPlayingSound
    {
        get { return _audioSource.isPlaying; }
    }

    // 초기화
    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }
}
