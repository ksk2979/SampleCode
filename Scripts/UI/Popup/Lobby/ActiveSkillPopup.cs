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
    const string message_alreadyEquiped = "�̹� ������ ��ų�Դϴ�";
    #region Init
    public void Init(LobbyUIManager uiM)
    {
        _uiManager = uiM;
        _readyScript = _uiManager.GetReadyPage;
        InitIconList();
    }

    /// <summary>
    /// ��ų ������ ����Ʈ �ʱ�ȭ
    /// </summary>
    void InitIconList()
    {
        _skillIconList = new List<SkillIcon>();

        CreateIcon(null);       // ��� ���� ������
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
        // ������ ������ ����
        GameObject icon = SimplePool.Spawn(CommonStaticDatas.RES_UI_ITEMICON, CommonStaticDatas.RES_SKILLICON, Vector3.zero, Quaternion.identity);
        icon.transform.SetParent(_skillListRect);
        RectTransform rt = icon.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;

        // ������ �ʱ�ȭ(�����Է�)
        SkillIcon iconScript = icon.GetComponent<SkillIcon>();
        iconScript.Init(this, skillData);
        _skillIconList.Add(iconScript);
    }

    /// <summary>
    /// ���� �Ǵ� ���õ� �����Ͱ� �ִ� ��� 
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
    /// ��ų����� ���� üũ
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
    /// ���ð��� ���� UI ǥ�� 
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
    /// �̹� ������ ��ų���� Ȯ���ϴ� �޼���
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
    /// ��ų ������ ����
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
    /// ��ų ���� ���� ��ȯ(��ų ����� ���� �ؽ�Ʈ(��ų�� �����ϼ���) ����ġ)
    /// </summary>
    /// <param name="isActive"></param>
    void SetExplanationState(bool isActive)
    {
        _explanationSetObj.SetActive(isActive);
        _waitSelectObject.SetActive(!isActive);
    }
    #endregion Setter
}
