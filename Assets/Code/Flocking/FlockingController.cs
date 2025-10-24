using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingController : MonoBehaviour
{
    //expose boid count to editor ui
    [Header("Boid Meta")]
    public int NumberOfBoids = 1000;

    //array of  boids
    private List<AIBase> m_boids;

    //flocking rule weights, retrieved from custom ui sliders
    private float m_AlignmentWeight = 0;
    private float m_CohesionWeight = 0;
    private float m_AvoidanceWeight = 0;

    void Start()
    {
        //grab the nav volume from the scene
        AI_Nav_Volume NavVolume = FindObjectOfType<AI_Nav_Volume>();
        if (NavVolume == null)
            return;

        //create the boid array
        m_boids = new List<AIBase>();

        //create, configure and assign boid to boid array
        for (int i = 0; i < NumberOfBoids; i++)
        {
            GameObject BoidObject = new GameObject("Boid_" + i);
            Boid LocalBoid = BoidObject.AddComponent<Boid>();

            m_boids.Add(LocalBoid);

            LocalBoid.m_position = NavVolume.GetRandomPointWithinVolume();
        }
    }

    /*
     * This can be substituted for spacial partitioning later on
     */
    GameObject tree = null;
    List<Neighbor> GetNeighborsOfBoid(int Index)
    {
        if (tree == null)
        {
            tree = new GameObject("KD_Tree");
            tree.AddComponent<KD_Tree>();
            tree.GetComponent<KD_Tree>().CreateKDTree(m_boids);
        }

        Boid LocalBoid = (Boid)m_boids[Index];
        List<Neighbor> Neighbors = new List<Neighbor>();
        tree.GetComponent<KD_Tree>().m_Root.GetNearestNeighbors(LocalBoid.m_position, LocalBoid.m_neighbor_search_radius * LocalBoid.m_neighbor_search_radius, LocalBoid.m_max_number_of_neighbors, ref Neighbors);

        QuickSort<Neighbor> QS = new QuickSort<Neighbor>();
        QS.Sort(ref Neighbors, (a, b) => a.m_Distance.CompareTo(b.m_Distance));

        while (Neighbors.Count > LocalBoid.m_max_number_of_neighbors)
            Neighbors.RemoveAt(Neighbors.Count - 1);

        return Neighbors;
    }

    Vector3 GetAlignmentContribution(int Index, List<Neighbor> Neighbors)
    {
        //check for empty neighbors
        if (Neighbors.Count == 0) return Vector3.zero;

        //define empty resultant
        Vector3 Resultant = Vector3.zero;

        //loop over neighbors and sum the velocities
        for (int I = 0; I < Neighbors.Count; I++)
        {
            AIBase neighbor_boid = Neighbors[I].m_Data;
            Resultant += neighbor_boid.m_velocity;
        }
        //return the average velocity normaized into the alignment weight
        return (Resultant / Neighbors.Count).normalized * m_AlignmentWeight;
    }

    Vector3 GetAveragePositionOfNeighboringBoids(List<Neighbor> Neighbors)
    {
        //define empty resultant
        Vector3 Resultant = Vector3.zero;

        //loop over all neighbors and add position to resultant
        for (int i = 0; i < Neighbors.Count; i++)
        {
            Resultant += Neighbors[i].m_Data.m_position;
        }
        return Resultant / Neighbors.Count;
    }

    Vector3 GetAvoidanceContribution(int Index, List<Neighbor> Neighbors)
    {
        //check for empty neighbors
        if (Neighbors.Count == 0) return Vector3.zero;

        //grab average position of neighbor boids
        Vector3 ToNeighboringBoids = GetAveragePositionOfNeighboringBoids(Neighbors);

        //grab local boid position
        Vector3 LocalBoidPosition = m_boids[Index].GetComponent<Boid>().m_position;

        //compute resultant
        return -(ToNeighboringBoids - LocalBoidPosition).normalized * m_AvoidanceWeight;
    }

    Vector3 GetCohesionContribution(int Index, List<Neighbor> Neighbors)
    {
        //check for empty neighbors
        if (Neighbors.Count == 0) return Vector3.zero;

        //grab average position of neighbor boids
        Vector3 ToNeighboringBoids = GetAveragePositionOfNeighboringBoids(Neighbors);

        //grab local boid position
        Vector3 LocalBoidPosition = m_boids[Index].GetComponent<Boid>().m_position;

        //compute resultant
        return (ToNeighboringBoids - LocalBoidPosition).normalized * m_CohesionWeight;
    }


    void Update()
    {
        //update flocking rule coefficients
        CreateSlider slider = FindObjectOfType<CreateSlider>();
        if (slider == null)
            return;

        m_CohesionWeight = (float)System.Math.Round((float)slider.m_cohesion_slider.value / 100.0f, 2);
        m_AlignmentWeight = (float)System.Math.Round((float)slider.m_alignment_slider.value / 100.0f, 2);
        m_AvoidanceWeight = (float)System.Math.Round((float)slider.m_avoidance_slider.value / 100.0f, 2);

        //for each boid
        for (int i = 0; i < m_boids.Count; i++)
        {
            //grab all neighbors
            List<Neighbor> Neighbors = GetNeighborsOfBoid(i);

            //compute final resultant
            Vector3 Resultant = Vector3.zero;
            Resultant += GetAlignmentContribution(i, Neighbors);
            Resultant += GetCohesionContribution(i, Neighbors);
            Resultant += GetAvoidanceContribution(i, Neighbors);

            //add some randomness
            Vector3 RandomVec3 = new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10)).normalized;
            RandomVec3.y = 0.0f;
            RandomVec3.z = 0.0f;
            Resultant += RandomVec3 * 0.2f;


            //test code
            //little rule to keep boids draw to centre of scene
            //Resultant += (Vector3.zero - m_boids[i].GetComponent<Boid>().m_position) * 0.4f;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 boidPos = m_boids[i].GetComponent<Boid>().m_position;
                Vector3 ToHitPoint = hit.point - boidPos;
                ToHitPoint.y = 0.0f;
                Resultant += ToHitPoint.normalized * 0.4f;
            }
            //test code

            //set boid velocity
            Boid LocalBoid = m_boids[i].GetComponent<Boid>();

            //LocalBoid.m_velocity = Resultant * (Random.Range(1, 5));
            float interpSpeed = 8.0f;
            LocalBoid.m_velocity += (Resultant - LocalBoid.m_velocity) * (1 - Mathf.Exp(-interpSpeed * Time.deltaTime));
        }
    }



}
