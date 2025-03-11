using MyData;
using MyStructData;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.NetworkInformation;
using UnityEngine;

public class ItemUpgrade : MonoBehaviour
{
    ItemEditorScript _itemEditor;
    UserData _userData;     // ����� ����
    UnitIcon _targetIcon;
    bool _reasonMaxLevel = false;          // ���� ���� : ���� ����
    bool _reasonEnoughReward = false;      // ���� ���� : ��ȭ ����
    int[] _myMaterials;
    int[] _needMaterials;

    bool _isWorking = false;
    public void Init(ItemEditorScript itemEditor, UnitIcon icon, int[] myMat)
    {
        if (_itemEditor == null) _itemEditor = itemEditor;
        _targetIcon = icon;
        _myMaterials = myMat;
        _needMaterials = _targetIcon.GetNeedMatAmount.ToArray();
        //var data = DataManager.GetInstance.FindData(DataManager.KEY_UPGRADE, _targetIcon.GetLevel) as UpgradeData;
        //for (int i = 0; i < _needMaterials.Length; i++)
        //{
        //    if (i == (int)EInvenType.Money)
        //    {
        //        _needMaterials[i] *= _targetIcon.GetLevel;
        //    }
        //    else if (_needMaterials[i] > 0)
        //    {
        //        _needMaterials[i] *= data.cost;
        //    }
        //}
        if (_userData == null) _userData = UserData.GetInstance;

        _reasonMaxLevel = false;
        _reasonEnoughReward = false;
    }

    #region Upgrade
    public void Upgrade()
    {
        if (_isWorking) return;
        _isWorking = true;
        CheckUpgradeable();
        if (_reasonMaxLevel || _reasonEnoughReward)
        {
            SendMessage();
            _isWorking = false;
            return;
        }
        UseMaterial(_needMaterials, true);
        LevelUp();
        CheckDailyQuest();
        _itemEditor.UpdateUpgradeInfo();
        _itemEditor.RefreshUI();
        CheckFirebaseLog();
        _isWorking = false;
    }

    public void BatchUpgrade()
    {
        if (_isWorking) return;
        _isWorking = true;
        CheckUpgradeable();
        if (_reasonMaxLevel || _reasonEnoughReward)
        {
            SendMessage();
            _isWorking = false;
            return;
        }
        while (!_reasonMaxLevel && !_reasonEnoughReward)
        {
            UseMaterial(_needMaterials, false);

            // ���� 
            _targetIcon.ChangeLevel(_targetIcon.GetLevel + 1);
            _userData.UnitLevelUpNoSave(_targetIcon.GetItemType, _targetIcon.GetDataIndex);

            // MaxLevel & Material Check (����� reason �� ����)
            // ���� ���� �ʱ�ȭ
            _itemEditor.SetRequiredMatInfo(_targetIcon.GetNeedMatAmount.ToArray(), _targetIcon.GetLevel, _targetIcon.GetGrade, _targetIcon.GetItemType);

            CheckUpgradeable();
            CheckFirebaseLog();
        }
        _userData.MaterialSave(_myMaterials);         // Materials
        LevelUp(true);
        CheckDailyQuest();
        _itemEditor.UpdateUpgradeInfo();
        _itemEditor.RefreshUI();
        _isWorking = false;
    }
    #endregion Upgrade

    #region Function
    void LevelUp(bool isAlready = false)
    {
        if(!isAlready)
        {
            _targetIcon.ChangeLevel(_targetIcon.GetLevel + 1);
        }
        if(_targetIcon.GetItemType == EItemList.BOAT)
        {
            _userData.BoatLevelUp(_targetIcon);
            return;
        }
        else
        {
            UnitIcon boat = null;
            // Ȥ�� �������� ��Ʈ�� �ִ°�?
            if (_targetIcon.GetIngerenceID != 0)
            {
                List<UnitIcon> boatList = _itemEditor.GetUnitList(EItemList.BOAT);
                for (int i = 0; i < boatList.Count; ++i)
                {
                    if (boatList[i].GetIngerenceID == _targetIcon.GetIngerenceID)
                    {
                        boat = boatList[i];
                        _userData.UnitLevelUp(_targetIcon, boat);
                        if (_targetIcon.GetItemType == EItemList.WEAPON)
                        {
                            boat._playerInfo.SetPlayerValue(ECoalescenceType.WEAPON_ID, _targetIcon.GetID);
                            boat._playerInfo.SetPlayerValue(ECoalescenceType.WEAPON_LEVEL, _targetIcon.GetLevel);
                        }
                        else if (_targetIcon.GetItemType == EItemList.DEFENSE)
                        {
                            boat._playerInfo.SetPlayerValue(ECoalescenceType.DEFENSE_ID, _targetIcon.GetID);
                            boat._playerInfo.SetPlayerValue(ECoalescenceType.DEFENSE_LEVEL, _targetIcon.GetLevel);
                        }
                        else if (_targetIcon.GetItemType == EItemList.CAPTAIN)
                        {
                            boat._playerInfo.SetPlayerValue(ECoalescenceType.CAPTAIN_ID, _targetIcon.GetID);
                            boat._playerInfo.SetPlayerValue(ECoalescenceType.CAPTAIN_LEVEL, _targetIcon.GetLevel);
                        }
                        else if (_targetIcon.GetItemType == EItemList.SAILOR)
                        {
                            boat._playerInfo.SetPlayerValue(ECoalescenceType.SAILOR_ID, _targetIcon.GetID);
                            boat._playerInfo.SetPlayerValue(ECoalescenceType.SAILOR_LEVEL, _targetIcon.GetLevel);
                        }
                        else if (_targetIcon.GetItemType == EItemList.ENGINE)
                        {
                            boat._playerInfo.SetPlayerValue(ECoalescenceType.ENGINE_ID, _targetIcon.GetID);
                            boat._playerInfo.SetPlayerValue(ECoalescenceType.ENGINE_LEVEL, _targetIcon.GetLevel);
                        }
                        break;
                    }
                }
            }
            else
            {
                _userData.UnitLevelUp(_targetIcon);
            }
        }
    }

