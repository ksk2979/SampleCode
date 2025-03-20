using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SceneStaticObj<UIManager>
{
    [SerializeField] Transform _hpbarTrans;
    [SerializeField] PlayerUIInfo _playerUIInfo;
    [SerializeField] MainBtnScript _mainBtnS;

    private void Start()
    {
        _playerUIInfo.Init();
        _mainBtnS.Init(this);
    }

    private void Update()
    {
        _playerUIInfo.MoveUpdate();
        _playerUIInfo.BountyUpdate();
    }

    public void PlayerUIToggle()
    {
        _playerUIInfo.ToggleUIPosition();
    }

    public void InfoUpdate(int arr, int count)
    {
        _playerUIInfo.GetGameInfo.UpdateText(arr, count);
    }
    public void GoldUpdate(int gold)
    {
        string str = string.Format("Gold : {0}", gold);
        _mainBtnS.SpellTextUpdate(str);
    }
    public Transform GetHpbarTrans { get { return _hpbarTrans; } }
    public PlayerUIInfo GetPlayerUIInfo { get { return _playerUIInfo; } } 
}
