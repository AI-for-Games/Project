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

    void OnStart()
    {
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
