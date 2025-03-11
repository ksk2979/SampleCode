using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �޴���ư(�������� ��ɸ� �����ϴ� ��ư) ���� �ϰ� ������ ���� Ŭ����
/// ���� �� �帧
/// 1. ��ư�� Ÿ���� ����� Ŭ���� ���ο� ��Ʈ�ѷ� ȣ��
///   [Header("Menu Button")]                    // �޴� Ÿ�԰� ��ư�� 1��1 ��Ī���� ����
///   [SerializeField] MenuType[] _menuTypeArr;  // �޴�Ÿ��
///   [SerializeField] Button[] _buttonArr;      // ��ư
///   MenuController _menuController;
/// 2. AddButton(MenuType type, Button button, System.Action action)����
///   ��ư�� ����� �߰�
/// 3. ��ư ���� OnTouchButton(�޴�Ÿ��)�� ���ؼ� �� Ŭ������ ����
/// 4. GetEvent���� ��ϵ� ����� ã�� ����
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
    /// ��ư ��� ����
    /// </summary>
    /// <param name="idx">�޴� Ÿ�� ��ȣ</param>
    public void OnTouchButton(int idx)
    {
        var ev = GetEvent((MenuType)idx);
        if(ev != null)
        {
            ev.Invoke();
        }
    }

    /// <summary>
    /// �޴� ��ư �� ��� �߰�
    /// </summary>
    /// <param name="type">�޴� Ÿ��</param>
    /// <param name="button">����� ��ư</param>
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
    /// ��ϵ� ��ư ���(�븮��)�� ã���ִ� �޼���
    /// </summary>
    /// <param name="type">�޴� Ÿ��</param>
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
