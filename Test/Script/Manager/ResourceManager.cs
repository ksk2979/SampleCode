using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private Dictionary<string, GameObject> _objCliplist = null;
    private Dictionary<string, AudioClip> _audioCliplist = null;
    private Dictionary<string, Sprite> _spriteCliplist = null;

    protected override void Awake()
    {
        base.Awake();
        _objCliplist = new Dictionary<string, GameObject>();
        _audioCliplist = new Dictionary<string, AudioClip>();
        _spriteCliplist = new Dictionary<string, Sprite>();
    }

    public GameObject GetObjClipForKey(string filePath, bool isAutoCreate = true)
    {
        if (isAutoCreate && !_objCliplist.ContainsKey(filePath))
        {
            var obj = Resources.Load<GameObject>(filePath);
            if (obj == null) { Debug.Log(string.Format("{0}: 오브젝트 경로에 없습니다", filePath)); return null; }
            _objCliplist.Add(filePath, obj);
        }

        return _objCliplist[filePath];
    }

    public AudioClip GetAudioClipForKey(string filePath, bool isAutoCreate = true)
    {
        if (isAutoCreate && !_audioCliplist.ContainsKey(filePath))
        {
            var audioClip = Resources.Load<AudioClip>(filePath);
            if (audioClip == null) { Debug.Log(string.Format("{0}: 오디오 경로에 없습니다", filePath)); return null; }
            _audioCliplist.Add(filePath, audioClip);
        }

        return _audioCliplist[filePath];
    }

    public Sprite GetSpriteClipForKey(string filePath, bool isAutoCreate = true)
    {
        if (isAutoCreate && !_spriteCliplist.ContainsKey(filePath))
        {
            var spriteClip = Resources.Load<Sprite>(filePath);
            if (spriteClip == null) { Debug.Log(string.Format("{0}: 스프라이트 경로에 없습니다", filePath)); return null; }
            _spriteCliplist.Add(filePath, spriteClip);
        }

        return _spriteCliplist[filePath];
    }
}
