using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private Dictionary<string, AudioClip> _audioCliplist = null;
    private Dictionary<string, Sprite> _spriteCliplist = null;

    protected override void Awake()
    {
        base.Awake();
        _audioCliplist = new Dictionary<string, AudioClip>();
        _spriteCliplist = new Dictionary<string, Sprite>();
    }

    public AudioClip GetAudioClipForKey(string filepath, bool isAutoCreate = true)
    {
        if (isAutoCreate && !_audioCliplist.ContainsKey(filepath))
        {
            var audioClip = Resources.Load<AudioClip>(filepath);
            _audioCliplist.Add(filepath, audioClip);
        }

        return _audioCliplist[filepath];
    }

    public Sprite GetSpriteClipForKey(string filePath, bool isAutoCreate = true)
    {
        if (isAutoCreate && !_spriteCliplist.ContainsKey(filePath))
        {
            var spriteClip = Resources.Load<Sprite>(filePath);
            _spriteCliplist.Add(filePath, spriteClip);
        }

        return _spriteCliplist[filePath];
    }
}
