using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * TODO: Use provided data from unity framework for now until we have our own
 */

public class AI_Nav_Volume : MonoBehaviour
{
    //Size
    [Header("Bounds")]
    public float Width = 20.0f;
    public float Height = 20.0f;
    public float Depth = 20.0f;

    public Vector3 GetRandomPointWithinVolume()
    {
        return new Vector3(
                    transform.position.x + Random.Range(-Width / 2f, Width / 2f + 1.0f),
                    transform.position.y + Random.Range(-Height / 2f, Height / 2f + 1.0f),
                    transform.position.z + Random.Range(-Depth / 2f, Depth / 2f + 1.0f)
                );
    }

    void Start()
    {
        Random.InitState(0);
        transform.position = Vector3.zero;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height, Depth));
    }

    void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
    }
}
