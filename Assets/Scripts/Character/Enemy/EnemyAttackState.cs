using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy e) : base(e) { }

    public override void Enter()
    {
        base.Enter();
        enemy.lastAttackTime = Time.time;
        enemy.animator.SetBool("IsAttack", true);

        Transform target = enemy.GetAttackTarget();
        if (target != null)
        {
            Vector2 direction = (target.position - enemy.transform.position).normalized;
            enemy.SetDirection(direction);
        }
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsAttack", false);
        base.Exit();
    }
}
