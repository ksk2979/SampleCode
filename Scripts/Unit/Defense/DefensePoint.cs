using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 0 - ¾Õ, 1 - µÚ 2 - ¿Þ, 3 - ¿À
 4 - ¾ÕµÚ 5 - ¿Þ¿À 6 - ¾Õ¿Þ¿À 7 - ¾Õ¿Þ¿ÀµÚ
 */
public class DefensePoint : MonoBehaviour
{
    [SerializeField] Transform[] _pointTrans;

    public void GetPointSetting(EDefensePoint arr, string path, string resName, GameObject mainObj) 
    { 
        if (arr == EDefensePoint.FRONT || arr == EDefensePoint.BACK || arr == EDefensePoint.LEFT || arr == EDefensePoint.RIGHT)
        {
            mainObj.transform.SetParent(_pointTrans[(int)arr]);
            ObjResetSetting(mainObj);
        }
        else if (arr == EDefensePoint.FRONT_BACK)
        {
            mainObj.transform.SetParent(_pointTrans[(int)EDefensePoint.FRONT]);
            ObjResetSetting(mainObj);
            SpawnCreate(path, resName, "_B", EDefensePoint.BACK);
        }
        else if (arr == EDefensePoint.LEFT_RIGHT)
        {
            mainObj.transform.SetParent(_pointTrans[(int)EDefensePoint.LEFT]);
            ObjResetSetting(mainObj);
            SpawnCreate(path, resName, "_R", EDefensePoint.RIGHT);
        }
        else if (arr == EDefensePoint.FRONT_LEFT_RIGHT)
        {
            mainObj.transform.SetParent(_pointTrans[(int)EDefensePoint.FRONT]);
            ObjResetSetting(mainObj);
            SpawnCreate(path, resName, "_L", EDefensePoint.LEFT);
            SpawnCreate(path, resName, "_R", EDefensePoint.RIGHT);
        }
        else if (arr == EDefensePoint.ALL)
        {
            mainObj.transform.SetParent(_pointTrans[(int)EDefensePoint.FRONT]);
            ObjResetSetting(mainObj);
            SpawnCreate(path, resName, "_L", EDefensePoint.LEFT);
            SpawnCreate(path, resName, "_R", EDefensePoint.RIGHT);
            SpawnCreate(path, resName, "_B", EDefensePoint.BACK);
        }
    }

    void SpawnCreate(string path, string resName, string tag, EDefensePoint point)
    {
        GameObject obj = SimplePool.Spawn(path, resName + tag, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(_pointTrans[(int)point]);
        ObjResetSetting(obj);
    }
    void ObjResetSetting(GameObject obj)
    {
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
