using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyData;
using UniRx.Async.Triggers;

public class PlayerSkillSet : MonoBehaviour
{
    InGameUIManager _uiManager;

    [SerializeField] Button _skillButton;
    [SerializeField] Image _skillIcon;
    [SerializeField] Image _coolTimeBlocker;
    [SerializeField] TextMeshProUGUI _coolTimeText;

    ActiveSkillData _skillData;
    float _coolTimeMax;
    float _tmpTime;
    bool _deactivate = false;
    bool _canUse = true;
    bool _noneData = false;

    const string iconSpritePathFormat = "ItemIcon/Skill{0:D2}";

    public void Init(ActiveSkillData data)
    {
        _skillData = data;
        int skillID = 0;
        if (_skillData == null)
        {
            _noneData = true;
            _coolTimeBlocker.fillAmount = 1;
            _coolTimeText.text = string.Empty;
        }
        else
        {
            skillID = _skillData.nId;
            _coolTimeMax = _skillData.coolTime;
            _coolTimeBlocker.fillAmount = 0;
            _coolTimeText.text = string.Empty;
        }
        _skillIcon.sprite = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(iconSpritePathFormat, skillID));
        _skillButton.onClick.RemoveAllListeners();
        _skillButton.onClick.AddListener(OnTouchSkillButton);
    }

    private void Update()
    {
        if (_deactivate)
        {
            _tmpTime += Time.deltaTime;
            var remainTime = _coolTimeMax - _tmpTime;
            string timeText = string.Empty;
            if (remainTime > 10)
            {
                timeText = Mathf.RoundToInt(remainTime).ToString();
            }
            else
            {
                timeText = System.Math.Round(remainTime, 1).ToString();
                string[] timeArr = timeText.Split('.');
                if (timeArr.Length <= 1)
                {
                    timeText += ".0";
                }
            }
            _coolTimeText.text = timeText;
            _coolTimeBlocker.fillAmount = 1 - (_tmpTime / _coolTimeMax);
            if (_tmpTime >= _coolTimeMax)
            {
                _deactivate = false;
                _coolTimeText.text = string.Empty;
                _canUse = true;
                _tmpTime = 0;
            }
        }
    }

    /// <summary>
    /// 사용 상태로 전환
    /// </summary>
    void SetUseState()
    {
        _deactivate = true;
        _canUse = false;
    }

    public void OnTouchSkillButton()
    {
        if (_noneData)
        {
            return;
        }

        if (_canUse)
        {
            UseSkill();
        }
    }

    /// <summary>
    /// 개별 스킬 사용 메서드
    /// </summary>
    void UseSkill()
    {
        switch (SkillID)
        {
            case 1:
                Accelerate();
                //Explosion();
                break;
            case 2:
                break;
            default:
                break;
        }
    }

    public void ActivateSkill(System.Action skillAction, float waitTime)
    {
        StartCoroutine(DelayedActivate(skillAction, waitTime));
    }
    IEnumerator DelayedActivate(System.Action skillAction, float waitTime)
    {
        yield return YieldInstructionCache.WaitForSeconds(waitTime);
        skillAction.Invoke();
    }
    public int SkillID => _skillData.nId;

    #region Skill
    /// <summary>
    /// 자폭 스킬
    /// </summary>
    void Explosion()
    {
        ExplosionPopup popup = InGameUIManager.GetInstance.GetPopup<ExplosionPopup>(PopupType.EXPLOSION);
        popup.Init();
        popup.SetSkillScript(this);
        popup.AddBoatInfo(UserData.GetInstance.UnitInfo, StageManager.GetInstance.GetPlayers());
        popup.OnOpenEventListener += () =>
        {
            Time.timeScale = 0;
        };
        popup.OnCloseEventListener += () =>
        {
            Time.timeScale = 1;
        };
        popup.OnSelectEventListener += () =>
        {
            SetUseState();
        };
        popup.OpenPopup();
    }

    void Accelerate()
    {
        if (_uiManager == null) _uiManager = InGameUIManager.GetInstance;
        SetUseState();
        _uiManager.Accelerate(true);
    }
    #endregion Skill

    public void ActivateSkillUI(bool state)
    {
        gameObject.SetActive(state);
    }
}
