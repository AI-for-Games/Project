using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NavGrid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridSize;
    public float nodeRadius;
    GridNode[,] grid;
    
    float nodeDiameter;
    int gridSizeX, gridSizeY;
    
    // Start is called before the first frame update
    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateGrid()
    {
        grid = new GridNode[gridSizeX, gridSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * gridSizeX/2 - Vector3.forward * gridSizeY/2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 pos = bottomLeft + (Vector3.right * (x * nodeDiameter + nodeRadius)
                                         + Vector3.forward * (y * nodeDiameter + nodeRadius));
                bool walkable = !(Physics.CheckSphere(pos, nodeRadius));
                grid[x,y] = new GridNode(walkable, pos);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSizeX, gridSizeY, 1));

        if (grid != null)
        {
            foreach (GridNode node in grid)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(node.position, Vector3.one * nodeDiameter);
            }
        }
    }
}
