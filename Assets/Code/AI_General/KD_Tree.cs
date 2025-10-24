using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Neighbor
{
    public AIBase m_Data;
    public float m_Distance;
}

public class KD_Tree_Node
{
    public AIBase m_Data;
    public KD_Tree_Node m_Left;
    public KD_Tree_Node m_Right;


    public void GetNearestNeighbors(Vector3 Target, float SearchRadiusSqrd, int MaxNeighbors, ref List<Neighbor> Neighbors, int Axis = 0)
    {
        float DistanceSqrd = (Target - m_Data.transform.position).sqrMagnitude;

        if (DistanceSqrd <= SearchRadiusSqrd)
        {
            Neighbors.Add(new Neighbor { m_Data = m_Data, m_Distance = DistanceSqrd });
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

        int NextAxis = (Axis + 1) % 3;

        if (Priority != null)
            Priority.GetNearestNeighbors(Target, SearchRadiusSqrd, MaxNeighbors, ref Neighbors, NextAxis);

        float diff = Target[Axis] - m_Data.transform.position[Axis];
        if (diff * diff <= SearchRadiusSqrd && SecondPriority != null)
            SecondPriority.GetNearestNeighbors(Target, SearchRadiusSqrd, MaxNeighbors, ref Neighbors, NextAxis);
    }
}

public class KD_Tree : MonoBehaviour
{
    private int m_TickModulus = 0;
    private const int m_UpdateEveryNTicks = 60;

    public KD_Tree_Node m_Root = null;
    List<AIBase> m_AIBases = null;

    private void InsertNode(AIBase Object)
    {
        if (m_Root == null)
        {
            m_Root = new KD_Tree_Node();
            m_Root.m_Data = Object;
            return;
        }

        KD_Tree_Node CurrentNode = m_Root;
        int Axis = 0;

        while (true)
        {
            //go left
            if (Object.transform.position[Axis] <= CurrentNode.m_Data.transform.position[Axis])
            {
                if (CurrentNode.m_Left == null)
                {
                    CurrentNode.m_Left = new KD_Tree_Node();
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
                    CurrentNode.m_Right = new KD_Tree_Node();
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

    public void CreateKDTree(List<AIBase> Objects)
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            AIBase Obj = Objects[i];
            InsertNode(Obj);
        }
    }

    void Start()
    {
        m_AIBases = new List<AIBase>(0);
    }

    void Update()
    {
        if (m_TickModulus == 0)
        {
            if(m_AIBases.Count == 0)
            {
                m_AIBases = new List<AIBase>(FindObjectsOfType<AIBase>());   
            }
            m_Root = null;
            CreateKDTree(m_AIBases);
        }

        m_TickModulus++;
        m_TickModulus %= m_UpdateEveryNTicks;
    }
}
