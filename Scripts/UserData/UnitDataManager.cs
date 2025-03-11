using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;
using System.Reflection;

public class UnitDataManager : Singleton<UnitDataManager>
{

    private Dictionary<EItemList, string> _dataMappings = new Dictionary<EItemList, string>
    {
        { EItemList.BOAT, DataManager.KEY_BOAT},
        { EItemList.WEAPON, DataManager.KEY_WEAPON},
        { EItemList.DEFENSE, DataManager.KEY_DEFENSE},
        { EItemList.CAPTAIN, DataManager.KEY_CAPTAIN},
        { EItemList.SAILOR, DataManager.KEY_SAILOR},
        { EItemList.ENGINE, DataManager.KEY_ENGINE },
    };

    /// <summary>
    /// 정수형 스테이터스 검색
    /// </summary>
    /// <param name="itemType">아이템 타입</param>
    /// <param name="id">ID</param>
    /// <param name="statType">찾고자 하는 타입의 스텟</param>
    /// <returns></returns>
    public int GetIntValue(EItemList itemType, int id, Enum statType)
    {
        var data = GetOriginData(itemType, id);
        if(data != null)
        {
            switch (itemType)
            {
                case EItemList.BOAT:
                    {
                        var boat = data as BoatData;
                        object value = GetVariableValue(boat, statType.ToString());
                        if(value != null)
                        {
                            return (int)value;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case EItemList.WEAPON:
                    {
                        var weapon = data as WeaponData;
                        object value = GetVariableValue(weapon, statType.ToString());
                        if(value != null)
                        {
                            return (int)value;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case EItemList.DEFENSE:
                    {
                        var defense = data as DefenseData;
                        object value = GetVariableValue(defense, statType.ToString());
                        if(value != null)
                        {
                            return (int)value;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case EItemList.CAPTAIN:
                    {
                        var captain = data as CaptainData;
                        object value = GetVariableValue(captain, statType.ToString());
                        if(value != null)
                        {
                            return (int)value;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case EItemList.SAILOR:
                    {
                        var sailor = data as SailorData;
                        object value = GetVariableValue(sailor, statType.ToString());
                        if(value != null)
                        {
                            return (int)value;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case EItemList.ENGINE:
                    {
                        var engine = data as EngineData;
                        object value = GetVariableValue(engine, statType.ToString());
                        if(value != null)
                        {
                            return (int)value;
                        }
                        else
                        {
                            return 0;
                        }

                    }
            }
        }
        return 0;
    }

    /// <summary>
    /// 실수형 스테이터스 검색
    /// </summary>
    /// <param name="itemType">아이템 타입</param>
    /// <param name="id">ID</param>
    /// <param name="statType">찾고자 하는 타입의 스텟</param>
    /// <returns></returns>
    public float GetFloatValue(EItemList itemType, int id, Enum statType)
    {
        var data = GetOriginData(itemType, id);
        if (data != null)
        {
            switch (itemType)
            {
                case EItemList.BOAT:
                    {
                        var boat = data as BoatData;
                        object value = GetVariableValue(boat, statType.ToString());
                        if (value != null)
                        {
                            return (float)value;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case EItemList.WEAPON:
                    {
                        var weapon = data as WeaponData;
                        object value = GetVariableValue(weapon, statType.ToString());
                        if (value != null)
                        {
                            return (float)value;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case EItemList.DEFENSE:
                    {
                        var defense = data as DefenseData;
                        object value = GetVariableValue(defense, statType.ToString());
                        if (value != null)
                        {
                            return (float)value;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case EItemList.CAPTAIN:
                    {
                        var captain = data as CaptainData;
                        object value = GetVariableValue(captain, statType.ToString());
                        if (value != null)
                        {
                            return (float)value;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case EItemList.SAILOR:
                    {
                        var sailor = data as SailorData;
                        object value = GetVariableValue(sailor, statType.ToString());
                        if (value != null)
                        {
                            return (float)value;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case EItemList.ENGINE:
                    {
                        var engine = data as EngineData;
                        object value = GetVariableValue(engine, statType.ToString());
                        if (value != null)
                        {
                            return (float)value;
                        }
                        else
                        {
                            return 0;
                        }

                    }
            }
        }
        return 0;
    }

    /// <summary>
    /// 문자열 스테이터스 검색
    /// </summary>
    /// <param name="itemType">아이템 타입</param>
    /// <param name="id">ID</param>
    /// <param name="statType">찾고자 하는 타입의 스텟</param>
    /// <returns></returns>
    public string GetStringValue(EItemList itemType, int id, Enum statType)
    {
        var data = GetOriginData(itemType, id);
        if (data != null)
        {
            switch (itemType)
            {
                case EItemList.BOAT:
                    {
                        var boat = data as BoatData;
                        object value = GetVariableValue(boat, statType.ToString());
                        if (value != null)
                        {
                            return (string)value;
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                case EItemList.WEAPON:
                    {
                        var weapon = data as WeaponData;
                        object value = GetVariableValue(weapon, statType.ToString());
                        if (value != null)
                        {
                            return (string)value;
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                case EItemList.DEFENSE:
                    {
                        var defense = data as DefenseData;
                        object value = GetVariableValue(defense, statType.ToString());
                        if (value != null)
                        {
                            return (string)value;
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                case EItemList.CAPTAIN:
                    {
                        var captain = data as CaptainData;
                        object value = GetVariableValue(captain, statType.ToString());
                        if (value != null)
                        {
                            return (string)value;
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                case EItemList.SAILOR:
                    {
                        var sailor = data as SailorData;
                        object value = GetVariableValue(sailor, statType.ToString());
                        if (value != null)
                        {
                            return (string)value;
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                case EItemList.ENGINE:
                    {
                        var engine = data as EngineData;
                        object value = GetVariableValue(engine, statType.ToString());
                        if (value != null)
                        {
                            return (string)value;
                        }
                        else
                        {
                            return string.Empty;
                        }

                    }
            }
        }
        return string.Empty;
    }

    /// <summary>
    /// 문자열 스테이터스 검색
    /// </summary>
    /// <param name="itemType">아이템 타입</param>
    /// <param name="id">ID</param>
    /// <param name="statType">찾고자 하는 타입의 스텟</param>
    /// <returns></returns>
    public List<int> GetIntListValue(EItemList itemType, int id, Enum statType)
    {
        var data = GetOriginData(itemType, id);
        if (data != null)
        {
            switch (itemType)
            {
                case EItemList.BOAT:
                    {
                        var boat = data as BoatData;
                        object value = GetVariableValue(boat, statType.ToString());
                        if (value != null)
                        {
                            return (List<int>)value;
                        }
                        else
                        {
                            return new List<int>();
                        }
                    }
                case EItemList.WEAPON:
                    {
                        var weapon = data as WeaponData;
                        object value = GetVariableValue(weapon, statType.ToString());
                        if (value != null)
                        {
                            return (List<int>)value;
                        }
                        else
                        {
                            return new List<int>();
                        }
                    }
                case EItemList.DEFENSE:
                    {
                        var defense = data as DefenseData;
                        object value = GetVariableValue(defense, statType.ToString());
                        if (value != null)
                        {
                            return (List<int>)value;
                        }
                        else
                        {
                            return new List<int>();
                        }
                    }
                case EItemList.CAPTAIN:
                    {
                        var captain = data as CaptainData;
                        object value = GetVariableValue(captain, statType.ToString());
                        if (value != null)
                        {
                            return (List<int>)value;
                        }
                        else
                        {
                            return new List<int>();
                        }
                    }
                case EItemList.SAILOR:
                    {
                        var sailor = data as SailorData;
                        object value = GetVariableValue(sailor, statType.ToString());
                        if (value != null)
                        {
                            return (List<int>)value;
                        }
                        else
                        {
                            return new List<int>();
                        }
                    }
                case EItemList.ENGINE:
                    {
                        var engine = data as EngineData;
                        object value = GetVariableValue(engine, statType.ToString());
                        if (value != null)
                        {
                            return (List<int>)value;
                        }
                        else
                        {
                            return new List<int>();
                        }

                    }
            }
        }
        return new List<int>();
    }

    /// <summary>
    /// 원본 데이터 검색
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    BData GetOriginData(EItemList itemType, int id)
    {
        if (_dataMappings.TryGetValue(itemType, out var mapping))
        {
            return DataManager.GetInstance.GetData(mapping, id, 1);
        }
        return null;
    }

    /// <summary>
    /// 이름으로 변수 찾아주는 메서드
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    object GetVariableValue(object obj, string variableName)
    {
        FieldInfo fieldInfo = obj.GetType().GetField(variableName);
        if (fieldInfo != null)
        {
            return fieldInfo.GetValue(obj);
        }
        PropertyInfo propertyInfo = obj.GetType().GetProperty(variableName);
        if (propertyInfo != null)
        {
            return propertyInfo.GetValue(obj);
        }

        return null;
    }
}

#region StatusType

public enum StatusType
{
    defenseType,
    weaponType,
    equipType,
    maxLevel,
    name,
    resName,
    grade,
    needMatAmount,
    explanation,
    basicWeaponType,
    hp,
    addHp,
    damage,
    addDamage,
    speedAccelate,
    moveSpeed,
    defensive,
    rotationSpeed,
    rotationAccelateSpeed,
    braking,
    defensePoint,
    value,
    addValue,
    atkValue,
    fireRate,
    shootingRange,
    ability,

}

#endregion StatusType
