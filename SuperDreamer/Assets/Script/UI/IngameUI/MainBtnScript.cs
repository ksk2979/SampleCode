using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainBtnScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _spellText;
    UIManager _uiManager;
    public void Init(UIManager ui)
    {
        _uiManager = ui;
    }

    public void BtnPageActive(int arr)
    {
        if (_uiManager.GetPlayerUIInfo.IsMoveing) { return; }
        // 안올라와 있으면 올라오게 한다
        if (!_uiManager.GetPlayerUIInfo.IsUp)
        {
            _uiManager.PlayerUIToggle();
            _uiManager.GetPlayerUIInfo.AddPageOpen(arr);
        }
        else
        {
            // 페이지 체크해서 열려있으면
            if (_uiManager.GetPlayerUIInfo.AddPageCheck(arr))
            {
                // 닫아주기
                _uiManager.PlayerUIToggle();
                _uiManager.GetPlayerUIInfo.CloseAddPage();
            }
            else
            {
                // 다른 페이지가 열려있는거니깐 그냥 페이지만 딸깍
                _uiManager.GetPlayerUIInfo.AddPageOpen(arr);
            }
        }
    }
    public void SpellCreateBtn()
    {
        StageManager.GetInstance.SpellCreate();
    }
    public void SpellTextUpdate(string str)
    {
        _spellText.text = str;
    }
}
