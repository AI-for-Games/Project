using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float mouseSensitivity = 2f;

    float rotationX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotationX, transform.localEulerAngles.y + mouseX, 0f);
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        transform.position += move * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.E))
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q))
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;
    }
}
