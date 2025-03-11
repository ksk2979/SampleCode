using MyData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPosition : MonoBehaviour
{
    public EUnitPosition _eUnitPos;
    [SerializeField] GameObject _plusIcon;
    Transform _previewPos;
    ReadyScript _readyPage;
    GameObject _boatObj;
    Button _posButton;

    public void Init()
    {

    }

    public void Init(ReadyScript readyPage)
    {
        _readyPage = readyPage;
        _previewPos = LobbyUIManager.GetInstance.GetPreviewPos((int)_eUnitPos);
        _posButton = GetComponent<Button>();
        _posButton.onClick.AddListener(PositionBtn);
    }

    public void PositionBtn()
    {
        // 선택
        UserData ud = UserData.GetInstance;

        // 취소
        if (ud.BoatTemp == null && ud.ChooseBoatTemp[(int)_eUnitPos] != null)
        {
            CancelBoat(ud);//ud.GetSelectUnitData._boatSPId[(int)_eUnitPos] = 0;
            return;
        } // 아무것도 없을때 보트 선택
        else if (ud.BoatTemp == null)
        {
            _readyPage.SetPositionBlockerState(true);
            _readyPage.SetTargetPosition(_eUnitPos);
            return;
        }

        // 처음
        if (ud.ChooseBoatTemp[(int)_eUnitPos] == null)
        {
            CreateBoat(ud.BoatTemp.GetIngerenceID);

            ud.ChooseBoatTemp[(int)_eUnitPos] = ud.BoatTemp;
            //UnitPosFuntion(ud);

            var data = ud.GetSelectData;
            data._boatSPId[(int)_eUnitPos] = ud.BoatTemp.GetIngerenceID;
            ud.SaveUnitSelect();

            ud.BoatTemp.gameObject.SetActive(false);
            ud.BoatTemp = null;

            _readyPage.SetPositionBlockerState(false);
            //Debug.Log("bID: " + ud.ChooseBoatTemp[(int)_eUnitPos]._playerInfo._boatId + " bLV: " + ud.ChooseBoatTemp[(int)_eUnitPos]._playerInfo._boatLevel + " wID: " + ud.ChooseBoatTemp[(int)_eUnitPos]._playerInfo._weaponId + " wLV: " + ud.ChooseBoatTemp[(int)_eUnitPos]._playerInfo._weaponLevel);
        }
        // 변경
        else
        {
            Debug.Log("Point D");
            // 혹시 선택 했는데 같은거 눌러 취소할 경우
            if (ud.ChooseBoatTemp[(int)_eUnitPos].GetIngerenceID == ud.BoatTemp.GetIngerenceID)
            {
                CancelBoat(ud);
                return;
            }
            CreateBoat(ud.BoatTemp.GetIngerenceID);

            ud.BoatTemp.gameObject.SetActive(false); // 원래 있던 자리 없애주고
            ud.ChooseBoatTemp[(int)_eUnitPos].gameObject.SetActive(true); // 자리에 있던 것은 내려와야하니깐 생성하고
            if (ud.ChooseBoatTemp[(int)_eUnitPos]._check != null)
                ud.ChooseBoatTemp[(int)_eUnitPos]._check.gameObject.SetActive(false);
            ud.ChooseBoatTemp[(int)_eUnitPos] = ud.BoatTemp;

            var data = ud.GetSelectData;
            data._boatSPId[(int)_eUnitPos] = ud.BoatTemp.GetIngerenceID;
            ud.SaveUnitSelect();

            ud.BoatTemp = null;
        }
        Closebtn();
    }

    void CancelBoat(UserData ud)
    {
        ud.ChooseBoatTemp[(int)_eUnitPos].gameObject.SetActive(true);
        if (ud.ChooseBoatTemp[(int)_eUnitPos]._check != null)
        {
            ud.ChooseBoatTemp[(int)_eUnitPos]._check.gameObject.SetActive(false);
        }
        ud.ChooseBoatTemp[(int)_eUnitPos] = null;
        var data = ud.GetSelectData;
        data._boatSPId[(int)_eUnitPos] = 0;
        ud.SaveUnitSelect();

        _plusIcon.SetActive(true);
        if (_boatObj != null)
        {
            Destroy(_boatObj);
        }
    }

    /// <summary>
    /// 보트 생성
    /// </summary>
    /// <param name="ingerenceId"></param>
    void CreateBoat(int ingerenceId)
    {
        var ingerenceData = UserData.GetInstance.FindCoalescence(ingerenceId.ToString());
        if (_boatObj != null)
        {
            Destroy(_boatObj);
        }
        var boatData = DataManager.GetInstance.GetData(DataManager.KEY_BOAT, ingerenceData[(int)ECoalescenceType.BOAT_ID], 1) as BoatData;
        GameObject boatObj = SimplePool.Spawn(CommonStaticDatas.RES_BOATLOBBY, boatData.resName, Vector3.zero, Quaternion.identity);

        Debug.Log("Preview Pos : " + _previewPos.transform.name);
        boatObj.transform.SetParent(_previewPos);
        ObjResetSetting(boatObj, boatData.grade);
        _boatObj = boatObj;
        _plusIcon.SetActive(false);

        int weaponId = 0;
        if (ingerenceData[(int)ECoalescenceType.WEAPON_ID] > 0)
        {
            weaponId = ingerenceData[(int)ECoalescenceType.WEAPON_ID];
        }
        else
        {
            weaponId = UserData.GetInstance.BoatTemp.GetBasicWeaponType;
        }
        var weaponData = DataManager.GetInstance.GetData(DataManager.KEY_WEAPON, weaponId, 1) as WeaponData;
        GameObject weaponObj = SimplePool.Spawn(CommonStaticDatas.RES_WEAPONLOBBY, weaponData.resName, Vector3.zero, Quaternion.identity);
        weaponObj.transform.SetParent(_boatObj.GetComponent<BoatInfo>()._boatWeaponPos.transform);
        ObjResetSetting(weaponObj, weaponData.grade);

        if (ingerenceData[(int)ECoalescenceType.DEFENSE_ID] > 0)
        {
            var defData = DataManager.GetInstance.GetData(DataManager.KEY_DEFENSE, ingerenceData[(int)ECoalescenceType.DEFENSE_ID], 1) as DefenseData;
            GameObject defObj = SimplePool.Spawn(CommonStaticDatas.RES_DEFENSE, defData.resName, Vector3.zero, Quaternion.identity);
            _boatObj.GetComponent<BoatInfo>()._boatDefensePos.GetPointSetting((EDefensePoint)defData.defensePoint, CommonStaticDatas.RES_DEFENSE, defData.resName, defObj);
        }
    }

    /// <summary>
    /// 미리보기(View)에서 오브젝트 위치 재조정
    /// </summary>
    /// <param name="obj"></param>
    void ObjResetSetting(GameObject obj, int grade)
    {
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        if (grade == 1) { obj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); }
        else if (grade == 2) { obj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); }
        else { obj.transform.localScale = new Vector3(1f, 1f, 1f); }
    }

    // 포지션 선택했을때 취소함수
    public void Closebtn()
    {
        _readyPage.SetBoatListBlockerState(false);
        _readyPage.SetTargetPosition(EUnitPosition.NONE);
    }

    /// <summary>
    /// 포지션 슬롯 정보 비우는 메서드
    /// </summary>
    public void ClearPosition()
    {
        _plusIcon.SetActive(true);
        if (_boatObj != null)
        {
            Destroy(_boatObj);
        }
        UserData.GetInstance.ChooseBoatTemp[(int)_eUnitPos] = null;
    }
}

public enum EUnitPosition
{
    NONE = -1,
    MIDDLE = 0,
    LEFT, RIGHT, FRONT, BACK
}