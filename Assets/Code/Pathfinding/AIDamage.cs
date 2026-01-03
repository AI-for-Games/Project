using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDamage : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Health health = collision.collider.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(1);
        }
    }
}
