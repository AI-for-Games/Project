using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 m_position = Vector3.zero;
    public Vector3 m_velocity = Vector3.zero;


    private GameObject m_game_object;
    private Vector3 m_scale = Vector3.one * 0.7f;

    void Start()
    {
        m_game_object = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    }

    void Update()
    {
        m_position += m_velocity * Time.deltaTime;
        transform.position = m_position;

        if (m_game_object != null)
        {
            m_game_object.transform.position = m_position; ;
            m_game_object.transform.localScale = m_scale;
            m_game_object.transform.SetParent(transform);
        }
    }
}
