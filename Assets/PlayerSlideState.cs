using UnityEngine;

public class PlayerSlideState : PlayerState
{
    float timer;

    public PlayerSlideState(Player player) : base(player) { }

    public override void Enter()
    {
        if (!player.isGrounded)
        {
            player.ChangeState(player.fallState);
            return;
        }

        timer = player.slideDuration;
        player.isSliding = true;

        player.SetColliderSlide();
        anim.SetBool("isSliding", true);
    }

    public override void Exit()
    {
        anim.SetBool("isSliding", false);
        player.isSliding = false;
        player.SetColliderNormal();
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 || !player.isGrounded)
        {
            player.ChangeState(player.idleState);
        }
    }

    public override void FixedUpdate()
    {
        float slideSpeed = player.slideSpeed * player.facingDirection;
        rb.linearVelocity = new Vector2(slideSpeed, rb.linearVelocity.y);
    }
}
