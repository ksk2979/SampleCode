using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MyData;
//using static UnityEditor.PlayerSettings;

/// <summary>
/// 스크롤 매니저
/// </summary>
public class NestedScrollManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Scrollbar _scrollbar;
    public RectTransform _rtContent;
    HorizontalLayoutGroup _layout;

    /// <summary>
    /// 스크롤 안에 콘텐츠 오브젝트의 개수
    /// </summary>
    int _size;
    /// <summary>
    /// 오브젝트 스크롤 위치값
    /// </summary>
    float[] _pos;
    /// <summary>
    /// 스크롤 오브젝트들의 사이의 거리
    /// </summary>
    float _distance, _targetPos;
    /// <summary>
    /// 드래그시에 업데이트 관여 안하게
    /// </summary>
    bool _isDrag = false;
    // 절반 거리가 넘지 않더라고 마우스 가속도에 의한 페이지 넘어감
    // 기존 포스와 타겟 포스의 위치가 서로 같아야 하고 만약 다르면 넘어간다
    float _curPos;
    [SerializeField] int _targetIndex = 0; //return pos 부분의 i를 저장을 위해서 
    // 컨텐츠 각각의 거리
    float[] _contentPos;
    float _contentDistance;

    // 컨텐츠 크기
    float _contentObjSize;

    public delegate void StageEventDelegate();
    public event StageEventDelegate OnStageEventListener;

    public void InitTankSize()
    {
        //SIZE = UserData.GetInstance.GetTankInven;
    }
    public void Init()
    {
        if (_layout == null)
        {
            _layout = _rtContent.GetComponent<HorizontalLayoutGroup>();
            _contentObjSize = _layout.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
        }

        _size = DataManager.GetInstance.GetList<StageData>(DataManager.KEY_STAGE).Count;
        _pos = new float[_size];
        _contentPos = new float[_size];

        if (_size == 1)
        {
            _pos[0] = 0;
            _contentPos[0] = 0;
            _targetIndex = 0;
            return;
        }
        // 거리에 따라 0~1인 pos대입
        _distance = 1f / (_size - 1);
        for (int i = 0; i < _size; i++)
        {
            _pos[i] = _distance * i;
        }

        //_contentDistance = _rtContent.sizeDelta.x / (SIZE - 1);
        _contentDistance = _contentObjSize + _layout.spacing;
        for (int i = 0; i < _size; i++)
        {
            _contentPos[i] = -(_contentDistance * i);
        }
    }
    public void OnBeginDrag(PointerEventData eventData) => _curPos = SetPos();

    public void OnDrag(PointerEventData eventData) => _isDrag = true;

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDrag = false;
        _targetPos = SetPos();
        // eventData.delta.x 마우스의 빠르기 측정 왼쪽으로 가면 - 오른쪽으로 가면 + 적당히 18정도가 적당함
        // 절반거리를 넘지 않아도 마우스를 빠르게 이동하면
        if (_curPos == _targetPos)
        {
            // <- 으로 가려면 목표가 하나 감소
            if (eventData.delta.x > 18 && _curPos - _distance >= 0)
            {
                --_targetIndex;
                if (_targetIndex < 0) { _targetIndex = 0; }
                _targetPos = _curPos - _distance;
            }
            // ->으로 가려면 목표가 하나 증가
            else if (eventData.delta.x < -18 && _curPos + _distance <= 1.01f) // 큰것은 오차가 있기 때문에 1.01f 했음
            {
                ++_targetIndex;
                if (_targetIndex > _size) { _targetIndex = _size - 1; }
                _targetPos = _curPos + _distance;
            }
        }
        CheckUIState();
    }

    private void Update()
    {
        if (!_isDrag) { _scrollbar.value = Mathf.Lerp(_scrollbar.value, _targetPos, 0.1f); }
    }

    float SetPos()
    {
        // 절반거리를 기준으로 가까운 위치를 반환
        for (int i = 0; i < _size; i++)
        {
            // 절반거리 + 부분, 절반거리 - 부분 (오브젝트 끌었을때 중앙이 넘었나 체크 부분)
            if (_scrollbar.value < _pos[i] + _distance * 0.5f && _scrollbar.value > _pos[i] - _distance * 0.5f)
            {
                _targetIndex = i;
                CheckUIState();
                return _pos[i];
            }
        }
        if (_size == 1 || _targetIndex == -1) { _targetIndex = 0; }
        CheckUIState();
        return _pos[_targetIndex];
    }

    // 즉시 스테이지 셋팅
    public void ImmediatelyStagePos(int pos)
    {
        int invID = pos;
        _targetIndex = invID;
        _targetPos = _pos[invID];
        _rtContent.anchoredPosition = new Vector2(_contentPos[invID], 0f);
        _scrollbar.value = _pos[invID];

        OnStageEventListener?.Invoke();
        OnStageEventListener = null;
    }
    
    void MoveStagePos(int pos)
    {
        int invID = pos;
        _targetIndex = invID;
        _targetPos = _pos[invID];
        _scrollbar.value = _pos[invID];
    }
    public void ArrowBtn(bool left)
    {
        if (_size == 1) { return; }

        if (left)
        {
            _targetIndex--;
            if (0 > _targetIndex) { _targetIndex++; return; }
        }
        else
        {
            _targetIndex++;
            if (_size == _targetIndex) { _targetIndex--; return; }
        }

        CheckUIState();
        _targetPos = _pos[TargetIndex];
    }

    /// <summary>
    /// 알림 뱃지, 스테이지 이동 화살표 표시 체크
    /// </summary>
    public void CheckUIState()
    {
        LobbyUIManager uiManager = LobbyUIManager.GetInstance;
        uiManager.GetPopup<QuestPopup>(PopupType.QUEST).CheckMainQuestSet();

        var stageList = DataManager.GetInstance.GetList<StageData>(DataManager.KEY_STAGE);
        if (_targetIndex == 0)
        {
            var button = uiManager.GetMenuController.GetButton(MenuType.PREV);
            if (button != null)
            {
                button.transform.parent.gameObject.SetActive(false);
            }
        }
        else if (_targetIndex >= stageList.Count - 1)
        {
            var button = uiManager.GetMenuController.GetButton(MenuType.NEXT);
            {
                button.transform.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            var buttonPrev = uiManager.GetMenuController.GetButton(MenuType.PREV);
            var buttonNext = uiManager.GetMenuController.GetButton(MenuType.NEXT);
            if (buttonPrev != null)
            {
                buttonPrev.transform.parent.gameObject.SetActive(true);
            }
            if (buttonNext != null)
            {
                buttonNext.transform.parent.gameObject.SetActive(true);
            }
        }
    }

    public int TargetIndex { get { return _targetIndex; } }
}
