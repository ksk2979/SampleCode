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
    UserData _userData;     // 사용자 정보
    UnitIcon _targetIcon;
    bool _reasonMaxLevel = false;          // 실패 사유 : 레벨 상한
    bool _reasonEnoughReward = false;      // 실패 사유 : 재화 부족
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

            // 레벨 
            _targetIcon.ChangeLevel(_targetIcon.GetLevel + 1);
            _userData.UnitLevelUpNoSave(_targetIcon.GetItemType, _targetIcon.GetDataIndex);

            // MaxLevel & Material Check (상단의 reason 값 갱신)
            // 다음 광물 초기화
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
            // 혹시 착용중인 보트가 있는가?
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
    /// 업그레이드가 가능한지 확인하는 메서드
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
    /// 퀘스트 클리어 조건에 부합하는지
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
    /// 광물이 충분한지 체크
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
    /// 광물 사용 처리(정보 저장)
    /// </summary>
    /// <param name="current">현재 소지 광물</param>
    /// <param name="target">소모 할 광물 수량</param>
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
    /// 필요 광물수 받는 메서드
    /// </summary>
    /// <param name="itemType">아이템 타입</param>
    /// <returns></returns>
    public int[] GetNeedMatArray()
    {
        return _targetIcon.GetNeedMatAmount.ToArray();
    }

    public int[] GetMyMaterials() => _myMaterials;
    public void SetNeedMaterials(int[] materials) => _needMaterials = materials;
    #endregion Materials

    /// <summary>
    /// 업그레이드 안되는 사유 메세지 표기
    /// </summary>
    void SendMessage()
    {
        if (_reasonMaxLevel)
        {
            MessageHandler.GetInstance.ShowMessage("최대 레벨 도달, 업그레이드는 하지 않습니다", 1.5f);
        }
        if (_reasonEnoughReward)
        {
            MessageHandler.GetInstance.ShowMessage("업그레이드를 못했습니다\n재료가 부족합니다", 1.5f);
        }
    }

    #region Firebase
    void CheckFirebaseLog()
    {
        int grade = _targetIcon.GetGrade;
        int minLevel = 0;
        // C~E 등급
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
        // B(25 이상) ~ S(15 이상) 등급
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
