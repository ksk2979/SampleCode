using MyData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEditor.Build;

public class SkillIcon : MonoBehaviour, IComparable
{
    ActiveSkillPopup _popup;

    [Header("UI")]
    [SerializeField] Image _skillIcon;
    [SerializeField] GameObject _blocker;
    [SerializeField] Button _skillButton;

    [Header("Info")]
    [SerializeField] int _skillID;
    bool _isEquiped = false;
    bool _isUnlocked = false;

    ActiveSkillData _skillData;

    const string iconFilePathFormat = "ItemIcon/Skill{0:D2}";


    public void Init(ActiveSkillPopup popup, ActiveSkillData skillData)
    {
        _popup = popup;

        _skillData = skillData;
        if(skillData != null)
        {
            _skillID = _skillData.nId;
        }
        else
        {
            _skillID = 0;
        }
        _skillIcon.sprite = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(iconFilePathFormat, _skillID));
        _skillButton.onClick.RemoveListener(OnTouchIconButton);
        _skillButton.onClick.AddListener(OnTouchIconButton);
    }

    public void OnTouchIconButton()
    {
        _popup.SetSelectedSkill(_skillData);
    }

    #region Setter

    public void SetShowState()
    {
        _blocker.SetActive(false);
        //gameObject.SetActive(true);
    }

    public void SetHideState() 
    {
        _blocker.SetActive(true);
        //gameObject.SetActive(false);
    }

    /// <summary>
    /// 장착상태 설정 
    /// </summary>
    /// <param name="state"></param>
    public void SetEquipState(bool state)
    {
        _isEquiped = state;
        _blocker.SetActive(_isEquiped);
    }

    /// <summary>
    /// 해금 상태 설정
    /// </summary>
    /// <param name="state"></param>
    public void SetUnlockState(bool state)
    {
        _isUnlocked = state;
        _blocker.SetActive(_isUnlocked);
    }
    #endregion Setter

    public int CompareTo(object obj)
    {
        throw new NotImplementedException();
    }
    public int SkillID => _skillID;
}
