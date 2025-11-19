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

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float invulnerabilityTime = 0.1f; // Prevent damage spam

    [Header("Score")]
    [SerializeField] private float scorePerSecond = 10f; // Points gained per second when aligned

    // Movement state
    private float verticalVelocity { get; set; } = 0f;
    private float moveInput = 0f;

    // Health state
    private float currentHealth;
    private float lastDamageTime = -999f;

    // Score state
    private float currentScore = 0f;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0f;
    public float CurrentScore => currentScore;

    void Start()
    {
        // Get sprite renderer if not assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Initialize health
        currentHealth = maxHealth;
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

    public void TakeDamage(float damage)
    {
        if (!IsAlive)
            return;

        // Check invulnerability timer
        if (Time.time - lastDamageTime < invulnerabilityTime)
            return;

        currentHealth -= damage;
        lastDamageTime = Time.time;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        // TODO: Trigger game over
    }

    public void AddScore(float points)
    {
        if (!IsAlive)
            return;

        currentScore += points;
    }
}
