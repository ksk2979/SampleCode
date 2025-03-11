using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 무적 스크립트
public class Invincible : MonoBehaviour
{
    public Image _frontImg;
    const int _maxCount = 3; // 무적 먹은 횟수 (총3회 맥스) 180초, 90초, 60초
    public int _count = 0;

    float _time_Cooltime = 5f;
    float _time;
    float _time_Start;
    bool _isEnded = true;

    PlayerStats _playerStats = null;
    GameObject _invincibleEffect;

    public void InvincibleCountUp()
    {
        if (_playerStats == null) { _playerStats = GameObject.FindGameObjectWithTag(CommonStaticDatas.TAG_PLAYER).GetComponent<PlayerStats>(); }
        _count++;
        if (_count == 1) { _time_Cooltime = 90f; }
        else if (_count == 2) { _time_Cooltime = 70; }
        else if (_count >= 3) { _time_Cooltime = 50f; }
        StartCoroutine(StartGo());
    }

    IEnumerator StartGo()
    {
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(1f);
            StartCoolTimeBtn();
        }
    }

    public void StartCoolTimeBtn()
    {
        if (!_isEnded) { return; }
        if (_playerStats.GetIsInvincibility()) { return; }

        // 쿨타임 스킬 실행
        //Debug.Log("무적 실행?");
        if (_invincibleEffect == null) { _invincibleEffect = SimplePool.Spawn(CommonStaticDatas.RES_EX, "FX_Hit_12", _playerStats.transform, Vector3.zero, Quaternion.identity); }
        //if (_invincibleEffect != null) { _invincibleEffect.SetActive(true); }
        StartCoroutine(_playerStats.InvincibleStart(_invincibleEffect));
        
        // 쿨타임 돌게
        StartCoroutine(ICheck_CoolTime());
    }

    IEnumerator ICheck_CoolTime()
    {
        Reset_CoolTime();
        while (!_isEnded)
        {
            _time = Time.time - _time_Start;
            if (_time < _time_Cooltime)
            {
                Set_FillAmount(_time_Cooltime - _time);
            }
            else if (!_isEnded)
            {
                End_CoolTime();
            }
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }
    }

    private void End_CoolTime()
    {
        _frontImg.gameObject.SetActive(false);
        Set_FillAmount(0);
        _isEnded = true;
    }

    private void Reset_CoolTime()
    {
        _frontImg.gameObject.SetActive(true);
        _time = _time_Cooltime;
        _time_Start = Time.time;
        Set_FillAmount(_time_Cooltime);
        _isEnded = false;
    }
    private void Set_FillAmount(float _value)
    {
        _frontImg.fillAmount = _value / _time_Cooltime;
    }
}