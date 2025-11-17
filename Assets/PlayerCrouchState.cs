using UnityEngine;

public class PlayerCrouchState : PlayerState
{
    public PlayerCrouchState(Player player) : base(player) { }

    public override void Enter()
    {
        player.isCrouching = true;
        anim.SetBool("isCrouching", true);

        player.SetColliderCrouch();
    }

    public override void Exit()
    {
        player.isCrouching = false;
        anim.SetBool("isCrouching", false);

        player.SetColliderNormal();
    }

    public override void Update()
    {
        // 松开下 → 尝试起身
        if (player.moveInput.y >= 0)
        {
            bool headBlocked = Physics2D.OverlapCircle(
                player.headCheck.position,
                player.headCheckRadius,
                player.groundLayer
            );

            if (!headBlocked)
            {
                player.ChangeState(player.idleState);
                return;
            }
        }

        // 跳跃
        if (player.CanJump())
        {
            player.ChangeState(player.jumpState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        // 下蹲不能水平移动
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }
}
