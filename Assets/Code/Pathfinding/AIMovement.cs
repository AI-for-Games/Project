using UnityEngine;
using System.Collections.Generic;

public class AIMovement : MonoBehaviour
{
    public Transform target;
    public float speed = 3f;

    Pathfinding pathfinding;
    List<Vector3> path;
    int pathIndex;

    void OnEnable()
    {
        NavGrid.OnGridComplete += StartMovement;
    }

    void OnDisable()
    {
        NavGrid.OnGridComplete -= StartMovement;
    }

    void Start()
    {
        pathfinding = FindObjectOfType<Pathfinding>();
        //InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }

    void StartMovement()
    {
        Debug.Log("Starting movement");
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    } 


    void UpdatePath()
    {
        var nodePath = pathfinding.FindPath(transform.position, target.position);
        path = pathfinding.GetSmoothPath(nodePath);
        pathIndex = 0;
    }

    void Update()
    {
        DrawPath();

        if (path == null || pathIndex >= path.Count)
            return;

        Vector3 nextPos = path[pathIndex];
        nextPos.y = transform.position.y;
        
        Vector3 moveDir = (nextPos - transform.position).normalized;
        transform.position += moveDir * (speed * Time.deltaTime);
        if (moveDir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, lookRot, Time.deltaTime * 8f
            );
        }
        
        if (Vector3.Distance(transform.position, nextPos) < 0.1f)
            pathIndex++;
    }
    
    void DrawPath()
    {
        if (path == null || path.Count == 0)
            return;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], Color.cyan);
        }

        foreach (var point in path)
        {
            Debug.DrawRay(point + Vector3.up * 0.1f, Vector3.up * 0.1f, Color.cyan);
        }
    }
}
