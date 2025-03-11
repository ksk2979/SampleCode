using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UserData : Singleton<UserData>
{
    #region Daily Quest
    /// <summary>
    /// ��������Ʈ �ʱ�ȭ
    /// </summary>
    public void ResetDailyQuest()
    {

        for (int i = 0; i < (int)EDailyQuest.NONE; i++)
        {
            // AllClear 7ȸ ���� �ʱ�ȭ
            if (i < (int)EDailyQuest.SEVENCLEAR)
            {
                _stQuestData.dailyQuestList[i] = 0;
                _stQuestData.dailyReceivedList[i] = false;
            }
            else
            {
                // AllClear 7ȸ �ʱ�ȭ (7ȸ �޼� ���� ���� �ߴٸ�)
                if(i == (int)EDailyQuest.SEVENCLEAR && _stQuestData.dailyReceivedList[i])
                {
                    _stQuestData.dailyQuestList[i] = 0;
                    _stQuestData.dailyReceivedList[i] = false;
                }
            }
        }
        // �α���
        _stQuestData.dailyQuestList[0] = 1;
        SaveQuestData();
    }

    /// <summary>
    /// ���� ����Ʈ ��� (�ܺο��� ���̺� ���� �ʿ�)
    /// </summary>
    /// <param name="questType">����Ʈ Ÿ��</param>
    /// <param name="value">����Ʈ ��ġ</param>
    public void SetDailyQuest(EDailyQuest questType, int value)
    {
        if (questType == EDailyQuest.NONE) return;
        _stQuestData.dailyQuestList[(int)questType] = value;
    }

    /// <summary>
    /// ���� ����Ʈ ���� ��Ȳ ���
    /// </summary>
    /// <param name="questType">����Ʈ Ÿ��</param>
    /// <param name="state">���� ����</param>
    public void SetDailyQuestState(EDailyQuest questType, bool state)
    {
        if (questType == EDailyQuest.NONE) return;
        _stQuestData.dailyReceivedList[(int)questType] = state;
        SaveQuestData(true);
    }

    /// <summary>
    /// ���� ����Ʈ ���� ��Ȳ �ε�
    /// </summary>
    /// <returns></returns>
    public List<int> GetDailyQuest() => _stQuestData.dailyQuestList;

    /// <summary>
    /// ���� ����Ʈ ���� ��Ȳ �ε�
    /// </summary>
    /// <returns></returns>
    public List<bool> GetDailyQuestState() => _stQuestData.dailyReceivedList;

    #endregion Daily Quest

    #region Main Quest
    /// <summary>
    /// ���� ����Ʈ ���
    /// </summary>
    /// <param name="stage">�ش� �������� �ѹ�</param>
    /// <param name="value">Ŭ���� �� ���̺�</param>
    public void SetMainQuest(int stage, int value)
    {
        if (stage <= 0 || stage >= 99) return;
        int num = stage - 1;

        // �ش� �������� ù �÷��� ���
        if (_stQuestData.mainQuestList.Count < stage)
        {
            _stQuestData.mainQuestList.Add(value);
        }
        else
        {
            // �ش� �������� ��ȸ�� �÷��� ���
            _stQuestData.mainQuestList[num] = value;
        }
        SaveQuestData();
    }

    /// <summary>
    /// ���� ����Ʈ ��� ��ȸ
    /// </summary>
    /// <param name="stage">Ŭ���� �� ���̺�</param>
    /// <returns></returns>
    public int GetMainQuest(int stage)
    {
        if (stage <= 0 || stage >= 99) return -1;
        int num = stage - 1;

        // �ش� �������� �÷��� ��� ����
        if(_stQuestData.mainQuestList.Count < stage)
        {
            SetMainQuest(stage, 0);
        }
        return _stQuestData.mainQuestList[num];
    }

    /// <summary>
    /// ���� ����Ʈ ���� ��� �޼���
    /// </summary>
    /// <param name="stage">��������</param>
    /// <param name="idx">�ش� �ε���</param>
    /// <param name="state">����</param>
    public void SetMainQuestState(int stage, EMainQuest questType, bool state)
    {
        if (stage <= 0) return;
        int num = (stage - 1) * 3;       // ��ȸ ������ ù �ε��� Ȯ��

        int idx = (int)questType - (int)EMainQuest.WAVE3;
        // ����Ʈ ���ɱ���� ���� ��� ����
        if (_stQuestData.mainReceivedList.Count - 1 < num)
        {
            for (int i = 0; i < 3; i++)
            {
                _stQuestData.mainReceivedList.Add(false);
            }
        }
        _stQuestData.mainReceivedList[num + idx] = state;
        SaveQuestData();
    }

    /// <summary>
    /// ���� ����Ʈ ���� ��� ��ȯ
    /// </summary>
    /// <param name="stage">��������</param>
    /// <returns></returns>
    public List<bool> GetMainQuestState(int stage)
    {
        if (stage <= 0) return null;
        int num = (stage - 1) * 3;       // ��ȸ ������ ù �ε��� Ȯ��
        List<bool> stateList = new List<bool>();

        // ����Ʈ ���ɱ���� ���� ���
        if (_stQuestData.mainReceivedList.Count - 1 < num)
        {
            SetMainQuestState(stage, EMainQuest.WAVE3, false);
        }

        for (int j = num; j < num + 3; j++)
        {
            stateList.Add(_stQuestData.mainReceivedList[j]);
        }
        return stateList;
    }
    #endregion Main Quest
}
