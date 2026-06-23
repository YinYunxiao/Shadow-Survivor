using UnityEngine;

public class EnemyDeadState : EnemyState
{
    public EnemyDeadState(Enemy e) : base(e) { }

    public override void Enter()
    {
        base.Enter();
        enemy.animator.SetBool("IsDead", true);
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsDead", false);
        base.Exit();
    }
}
