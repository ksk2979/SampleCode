using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotListScript : MonoBehaviour
{
    [SerializeField] SpellScript[] _earthList;
    [SerializeField] SpellScript[] _fireList;
    [SerializeField] SpellScript[] _waterList;

    SpellInfo _spellInfo;
    SpellScript _tempSpell;
    PlayerController _player;
    int _touch = 0;

    public void Init(SpellInfo info)
    {
        _spellInfo = info;
        _player = StageManager.GetInstance.GetPlayer.GetPlayerController;
        for (int i = 0; i < _earthList.Length; ++i) { _earthList[i].Init(AttackType.EARTH, i, this); }
        for (int i = 0; i < _fireList.Length; ++i) { _fireList[i].Init(AttackType.FIRE, i, this); }
        for (int i = 0; i < _waterList.Length; ++i) { _waterList[i].Init(AttackType.WATER, i, this); }
    }

    public void UpdateCount(AttackType type, int arr, int count)
    {
        switch (type)
        {
            case AttackType.EARTH:
                _earthList[arr].CountUpdate(count);
                break;
            case AttackType.FIRE:
                _fireList[arr].CountUpdate(count);
                break;
            case AttackType.WATER:
                _waterList[arr].CountUpdate(count);
                break;
        }
    }
    public void UpdateCoolTime(AttackType type, int arr, float now, float max)
    {
        switch (type)
        {
            case AttackType.EARTH:
                _earthList[arr].CoolTime(now, max);
                break;
            case AttackType.FIRE:
                _fireList[arr].CoolTime(now, max);
                break;
            case AttackType.WATER:
                _waterList[arr].CoolTime(now, max);
                break;
        }
    }

    public void InfoOpen(SpellScript temp)
    {
        if (_tempSpell != null && _tempSpell != temp)
        {
            _touch = 0;
        }

        _touch++;
        if (_touch == 1)
        {
            _tempSpell = temp;
            double d = _player.DamageIndex(_tempSpell._type, _tempSpell._index);
            float t = _player.CoolTimeIndex(_tempSpell._type, _tempSpell._index);
            _spellInfo.OnInfo(d, t);
        }
        else if (_touch == 2)
        {
            _player.Upgrade(_tempSpell._type, _tempSpell._index);
            OffInfo();
        }
    }
    public void OffInfo()
    {
        _touch = 0;
        _tempSpell = null;
        _spellInfo.CloseInfo();
    }
}
