using UnityEngine;

public class TopDownPlayer : MonoBehaviour
{
    public float speed = 6f;
    public LayerMask groundMask;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;

    private CharacterController controller;
    private Camera cam;
    float nextFireTime;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    void Update()
    {
        Move();
        RotateToMouse();
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(x, 0f, z).normalized;
        controller.Move(move * (speed * Time.deltaTime));
    }

    void RotateToMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 lookDir = hit.point - transform.position;
            lookDir.y = 0f;

            if (lookDir != Vector3.zero)
                transform.forward = lookDir;
        }
    }
    
    void Shoot()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 dir = (hit.point - firePoint.position);
            dir.y = 0f;
            dir.Normalize();

            GameObject bulletObj = Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.LookRotation(dir)
            );

            bulletObj.GetComponent<Bullet>().Fire(dir);
        }
    }
}
