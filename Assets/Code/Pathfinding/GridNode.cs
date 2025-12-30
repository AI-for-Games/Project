using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridNode
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;  
    public int gridY;
    public int gCost;
    public int hCost;
    public GridNode Parent;
    public int fCost => gCost + hCost;

    public GridNode(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}
