using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 메뉴버튼(고정적인 기능만 수행하는 버튼) 사용시 일괄 관리를 위한 클래스
/// 사용법 및 흐름
/// 1. 버튼과 타입을 등록한 클래스 내부에 컨트롤러 호출
///   [Header("Menu Button")]                    // 메뉴 타입과 버튼은 1대1 매칭으로 구성
///   [SerializeField] MenuType[] _menuTypeArr;  // 메뉴타입
///   [SerializeField] Button[] _buttonArr;      // 버튼
///   MenuController _menuController;
/// 2. AddButton(MenuType type, Button button, System.Action action)으로
///   버튼과 기능을 추가
/// 3. 버튼 사용시 OnTouchButton(메뉴타입)을 통해서 본 클래스에 접근
/// 4. GetEvent에서 등록된 기능을 찾아 실행
/// </summary>

public enum MenuType
{
    // Lobby
    HOME = 0,
    SHOP = 1,
    INVEN = 2,
    PLAY = 3,
    OPTION = 4,
    DAILY = 5,
    COLLECTION = 6,
    ENERGY = 7,
    DIA = 8,
    MONEY = 9,
    INFO = 10,
    PREV = 11,
    NEXT = 12,
    MAINQUEST = 13,
    DAILYQUEST = 14,
    ROULETTE = 15,
}
public class MenuController : MonoBehaviour, IContentMenu
{
    public delegate void ButtonFunctionDelegate();

    [SerializeField] MenuType[] _menuTypeArr;
    [SerializeField] Button[] _menuButtonArr;

    Dictionary<MenuType, ButtonFunctionDelegate> _menuFuncDictionary;
    Dictionary<MenuType, Button> _menuButtonDictionary;

    public void Init()
    {
        _menuFuncDictionary = new Dictionary<MenuType, ButtonFunctionDelegate>();
        _menuButtonDictionary = new Dictionary<MenuType, Button>();

        for(int i = 0; i < _menuTypeArr.Length; i++)
        {
            if (!_menuButtonDictionary.ContainsKey(_menuTypeArr[i]))
            {
                _menuButtonDictionary.Add(_menuTypeArr[i], _menuButtonArr[i]);
            }
            else
            {
                Debug.LogError(string.Format("Registered Menu Button : {0}", _menuTypeArr[i]));
            }
        }
    }

    /// <summary>
    /// 버튼 기능 실행
    /// </summary>
    /// <param name="idx">메뉴 타입 번호</param>
    public void OnTouchButton(int idx)
    {
        var ev = GetEvent((MenuType)idx);
        if(ev != null)
        {
            ev.Invoke();
        }
    }

    /// <summary>
    /// 메뉴 버튼 및 기능 추가
    /// </summary>
    /// <param name="type">메뉴 타입</param>
    /// <param name="button">등록할 버튼</param>
    public void AddButton(MenuType type, Button button, System.Action action)
    {
        if(!_menuButtonDictionary.ContainsKey(type))
        {
            _menuButtonDictionary.Add(type, button);
        }
        else
        {
            Debug.LogError(string.Format("Registered Menu Button : {0}", type));
        }
        if (!_menuFuncDictionary.ContainsKey(type))
        {
            _menuFuncDictionary.Add(type, new ButtonFunctionDelegate(action));
        }
        else
        {
            Debug.LogError(string.Format("Registered Menu Button : {0}", type));
        }
        button.onClick.AddListener(() => OnTouchButton((int)type));
    }

    public void AddButton(MenuType type, System.Action action)
    {
        if (!_menuFuncDictionary.ContainsKey(type))
        {
            _menuFuncDictionary.Add(type, new ButtonFunctionDelegate(action));
        }
        else
        {
            Debug.LogError(string.Format("Registered Menu Button : {0}", type));
        }
        _menuButtonDictionary[type].onClick.AddListener(() => OnTouchButton((int)type));
    }

    /// <summary>
    /// 등록된 버튼 기능(대리자)를 찾아주는 메서드
    /// </summary>
    /// <param name="type">메뉴 타입</param>
    ButtonFunctionDelegate GetEvent(MenuType type)
    {
        if(_menuFuncDictionary.ContainsKey(type))
        {
            return _menuFuncDictionary[type];
        }
        else
        {
            Debug.LogError(string.Format("Unregistered button input {0}", type));
            return null;
        }
    }

    public MenuType[] RegisteredMenuType => _menuTypeArr;

    public Button GetButton(MenuType type)
    {
        if (_menuButtonDictionary.ContainsKey(type))
        {
            return _menuButtonDictionary[type];
        }
        else
        {
            return null;
        }
    }
}
