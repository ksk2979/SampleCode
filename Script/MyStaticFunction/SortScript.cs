using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SortScript
{
    /// <summary>
    /// 버블정렬 오름차순
    /// </summary>
    public static void BubbleSortUp(ref int[] data)
    {
        int temp;
        int lastArr = data.Length - 1;
        for (int i = 0; i < lastArr; ++i)
        {
            for (int j = 0; j < lastArr - i; ++j)
            {
                if (data[j] > data[j + 1])
                {
                    temp = data[j];
                    data[j] = data[j + 1];
                    data[j + 1] = temp;
                }
            }
        }
    }

    /// <summary>
    /// 버블정렬 내림차순
    /// </summary>
    public static void BubbleSortDown(ref int[] data)
    {
        int temp;
        int lastArr = data.Length;
        for (int i = 0; i < lastArr; ++i)
        {
            for (int j = 0; j < (lastArr - 1) - i; ++j)
            {
                if (data[j] < data[j + 1])
                {
                    temp = data[j];
                    data[j] = data[j + 1];
                    data[j + 1] = temp;
                }
            }
        }
    }
}
