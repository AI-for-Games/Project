using UnityEngine;
using System.Collections.Generic;

public class AIMovement : MonoBehaviour
{
    public Transform target;
    public float speed = 3f;

    Pathfinding pathfinding;
    List<GridNode> path;
    int pathIndex;

    void Start()
    {
        pathfinding = FindObjectOfType<Pathfinding>();
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }

    void UpdatePath()
    {
        path = pathfinding.FindPath(transform.position, target.position);
        pathIndex = 0;
        
        Debug.Log(path == null ? "PATH NULL" : $"PATH LENGTH: {path.Count}");
    }

    void Update()
    {
        if (path == null || pathIndex >= path.Count)
            return;

        Vector3 nextPos = path[pathIndex].worldPosition;
        transform.position = Vector3.MoveTowards(
            transform.position,
            nextPos,
            speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, nextPos) < 0.1f)
            pathIndex++;
    }
}
