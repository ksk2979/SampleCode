using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellScript : MonoBehaviour
{
    [SerializeField] GameObject _disableObj;
    [SerializeField] TextMeshProUGUI _countText;
    [SerializeField] Image _coolTimeImg;
    Button _eventBtn;
    [HideInInspector] public AttackType _type;
    [HideInInspector] public int _index = 0;

    SlotListScript _slotList;

    public void Init(AttackType type, int index, SlotListScript slotList)
    {
        _type = type;
        _index = index;
        _slotList = slotList;
        _eventBtn = GetComponent<Button>();
        _eventBtn.onClick.AddListener(() => PushBtn());
    }

    public void PushBtn()
    {
        _slotList.InfoOpen(this);
    }

    public void CountUpdate(int count)
    {
        _countText.text = count.ToString();
        if (count == 0) { _disableObj.SetActive(true); }
        else { _disableObj.SetActive(false); }
    }
    public void CoolTime(float now, float max)
    {
        _coolTimeImg.fillAmount = Mathf.Clamp01(now / max);
    }
}
