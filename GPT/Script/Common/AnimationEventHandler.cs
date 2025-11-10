using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 애니메이션 이벤트에 실행할 내용을 담아두는 클래스
/// </summary>
public class AnimationEventHandler : MonoBehaviour
{
    Dictionary<int, Action> _animEventDictionary = new Dictionary<int, Action>();

    /// <summary>
    /// 애니메이션 이벤트 등록 메서드
    /// </summary>
    /// <param name="idx">인덱스</param>
    /// <param name="animEvent">이벤트</param>
    public void SetEvent(int idx, Action animEvent)
    {
        // 변경 불가능하도록 처리
        if(!_animEventDictionary.ContainsKey(idx))
        {
            _animEventDictionary.Add(idx, animEvent);
        }
    }

    /// <summary>
    /// 애니메이션 이벤트 삭제 메서드
    /// </summary>
    /// <param name="idx"></param>
    public void DeleteEvent(int idx)
    {
        if(_animEventDictionary.ContainsKey(idx))
        {
            _animEventDictionary[idx] = null;
        }
    }

    /// <summary>
    /// 등록된 애니메이션 이벤트 전체 초기화(삭제)
    /// </summary>
    public void ResetEventHandler()
    {
        _animEventDictionary.Clear();
    }

    /// <summary>
    /// 애니메이션 이벤트 실행 메서드
    /// </summary>
    /// <param name="idx"></param>
    public void ExecuteEvent(int idx)
    {
        if (_animEventDictionary.ContainsKey(idx))
        {
            _animEventDictionary[idx].Invoke();
        }
        else
        {
            Debug.LogError(string.Format("{0} : There is no event registered at the specified index.", transform.name));
        }
    }
}
