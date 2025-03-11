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
    const string remainCountFormat = "잔여 횟수 : {0:D2}회";
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
    /// 팝업에 어빌리티 항목 설정
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
    /// 어빌리티 선택
    /// </summary>
    /// <param name="num">해당 어빌리티 버튼 번호</param>
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
