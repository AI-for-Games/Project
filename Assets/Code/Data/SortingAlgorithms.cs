using System.Collections;
using System.Collections.Generic;
using System;

public class QuickSort<T>
{
    public void Sort(ref List<T> RefList, Comparison<T> Compare)
    {
        if (RefList == null || RefList.Count == 0)
            return;

        SortMethod(ref RefList, 0, RefList.Count - 1, Compare);
    }

    private void SortMethod(ref List<T> RefList, int Left, int Right, Comparison<T> Compare)
    {
        if (Left >= Right)
            return;

        int Mid = Partition(ref RefList, Left, Right, Compare);

        SortMethod(ref RefList, Left, Mid - 1, Compare);
        SortMethod(ref RefList, Mid + 1, Right, Compare);
    }

    private void Swap(ref List<T> RefList, int i, int j)
    {
        T temp = RefList[i];
        RefList[i] = RefList[j];
        RefList[j] = temp;
    }

    private int Partition(ref List<T> RefList, int Left, int Right, Comparison<T> Compare)
    {
        int i = Left - 1;
        int Pivot = (Left + Right) / 2;
        for (int j = Left; j <= Right; j++)
        {
            if (j == Pivot)
                continue;

            if (Compare(RefList[j], RefList[Pivot]) <= 0)
            {
                i++;
                Swap(ref RefList, i, j);

                if (i == Pivot)
                    Pivot = j;
            }
        }
        i++;
        Swap(ref RefList, i, Pivot);
        return i;
    }
}