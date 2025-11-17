using UnityEngine;

public abstract class PlayerState
{
    protected Player player;
    protected Rigidbody2D rb;
    protected Animator anim;

    public PlayerState(Player player)
    {
        this.player = player;
        rb = player.rb;
        anim = player.anim;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }

    public virtual void Update() { }
    public virtual void FixedUpdate() { }
}
