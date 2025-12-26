using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Grid = UnityEngine.Grid;

public class Pathfinding : MonoBehaviour
{
    NavGrid grid;

    void Start()
    {
        grid = GetComponent<NavGrid>();
    }


    public List<GridNode> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        GridNode startNode = grid.NodeFromWorldPoint(startPos);
        GridNode targetNode = grid.NodeFromWorldPoint(targetPos);

        List<GridNode> openSet = new List<GridNode>();
        HashSet<GridNode> closedSet = new HashSet<GridNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            GridNode currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    openSet[i].fCost == currentNode.fCost &&
                    openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (GridNode neighbour in grid.GetNeighbours(currentNode))
            {
                if(!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }
                
                int newCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                
                if (newCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.Parent = currentNode;
                    
                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        return null;
    }

    List<GridNode> RetracePath(GridNode startNode, GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        
        path.Reverse();
        return path;
    }

    int GetDistance(GridNode startNode, GridNode endNode)
    {
        int distanceX = Mathf.Abs(startNode.gridX - endNode.gridX);
        int distanceY = Mathf.Abs(startNode.gridY - endNode.gridY);

        if (distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
