using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyData;
//using TMPro.EditorUtilities;

public class ActiveSkillPopup : PopupBase
{
    [Header("UI")]
    [SerializeField] Image _skillIconImage;
    [SerializeField] TextLocalizeSetter _skillNameText;
    [SerializeField] TextLocalizeSetter _skillExplanationText;
    [SerializeField] Button _equipButton;
    [SerializeField] Button _closeButton;
    [Space]
    [SerializeField] RectTransform _skillListRect;
    [SerializeField] GameObject _explanationSetObj;
    [SerializeField] GameObject _waitSelectObject;

    [Header("Components")]
    LobbyUIManager _uiManager;
    ReadyScript _readyScript;

    ActiveSkillData _selectedData;
    List<SkillIcon> _skillIconList;

    const string iconSpritePathFormat = "ItemIcon/Skill{0:D2}";
    const string message_alreadyEquiped = "이미 장착된 스킬입니다";
    #region Init
    public void Init(LobbyUIManager uiM)
    {
        _uiManager = uiM;
        _readyScript = _uiManager.GetReadyPage;
        InitIconList();
    }

    /// <summary>
    /// 스킬 아이콘 리스트 초기화
    /// </summary>
    void InitIconList()
    {
        _skillIconList = new List<SkillIcon>();

        CreateIcon(null);       // 취소 전용 아이콘
        var skillData = DataManager.GetInstance.GetList<ActiveSkillData>(DataManager.KEY_ACTIVESKILL);
        for (int i = 0; i < skillData.Count; i++)
        {
            CreateIcon(skillData[i]);
        }
    }
    #endregion Init

    public override void OpenPopup()
    {
        CheckExplanationState();
        CheckSelectableIcon();
        base.OpenPopup();
    }

    public override void ClosePopup()
    {
        _selectedData = null;
        base.ClosePopup();
    }

    void CreateIcon(ActiveSkillData skillData)
    {
        // 아이콘 프리팹 생성
        GameObject icon = SimplePool.Spawn(CommonStaticDatas.RES_UI_ITEMICON, CommonStaticDatas.RES_SKILLICON, Vector3.zero, Quaternion.identity);
        icon.transform.SetParent(_skillListRect);
        RectTransform rt = icon.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;

        // 아이콘 초기화(정보입력)
        SkillIcon iconScript = icon.GetComponent<SkillIcon>();
        iconScript.Init(this, skillData);
        _skillIconList.Add(iconScript);
    }

    /// <summary>
    /// 장착 또는 선택된 데이터가 있는 경우 
    /// </summary>
    /// <param name="data"></param>
    public void SetSelectedSkill(ActiveSkillData data)
    {
        _selectedData = data;
        if (data != null)
        {
            _skillNameText.key = _selectedData.name;
            _skillExplanationText.key = _selectedData.explanation;
            _skillIconImage.sprite = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(iconSpritePathFormat, _selectedData.nId));
        }
        CheckExplanationState();
    }

    /// <summary>
    /// 스킬설명란 상태 체크
    /// </summary>
    void CheckExplanationState()
    {
        if(_selectedData == null)
        {
            SetExplanationState(false);
        }
        else
        {
            SetExplanationState(true);
        }
    }

    /// <summary>
    /// 선택가능 여부 UI 표기 
    /// </summary>
    void CheckSelectableIcon()
    {
        foreach (var icon in _skillIconList)
        {
            icon.SetShowState();
        }

        UserData userData = UserData.GetInstance;   
        for(int i = 0; i < 2; i++)
        {
            int id = userData.GetEquipedSkill(i);
            if(id > 0)
            {
                for(int j = 0; j < _skillIconList.Count; j++)
                {
                    if(_skillIconList[j].SkillID == id)
                    {
                        _skillIconList[j].SetHideState();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 이미 장착된 스킬인지 확인하는 메서드
    /// </summary>
    /// <returns></returns>
    bool CheckAlreadyEquipedSkill()
    {
        if(_selectedData == null)
        {
            return false;
        }

        int id = _selectedData.nId;
        for(int i = 0; i < 2; i++)
        {
            if(id == UserData.GetInstance.GetEquipedSkill(i))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 스킬 아이콘 정렬
    /// </summary>
    public void SortIconList()
    {

    }

    #region Button
    public void OnTouchEquipButton()
    {
        if(CheckAlreadyEquipedSkill())
        {
            MessageHandler.GetInstance.ShowMessage(message_alreadyEquiped, 1.5f);
            return;
        }
        _uiManager.GetReadyPage.SetSkillSlot(_selectedData);
        ClosePopup();
    }
    #endregion Buttons

    #region Setter
    /// <summary>
    /// 스킬 설명 상태 전환(스킬 설명과 공백 텍스트(스킬을 선택하세요) 스위치)
    /// </summary>
    /// <param name="isActive"></param>
    void SetExplanationState(bool isActive)
    {
        _explanationSetObj.SetActive(isActive);
        _waitSelectObject.SetActive(!isActive);
    }
    #endregion Setter
}
