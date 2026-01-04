using UnityEngine;

public class AIDamage : MonoBehaviour
{
    public int damage = 1;
    public float damageInterval = 1f;
    public float damageDistance = 1.5f;

    float lastDamageTime;

    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= damageDistance)
            {
                if (Time.time >= lastDamageTime + damageInterval)
                {
                    Health health = GetComponent<Health>();
                    if (health != null)
                    {
                        health.TakeDamage(damage);
                        lastDamageTime = Time.time;
                    }
                }
            }
        }
    }
}
