using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;

    private bool isClimbing;
    private bool isNearLadder;
    public float climbSpeed = 5f;

    // Added to help the UIManager spawn blocks in front of the player
    public float FacingDirection { get; private set; } = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Block all movement while typing in an input field
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null
            && EventSystem.current.currentSelectedGameObject.GetComponent<TMPro.TMP_InputField>() != null)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }

        // A/D keys and Left/Right arrows
        float moveInput = Input.GetAxisRaw("Horizontal");
        float vertInput = Input.GetAxisRaw("Vertical");

        // Track facing direction and flip sprite
        if (moveInput > 0.01f) { FacingDirection = 1f; transform.localScale = new Vector3(1, 1, 1); }
        else if (moveInput < -0.01f) { FacingDirection = -1f; transform.localScale = new Vector3(-1, 1, 1); }

        // Ground check
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        else
            isGrounded = Mathf.Abs(rb.velocity.y) < 0.01f;

        // Climbing Logic
        if (isNearLadder && Mathf.Abs(vertInput) > 0.1f)
        {
            isClimbing = true;
        }

        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(moveInput * moveSpeed, vertInput * climbSpeed);

            // Let them jump off the ladder
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isClimbing = false;
                rb.gravityScale = 1f;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }
        else
        {
            rb.gravityScale = 1f; // Restore normal gravity
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            // Normal Jump
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        UpdateAnimator(moveInput);
    }

    private void UpdateAnimator(float moveInput)
    {
        if (animator == null) return;

        float speed = Mathf.Abs(moveInput);
        bool isJumping = !isGrounded && rb.velocity.y > 0.1f;
        bool isFalling = !isGrounded && rb.velocity.y < -0.1f;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsClimbing", isClimbing);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder") || IsDynamicLadder(collision))
        {
            isNearLadder = true;
            rb.velocity = new Vector2(rb.velocity.x, 0); // Stop falling momentum instantly when grabbing ladder
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder") || IsDynamicLadder(collision))
        {
            isNearLadder = false;
            isClimbing = false;
            rb.gravityScale = 1f;
        }
    }

    private bool IsDynamicLadder(Collider2D col)
    {
        DynamicObstacle obs = col.GetComponent<DynamicObstacle>();
        return obs != null && obs.isLadderBlock;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}