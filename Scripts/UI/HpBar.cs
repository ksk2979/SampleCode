using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HpBar : MonoBehaviour
{
    public Image _fg = null;
    public Image _bg = null;
    public float _upValue = 2;
    private RectTransform _myTransform = null;
    private Transform _target = null;
    private Vector2 _sizeDelta;
    private CharacterStats stat;
    private TextMeshProUGUI _hpText;

    [Header("Ability")]
    [SerializeField] Image _abilityImage;
    [SerializeField] TextLocalizeSetter _abilityName;

    [SerializeField] GameObject _explosionNotice;

    void Start()
    {
        _myTransform = transform as RectTransform;
        _sizeDelta = _myTransform.sizeDelta;
        _fg.rectTransform.sizeDelta = _sizeDelta;
        _bg.rectTransform.sizeDelta = _sizeDelta;

        _abilityImage.sprite = null;
        _abilityName.key = string.Empty;
        SetExplosionState(false);
        _abilityImage.transform.parent.gameObject.SetActive(false);
    }

    public void SetTarget(CharacterStats stat, float high)
    {
        this.stat = stat;
        _target = stat.transform;
        _upValue = high;
        if (this.stat.GetComponent<Player>() != null)
        {
            if (this.stat.GetComponent<Player>()._unit == UNIT.Boat)
            {
                _hpText = this.transform.Find("HpText").GetComponent<TextMeshProUGUI>();
                _hpText.gameObject.SetActive(true);
            }
        }
    }

    public void SetAbilityInfo(Sprite abilitySprite, string abilityName)
    {
        StopCoroutine(HideAbilityIcon());
        _abilityImage.sprite = abilitySprite;
        _abilityName.key = abilityName;
        _abilityImage.transform.parent.gameObject.SetActive(true);
        StartCoroutine(HideAbilityIcon());
    }

    IEnumerator HideAbilityIcon()
    {
        yield return YieldInstructionCache.WaitForSeconds(2.0f);
        _abilityImage.transform.parent.gameObject.SetActive(false);
    }
    
    void LateUpdate()
    {
        if (_target == null)
            return;

        _myTransform.position = Camera.main.WorldToScreenPoint(_target.TransformPoint(Vector3.up * _upValue));

        _fg.fillAmount = _target.GetComponent<CharacterStats>().HpRate();
        if (_hpText != null) { _hpText.text = _target.GetComponent<CharacterStats>().HpSRate(); }
    }

    public void SetExplosionState(bool state) => _explosionNotice.SetActive(state);
}
