using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePopup : PopupBase
{
    [Header("Components")]
    [SerializeField] Button _optionButton;
    [SerializeField] Button _playButton;
    [SerializeField] Button _homeButton;

    public event PopupEventDelegate OnPlayEventListener;
    public event PopupEventDelegate OnLobbyEventListener;

    [SerializeField] Transform _abilityListTrans;
    [SerializeField] GameObject _abilityInfoPrefab;

    public override void OpenPopup()
    {
        _optionButton.onClick.RemoveAllListeners();
        _optionButton.onClick.AddListener(OnTouchOptionButton);

        _playButton.interactable = true;
        _playButton.onClick.RemoveAllListeners();
        _playButton.onClick.AddListener(OnTouchPlayButton);
        
        _homeButton.onClick.RemoveAllListeners();
        _homeButton.onClick.AddListener(OnTouchHomeButton);

        base.OpenPopup();
    }

    public void OnTouchPlayButton()
    {
        _playButton.interactable = false;
        OnPlayEventListener?.Invoke();
        OnPlayEventListener = null;
        OnLobbyEventListener = null;
        ClosePopup();
    }

    public void OnTouchHomeButton()
    {
        _homeButton.interactable = false;
        OnLobbyEventListener?.Invoke();
        OnLobbyEventListener = null;
        ClosePopup();
    }

    public void OnTouchOptionButton()
    {
        popupController.GetPopup<OptionManager>(PopupType.OPTION).OpenPopup();
    }

    public void AddAbilityInfo(string name, Sprite sprite)
    {
        bool abilityExists = false;

        foreach (Transform child in _abilityListTrans)
        {
            if (child.name == name)
            {
                AbilityInfoScript abilityInfo = child.GetComponent<AbilityInfoScript>();
                if (abilityInfo != null) { abilityInfo.CountUp(); }
                abilityExists = true;
                break;
            }
        }

        if (!abilityExists)
        {
            GameObject newAbilityInfo = Instantiate(_abilityInfoPrefab, _abilityListTrans);
            newAbilityInfo.name = name;

            AbilityInfoScript abilityInfo = newAbilityInfo.GetComponent<AbilityInfoScript>();
            if (abilityInfo != null)
            {
                abilityInfo.Setting(sprite);
            }
        }
    }
}
