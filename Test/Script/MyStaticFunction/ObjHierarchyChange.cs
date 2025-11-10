using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjHierarchyChange
{
    /// <summary>
    /// Hierarchy에서 오브젝트 객체 첫번째로 이동
    /// </summary>
    public static void FirstObjMove(GameObject parnet, int getChild)
    {
        if (parnet.transform.GetChild(getChild).gameObject == null) { return; }

        // 자식 선택
        GameObject child = parnet.transform.GetChild(getChild).gameObject;

        // 자식 객체의 마지막 오브젝트를 처음으로 이동
        child.transform.SetAsFirstSibling();
    }

    /// <summary>
    /// Hierarchy에서 오브젝트 마지막 객체로 이동
    /// </summary>
    public static void LastObjMove(GameObject parnet, int getChild)
    {
        if (parnet.transform.GetChild(getChild).gameObject == null) { return; }

        // 자식 선택
        GameObject child = parnet.transform.GetChild(getChild).gameObject;

        child.transform.SetAsLastSibling();
    }

    public static void FirstObjMove(Transform target)
    {
        target.transform.SetAsFirstSibling();
    }

    public static void LastObjMove(Transform target)
    {
        target.transform.SetAsLastSibling();
    }

    /// <summary>
    /// Hierarchy에서 오브젝트 순서 변경
    /// </summary>
    public static void SetObjMove(Transform parnet, int getChild, int setChild)
    {
        if (parnet.GetChild(getChild).gameObject == null) { return; }
        if (parnet.childCount <= setChild) { return; }

        // 자식 선택
        GameObject child = parnet.GetChild(getChild).gameObject;

        // 원하는 순서로 변경
        child.transform.SetSiblingIndex(setChild);
    }
    public static void SetObjMove(Transform child, int setChild)
    {
        child.transform.SetSiblingIndex(setChild);
    }

    /// <summary>
    /// Hierarchy에서 오브젝트 현재 순서 반환 (get의 오브젝트가 없으면 -1 반환)
    /// </summary>
    /// <returns></returns>
    public static int GetObjOrder(GameObject parnet, int getChild)
    {
        if (parnet.transform.GetChild(getChild).gameObject == null) { return -1; }

        // 자식 선택
        GameObject child = parnet.transform.GetChild(getChild).gameObject;

        return child.transform.GetSiblingIndex();
    }
}
