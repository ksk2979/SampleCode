using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public partial class UserData : Singleton<UserData>
{
    // 현재 레벨 체크
    public int UnitLevelCheck(EItemList unit, int arr)
    {
        if (unit == EItemList.BOAT) { return _stUnitLevelData._boatLevel[arr]; }
        else if (unit == EItemList.WEAPON) { return _stUnitLevelData._weaponLevel[arr]; }
        else if (unit == EItemList.DEFENSE) { return _stUnitLevelData._defenseLevel[arr]; }
        else if (unit == EItemList.CAPTAIN) { return _stUnitLevelData._captainLevel[arr]; }
        else if (unit == EItemList.SAILOR) { return _stUnitLevelData._sailorLevel[arr]; }
        else if (unit == EItemList.ENGINE) { return _stUnitLevelData._engineLevel[arr]; }

        return 0;
    }
    public void UnitLevelUpNoSave(EItemList unit, int arr)
    {
        if (unit == EItemList.BOAT) { _stUnitLevelData._boatLevel[arr]++; }
        else if (unit == EItemList.WEAPON) { _stUnitLevelData._weaponLevel[arr]++; }
        else if (unit == EItemList.DEFENSE) { _stUnitLevelData._defenseLevel[arr]++; }
        else if (unit == EItemList.CAPTAIN) { _stUnitLevelData._captainLevel[arr]++; }
        else if (unit == EItemList.SAILOR) { _stUnitLevelData._sailorLevel[arr]++; }
        else if (unit == EItemList.ENGINE) { _stUnitLevelData._engineLevel[arr]++; }
    }
    // 유닛이 최고 레벨인가?
    public bool UnitLevelMaxCheck(UnitIcon unit)
    {
        int maxLevel = unit.GetMaxLevel;

        int level = UnitLevelCheck(unit.GetItemType, unit.GetDataIndex);
        return level >= maxLevel;
    }

    public void BoatLevelUp(UnitIcon unit)
    {
        for (int i = 0; i < _stCoalescenceData._boatNumID.Count; ++i)
        {
            // 보트와 값이 같은것을 찾는다
            if (_stCoalescenceData._boatNumID[i] == unit.GetIngerenceID.ToString())
            {
                // 찾으면 세이브한다
                unit._playerInfo.SetPlayerValue(ECoalescenceType.BOAT_LEVEL, unit.GetLevel);
                _stUnitLevelData._boatLevel[i] = unit.GetLevel;
                int[] arr = FindCoalescenceArr(i);
                arr[1] = unit.GetLevel;
                string str = CoalesceneceStr(arr);
                _stCoalescenceData._coalescence[i] = str;

                if (_server)
                {
                    GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA); // COALESCENECEDATA
                    //GameManager.GetInstance.GetServerSaveListAdd(EUserSaveType.COALESCENECEDATA);
                }
                GameManager.GetInstance.GetSaveListAdd(EUserSaveType.UNITLEVELDATA);
                GameManager.GetInstance.GetSaveListAdd(EUserSaveType.COALESCENECEDATA, true);
                break;
            }
        }
    }
    public void UnitLevelUp(UnitIcon unit, UnitIcon boat = null)
    {
        // 유닛은 착용중일때도 있고 아닐때도 있으니깐
        // 현재 다른 보트에 착용중인 것이면
        if (boat != null)
        {
            for (int i = 0; i < _stCoalescenceData._boatNumID.Count; ++i)
            {
                // 보트와 값이 같은것을 찾는다
                if (_stCoalescenceData._boatNumID[i] == unit.GetIngerenceID.ToString())
                {
                    // 찾으면 세이브한다
                    boat._playerInfo.LevelUp(unit.GetItemType);
                    int[] arr = FindCoalescenceArr(i);
                    if (unit.GetItemType == EItemList.WEAPON) { _stUnitLevelData._weaponLevel[unit.GetDataIndex] = unit.GetLevel; arr[3] = unit.GetLevel; }
                    else if (unit.GetItemType == EItemList.DEFENSE) { _stUnitLevelData._defenseLevel[unit.GetDataIndex] = unit.GetLevel; arr[5] = unit.GetLevel; }
                    else if (unit.GetItemType == EItemList.CAPTAIN) { _stUnitLevelData._captainLevel[unit.GetDataIndex] = unit.GetLevel; arr[7] = unit.GetLevel; }
                    else if (unit.GetItemType == EItemList.SAILOR) { _stUnitLevelData._sailorLevel[unit.GetDataIndex] = unit.GetLevel; arr[9] = unit.GetLevel; }
                    else if (unit.GetItemType == EItemList.ENGINE) { _stUnitLevelData._engineLevel[unit.GetDataIndex] = unit.GetLevel; arr[11] = unit.GetLevel; }
                    string str = CoalesceneceStr(arr);
                    _stCoalescenceData._coalescence[i] = str;
                    if (_server)
                    {
                        GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA); // UNITLEVELDATA
                        //GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.COALESCENECEDATA); // COALESCENECEDATA
                    }

                    GameManager.GetInstance.GetSaveListAdd(EUserSaveType.UNITLEVELDATA);
                    GameManager.GetInstance.GetSaveListAdd(EUserSaveType.COALESCENECEDATA, true);
                    break;
                }
            }
        }
        else
        {
            int i = unit.GetDataIndex;
            if (unit.GetItemType == EItemList.WEAPON) { _stUnitLevelData._weaponLevel[i] = unit.GetLevel; }
            else if (unit.GetItemType == EItemList.DEFENSE) { _stUnitLevelData._defenseLevel[i] = unit.GetLevel; }
            else if (unit.GetItemType == EItemList.CAPTAIN) { _stUnitLevelData._captainLevel[i] = unit.GetLevel; }
            else if (unit.GetItemType == EItemList.SAILOR) { _stUnitLevelData._sailorLevel[i] = unit.GetLevel; }
            else if (unit.GetItemType == EItemList.ENGINE) { _stUnitLevelData._engineLevel[i] = unit.GetLevel; }
            if (_server)
            {
                GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA); // UNITLEVELDATA
            }
            GameManager.GetInstance.GetSaveListAdd(EUserSaveType.UNITLEVELDATA, true);
        }
    }

    public void ChangePotential(UnitIcon unit, int idx, UnitIcon boat = null)
    {
        if(boat != null)
        {
            bool[] checkList = new bool[3] { false, false, false };
            for (int i = 0; i < _stCoalescenceData._boatNumID.Count; ++i)
            {
                if (_stCoalescenceData._boatNumID[i] == unit.GetIngerenceID.ToString())
                {
                    boat._playerInfo.ChangePotential(unit.GetItemType, unit.GetPotentialList);
                    EItemList type = unit.GetItemType;
                    switch (type)
                    {
                        case EItemList.CAPTAIN:
                            {
                                _stUnitPotentialData._capPotentialID[unit.GetDataIndex] = unit.GetPotentialString;
                            }
                            break;
                        case EItemList.SAILOR:
                            {
                                _stUnitPotentialData._salPotentialID[unit.GetDataIndex] = unit.GetPotentialString;
                            }
                            break;
                        case EItemList.ENGINE:
                            {
                                _stUnitPotentialData._engPotentialID[unit.GetDataIndex] = unit.GetPotentialString;
                            }
                            break;
                    }
                    var potentials = FindPotential(unit.GetIngerenceID.ToString());
                    if(potentials != null)
                    {
                        potentials[(int)type - 3] = unit.GetPotentialString;
                        _stCoalescenceData._potentials[i] = string.Join("_", potentials);
                        Debug.Log(_stCoalescenceData._potentials[i]);
                    }

                    if (_server)
                    {
                        GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA); // UNITPOTENTIALDATA
                        //GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.COALESCENECEDATA); // COALESCENECEDATA
                    }
                    GameManager.GetInstance.GetSaveListAdd(EUserSaveType.UNITPOTENTIALDATA, true);
                    GameManager.GetInstance.GetSaveListAdd(EUserSaveType.COALESCENECEDATA, true);
                    break;
                }
            }
        }
        else
        {
            int dataIdx = unit.GetDataIndex;
            switch (unit.GetItemType)
            {
                case EItemList.CAPTAIN:
                    {
                        _stUnitPotentialData._capPotentialID[dataIdx] = unit.GetPotentialString;
                    }
                    break;
                case EItemList.SAILOR:
                    {
                        _stUnitPotentialData._salPotentialID[dataIdx] = unit.GetPotentialString;
                    }
                    break;
                case EItemList.ENGINE:
                    {
                        _stUnitPotentialData._engPotentialID[dataIdx] = unit.GetPotentialString;
                    }
                    break;
            }
            if (_server)
            {
                GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA); // UNITPOTENTIALDATA
            }
            GameManager.GetInstance.GetSaveListAdd(EUserSaveType.UNITPOTENTIALDATA, true);
        }
    }
    // 유닛에서 고유 번호 주기 (id값과, 등급값)
    public int CreateUnitIngerenceID(int id, int grade)
    {
        int arrID = id - 1;
        int ingerenceID = _stUnitIDCount._id[arrID]++;
        string strID = id.ToString() + grade.ToString() + ingerenceID.ToString();

        if (_server) { GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA); } // GameManager.GetInstance.GetServerSaveListAdd(EUserSaveType.UNITCOUNTID);
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.UNITCOUNTID, true);
        return int.Parse(strID);
    }

    public void UnitDataAddTypeInit()
    {
        _unitTypeAddActions = new Dictionary<EItemList, Action<int, int>>()
        {
            { EItemList.BOAT, (id, level) => AddUnit(_stUnitData._boat, _stUnitLevelData._boatLevel, id, level) },
            { EItemList.WEAPON, (id, level) => AddUnit(_stUnitData._weapon, _stUnitLevelData._weaponLevel, id, level) },
            { EItemList.DEFENSE, (id, level) => AddUnit(_stUnitData._defense, _stUnitLevelData._defenseLevel, id, level) },
            { EItemList.CAPTAIN, (id, level) => AddUnit(_stUnitData._captain, _stUnitLevelData._captainLevel, id, level) },
            { EItemList.SAILOR, (id, level) => AddUnit(_stUnitData._sailor, _stUnitLevelData._sailorLevel, id, level) },
            { EItemList.ENGINE, (id, level) => AddUnit(_stUnitData._engine, _stUnitLevelData._engineLevel, id, level) },
        };
    }

    public void UnitPotentialAddInit()
    {
        _unitPotentialAddActions = new Dictionary<EItemList, Action<string>>()
        {
            { EItemList.CAPTAIN, (potential) => AddPotential(_stUnitPotentialData._capPotentialID, potential) },
            { EItemList.SAILOR, (potential) => AddPotential(_stUnitPotentialData._salPotentialID, potential) },
            { EItemList.ENGINE, (potential) => AddPotential(_stUnitPotentialData._engPotentialID, potential) },
        };
    }

    void AddUnit(List<int> unitList, List<int> levelList, int id, int level)
    {
        unitList.Add(id);
        levelList.Add(level);
    }

    void AddPotential(List<string> potentialList, string potential)
    {
        potentialList.Add(potential);
    }

    public void UnitTypeAdd(EItemList itemList, int id, int level = 1)
    {
        if (_unitTypeAddActions.TryGetValue(itemList, out var action))
        {
            action.Invoke(id, level);
        }
    }

    public void UnitPotentialAdd(EItemList itemList, string potential)
    {
        if (_unitPotentialAddActions.TryGetValue(itemList, out var action))
        {
            action.Invoke(potential);
        }
    }

    public void BoatCoalesceneceAdd(int ingerenceID, int id)
    {
        _stCoalescenceData._boatNumID.Add(ingerenceID.ToString());
        _stCoalescenceData._coalescence.Add(id.ToString() + ",1,0,0,0,0,0,0,0,0,0,0");
        _stCoalescenceData._potentials.Add("0_0_0");
    }
    Dictionary<EItemList, int> _unitDataMap;
    void UnitDataMapSetting()
    {
        if (_unitDataMap == null)
        {
            _unitDataMap = new Dictionary<EItemList, int>
            {
                { EItemList.BOAT, 0 },
                { EItemList.WEAPON, 1 },
                { EItemList.DEFENSE, 2 },
                { EItemList.CAPTAIN, 3 },
                { EItemList.SAILOR, 4 },
                { EItemList.ENGINE, 5 }
            };
        }
    }
    public void UnitTypeSave(EItemList itemList)
    {
        // 유닛 얻은거 세이브
        if (_server)
        {
            UnitDataMapSetting();
            UnitServerUpdate(itemList);
        }
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.UNITDATA);
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.UNITLEVELDATA, true);
        if(itemList > EItemList.DEFENSE)
        {
            GameManager.GetInstance.GetSaveListAdd(EUserSaveType.UNITPOTENTIALDATA, true);
        }
        if (itemList == EItemList.BOAT)
        {
            GameManager.GetInstance.GetSaveListAdd(EUserSaveType.COALESCENECEDATA);
        }
    }
    public void UnitAllTypeSave(List<EItemList> list)
    {
        if (_server)
        {
            UnitDataMapSetting();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                UnitServerUpdate(item);
            }
        }

        // 유닛 얻은거 세이브
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.UNITDATA);
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.UNITLEVELDATA);
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.UNITPOTENTIALDATA, true);
        GameManager.GetInstance.GetSaveListAdd(EUserSaveType.COALESCENECEDATA, true);
    }
    void UnitServerUpdate(EItemList key)
    {
        GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA);
        //if (_unitDataMap.ContainsKey(key))
        //{
        //    GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA); // UNITDATA
        //    //GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.UNITLEVELDATA); // UNITLEVELDATA
        //}
        //if (key == EItemList.BOAT)
        //{
        //    // BOAT는 특별한 데이터가 더 있으므로 별도 처리
        //    GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA); // COALESCENECEDATA
        //}
        //if (key > EItemList.DEFENSE)
        //{
        //    GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA); // UNITPOTENTIALDATA
        //}
    }
    //private string GetUnitDataKey(EItemList item)
    //{
    //    if (item == EItemList.BOAT) { return BackendGameInfo.Boat_Data; }
    //    else if (item == EItemList.WEAPON) { return BackendGameInfo.Weapon_Data; }
    //    else if (item == EItemList.DEFENSE) { return BackendGameInfo.Defense_Data; }
    //    else if (item == EItemList.CAPTAIN) { return BackendGameInfo.Captain_Data; }
    //    else if (item == EItemList.SAILOR) { return BackendGameInfo.Sailor_Data; }
    //    else if (item == EItemList.ENGINE) { return BackendGameInfo.Engine_Data; }
    //    else { Debug.Log("GetUnitDataKey Null"); return ""; }
    //}

    public void FusionUnit(EItemList type, int mainIconArr, int material1Arr, int material2Arr)
    {
        // 유닛 타입별 데이터 리스트와 처리 메서드를 매핑
        Dictionary<EItemList, Action<int>> fusionActions = new Dictionary<EItemList, Action<int>>
    {
        { EItemList.BOAT, BoatFusionUnit },
        { EItemList.WEAPON, WeaponFusionUnit },
        { EItemList.DEFENSE, DefenseFusionUnit },
        { EItemList.CAPTAIN, CaptainFusionUnit },
        { EItemList.SAILOR, SailorFusionUnit },
        { EItemList.ENGINE, EngineFusionUnit }
    };

        // 선택된 유닛 타입에 따라 합성 처리
        fusionActions[type](mainIconArr);
        fusionActions[type](material1Arr);
        fusionActions[type](material2Arr);

        CleanUpFusedUnits(type); // 합성된 유닛 데이터 정리
    }


    void BoatFusionUnit(int arr)
    {
        _stUnitData._boat[arr] = -1;
        _stUnitLevelData._boatLevel[arr] = -1;
        _stCoalescenceData._boatNumID[arr] = "";
        _stCoalescenceData._coalescence[arr] = "";
    }
    void WeaponFusionUnit(int arr)
    {
        _stUnitData._weapon[arr] = -1;
        _stUnitLevelData._weaponLevel[arr] = -1;
    }
    void DefenseFusionUnit(int arr)
    {
        _stUnitData._defense[arr] = -1;
        _stUnitLevelData._defenseLevel[arr] = -1;
    }
    void CaptainFusionUnit(int arr)
    {
        _stUnitData._captain[arr] = -1;
        _stUnitLevelData._captainLevel[arr] = -1;
    }
    void SailorFusionUnit(int arr)
    {
        _stUnitData._sailor[arr] = -1;
        _stUnitLevelData._sailorLevel[arr] = -1;
    }
    void EngineFusionUnit(int arr)
    {
        _stUnitData._engine[arr] = -1;
        _stUnitLevelData._engineLevel[arr] = -1;
    }

    void CleanUpFusedUnits(EItemList type)
    {
        List<int> unitDataList = GetUnitDataListByType(type); // 유닛 데이터 리스트 가져오기
        int count = 0;

        for (int i = unitDataList.Count - 1; i >= 0; i--)
        {
            if (unitDataList[i] == -1)
            {
                // 데이터 정리
                RemoveUnitAt(i, type);
                count++;
                if (count == 3) break;
            }
        }
    }
    List<int> GetUnitDataListByType(EItemList type)
    {
        switch (type)
        {
            case EItemList.BOAT:
                return _stUnitData._boat;
            case EItemList.WEAPON:
                return _stUnitData._weapon;
            case EItemList.DEFENSE:
                return _stUnitData._defense;
            case EItemList.CAPTAIN:
                return _stUnitData._captain;
            case EItemList.SAILOR:
                return _stUnitData._sailor;
            case EItemList.ENGINE:
                return _stUnitData._engine;
            default:
                return null;
        }
    }
    void RemoveUnitAt(int index, EItemList type)
    {
        switch (type)
        {
            case EItemList.BOAT:
                _stUnitData._boat.RemoveAt(index);
                _stUnitLevelData._boatLevel.RemoveAt(index);
                _stCoalescenceData._boatNumID.RemoveAt(index);
                _stCoalescenceData._coalescence.RemoveAt(index);
                _stCoalescenceData._potentials.RemoveAt(index);
                break;
            case EItemList.WEAPON:
                _stUnitData._weapon.RemoveAt(index);
                _stUnitLevelData._weaponLevel.RemoveAt(index);
                break;
            case EItemList.DEFENSE:
                _stUnitData._defense.RemoveAt(index);
                _stUnitLevelData._defenseLevel.RemoveAt(index);
                break;
            case EItemList.CAPTAIN:
                _stUnitData._captain.RemoveAt(index);
                _stUnitLevelData._captainLevel.RemoveAt(index);
                _stUnitPotentialData._capPotentialID.RemoveAt(index);
                break;
            case EItemList.SAILOR:
                _stUnitData._sailor.RemoveAt(index);
                _stUnitLevelData._sailorLevel.RemoveAt(index);
                _stUnitPotentialData._salPotentialID.RemoveAt(index);
                break;
            case EItemList.ENGINE:
                _stUnitData._engine.RemoveAt(index);
                _stUnitLevelData._engineLevel.RemoveAt(index);
                _stUnitPotentialData._engPotentialID.RemoveAt(index);
                break;
        }
    }

    public void FusionSort(ref List<UnitIcon> unit)
    {
        int count = GetUnitCountByType(unit[0].GetItemType);

        for (int i = 0; i < count; ++i)
        {
            unit[i].ChangeDataIndex(i);
        }
    }
    public int GetUnitCountByType(EItemList eitemList)
    {
        switch (eitemList)
        {
            case EItemList.BOAT:
                return _stUnitData._boat.Count;
            case EItemList.WEAPON:
                return _stUnitData._weapon.Count;
            case EItemList.DEFENSE:
                return _stUnitData._defense.Count;
            case EItemList.CAPTAIN:
                return _stUnitData._captain.Count;
            case EItemList.SAILOR:
                return _stUnitData._sailor.Count;
            case EItemList.ENGINE:
                return _stUnitData._engine.Count;
            default:
                return 0;
        }
    }
}
