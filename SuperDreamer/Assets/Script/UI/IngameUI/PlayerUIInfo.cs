using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUIInfo : MonoBehaviour
{
    [SerializeField] Vector3[] _uiMovePos;
    [SerializeField] GameObject[] _addPageArr;
    [SerializeField] SlotListScript _slotList;
    [SerializeField] GameInfo _gameInfo;
    [SerializeField] TextMeshProUGUI[] _waveText;
    [SerializeField] SpellInfo _spellInfo;
    [SerializeField] BountyScript _bountyS;
    RectTransform _playerUI;
    float _moveSpeed = 8f;

    bool _up = false;
    bool _isMoving = false;

    public void Init()
    {
        _playerUI = this.GetComponent<RectTransform>();
        _playerUI.anchoredPosition = _uiMovePos[0];
        _slotList.Init(_spellInfo);
    }

    public void MoveUpdate()
    {
        if (_isMoving)
        {
            Vector3 targetPos = _up ? _uiMovePos[1] : _uiMovePos[0];
            _playerUI.anchoredPosition = Vector3.Lerp(_playerUI.anchoredPosition, targetPos, _moveSpeed * Time.deltaTime);

            if (Vector3.Distance(_playerUI.anchoredPosition, targetPos) < 0.2f)
            {
                _playerUI.anchoredPosition = targetPos;
                _isMoving = false;
            }
        }
    }
    public void ToggleUIPosition()
    {
        if (_isMoving) return;
        _up = !_up;
        _isMoving = true;
    }
    public void AddPageOpen(int arr)
    {
        CloseAddPage();
        _addPageArr[arr].SetActive(true);
    }
    public void CloseAddPage()
    {
        for (int i = 0; i < _addPageArr.Length; ++i)
        {
            _addPageArr[i].SetActive(false);
        }
    }
    public bool AddPageCheck(int arr)
    {
        return _addPageArr[arr].activeSelf;
    }

    public void WaveCountUpdate(int count)
    {
        _waveText[0].text = string.Format("{0}", count);
    }
    public void WaveTimeUpdate(float time)
    {
        _waveText[1].text = string.Format("{0}", time.ToString("F0"));
    }

    public void BountyUpdate()
    {
        _bountyS.TimeUpdate();
    }

    public bool IsUp { get { return _up; } }
    public bool IsMoveing { get { return _isMoving; } }
    public SlotListScript GetSlotListS { get { return _slotList; } }
    public GameInfo GetGameInfo { get { return _gameInfo; } }
}
