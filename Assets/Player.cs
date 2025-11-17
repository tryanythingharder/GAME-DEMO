using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator anim;
    public PlayerInput playerInput;
    public CapsuleCollider2D playerCollider;

    [Header("Movement Variables")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 20f;
    public float jumpCutMultiplier = 0.5f;
    public float normalGravity = 6f;
    public float fallGravity = 9f;
    public float jumpGravity = 4f;

    [Header("Jump Settings")]
    public int maxJumps = 2;
    [HideInInspector] public int jumpCount;

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("Coyote Time")]
    public float coyoteTime = 0.12f;
    private float coyoteCounter;

    [HideInInspector] public bool jumpReleased;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.5f;
    public LayerMask groundLayer;
    [HideInInspector] public bool isGrounded;

    [Header("Crouch Settings (Auto)")]
    [HideInInspector] public float normalHeight;
    [HideInInspector] public Vector2 normalOffset;

    [HideInInspector] public float crouchHeight;
    [HideInInspector] public Vector2 crouchOffset;

    public Transform headCheck;
    public float headCheckRadius = 0.12f;
    [HideInInspector] public bool isCrouching;

    [Header("Slide Settings (Auto)")]
    public float slideDuration = 0.6f;
    public float slideSpeed = 12f;
    public float slideStopDuration = 0.15f;

    [HideInInspector] public float slideHeight;
    [HideInInspector] public Vector2 slideOffset;

    [HideInInspector] public bool isSliding;

    [Header("Input Values")]
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool runPressed;
    [HideInInspector] public int facingDirection = 1;

    // FSM
    public PlayerState currentState;
    public PlayerIdleState idleState;
    public PlayerWalkState walkState;
    public PlayerRunState runState;
    public PlayerJumpState jumpState;
    public PlayerFallState fallState;
    public PlayerSlideState slideState;
    public PlayerCrouchState crouchState;

    private void Start()
    {
        rb.gravityScale = normalGravity;
        jumpCount = maxJumps;

        // --------------------- 自动记录原始碰撞体 ---------------------
        normalHeight = playerCollider.size.y;
        normalOffset = playerCollider.offset;

        // 自动生成下蹲尺寸 & 偏移
        crouchHeight = normalHeight * 0.5f;
        crouchOffset = new Vector2(
            normalOffset.x,
            normalOffset.y - (normalHeight - crouchHeight) / 2f
        );

        // 自动生成滑铲尺寸 & 偏移
        slideHeight = normalHeight * 0.4f;
        slideOffset = new Vector2(
            normalOffset.x,
            normalOffset.y - (normalHeight - slideHeight) / 2f
        );
        // -------------------------------------------------------------

        idleState = new PlayerIdleState(this);
        walkState = new PlayerWalkState(this);
        runState = new PlayerRunState(this);
        jumpState = new PlayerJumpState(this);
        fallState = new PlayerFallState(this);
        slideState = new PlayerSlideState(this);
        crouchState = new PlayerCrouchState(this);

        ChangeState(idleState);
    }

    private void Update()
    {
        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;

        if (isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        CheckGroundStatus();
        currentState.Update();

        HandleAnimations();
        Flip();
        ApplyVariableGravity();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
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
            jumpBufferCounter = jumpBufferTime;
            jumpReleased = false;
        }
        else
            jumpReleased = true;
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded)
            jumpCount = maxJumps;
    }

    public bool CanJump()
    {
        if (jumpBufferCounter > 0 && (coyoteCounter > 0 || jumpCount > 0))
        {
            jumpBufferCounter = 0;
            return true;
        }
        return false;
    }

    private void ApplyVariableGravity()
    {
        if (rb.linearVelocity.y < -0.1f)
            rb.gravityScale = fallGravity;
        else if (rb.linearVelocity.y > 0.1f)
            rb.gravityScale = jumpGravity;
        else
            rb.gravityScale = normalGravity;
    }

    // ---------------- Collider 切换 ----------------
    public void SetColliderCrouch()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, crouchHeight);
        playerCollider.offset = crouchOffset;
    }

    public void SetColliderSlide()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, slideHeight);
        playerCollider.offset = slideOffset;
    }

    public void SetColliderNormal()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, normalHeight);
        playerCollider.offset = normalOffset;
    }
    // -------------------------------------------------

    private void Flip()
    {
        if (moveInput.x > 0.1f)
            facingDirection = 1;
        else if (moveInput.x < -0.1f)
            facingDirection = -1;

        transform.localScale = new Vector3(facingDirection, 1, 1);
    }

    private void HandleAnimations()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("isCrouching", isCrouching);

        anim.SetBool("isIdle", currentState is PlayerIdleState);
        anim.SetBool("isWalking", currentState is PlayerWalkState);
        anim.SetBool("isRunning", currentState is PlayerRunState);
        anim.SetBool("isJumping", currentState is PlayerJumpState);
        anim.SetBool("isFalling", currentState is PlayerFallState);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.blue;
        if (headCheck != null)
            Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);
    }
}
