using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Player p) : base(p) { }

    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("IsDead", true);
    }

    public override void Exit()
    {
        player.animator.SetBool("IsDead", false);
        base.Exit();
    }
}
