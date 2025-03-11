using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageBoxPopup : MonoBehaviour
{
    [SerializeField] UnitIcon[] _tenIcon;
    [SerializeField] GameObject _tenCloseBtn;
    [SerializeField] GameObject _tenObj;
    //[SerializeField] GameObject _skipBtn;
    [SerializeField] Animator _tenAnima;

    public void TenBoxOpen(int weapon, int costum, int nack, int belt, int glove, int shoes, int gem, UnitIcon[] unit)
    {
        _tenIcon[0].GetIconUI().GetLevelTMP.text = string.Format("x{0}", weapon);
        if (costum == 0) { _tenIcon[1].gameObject.SetActive(false); }
        else { _tenIcon[1].gameObject.SetActive(true); _tenIcon[1].GetIconUI().GetLevelTMP.text = string.Format("x{0}", costum); }

        if (nack == 0) { _tenIcon[2].gameObject.SetActive(false); }
        else { _tenIcon[2].gameObject.SetActive(true); _tenIcon[2].GetIconUI().GetLevelTMP.text = string.Format("x{0}", nack); }

        if (belt == 0) { _tenIcon[3].gameObject.SetActive(false); }
        else { _tenIcon[3].gameObject.SetActive(true); _tenIcon[3].GetIconUI().GetLevelTMP.text = string.Format("x{0}", belt); }

        if (glove == 0) { _tenIcon[4].gameObject.SetActive(false); }
        else { _tenIcon[4].gameObject.SetActive(true); _tenIcon[4].GetIconUI().GetLevelTMP.text = string.Format("x{0}", glove); }

        if (shoes == 0) { _tenIcon[5].gameObject.SetActive(false); }
        else { _tenIcon[5].gameObject.SetActive(true); _tenIcon[5].GetIconUI().GetLevelTMP.text = string.Format("x{0}", shoes); }

        _tenIcon[6].GetIconUI().GetLevelTMP.text = string.Format("x{0}", gem);

        int arr = 0;
        int max = unit.Length;
        for (int i = 7; i < _tenIcon.Length; ++i)
        {
           /* _tenIcon[i].gameObject.SetActive(true);
            _tenIcon[i].GetIconUI().GetOutLine.sprite = unit[arr].GetIconUI().GetOutLine.sprite;
            _tenIcon[i].GetIconUI().GetBasicImage.sprite = unit[arr].GetIconUI().GetBasicImage.sprite;
            _tenIcon[i].GetIconUI().GetBasicImage.color = unit[arr].GetIconUI().GetBasicImage.color;
            _tenIcon[i].GetIconUI().GetIconImage.sprite = unit[arr].GetIconUI().GetIconImage.sprite;
            _tenIcon[i].GetIconUI().GetLevelTMP.text = string.Format("{0}\n{1}", StandardFuncData.GradeCheck(unit[arr].GetGrade), LocalizeText.Get(unit[arr].GetName));*/
            arr++;
            if (max == arr) { break; }
        }
        _tenObj.SetActive(true);
        StartCoroutine(DelayTenCloseBtn());
    }
    IEnumerator DelayTenCloseBtn()
    {
        if (_tenAnima != null) { _tenAnima.SetTrigger(CommonStaticDatas.ANIMPARAM_OPEN); }
        
        //SoundManager.GetInstance.PlayAudioEffectSound("UI_10Box_Open");

        yield return YieldInstructionCache.WaitForSeconds(1f);
        _tenCloseBtn.SetActive(true);
    }
    public void TenCloseBtn()
    {
        _tenObj.SetActive(false);
        _tenCloseBtn.SetActive(false);
        for (int i = 7; i < _tenIcon.Length; ++i)
        {
            _tenIcon[i].gameObject.SetActive(false);
        }
    }
}
