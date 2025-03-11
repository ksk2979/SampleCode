using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyData;
using System.Globalization;
public class QuestScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _titleTMP;
    [SerializeField] QuestItem[] _questItemArr;
    [SerializeField] RectTransform _contentsRect;
    const string rateFormat = "{0} / {1}";
    const string spritePath = "ItemIcon/Inven{0}";

    public void SetTitleText(string title)
    {
        _titleTMP.text = title;
    }

    /// <summary>
    /// 퀘스트 정보 세팅
    /// </summary>
    public void SetQuestInfo(List<string> questName, List<string> context, 
        List<int> curCount, List<int> maxCount, List<bool> receiptedList, List<int> idList)
    {
        for(int i = 0; i < _questItemArr.Length; i++)
        {
            List<string> textList = new List<string>();
            textList.Add(questName[i]);
            textList.Add(context[i]);
            textList.Add(string.Format(rateFormat, curCount[i], maxCount[i]));
            
            // Data
            var data = DataManager.GetInstance.FindData(DataManager.KEY_QUEST, idList[i]) as QuestData;
            Sprite icon = null;
            int rCount = 0;
            if(data != null)
            {
                icon = ResourceManager.GetInstance.GetSpriteClipForKey(string.Format(spritePath, ((ERewardType)data.rewardType).ToString()));
                rCount = data.rewardValue;
            }
            _questItemArr[i].SetQuestItem(textList, icon, (ERewardType)data.rewardType, rCount, curCount[i] / (float)maxCount[i], receiptedList[i]);
        }
    }

    /// <summary>
    /// 버튼 기능 설정
    /// </summary>
    /// <param name="buttonActionList">기능 리스트</param>
    public void SetButtonAction(List<System.Action> buttonActionList)
    {
        for(int i = 0; i < _questItemArr.Length; i++)
        {
            _questItemArr[i].SetButton(buttonActionList[i]);
        }
    }

    /// <summary>
    /// 수령 가능한 보상이 있는지 확인
    /// </summary>
    /// <returns></returns>
    public bool CheckReceiveableItem()
    {
        foreach(var quest in _questItemArr)
        {
            if(quest.IsReceivable)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 퀘스트 정렬 메서드
    /// 1. 수령가능 / 2. 미클리어 / 3. 클리어 완료 순으로 정렬
    /// </summary>
    public void SortQuestList()
    {
        List<int> receiveableList = new List<int>();
        List<int> notClearedList = new List<int>();
        List<int> clearedList = new List<int>();
        
        var receivedList = UserData.GetInstance.GetDailyQuestState();
        for (int i = 0; i < _questItemArr.Length; i++)
        {
            if(_questItemArr[i].IsReceivable)
            {
                receiveableList.Add(i);
            }
            else
            {
                if(!receivedList[i])
                {
                    notClearedList.Add(i);
                }
                else
                {
                    clearedList.Add(i);
                }
            }
        }

        if(notClearedList.Count > 0)
        {
            notClearedList.Sort((a, b) => _questItemArr[b].Rate.CompareTo(_questItemArr[a].Rate));
        }
        List<int> sortedList = new List<int>();
        sortedList.AddRange(receiveableList);
        sortedList.AddRange(notClearedList);
        sortedList.AddRange(clearedList);

        Transform parent = _contentsRect.transform;

        for (int j = 0; j < sortedList.Count; j++)
        {
            //Debug.Log("SortQuestList : " + sortedList[j] + " to " + j);
            ObjHierarchyChange.SetObjMove(parent, _questItemArr[sortedList[j]].gameObject, j);
        }
    }

    public void OpenBoard()
    {
        _contentsRect.anchoredPosition = new Vector2(0, -(_contentsRect.sizeDelta.y / 2));
        gameObject.SetActive(true);
    }

    public void CloseBoard()
    {
        gameObject.SetActive(false);
    }

    public QuestItem[] GetQuestItemArr => _questItemArr;
}
