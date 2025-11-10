using UnityEngine;

public class CharacterManager : SceneStaticObj<CharacterManager>
{
    [SerializeField] BaseCharacterController[] _character;

    private void Awake()
    {
        for (int i = 0; i < _character.Length; ++i)
        {
            _character[i].OnStart();
        }
    }

    public PlayerController GetPlayer { get { return _character[0] as PlayerController; } }
}