    /// <summary>
    /// ���׷��̵尡 �������� Ȯ���ϴ� �޼���
    /// </summary>
    void CheckUpgradeable()
    {
        if (_targetIcon != null)
        {
            _reasonMaxLevel = _userData.UnitLevelMaxCheck(_targetIcon);
            _reasonEnoughReward = !CheckMaterials();
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// ����Ʈ Ŭ���� ���ǿ� �����ϴ���
    /// </summary>
    void CheckDailyQuest()
    {
        var questList = _userData.GetDailyQuest();
        if (questList[(int)EDailyQuest.UPGRADE] <= 0)
        {
            _userData.SetDailyQuest(EDailyQuest.UPGRADE, 1);
            var questpopup = LobbyUIManager.GetInstance.GetPopup<QuestPopup>(PopupType.QUEST);
            questpopup.CheckDailyQuestSet();
            _userData.SaveQuestData();
        }
    }
    #endregion Function

    #region Materials
    /// <summary>
    /// ������ ������� üũ
    /// </summary>
    /// <returns></returns>
    public bool CheckMaterials()
    {
        bool isEnough = true;
        for (int i = 0; i < _myMaterials.Length; i++)
        {
            if (_needMaterials[i] > 0)
            {
                if (_myMaterials[i] < _needMaterials[i])
                {
                    isEnough = false;
                    break;
                }
            }
        }
        //Debug.Log("isEnough : " + isEnough);
        return isEnough;
    }

    /// <summary>
    /// ���� ��� ó��(���� ����)
    /// </summary>
    /// <param name="current">���� ���� ����</param>
    /// <param name="target">�Ҹ� �� ���� ����</param>
    public void UseMaterial(int[] target, bool needToSave = false)
    {
        for (int i = 0; i < _myMaterials.Length; i++)
        {
            if (target[i] > 0)
            {
                _myMaterials[i] -= target[i];
            }
        }

        if (needToSave)
        {
            _userData.MaterialSave(_myMaterials);
        }
    }

    /// <summary>
    /// �ʿ� ������ �޴� �޼���
    /// </summary>
    /// <param name="itemType">������ Ÿ��</param>
    /// <returns></returns>
    public int[] GetNeedMatArray()
    {
        return _targetIcon.GetNeedMatAmount.ToArray();
    }

    public int[] GetMyMaterials() => _myMaterials;
    public void SetNeedMaterials(int[] materials) => _needMaterials = materials;
    #endregion Materials

    /// <summary>
    /// ���׷��̵� �ȵǴ� ���� �޼��� ǥ��
    /// </summary>
    void SendMessage()
    {
        if (_reasonMaxLevel)
        {
            MessageHandler.GetInstance.ShowMessage("�ִ� ���� ����, ���׷��̵�� ���� �ʽ��ϴ�", 1.5f);
        }
        if (_reasonEnoughReward)
        {
            MessageHandler.GetInstance.ShowMessage("���׷��̵带 ���߽��ϴ�\n��ᰡ �����մϴ�", 1.5f);
        }
    }

    #region Firebase
    void CheckFirebaseLog()
    {
        int grade = _targetIcon.GetGrade;
        int minLevel = 0;
        // C~E ���
        if(grade <= 3)
        {
            return;
        }
        else
        {
            if(grade == 6)
            {
                minLevel = 15;
            }
            else
            {
                minLevel = 25;
            }
        }
        // B(25 �̻�) ~ S(15 �̻�) ���
        if (_targetIcon.GetLevel >= minLevel)
        {
            if(_targetIcon.GetLevel % 5 == 0)
            {
                string log = string.Format("Upgrade_{0}_{1}", _targetIcon.GetID, _targetIcon.GetLevel);
                FirebaseManager.GetInstance.LogEvent(log);
                Debug.Log(log);
            }
        }
    }
    #endregion Firebase
}
