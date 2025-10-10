using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingController : MonoBehaviour
{
    [Header("Boid Meta")]
    public int NumberOfBoids = 10;

    private GameObject[] m_boids;

    private float m_AlignmentWeight = 0;
    private float m_CohesionWeight = 0;
    private float m_AvoidanceWeight = 0;


    void Start()
    {
        AI_Nav_Volume NavVolume = FindObjectOfType<AI_Nav_Volume>();
        if (NavVolume == null)
            return;

        m_boids = new GameObject[NumberOfBoids];

        for(int i = 0; i < m_boids.Length; i++)
        {
            m_boids[i] = new GameObject("Boid_" + i);
            Boid boid = m_boids[i].AddComponent<Boid>();
            boid.m_position = NavVolume.GetRandomPointWithinVolume();
        }
    }

    void Update()
    {
        CreateSlider slider = FindObjectOfType<CreateSlider>();
        if (slider == null)
            return;

        m_CohesionWeight = (float)System.Math.Round((float)slider.m_cohesion_slider.value / 100.0f, 2);
        m_AlignmentWeight = (float)System.Math.Round((float)slider.m_alignment_slider.value / 100.0f, 2);
        m_AvoidanceWeight = (float)System.Math.Round((float)slider.m_avoidance_slider.value / 100.0f, 2);

        for(int i = 0; i < m_boids.Length; i++)
        {
            //continue with rules here
        }
    }



}
