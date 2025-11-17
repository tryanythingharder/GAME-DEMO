using UnityEngine;

public class PlayerRunState : PlayerState
{
    public PlayerRunState(Player player) : base(player) { }

    public override void Enter()
    {
        anim.SetBool("isRunning", true);
    }

    public override void Exit()
    {
        anim.SetBool("isRunning", false);
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

        // ¡ý ½øÈë»¬²ù
        if (player.moveInput.y < -0.1f)
        {
            player.ChangeState(player.slideState);
            return;
        }


        if (!player.runPressed)
        {
            player.ChangeState(player.walkState);
            return;
        }

        if (Mathf.Abs(player.moveInput.x) < 0.1f)
        {
            player.ChangeState(player.idleState);
            return;
        }

        if (player.moveInput.y < -0.1f)
        {
            player.ChangeState(player.slideState);
        }
    }

    public override void FixedUpdate()
    {
        float x = player.moveInput.x * player.runSpeed;
        rb.linearVelocity = new Vector2(x, rb.linearVelocity.y);
    }
}
