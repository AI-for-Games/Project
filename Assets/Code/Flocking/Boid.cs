using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    //We define our own position and velocity for more control
    public Vector3 m_position = Vector3.zero;
    public Vector3 m_velocity = Vector3.one;

    //Neighboring boids queerying data
    public float m_neighbor_search_radius = 1000.0f;
    public int m_max_number_of_neighbors = 1000;

    //visual representation 
    private GameObject m_game_object;
    private Vector3 m_scale = Vector3.one * 0.7f;

    //Create the visual representation
    void Start()
    {
        m_game_object = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    }

    void Update()
    {
        //update position using velocity relative to dt, and have this reflect in the owner 
        m_position += m_velocity * Time.deltaTime;
        transform.position = m_position;

        //do the same for the visual representation -> see comment above 
        if (m_game_object != null)
        {
            m_game_object.transform.position = m_position; ;
            m_game_object.transform.localScale = m_scale;
            m_game_object.transform.SetParent(transform);
        }
    }
}
