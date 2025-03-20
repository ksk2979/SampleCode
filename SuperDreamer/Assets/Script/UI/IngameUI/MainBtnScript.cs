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
        // �ȿö�� ������ �ö���� �Ѵ�
        if (!_uiManager.GetPlayerUIInfo.IsUp)
        {
            _uiManager.PlayerUIToggle();
            _uiManager.GetPlayerUIInfo.AddPageOpen(arr);
        }
        else
        {
            // ������ üũ�ؼ� ����������
            if (_uiManager.GetPlayerUIInfo.AddPageCheck(arr))
            {
                // �ݾ��ֱ�
                _uiManager.PlayerUIToggle();
                _uiManager.GetPlayerUIInfo.CloseAddPage();
            }
            else
            {
                // �ٸ� �������� �����ִ°Ŵϱ� �׳� �������� ����
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
