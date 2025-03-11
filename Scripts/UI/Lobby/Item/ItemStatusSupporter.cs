using MyData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ItemStatusSupporter : MonoBehaviour
{
    UnitIcon _targetIcon = null;
    public void Init(UnitIcon targetIcon)
    {
        _targetIcon = targetIcon;
    }

    /// <summary>
    /// 장비의 스텟들을 배열로 찾아주는 메서드
    /// </summary>
    public float[] FindStatusValues(bool isNext = false)
    {
        if (_targetIcon == null)
        {
            Debug.LogError("Target Icon is null");
            return null;
        }

        float upgradeValue = 0;
        if (isNext)
        {
            upgradeValue = _targetIcon.GetNextLevelValue;
        }
        else
        {
            upgradeValue = _targetIcon.GetValue;
        }
        float[] values = new float[3] { upgradeValue, 0, 0 };

        switch (_targetIcon.GetItemType)
        {
            case EItemList.BOAT:
                {
                    values[1] = _targetIcon.GetTwoValue;
                    values[2] = _targetIcon.GetMoveSpeed;
                }
                break;
            case EItemList.WEAPON:
                {
                    values[1] = _targetIcon.GetShootingRange;
                    values[2] = _targetIcon.GetFireRate;
                }
                break;
            case EItemList.DEFENSE:
                {
                    var nowC = DataManager.GetInstance.GetData(DataManager.KEY_DEFENSE, _targetIcon.GetID, 1) as DefenseData;
                    values[1] = nowC.damage;
                    values[2] = 0;
                }
                break;
        }
        return values;
    }

    /// <summary>
    /// 스텟 이름 반환
    /// </summary>
    /// <returns></returns>
    public string[] FindStatusTitles()
    {
        if (_targetIcon == null)
        {
            Debug.LogError("Target Icon is null");
            return null;
        }

        string[] titles = new string[3];

        switch (_targetIcon.GetItemType)
        {
            case EItemList.BOAT:
                {
                    titles[0] = "HP";
                    titles[1] = "ATK";
                    titles[2] = "SPD";
                }
                break;
            case EItemList.WEAPON:
                {
                    titles[0] = "ATK";
                    titles[1] = "RANGE";
                    titles[2] = "AS";
                }
                break;
            case EItemList.DEFENSE:
                {
                    titles[0] = "HP";
                    titles[1] = "ATK";
                    titles[2] = string.Empty;
                }
                break;
            case EItemList.CAPTAIN:
                {
                    titles[0] = "ATK";
                    titles[1] = string.Empty;
                    titles[2] = string.Empty;
                }
                break;
            case EItemList.SAILOR:
                {
                    titles[0] = "HP";
                    titles[1] = string.Empty;
                    titles[2] = string.Empty;
                }
                break;
            case EItemList.ENGINE:
                {
                    titles[0] = "SPD";
                    titles[1] = string.Empty;
                    titles[2] = string.Empty;
                }
                break;
        }
        return titles;
    }
}
