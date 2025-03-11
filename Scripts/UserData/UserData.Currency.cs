using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UserData : Singleton<UserData>
{ 
    /// <summary>
    /// 재화, 광물 정보 저장(변경된 정보로 덮어씌움)
    /// </summary>
    /// <param name="materialArr"></param>
    public void MaterialSave(int[] materialArr)
    {
        List<string> serverMaterialName = new List<string>();
        List<int> serverMaterial = new List<int>();

        var materialTypes = new EInvenType[] { EInvenType.Money, EInvenType.Diamond, EInvenType.Copper, EInvenType.Zinc, EInvenType.Aluminum, EInvenType.Steel, EInvenType.Gold, EInvenType.Oil, EInvenType.Evolution };

        foreach (var type in materialTypes)
        {
            int currentIndex = (int)type;
            if (materialArr.Length > currentIndex)
            {
                UpdateMaterial(type, materialArr[currentIndex], ref serverMaterialName, ref serverMaterial);
            }
        }

        if (_server && serverMaterialName.Count > 0)
        {
            GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA);
            //GameManager.GetInstance.GetServerSaveListAdd(EUserSaveType.USERDATA, 1);
        }
        SaveUserData();
    }

    /// <summary>
    /// 개별 재화, 광물정보 갱신 메서드
    /// </summary>
    void UpdateMaterial(EInvenType type, int newValue, ref List<string> serverMaterialName, ref List<int> serverMaterial)
    {
        int oldValue = GetCurrency(type);

        if (newValue != oldValue)
        {
            SetCurrency(type, newValue);

            string jsonKey = GetJsonKey(type);
            if (!string.IsNullOrEmpty(jsonKey) && _server)
            {
                serverMaterialName.Add(jsonKey);
                serverMaterial.Add(newValue);
            }
        }
    }

    string GetJsonKey(EInvenType type)
    {
        if(EInvenType.None != type)
        {
            return type.ToString().ToUpper();
        }
        return "";
    }

    // EInvenType.Evolution 미구현 상태 PropertyType에 속하므로 요주의
    void SetCurrency(EInvenType type, int value)
    {
        if(type <= EInvenType.Diamond)
        {
            _stUserData._property[(int)type] = value;
        }
        else
        {
            int num = (int)type - 2;
            _stMaterialData._material[num] = value;
        }
    }

    /*
     * AddCurreny로 재화, 광물 추가 가능
     * EPropertyTYpe / EMaterialType / EInvenType 호환
     */
    public void AddCurrency(EPropertyType type, int value)
    {
        _stUserData._property[(int)type] += value;
        if (_server) 
        {
            GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA);
        }
    }

    public void AddCurrency(EMaterialType type, int value)
    {
        //string updateName = "";
        _stMaterialData._material[(int)type] += value;

        //if(type != EMaterialType.NONE)
        //{
        //    updateName = type.ToString();
        //}
        if (_server) 
        {
            GameManager.GetInstance.GetServerSaveListAdd(EUserServerSaveType.USERDATA);
            //GameManager.GetInstance.GetServerSaveListAdd(EUserSaveType.USERDATA, 1);
        }
    }

    // EInvenType.Evolution 미구현 상태 PropertyType에 속하므로 요주의
    public void AddCurrency(EInvenType type, int value)
    {
        if (type == EInvenType.None) return;
        if(type <= EInvenType.Diamond)
        {
            int num = (int)type;
            AddCurrency((EPropertyType)num, value);
        }
        else
        {
            int num = (int)type - 2;
            AddCurrency((EMaterialType)num, value);
        }
    }

    /// <summary>
    /// RewardType의 형식
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void AddCurrency(ERewardType type, int value)
    {
        if (type == ERewardType.None) return;
        if(type <= ERewardType.ActionEnergy)
        {
            int num = (int)type;
            AddCurrency((EPropertyType)num, value);
        }
        else
        {
            int num = (int)type - (int)ERewardType.Copper;
            AddCurrency((EMaterialType)num, value);
        }
    }

    /*
     * SubCurrency로 재화, 광물 감소 가능
     * EPropertyTYpe / EMaterialType / EInvenType 호환
     */
    public void SubCurrency(EPropertyType type, int value)
    {
        AddCurrency(type, value * -1);
    }

    public void SubCurrency(EMaterialType type, int value)
    {
        AddCurrency(type, value * -1);
    }

    public void SubCurrency(EInvenType type, int value)
    {
        if (type == EInvenType.None) return;

        value *= -1;
        if(type <= EInvenType.Diamond)
        {
            int num = (int)type;
            AddCurrency((EPropertyType)num, value);
        }
        else
        {
            int num = (int)type - 2;
            AddCurrency((EPropertyType)num, value);
        }
    }

    /*
     * GetCurreny로 현재 보유중인 재화, 광물 정보 가져옴
     * EPropertyTYpe / EMaterialType / EInvenType 호환
     */
    public int GetCurrency(EPropertyType type) => _stUserData._property[(int)type];
    public int GetCurrency(EMaterialType type) => _stMaterialData._material[(int)type];
    public int GetCurrency(EInvenType type)
    {
        if (type == EInvenType.None) return 0;

        if(type <= EInvenType.Diamond)
        {
           return _stUserData._property[(int)type];
        }
        else
        {
            int num = (int)type - 2;
            return _stMaterialData._material[num];
        }
    }

    /// <summary>
    /// 돈, 다이아 및 광물 정보 
    /// </summary>
    public int[] GetAllMaterials()
    {
        int[] temp = new int[(int)EInvenType.None];
        for (int i = 0; i < temp.Length; ++i)
        {
            temp[i] = GetCurrency((EInvenType)i);
        }

        return temp;
    }
}
