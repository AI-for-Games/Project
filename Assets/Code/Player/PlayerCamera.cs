using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10, 0);

    void Update()
    {
        transform.position = target.position + offset;
    }
}
