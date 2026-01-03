using UnityEngine;
using System.Collections.Generic;

public class AIMovement : MonoBehaviour
{
    public Transform target;
    public float speed = 3f;

    Pathfinding pathfinding;
    List<GridNode> path;
    int pathIndex;

    public void Init(Transform movementTarget)
    {
        target = movementTarget.transform;
    }

    void Start()
    {
        pathfinding = FindObjectOfType<Pathfinding>();
        InvokeRepeating(nameof(UpdatePath), 0f, 0.1f);
    }

    void UpdatePath()
    {
        path = pathfinding.FindPath(transform.position, target.position);
        pathIndex = 0;
    }

    void Update()
    {
        if (path == null || pathIndex >= path.Count)
            return;

        Vector3 nextPos = path[pathIndex].worldPosition;
        nextPos.y = transform.position.y;

        transform.position = Vector3.MoveTowards(
            transform.position,
            nextPos,
            speed * Time.deltaTime
        );

        Vector3 dir = nextPos - transform.position;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, lookRot, Time.deltaTime * 8f);
        }

        if (Vector3.Distance(transform.position, nextPos) < 0.1f)
            pathIndex++;
    }

    void OnDrawGizmos()
    {
        if (path == null) return;

        Gizmos.color = Color.cyan;
        foreach (GridNode n in path)
        {
            Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.3f);
        }
    }
}
