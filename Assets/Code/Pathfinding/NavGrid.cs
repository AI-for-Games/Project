using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NavGrid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridSize;
    public float nodeRadius = 0.5f;
    
    GridNode[,] grid;
    float nodeDiameter;
    int grideSizeX, grideSizeY;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        grideSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        grideSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new GridNode[grideSizeX, grideSizeY];
        Vector3 worldBottomLeft = transform.position
                                  - Vector3.right * gridSize.x / 2
                                  - Vector3.forward * gridSize.y / 2;

        for (int x = 0; x < grideSizeX; x++)
        {
            for (int y = 0; y < grideSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft
                                     + Vector3.right * (x * nodeDiameter)
                                     + Vector3.forward * (y * nodeDiameter);
                
                bool walkable =!Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[x,y] = new GridNode(walkable, worldPoint, x, y);
            }
        }
    }

    public GridNode NodeFromWorldPoint(Vector3 worldPosition)
    {
        float PercentageX = (worldPosition.x / gridSize.x) * nodeDiameter;
        float PercentageY = (worldPosition.y / gridSize.y) * nodeDiameter;
        
        PercentageX = Mathf.Clamp01(PercentageX);
        PercentageY = Mathf.Clamp01(PercentageY);
        
        int x = Mathf.RoundToInt((gridSize.x - 1) * grideSizeX);
        int y = Mathf.RoundToInt((gridSize.y - 1) * grideSizeY);
        
        return grid[x, y];
    }

    public System.Collections.Generic.List<GridNode> GetNeighbours(GridNode node)
    {
        var neighbours = new System.Collections.Generic.List<GridNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ( x == 0 && y == 0)
                    continue;
                
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < grideSizeX && checkY >= 0 && checkY < grideSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));
        
        if (grid != null) return;

        foreach (GridNode node in grid)
        {
            Gizmos.color = node.walkable ? Color.white : Color.red;
            Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
        }
    }
}