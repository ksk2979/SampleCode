using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyData;

public class AbilityPopup : PopupBase
{
    [SerializeField] AbilityScript[] _abilityArr;
    [SerializeField] TextLocalizeSetter _remainCountText;
    PlayerAbility _playerAbility;
    StageManager _stageManager;

    const string filePath = "ItemIcon/Ability{0:D2}";
    const string remainCountFormat = "�ܿ� Ƚ�� : {0:D2}ȸ";
    public void Init(PlayerAbility playerAbility)
    {
        _playerAbility = playerAbility;
        _stageManager = StageManager.GetInstance;
    }

    void SetRemainCountText(int count)
    {
        if(count > 0)
        {
            _remainCountText.key = string.Format(remainCountFormat, count);
            _remainCountText.gameObject.SetActive(true);
        }
        else
        {
            _remainCountText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// �˾��� �����Ƽ �׸� ����
    /// </summary>
    public void SetRandomAbility()
    {
        Time.timeScale = 0;
        _stageManager._hpCanvas.SetActive(false);
        SetRemainCountText(InGameUIManager.GetInstance.GetRemainAbilityCount);
        List<AbilityData> abilityList = _playerAbility.RandomAbilityFuntion();
        for (int i = 0; i < _abilityArr.Length; ++i)
        {
            int currentNum = i;
            Sprite iconImage = null;
            iconImage = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(filePath, abilityList[i].nId));
            _abilityArr[i].Init(LocalizeText.Get(abilityList[i].name),
                iconImage,
                LocalizeText.Get(abilityList[i].explanation),
                () => { SelectAbility(currentNum); });
        }
        OpenPopup();
    }

    /// <summary>
    /// �����Ƽ ����
    /// </summary>
    /// <param name="num">�ش� �����Ƽ ��ư ��ȣ</param>
    void SelectAbility(int num)
    {
        Time.timeScale = 1;
        _playerAbility.AbilityBtn(num);
        _stageManager._hpCanvas.SetActive(true);
        _stageManager._chapter[_stageManager._cc].SetSelectAbilityState(false);

        if (UserData.GetInstance.TutorialCheck() == 6 || UserData.GetInstance.TutorialCheck() == 7)
        {
            Camera.main.GetComponent<CameraManager>().ShakeCamera(3f, 1f);
        }
        ClosePopup();
    }
}
