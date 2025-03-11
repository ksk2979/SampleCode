using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DebugTestInfo : MonoBehaviour
{
    [SerializeField] GameObject[] _infoObj;
    [SerializeField] TMP_InputField[] _inputFieldEnemy; // 0: id, 1: number
    [SerializeField] TMP_InputField _inputFieldPlayerNumber;
    [SerializeField] TMP_InputField[] _inputFieldPlayer;
    [SerializeField] TMP_InputField[] _inputFieldPlayerLevel;

    SpawnTestManager _spawnTestManager;

    [Tooltip("어빌리티 선택")]
    [SerializeField] TMP_InputField _inputAbility;

    public void Init(SpawnTestManager spawn)
    {
        _spawnTestManager = spawn;
    }

    public void InfoOpen(int num)
    {
        _infoObj[num].SetActive(_infoObj[num].activeSelf ? false : true);
    }

    public void EnemySpawn()
    {
        for (int i = 0; i < _inputFieldEnemy.Length; ++i) { if (string.IsNullOrEmpty(_inputFieldEnemy[i].text) || int.Parse(_inputFieldEnemy[i].text) == 0) { Debug.Log($"Null Enemy {i}"); return; } }

        int id = int.Parse(_inputFieldEnemy[0].text);
        int max = int.Parse(_inputFieldEnemy[1].text);
        for (int i = 0; i < max; ++i)
        {
            _spawnTestManager.CreateEnemy(id);
        }
        _spawnTestManager._maxUI += max;
        InGameUIManager.GetInstance.UpdateEnemyCount(_spawnTestManager._maxUI.ToString());

        Time.timeScale = 0;

        _spawnTestManager._testGameData._enemyID = _spawnTestManager.StringIsNullOrEmptyReturn(_spawnTestManager._testGameData._enemyID, id);
        _spawnTestManager._testGameData._enemyCount = _spawnTestManager.StringIsNullOrEmptyReturn(_spawnTestManager._testGameData._enemyCount, max);
    }

    public void PlayGoBtn()
    {
        if (!_spawnTestManager.GetSpawn())
        {
            _spawnTestManager.ResetStopwatch();
            _spawnTestManager.StartStopwatch();
        }
        Time.timeScale = 1;
        _spawnTestManager.SpawnEnemyGo();
    }

    public void PlayerSpawn()
    {
        //for (int i = 0; i < _inputFieldPlayer.Length; ++i) { if (string.IsNullOrEmpty(_inputFieldPlayer[i].text) || int.Parse(_inputFieldPlayer[i].text) == 0) { Debug.Log($"Null Player {i}"); } }

        //if (_spawnTestManager._playerInfoArr[0] != null) { return; }

        // 플레이어 정보가 비어 있는지 체크하고, 비어 있다면 "0"으로 설정
        for (int i = 0; i < _inputFieldPlayer.Length; ++i)
        {
            if (string.IsNullOrEmpty(_inputFieldPlayer[i].text))
            {
                OnInputFieldChanged(_inputFieldPlayer[i]);
            }
        }

        for (int i = 0; i < _inputFieldPlayerLevel.Length; ++i)
        {
            if (string.IsNullOrEmpty(_inputFieldPlayerLevel[i].text))
            {
                OnInputFieldChanged(_inputFieldPlayerLevel[i]);
            }
        }

        if (string.IsNullOrEmpty(_inputFieldPlayerNumber.text))
        {
            OnInputFieldChanged(_inputFieldPlayerNumber);
        }

        int[] infoArr = new int[System.Enum.GetValues(typeof(ECoalescenceType)).Length];
        infoArr[(int)ECoalescenceType.BOAT_ID] = int.Parse(_inputFieldPlayer[0].text);
        infoArr[(int)ECoalescenceType.BOAT_LEVEL] = int.Parse(_inputFieldPlayerLevel[0].text);

        infoArr[(int)ECoalescenceType.WEAPON_ID] = int.Parse(_inputFieldPlayer[1].text);
        infoArr[(int)ECoalescenceType.WEAPON_LEVEL] = int.Parse(_inputFieldPlayerLevel[1].text);

        infoArr[(int)ECoalescenceType.DEFENSE_ID] = int.Parse(_inputFieldPlayer[2].text);
        infoArr[(int)ECoalescenceType.DEFENSE_LEVEL] = int.Parse(_inputFieldPlayerLevel[2].text);

        infoArr[(int)ECoalescenceType.CAPTAIN_ID] = int.Parse(_inputFieldPlayer[3].text);
        infoArr[(int)ECoalescenceType.CAPTAIN_LEVEL] = int.Parse(_inputFieldPlayerLevel[3].text);

        infoArr[(int)ECoalescenceType.SAILOR_ID] = int.Parse(_inputFieldPlayer[4].text);
        infoArr[(int)ECoalescenceType.SAILOR_LEVEL] = int.Parse(_inputFieldPlayerLevel[4].text);

        infoArr[(int)ECoalescenceType.ENGINE_ID] = int.Parse(_inputFieldPlayer[5].text);
        infoArr[(int)ECoalescenceType.ENGINE_LEVEL] = int.Parse(_inputFieldPlayerLevel[5].text);
        Info.PlayerInfo info = new Info.PlayerInfo(infoArr);

        string value = GetValueString(_inputFieldPlayer);
        string valueLevel = GetValueString(_inputFieldPlayerLevel);

        _spawnTestManager.CreateBoat(int.Parse(_inputFieldPlayerNumber.text) - 1, info);
        _spawnTestManager._testGameData._boatDataID = value;
        _spawnTestManager._testGameData._boatDataLevel = valueLevel;
    }

    private string GetValueString(TMP_InputField[] inputFields)
    {
        string[] values = new string[inputFields.Length];

        for (int i = 0; i < inputFields.Length; i++)
        {
            values[i] = inputFields[i].text;
        }

        return string.Join("/", values);
    }

    void OnInputFieldChanged(TMP_InputField inputField)
    {
        // 텍스트가 비어 있으면 "0"으로 설정
        if (string.IsNullOrEmpty(inputField.text))
        {
            inputField.text = "0";
        }
    }

    public void AbilitySelect()
    {
        if (string.IsNullOrEmpty(_inputAbility.text)){ OnInputFieldChanged(_inputAbility); }

        int ability = int.Parse(_inputAbility.text);
        _spawnTestManager.PlayerAbilitySelect(ability);
    }

    public void ResetBtn()
    {
        _spawnTestManager.ResetBtn();
    }

    public void ExportFile() => _spawnTestManager.ExportFile();
}
