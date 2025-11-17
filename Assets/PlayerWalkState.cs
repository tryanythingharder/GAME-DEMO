using UnityEngine;

public class PlayerWalkState : PlayerState
{
    public PlayerWalkState(Player player) : base(player) { }

    public override void Enter()
    {
        anim.SetBool("isWalking", true);
    }

    public override void Exit()
    {
        anim.SetBool("isWalking", false);
    }

    public override void Update()
    {
        if (player.CanJump())
        {
            player.ChangeState(player.jumpState);
            return;
        }

        if (!player.isGrounded)
        {
            player.ChangeState(player.fallState);
            return;
        }

        // ¡ý ÏÂ¶×
        if (player.moveInput.y < -0.1f)
        {
            player.ChangeState(player.crouchState);
            return;
        }

        if (Mathf.Abs(player.moveInput.x) < 0.1f)
        {
            player.ChangeState(player.idleState);
            return;
        }

        if (player.runPressed)
        {
            player.ChangeState(player.runState);
            return;
        }

        if (player.moveInput.y < -0.1f && player.runPressed)
        {
            player.ChangeState(player.slideState);
        }
    }

    public override void FixedUpdate()
    {
        float x = player.moveInput.x * player.walkSpeed;
        rb.linearVelocity = new Vector2(x, rb.linearVelocity.y);
    }
}
