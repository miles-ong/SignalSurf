using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float drag = 10f;

    [Header("Bounds")]
    [SerializeField] private float minY = -4f;
    [SerializeField] private float maxY = 4f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Movement state
    private float verticalVelocity = 0f;
    private float moveInput = 0f;

    // Public properties
    public Vector3 Position => transform.position;
    public float Velocity => verticalVelocity;

    void Start()
    {
        // Get sprite renderer if not assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Get input (legacy for now, can switch to New Input System later)
        moveInput = 0f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            moveInput = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            moveInput = -1f;

        // Apply acceleration or drag
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            // Accelerate in input direction
            verticalVelocity += moveInput * acceleration * Time.deltaTime;
        }
        else
        {
            // Apply drag when no input
            verticalVelocity = Mathf.MoveTowards(verticalVelocity, 0f, drag * Time.deltaTime);
        }

        // Clamp velocity to max speed
        verticalVelocity = Mathf.Clamp(verticalVelocity, -moveSpeed, moveSpeed);

        // Update position
        Vector3 newPosition = transform.position;
        newPosition.y += verticalVelocity * Time.deltaTime;

        // Clamp position to bounds
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;
    }

    // Optional: Visualize bounds in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 leftPoint = new Vector3(transform.position.x - 1f, minY, 0f);
        Vector3 rightPoint = new Vector3(transform.position.x + 1f, minY, 0f);
        Gizmos.DrawLine(leftPoint, rightPoint);

        leftPoint = new Vector3(transform.position.x - 1f, maxY, 0f);
        rightPoint = new Vector3(transform.position.x + 1f, maxY, 0f);
        Gizmos.DrawLine(leftPoint, rightPoint);
    }
}
