using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingController : MonoBehaviour
{
    [Header("Boid Meta")]
    public int NumberOfBoids = 1000;

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

        for (int i = 0; i < m_boids.Length; i++)
        {
            m_boids[i] = new GameObject("Boid_" + i);
            Boid boid = m_boids[i].AddComponent<Boid>();
            boid.m_position = NavVolume.GetRandomPointWithinVolume();
        }
    }

    /*
     * This can be substituted for spacial partitioning later on
     */
    List<int> GetNeighborsOfBoid(int index)
    {
        List<int> Neighbors = new List<int>();

        Boid local_boid = m_boids[index].GetComponent<Boid>();

        for (int i = 0; i < m_boids.Length; i++)
        {
            if (i == index) continue;

            Boid possible_neighbor = m_boids[i].GetComponent<Boid>();

            if ((local_boid.m_position - possible_neighbor.m_position).sqrMagnitude < local_boid.m_neighbor_search_radius * local_boid.m_neighbor_search_radius)
            {
                Neighbors.Add(i);
                if (Neighbors.Count >= local_boid.m_max_number_of_neighbors)
                    break;
            }
        }

        return Neighbors;
    }

    Vector3 GetAlignmentContribution(int Index, List<int> Neighbors)
    {
        if (Neighbors.Count == 0) return Vector3.zero;

        Vector3 Resultant = Vector3.zero;

        for (int i = 0; i < Neighbors.Count; i++)
        {
            Boid neighbor_boid = m_boids[Neighbors[i]].GetComponent<Boid>();
            Resultant += neighbor_boid.m_velocity;
        }
        return Resultant / Neighbors.Count * m_AlignmentWeight;
    }

    Vector3 GetAvoidanceContribution(int Index, List<int> Neighbors)
    {
        if (Neighbors.Count == 0) return Vector3.zero;

        Vector3 Resultant = Vector3.zero;

        for (int i = 0; i < Neighbors.Count; i++)
        {
            Boid neighbor_boid = m_boids[Neighbors[i]].GetComponent<Boid>();
            Resultant += neighbor_boid.m_position - m_boids[Index].GetComponent<Boid>().m_position;
        }
        return Resultant / Neighbors.Count * -m_AvoidanceWeight;
    }

    Vector3 GetCohesionContribution(int Index, List<int> Neighbors)
    {
        if (Neighbors.Count == 0) return Vector3.zero;

        Vector3 Resultant = Vector3.zero;

        for (int i = 0; i < Neighbors.Count; i++)
        {
            Boid neighbor_boid = m_boids[Neighbors[i]].GetComponent<Boid>();
            Resultant += neighbor_boid.m_position;
        }
        return ((Resultant / Neighbors.Count)-m_boids[Index].GetComponent<Boid>().m_position) * m_CohesionWeight;
    }


    void Update()
    {
        CreateSlider slider = FindObjectOfType<CreateSlider>();
        if (slider == null)
            return;

        m_CohesionWeight = (float)System.Math.Round((float)slider.m_cohesion_slider.value / 100.0f, 2);
        m_AlignmentWeight = (float)System.Math.Round((float)slider.m_alignment_slider.value / 100.0f, 2);
        m_AvoidanceWeight = (float)System.Math.Round((float)slider.m_avoidance_slider.value / 100.0f, 2);

        for (int i = 0; i < m_boids.Length; i++)
        {
            List<int> Neighbors = GetNeighborsOfBoid(i);
            Vector3 Resultant = GetAlignmentContribution(i, Neighbors) +GetCohesionContribution(i, Neighbors) +GetAvoidanceContribution(i, Neighbors);

            //add some randomness
            Resultant += new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10)).normalized *0.5f;
            m_boids[i].GetComponent<Boid>().m_velocity = Resultant;
        }
    }



}
