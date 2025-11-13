using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator anim;

    [Header("Movement Variables")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float jumpForce = 12f;
    public float jumpCutMultiplier = 0.5f;
    public float normalGravity = 3f;
    public float fallGravity = 5f;
    public float jumpGravity = 2f;

    [Header("Jump Settings")]
    public int maxJumps = 2;
    private int jumpCount;
    private bool jumpPressed;
    private bool jumpReleased;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Vector2 moveInput;
    private bool runPressed;
    private int facingDirection = 1;

    private void Start()
    {
        rb.gravityScale = normalGravity;
        jumpCount = maxJumps;
    }

    void Update()
    {
        Flip();
        HandleAnimations();
    }

    void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
        HandleJump();
        ApplyVariableGravity();
    }

    private void HandleMovement()
    {
        float speed = runPressed ? runSpeed : walkSpeed;
        float targetSpeed = moveInput.x * speed;

        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (isGrounded && rb.linearVelocity.y <= 0.05f)
        {
            jumpCount = maxJumps;
        }

        if (jumpPressed && jumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
            jumpReleased = false;

            jumpCount--;
        }

        if (jumpReleased && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            jumpReleased = false;
        }
    }

    void ApplyVariableGravity()
    {
        if (rb.linearVelocity.y < -0.01f)
            rb.gravityScale = fallGravity;
        else if (rb.linearVelocity.y > 0.01f)
            rb.gravityScale = jumpGravity;
        else
            rb.gravityScale = normalGravity;
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    void HandleAnimations()
    {
        bool isMoving = Mathf.Abs(moveInput.x) > 0.1f;

        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isJumping", rb.linearVelocity.y > 0.1f);

        if (!isGrounded)
        {
            anim.SetBool("isIdle", false);
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
            return;
        }

        if (!isMoving)
        {
            anim.SetBool("isIdle", true);
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
        }
        else if (isMoving && !runPressed)
        {
            anim.SetBool("isIdle", false);
            anim.SetBool("isWalking", true);
            anim.SetBool("isRunning", false);
        }
        else if (isMoving && runPressed)
        {
            anim.SetBool("isIdle", false);
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", true);
        }
    }

    void Flip()
    {
        if (moveInput.x > 0.1f)
            facingDirection = 1;
        else if (moveInput.x < -0.1f)
            facingDirection = -1;

        transform.localScale = new Vector3(facingDirection, 1, 1);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnRun(InputValue value)
    {
        runPressed = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true;
            jumpReleased = false;
        }
        else
        {
            jumpReleased = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
