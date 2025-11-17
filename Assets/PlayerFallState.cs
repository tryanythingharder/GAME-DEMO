using UnityEngine;

public class PlayerFallState : PlayerState
{
    public PlayerFallState(Player player) : base(player) { }

    public override void Update()
    {
        if (player.CanJump()) // ¶þ¶ÎÌøÖ§³Ö
        {
            player.ChangeState(player.jumpState);
            return;
        }

        if (player.isGrounded)
        {
            player.ChangeState(player.idleState);
        }
    }

    public override void FixedUpdate()
    {
        float speed = player.runPressed ? player.runSpeed : player.walkSpeed;
        rb.linearVelocity = new Vector2(player.moveInput.x * speed, rb.linearVelocity.y);
    }
}
