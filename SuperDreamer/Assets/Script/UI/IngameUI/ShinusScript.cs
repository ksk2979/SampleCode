using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShinusScript : MonoBehaviour
{
    bool _on = false;
    public void ShinusBtn(int index)
    {
        if (_on) { MessageHandler.Getinstance.ShowMessage("AllSpawn", 2f); return; }
        // �ش� �ż��� �����͸� �����ͼ� �ؾߵǴµ� �ð� ������ �׳� ��ȯ
        if (StageManager.GetInstance.GetPlayer.GetPlayerController._waterAttack[0] && StageManager.GetInstance.GetPlayer.GetPlayerController._fireAttack[0] && StageManager.GetInstance.GetPlayer.GetPlayerController._fireAttack[1])
        {
            _on = true;
            StageManager.GetInstance.ShinusCreate(index);
        }
        else { MessageHandler.Getinstance.ShowMessage("Not Ready", 2f); }
    }
}
