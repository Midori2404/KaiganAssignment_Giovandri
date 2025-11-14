using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float verticalSpeed = 3f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 150f;

    private float rotationX;
    private float rotationY;

    private CharacterController controller;
    private Vector3 velocity;

    private bool cameraLookEnabled = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        LockCursor();
    }

    void Update()
    {
        HandleCursorState();

        HandleMovement();

        if (cameraLookEnabled)
        {
            HandleMouseLook();
        }
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = (transform.forward * v + transform.right * h).normalized;

        float vertical = 0f;

        if (Input.GetKey(KeyCode.E))
            vertical = +1f;
        else if (Input.GetKey(KeyCode.Q))
            vertical = -1f;

        velocity = move * moveSpeed + Vector3.up * vertical * verticalSpeed;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    void HandleCursorState()
    {
        // If player holds Left Alt, unlock mouse and stop panning
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            UnlockCursor();
            cameraLookEnabled = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            LockCursor();
            cameraLookEnabled = true;
        }
    }
    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
