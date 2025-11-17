using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player) { }

    public override void Enter()
    {
        anim.SetBool("isIdle", true);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        anim.SetBool("isIdle", false);
    }

    public override void Update()
    {
        // ¡ý ÏÂ¶×
        if (player.moveInput.y < -0.1f)
        {
            player.ChangeState(player.crouchState);
            return;
        }

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

        if (Mathf.Abs(player.moveInput.x) > 0.1f)
        {
            if (player.runPressed)
                player.ChangeState(player.runState);
            else
                player.ChangeState(player.walkState);
        }

        if (player.moveInput.y < -0.1f && player.runPressed)
        {
            player.ChangeState(player.slideState);
        }
    }
}
