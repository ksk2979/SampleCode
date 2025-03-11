using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ShopBoxPopup : PopupBase
{
    [Header("One Box")]
    [SerializeField] UnitIcon _icon;
    [SerializeField] Image[] _boxImage;

    [Header("Ten Boxes")]
    [SerializeField] GameObject _tenBoxesResult;
    [SerializeField] UnitIcon[] _unitIconArr;

    [Header("Animator")]
    Animator _oneboxAnimator;
    Animator _tenBoxesAnimator;

    [SerializeField] GameObject _skipBtn;
    [SerializeField] GameObject _closeBtn;
    [SerializeField] GameObject _tenCloseBtn;

    int _itemCount = 0;
    bool _tenOn = false;
    bool _skip = false;
    bool _showNextBox = false;
    bool _endShow = false;

    #region Init / Set Info
    public void Init()
    {
        _oneboxAnimator = GetComponent<Animator>();
        _tenBoxesAnimator = _tenBoxesResult.GetComponent<Animator>();

        _tenBoxesResult.SetActive(false);
    }

    public void SetBoxImages(Sprite boxImg01, Sprite boxImg02)
    {
        _boxImage[0].sprite = boxImg01;
        _boxImage[1].sprite = boxImg02;
    }

    public void SetResultInfo(UnitIcon[] unitArr)
    {
        if (unitArr.Length > 1)
        {
            for (int i = 0; i < _unitIconArr.Length; ++i)
            {
                string nameText = string.Format("{0}\n{1}",
                    StandardFuncData.GradeCheck(unitArr[i].GetGrade),
                    LocalizeText.Get(UnitDataManager.GetInstance.GetStringValue(unitArr[i].GetItemType, unitArr[i].GetID, StatusType.name)));
                _unitIconArr[i].GetIconUI().CopyUISetting(unitArr[i].GetIconUI());
                _unitIconArr[i].UpdateNameText(nameText);
            }
        }
    }

    void ResetTenBoxes()
    {
        _itemCount = 0;
        _tenOn = false;
        _endShow = false;
        _skipBtn.SetActive(false);
        _tenCloseBtn.SetActive(false);
        _tenBoxesResult.SetActive(false);
        _oneboxAnimator.enabled = true;
    }
    #endregion Init / Set Info

    #region Open Box
    public void OpenOneBox(UnitIcon unit)
    {
        _icon.gameObject.SetActive(true);
        string nameText = string.Format("{0}\n{1}",
            StandardFuncData.GradeCheck(unit.GetGrade),
            LocalizeText.Get(UnitDataManager.GetInstance.GetStringValue(unit.GetItemType, unit.GetID, StatusType.name)));
        _icon.GetIconUI().CopyUISetting(unit.GetIconUI());
        _icon.UpdateNameText(nameText);

        _closeBtn.SetActive(false);
        SoundManager.GetInstance.PlayAudioEffectSound("UI_Box_Open");

        if (!gameObject.activeSelf)
        {
            OnOpenEventListener += () => _oneboxAnimator.enabled = true;
            OpenPopup();
        }
        StartCoroutine(DelayCloseBtn());
    }

    public void OpenTenBoxes(UnitIcon[] unitArr)
    {
        SetResultInfo(unitArr);
        _tenOn = true;
        OnCloseEventListener += ResetTenBoxes;

        OpenPopup();

        StartCoroutine(DelaySkipBtn());
        StartCoroutine(DelayTenCloseBtn(unitArr));
    }

    #endregion Open Box

    #region IEnumerator
    IEnumerator DelaySkipBtn()
    {
        yield return YieldInstructionCache.WaitForSeconds(1f);
        _skipBtn.SetActive(true);
    }
    IEnumerator DelayTenCloseBtn(UnitIcon[] unit)
    {
        // 각각 표시
        while (_itemCount < 10)
        {
            if (_skip) { break; }
            _showNextBox = false;
            _oneboxAnimator.SetTrigger(CommonStaticDatas.ANIMPARAM_OPEN);

            yield return YieldInstructionCache.RealTimeWaitForSeconds(0.5f);

            OpenOneBox(unit[_itemCount]);

            if (_skip) { break; }

            while (!_showNextBox)
            {
                if (_skip) { break; }
                yield return YieldInstructionCache.WaitForFixedUpdate;
            }
            _itemCount++;
            _closeBtn.SetActive(false);
        }
        _oneboxAnimator.enabled = false;
        _boxImage[0].gameObject.SetActive(false);
        _boxImage[1].gameObject.SetActive(true);
        _icon.gameObject.SetActive(false);
        _tenBoxesResult.SetActive(true);
        _tenBoxesAnimator.enabled = true;
        SoundManager.GetInstance.PlayAudioEffectSound("UI_10Box_Open");

        _endShow = true;
        _skip = false;
        _closeBtn.SetActive(false);
        _skipBtn.SetActive(false);

        yield return YieldInstructionCache.WaitForSeconds(1f);
        _tenCloseBtn.SetActive(true);
    }

    IEnumerator DelayCloseBtn()
    {
        yield return YieldInstructionCache.WaitForSeconds(1.5f);
        _closeBtn.SetActive(true);
    }
    #endregion IEnumerator

    #region Buttons
    public void OnTouchSkipButton()
    {
        _skip = true;
    }

    public void OnTouchCloseButton()
    {
        if (_tenOn)
        {
            if (_endShow)
            {
                ClosePopup();
            }
            else
            {
                _showNextBox = true;
            }
        }
        else
        {
            ClosePopup();
        }
    }
    #endregion Buttons

    public override void ClosePopup()
    {
        base.ClosePopup();
    }
}
