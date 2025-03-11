using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyData;

public class UserInterface : MonoBehaviour
{
    UserData _userData;

    [SerializeField] TextMeshProUGUI[] _uiTextArr;
    [SerializeField] TextMeshProUGUI _userLevel;
    [SerializeField] TextMeshProUGUI _remainTimeText;
    [SerializeField] Slider _userExpSlider;
    [SerializeField] RectTransform[] _userInterfaceIcon; // 실제 아이콘이 있는 곳
    [SerializeField] TextMeshProUGUI _userNickName;

    #region Text
    public void UpdateRemainTimeText(string remainTime)
    {
        _remainTimeText.text = remainTime;
        if(remainTime.Length > 0)
        {
            _remainTimeText.gameObject.SetActive(true);
        }
        else
        {
            _remainTimeText.gameObject.SetActive(false);
        }
    }

    public void UpdateUIText()
    {
        if (_userData == null) { _userData = UserData.GetInstance; }
        _uiTextArr[(int)EPropertyType.MONEY].text = _userData.GetCurrency(EPropertyType.MONEY).ToString();
        _uiTextArr[(int)EPropertyType.DIAMOND].text = _userData.GetCurrency(EPropertyType.DIAMOND).ToString();
        _uiTextArr[(int)EPropertyType.ACTIONENERGY].text = string.Format("{0}/{1}", _userData.GetCurrency(EPropertyType.ACTIONENERGY), 30);
    }
    public void UpdateLevelText()
    {
        if (_userData == null) { _userData = UserData.GetInstance; }
        _userLevel.text = _userData.GetUserLevel.ToString();
        var nextLevelData = DataManager.GetInstance.FindData(DataManager.KEY_LEVEL, _userData.GetUserLevel + 1) as LevelData;
        if(nextLevelData != null)
        {
            _userExpSlider.value = (float)_userData.GetUserExp / (float)nextLevelData.exp;
        }
        else
        {
            _userExpSlider.value = 1.0f;
        }
    }

    public void UpdateAllText()
    {
        UpdateUIText();
        UpdateLevelText();
    }
    #endregion Text

    public RectTransform GetRectTrans => GetComponent<RectTransform>();
    public RectTransform[] GetInterfaceIcon() { return _userInterfaceIcon; }
}
