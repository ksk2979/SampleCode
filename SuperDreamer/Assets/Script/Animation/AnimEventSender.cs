using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// �ִϸ��̼� Ű�̺�Ʈ�� ȣ��޾� �̸� ����� �̺�Ʈ�� ȣ�� �ϴ� Ŭ����
/// </summary>
public class AnimEventSender : MonoBehaviour
{
    private csHashTableDic _dic = new csHashTableDic();
    public void AddEvent(string event_, Action act_)
    {
        if (_dic.ContainKey(event_) == false)
            _dic.Add(event_, act_);
    }

    public void CallEvent(string event_)
    {
        if (_dic.Get(name, event_) != null)
            (_dic.Get(name, event_) as Action)();
    }

    internal void AddEvent(string attacEnd1, string attacEnd2)
    {
        throw new NotImplementedException();
    }
}

public class csHashTableDic
{
    Hashtable listTable = new Hashtable();

    public void Add(object key_, object data_)
    {
        if (listTable.ContainsKey(key_))
        {
            Debug.Log(key_ + " is contains");
            return;
        }
        listTable.Add(key_, data_);
    }

    public object Get(string name, object key_)
    {
        if (!listTable.ContainsKey(key_))
        {
            Debug.Log(name + " name/ " + key_ + " Not Find ");
            return null;
        }
        return listTable[key_];
    }

    public Hashtable GetTable()
    {
        return listTable;
    }

    public bool ContainKey(object key)
    {
        return listTable.ContainsKey(key);
    }

    public void Claer()
    {
        listTable.Clear();
    }
}
