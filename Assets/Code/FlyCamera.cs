using UnityEngine;

namespace Code
{
    public class FlyCamera : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 10f;        // Movement speed
        public float boostMultiplier = 2f;   // Speed boost when holding Shift
        public float lookSpeed = 3f;         // Mouse look sensitivity

        private bool _cursorLocked;

        void Start()
        {
            LockCursor(false);
        }

        void Update()
        {
            // Only respond if the game window is focused
            if (!Application.isFocused) return;

            // Toggle cursor lock with Escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _cursorLocked = !_cursorLocked;
                LockCursor(_cursorLocked);
            }

            if (_cursorLocked)
            {
                HandleLook();
                HandleMovement();
            }
        }

        private void HandleLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

            // Rotate camera horizontally
            transform.Rotate(Vector3.up, mouseX, Space.World);

            // Rotate camera vertically
            transform.Rotate(Vector3.left, mouseY, Space.Self);
        }

        private void HandleMovement()
        {
            float speed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                speed *= boostMultiplier;

            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            Vector3 up = transform.up;

            Vector3 move = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) move += forward;
            if (Input.GetKey(KeyCode.S)) move -= forward;
            if (Input.GetKey(KeyCode.A)) move -= right;
            if (Input.GetKey(KeyCode.D)) move += right;
            if (Input.GetKey(KeyCode.Q)) move -= up;
            if (Input.GetKey(KeyCode.E)) move += up;

            transform.position += move * (speed * Time.deltaTime);
        }

        private void LockCursor(bool locked)
        {
            Cursor.visible = !locked;
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}
