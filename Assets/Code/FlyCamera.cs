using UnityEngine;

namespace Code
{
    public class FlyCamera : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 10f;        // Movement speed
        public float boostMultiplier = 2f;   // Speed boost when holding Shift
        public float lookSpeed = 3f;         // Mouse look sensitivity

        private bool _cameraFocused;

        private void Start()
        {
            SetFocus(false);
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) 
                SetFocus(true);
        }

        private void Update()
        {
            // Only respond if the game window is focused
            if (!Application.isFocused) return;

            if (!_cameraFocused && Input.GetMouseButton(0))
                SetFocus(true);

            // Toggle cursor lock with Escape
            if (Input.GetKeyDown(KeyCode.Escape))
                SetFocus(!_cameraFocused);
            
            if (!_cameraFocused) return;
            
            HandleLook();
            HandleMovement();
        }

        private void HandleLook()
        {
            var mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            var mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

            // Rotate camera horizontally
            transform.Rotate(Vector3.up, mouseX, Space.World);

            // Rotate camera vertically
            transform.Rotate(Vector3.left, mouseY, Space.Self);
        }

        private void HandleMovement()
        {
            var speed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                speed *= boostMultiplier;

            var forward = transform.forward;
            var right = transform.right;
            var up = transform.up;

            var move = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) move += forward;
            if (Input.GetKey(KeyCode.S)) move -= forward;
            if (Input.GetKey(KeyCode.A)) move -= right;
            if (Input.GetKey(KeyCode.D)) move += right;
            if (Input.GetKey(KeyCode.Q)) move -= up;
            if (Input.GetKey(KeyCode.E)) move += up;

            transform.position += move * (speed * Time.deltaTime);
        }

        private void SetFocus(bool focus)
        {
            _cameraFocused = focus;
            Cursor.visible = !focus;
            Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}
