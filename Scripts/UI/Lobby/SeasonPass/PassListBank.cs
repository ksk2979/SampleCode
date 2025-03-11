using System.Collections.Generic;
using UnityEngine;
using AirFishLab.ScrollingList;
using AirFishLab.ScrollingList.ContentManagement;

public class PassListBank : BaseListBank
{
    private int _numOfContents;

    private readonly List<int> _contents = new List<int>();
    private readonly PassListContent _contentWrapper = new PassListContent();
    
    public void Init(int maxCount)
    {
        _numOfContents = maxCount;
        for (var i = 0; i < _numOfContents; ++i)
            _contents.Add(i + 1);

    }
    public void SetListBankCount(int count)
    {
        _numOfContents = count;
    }

    public override IListContent GetListContent(int index)
    {
        _contentWrapper.Value = _contents[index];
        return _contentWrapper;
    }

    public override int GetContentCount()
    {
        return _contents.Count;
    }
}
