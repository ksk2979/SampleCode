using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/CraftingRecipeData")]
public class CraftingRecipeData : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public ItemData _a;
        public ItemData _b;
        public ItemData _result;
        public int _resultCount;   // 결과 수량(기본 1 권장)
    }

    [SerializeField] List<Entry> _entries = new List<Entry>();

    // A,B 순서 무시하고 매칭
    public bool TryFindResult(ItemData a, ItemData b, out ItemData result, out int resultCount)
    {
        result = null;
        resultCount = 0;
        if (a == null || b == null) { return false; }

        int idA = a._id, idB = b._id;
        if (idA == 0 || idB == 0) { return false; }

        for (int i = 0; i < _entries.Count; i++)
        {
            var e = _entries[i];
            if (e._a == null || e._b == null || e._result == null) continue;

            int ea = e._a._id;
            int eb = e._b._id;
            if ((ea == idA && eb == idB) || (ea == idB && eb == idA))
            {
                result = e._result;
                resultCount = Mathf.Max(1, e._resultCount);
                return true;
            }
        }
        return false;
    }
}
