using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager
{
    public CharacterController _character;
    public eCharacterStates _currentState;
    public NormalDel[] _initDels = new NormalDel[(int)eCharacterStates.Max];
    public NormalDel[] _updateDels = new NormalDel[(int)eCharacterStates.Max];
    public NormalDel[] _finishDels = new NormalDel[(int)eCharacterStates.Max];

    public StateManager(CharacterController character)
    {
        _character = character;
        _currentState = eCharacterStates.Spawn;
    }

    public void RegisterStateInit(eCharacterStates stats, NormalDel init, NormalDel update, NormalDel finish)
    {
        _initDels[(int)stats] = init;
        _updateDels[(int)stats] = update;
        _finishDels[(int)stats] = finish;
    }

    public void SetStates(eCharacterStates newState)
    {
        if (_finishDels[(int)_currentState] != null)
            _finishDels[(int)_currentState]();
        _currentState = newState;
        if (_initDels[(int)_currentState] != null)
            _initDels[(int)_currentState]();
    }

    public void UpdateState()
    {
        if (_updateDels[(int)_currentState] != null)
            _updateDels[(int)_currentState]();
    }
}