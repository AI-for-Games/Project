using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSort<T>
{
    public void Sort(ref List<T> RefList, Comparison<T> Compare)
    {
        if (RefList == null || RefList.Count == 0)
            return;

        SortMethod(ref RefList, 0, RefList.Count-1, Compare);
    }

    private void SortMethod(ref List<T> RefList, int Left, int Right, Comparison<T> Compare)
    {
        if (Left >= Right)
            return;

        int Mid = Partition(ref RefList, Left, Right, Compare);

        SortMethod(ref RefList, Left, Mid-1, Compare);
        SortMethod(ref RefList, Mid+1, Right, Compare);
    }

    private void Swap(ref List<T> RefList, int i, int j)
    {
        T temp = RefList[i];
        RefList[i] = RefList[j];
        RefList[j] = temp;
    }

    private int Partition(ref List<T> RefList, int Left, int Right, Comparison<T> Compare)
    {
        int i = Left-1;
        int Pivot = (Left + Right)/2;
        for(int j = Left; j <= Right; j++)
        {
            if (j == Pivot)
                continue;

            if(Compare(RefList[j], RefList[Pivot]) <= 0)
            {
                i++;
                Swap(ref RefList, i, j);

                if(i ==  Pivot)
                    Pivot = j;
            }
        }
        i++;
        Swap(ref RefList, i, Pivot);
        return i;
    }
}

public class KD_Tree_Node<T> where T : MonoBehaviour
{
    public T m_Data;
    public KD_Tree_Node m_Left;
    public KD_Tree_Node m_Right;


    void GetNearestNeighbors(Vector3 Target, float SearchRadiusSqrd, int MaxNeighbors, ref List<T, float> Neighbors, int Axis = 0)
    {
        float DistanceSqrd = (Target - m_Data.transform.position).sqrMagnitude;

        if(DistanceSqrd <= SearchRadiusSqrd)
        {
            Neighbors.Add({m_Data, DistanceSqrd});
            QuickSort<T, float> QS = new QuickSort<T, float>();
            QS.Sort(Neighbors, (a, b)=>a.second <= b.second));
       
            if (Neighbors.Count == MaxNeighbors)
            Neighbors.RemoveAt(Neighbors.Count - 1);
        }

        KD_Tree_Node Priority = null;
        KD_Tree_Node SecondPriority = null;

        if (Target[Axis] <= m_Data.transform.position[Axis])
        {
            Priority = m_Left;
            SecondPriority = m_Right;
        }
        else
        {
            Priority = m_Right;
            SecondPriority = m_Left;
        }

        if(Priority != null)
            Priority.GetNearestNeighbors(Target, SearchRadiusSqrd, MaxNeighbors, ref Neighbors);

        if (Neighbors.Count < MaxNeighbors && SecondPriority != null)
            SecondPriority.GetNearestNeighbors(Target, SearchRadiusSqrd, MaxNeighbors, ref Neighbors);
    }
}

public class KD_Tree<T> : MonoBehaviour where T : MonoBehaviour
{
    private int m_TickModulus = 0;
    private const int m_UpdateEveryNTicks = 5;

    private KD_Tree_Node m_Root;

    private void InsertNode(T Object)
    {
        if (m_CompareDelegateArray == null)
        {
            return;
        }

        if(m_Root == null)
        {
            m_Root = new KD_Tree_Node<T>();
            m_Root.m_Data = Object;
            return;
        }

        KD_Tree_Node<T> CurrentNode = m_Root;
        int Axis = 0;

        while(true)
        {
            //go left
            if (Object.transform.position[Axis] <= CurrentNode.m_Data.transform.position[Axis])
            {
                if (CurrentNode.m_Left == null)
                {
                    CurrentNode.m_Left = new KD_Tree_Node<T>();
                    CurrentNode.m_Left.m_Data = Object;
                    break;
                }
                else
                {
                    CurrentNode = CurrentNode.m_Left;
                    Axis++;
                    Axis %= 3;
                }
            }
            //go right
            else
            {
                if (CurrentNode.m_Right == null)
                {
                    CurrentNode.m_Right = new KD_Tree_Node<T>();
                    CurrentNode.m_Right.m_Data = Object;
                    break;
                }
                else
                {
                    CurrentNode = CurrentNode.m_Right;
                    Axis++;
                    Axis %= 3;
                }
            }
        }
    }

    private void CreateKDTree(ref List<T> Objects)
    {
        for(int i = 0; i < Objects.Count; i++)
        {
            T Obj = Objects[i];
            InsertNode(Obj);
        }
    };

    void Update()
    {
        if(m_TickModulus == 0)
        {
            if(m_Root != null)
                m_Root = null;

            CreateKDTree(FindObjectsOfType<T>());
        }

        m_TickModulus++;
        m_TickModulus %= m_UpdateEveryNTicks;
    }
}
