using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class CommonFunc : MonoBehaviour
{
    
    /// <summary>
    /// persent 기준값의 퍼센테이지 만큼값을 리턴
    /// add 기준값에 해당값을 더한값을 리턴
    /// </summary>
    static public float OperatorValue(float defualtValue, float v, OperatorCategory operatorCategory)
    {
        switch (operatorCategory)
        {
            case OperatorCategory.persent: return defualtValue * (v / 100.0f);
            case OperatorCategory.add: return defualtValue + v;
            case OperatorCategory.sub: return defualtValue - v;
            case OperatorCategory.partial: return v / defualtValue * 100; // 전체값에서 일부값의 퍼센트 계산
            case OperatorCategory.PersentAdd: return defualtValue * (1 + v / 100); // 숫자(전체값)를 몇 퍼센트 증가시키기
            case OperatorCategory.PersentSub: return defualtValue * (1 - v / 100); // 숫자(전체값)를 몇 퍼센트 감소시키기
        }
        return 0;
    }
}
