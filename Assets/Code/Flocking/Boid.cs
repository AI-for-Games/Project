using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : AIBase
{
    //Neighboring boids queerying data
    public float m_neighbor_search_radius = 8.0f;
    public int m_max_number_of_neighbors = 16;

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
        base.Update();
        //do the same for the visual representation -> see comment above 
        if (m_game_object != null)
        {
            m_game_object.transform.position = m_position;
            m_game_object.transform.localScale = m_scale;
            m_game_object.transform.SetParent(transform);
        }
    }
}
