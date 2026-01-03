using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Fire(Vector3 direction)
    {
        GetComponent<Rigidbody>().velocity = direction * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        Health health = collision.collider.GetComponent<Health>();
        if (health != null && !collision.collider.CompareTag("Player"))
        {
            health.TakeDamage(1);
        }

        Destroy(gameObject);
    }
}
