using System;
using System.Collections;
using System.Collections.Generic;
using Code.Generation;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NavGrid : MonoBehaviour
{
    public static event Action OnGridComplete;  // Event that fires when generation is done
    
    public LayerMask unwalkableMask;
    public Vector2 gridSize;
    public float nodeRadius = 0.5f;
    
    GridNode[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void OnEnable()
    {
        DungeonGenerator.OnDungeonGenerated += BuildGrid;  // Sign up to dungeon generated event
    }

    void OnDisable()
    {
        DungeonGenerator.OnDungeonGenerated -= BuildGrid;
    }

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        //CreateGrid();
    }

    void BuildGrid()
    {
        StartCoroutine(BuildGridNextPhysicsFrame());
    }

    IEnumerator BuildGridNextPhysicsFrame()
    {
        yield return new WaitForFixedUpdate();  // Wait for physics to sync
        CreateGrid();
    }

    void CreateGrid()
    {
        Debug.Log("CreateGrid called. Walls in scene: " +
                  FindObjectsOfType<Collider>().Length);

        grid = new GridNode[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position
                                  - Vector3.right * gridSize.x / 2
                                  - Vector3.forward * gridSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft
                                     + Vector3.right * (x * nodeDiameter)
                                     + Vector3.forward * (y * nodeDiameter);
                
                bool walkable =!Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[x,y] = new GridNode(walkable, worldPoint, x, y);
            }
        }
        
        OnGridComplete?.Invoke();
    }

    public GridNode NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x - transform.position.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPosition.z - transform.position.z + gridSize.y / 2) / gridSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(
            Mathf.FloorToInt(percentX * gridSizeX),
            0, gridSizeX - 1
        );

        int y = Mathf.Clamp(
            Mathf.FloorToInt(percentY * gridSizeY),
            0, gridSizeY - 1
        );

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

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
    
    void OnDrawGizmos()
    {
        if (!Application.isPlaying && grid == null)
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
            CreateGrid();
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position,
            new Vector3(gridSize.x, 1, gridSize.y));

        if (grid == null) return;

        foreach (GridNode n in grid)
        {
            Gizmos.color = n.walkable ? Color.white : Color.red;
            Gizmos.DrawCube(n.worldPosition,
                Vector3.one * (nodeDiameter - 0.1f));
        }
    }
}