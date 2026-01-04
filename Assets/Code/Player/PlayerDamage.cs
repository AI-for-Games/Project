using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public int damage = 1;
    public float damageInterval = 1f;

    float lastDamageTime;

    void OnTriggerStay(Collider other)
    {
        Debug.Log("Player Damage Triggered");
        if (!other.CompareTag("Enemy")) return;

        if (Time.time < lastDamageTime + damageInterval) return;

        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            lastDamageTime = Time.time;
        }
    }
}
