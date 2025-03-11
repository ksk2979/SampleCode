using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyData;
using MyStructData;

public class OneDayRewardPopup : PopupBase
{
    LobbyUIManager _lobbyManager;
    UserData _userData;
    [SerializeField] OneDayRewardScript[] _iconBoxArr;
    [SerializeField] Button[] _closeBtn; // 첫 입장 출첵 완료되면 시간이 지나면 창 닫기 나오게

    [SerializeField] Sprite[] _iconBoxSpriteArr;

    bool _doneIconInit = false;
    int _rewardCount = 0;

    const string _spritePath = "ItemIcon/{0}";
    const string _prevFileName = "Inven";
    const int _maxDay = 28;
    const int _blueStartDay = 3;
    const int _redStartDay = 7;
    const string _titleText = "일일 보상";
    public void Init(bool oneDay)
    {
        CheckComponents();
        IconInit();
        _rewardCount = _userData.GetDailyLoginCount();

        if (oneDay)
        {
            _closeBtn[0].gameObject.SetActive(false);
            _closeBtn[1].interactable = false;
            OpenPopup();
            //OnCloseEventListener += ShowRewardEffect;
            SetIconBlurState(_iconBoxArr.Length, false);

            if (_rewardCount == _iconBoxArr.Length)
            {
                _rewardCount = 0;
            }
            ReceiveReward(_rewardCount);
            StartCoroutine(OneDayDelayBlocker(_rewardCount));
            _rewardCount++;
            _userData.SetDailyLoginCount(_rewardCount);

            // 일일 상품 초기화
            _lobbyManager.GetShopPage.RewardInit(true);

            // 룰렛 초기화
            _userData.SetRouletteCount(0);

            _userData.SaveDailyRewardData();
            _lobbyManager.ResetCheckTime(ETimeCheckType.ONEDAY, false);
            InitIconBlurState(_rewardCount - 1);

            // 서버가 실행중이면 초기화시 전체적으로 올려준다
            if (UserData.GetInstance.GetServer) { UserData.GetInstance.ServerSave(); }
        }
        else
        {
            _lobbyManager.GetShopPage.RewardInit(false);
            InitIconBlurState(_rewardCount);
        }
    }

    void IconInit()
    {
        if(!_doneIconInit)
        {
            _doneIconInit = true;
            var dataList = DataManager.GetInstance.GetList<OneDayRewardData>(DataManager.KEY_ONEDAYREWARD);

            int blueDay = _blueStartDay;
            int redDay = _redStartDay;
            for(int i = 0; i < dataList.Count; i++)
            {
                Sprite boxSprite = null;
                // 현재 아이콘 정립이 안되어있는 부분도 있기때문에 배열의 첫번째 타입을 아이콘으로 사용한다
                string fileName = _prevFileName + ((ERewardType)dataList[i].rewardType[0]).ToString();
                Sprite iconSprite = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(_spritePath, fileName));
                if (blueDay == dataList[i].nId)
                {
                    blueDay += 7;
                    boxSprite = _iconBoxSpriteArr[1];
                }
                else if(redDay == dataList[i].nId)
                {
                    redDay += 7;
                    boxSprite = _iconBoxSpriteArr[2];
                }
                else
                {
                    boxSprite = _iconBoxSpriteArr[0];
                }

                _iconBoxArr[i].Init(iconSprite, boxSprite, dataList[i].rewardValue[0].ToString());
            }
        }
    }

    /// <summary>
    /// 컴포넌트 체크
    /// </summary>
    void CheckComponents()
    {
        if (_lobbyManager == null)
        {
            _lobbyManager = LobbyUIManager.GetInstance;
        }
        if (_userData == null)
        {
            _userData = UserData.GetInstance;
        }
    }

    /// <summary>
    /// 버튼으로 팝업 진입
    /// </summary>
    public void OnTouchOpenPopup()
    {
        _lobbyManager.CheckRewardTimes((int)ETimeCheckType.ONEDAY);
        OpenPopup();
    }

    void SetIconBlurState(int max, bool blurState)
    {
        for (int i = 0; i < max; ++i)
        {
            _iconBoxArr[i].SetBlur(blurState);
        }
    }

    /// <summary>
    /// 보상 수령
    /// </summary>
    /// <param name="number"></param>
    public void ReceiveReward(int number)
    {
        var data = DataManager.GetInstance.FindData(DataManager.KEY_ONEDAYREWARD, number + 1) as OneDayRewardData;

        if(data.rewardType.Length == 1)
        {
            ERewardType rType = (ERewardType)data.rewardType[0];
            int rValue = data.rewardValue[0];

            _lobbyManager.GetShopPage.ProvideNotPurchasingReward(EPayType.NONE, rType, rValue, _titleText, false);
        }
        else
        {
            ERewardType[] rTypeArr = new ERewardType[data.rewardType.Length];
            int[] rValueArr = new int[data.rewardValue.Length];

            for (int i = 0; i < data.rewardType.Length; i++)
            {
                rTypeArr[i] = (ERewardType)data.rewardType[i];
                rValueArr[i] = data.rewardValue[i];
            }   

            _lobbyManager.GetShopPage.ProvideNotPurchasingAllReward(EPayType.NONE, rTypeArr, rValueArr, _titleText, false);
        }
    }
    IEnumerator OneDayDelayBlocker(int number)
    {
        int num = number;
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        _iconBoxArr[num].SetBlur(true);
        yield return YieldInstructionCache.WaitForSeconds(1f);
        _closeBtn[0].gameObject.SetActive(true);
        _closeBtn[1].interactable = true;
    }

    void InitIconBlurState(int curCount)
    {
        SetIconBlurState(_iconBoxArr.Length, false);
        SetIconBlurState(curCount, true);
    }
}