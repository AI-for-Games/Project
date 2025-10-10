using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * TODO: Use provided data from unity framework for now until we have our own
 */

[ExecuteAlways]
public class AI_Nav_Volume : MonoBehaviour
{
    //Size
    [Header("Bounds")]
    public float Width = 20.0f;
    public float Height = 20.0f;
    public float Depth = 20.0f;

    private GameObject m_cube_wire_frame;

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

        DrawWireFrame();
    }

    void OnValidate()
    {

    }

    void DrawWireFrame()
    {
        if (m_cube_wire_frame != null)
            DestroyImmediate(m_cube_wire_frame);

        m_cube_wire_frame = new GameObject("WireCube");
        m_cube_wire_frame.transform.SetParent(transform);
        m_cube_wire_frame.transform.localPosition = Vector3.zero;

        LineRenderer lr = m_cube_wire_frame.AddComponent<LineRenderer>();
        lr.positionCount = 16;
        lr.loop = false;
        lr.widthMultiplier = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = Color.white;

        Vector3 w = new Vector3(Width / 2f, Height / 2f, Depth / 2f);

        Vector3[] verts = new Vector3[]
        {
            new Vector3(-w.x, -w.y, -w.z),
            new Vector3(w.x, -w.y, -w.z),
            new Vector3(w.x, -w.y, w.z),
            new Vector3(-w.x, -w.y, w.z),
            new Vector3(-w.x, -w.y, -w.z),

            new Vector3(-w.x, w.y, -w.z),
            new Vector3(w.x, w.y, -w.z),
            new Vector3(w.x, -w.y, -w.z),

            new Vector3(w.x, w.y, -w.z),
            new Vector3(w.x, w.y, w.z),
            new Vector3(w.x, -w.y, w.z),
            new Vector3(w.x, w.y, w.z),

            new Vector3(-w.x, w.y, w.z),
            new Vector3(-w.x, -w.y, w.z),
            new Vector3(-w.x, w.y, w.z),
            new Vector3(-w.x, w.y, -w.z)
        };

        lr.SetPositions(verts);
    }
};