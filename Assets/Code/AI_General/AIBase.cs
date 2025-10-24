using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase : MonoBehaviour
{
    //We define our own position and velocity for more control
    public Vector3 m_position = Vector3.zero;
    public Vector3 m_velocity = Vector3.one;

    public void Update()
    {
        //update position using velocity relative to dt, and have this reflect in the owner 
        m_position += m_velocity * Time.deltaTime;
        transform.position = m_position;
    }
}
