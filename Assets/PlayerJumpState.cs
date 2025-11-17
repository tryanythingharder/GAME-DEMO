using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player) : base(player) { }

    public override void Enter()
    {
        float vx = rb.linearVelocity.x;
        rb.linearVelocity = new Vector2(vx, player.jumpForce);

        player.jumpCount--;
        player.jumpReleased = false;
    }

    public override void Update()
    {
        if (rb.linearVelocity.y < 0)
        {
            player.ChangeState(player.fallState);
        }
    }

    public override void FixedUpdate()
    {
        float speed = player.runPressed ? player.runSpeed : player.walkSpeed;

        rb.linearVelocity = new Vector2(player.moveInput.x * speed, rb.linearVelocity.y);

        if (player.jumpReleased && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * player.jumpCutMultiplier
            );
        }
    }
}
