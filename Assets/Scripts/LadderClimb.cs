using UnityEngine;

public class LadderClimb : MonoBehaviour
{
    public float climbSpeed = 4f;

    private bool isOnLadder;
    private Rigidbody2D rb;

    private float verticalInput;
    private float originalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");

        if (isOnLadder)
        {
            rb.gravityScale = 0;

            rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbSpeed);
        }
        else
        {
            rb.gravityScale = originalGravity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = false;
        }
    }
}