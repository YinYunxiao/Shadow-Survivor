using UnityEngine;

public class EnemyMoveState : EnemyState
{
    public EnemyMoveState(Enemy e) : base(e) { }

    public override void Enter()
    {
        base.Enter();
        enemy.animator.SetBool("IsMove", true);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (enemy.IsStunned) return;

        Transform target = enemy.GetAttackTarget();
        if (target == null) return;

        Vector2 direction = (target.position - enemy.transform.position).normalized;
        float dist = Vector2.Distance(enemy.transform.position, target.position);

        if (dist <= enemy.Stats.attackRange && enemy.CanAttack())
        {
            enemy.stateMachine.ChangeState(enemy.AttackState);
            return;
        }

        enemy.SetDirection(direction);
        enemy.rb.MovePosition(
            enemy.rb.position + direction * (enemy.moveSpeed * Time.fixedDeltaTime)
        );
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsMove", false);
        base.Exit();
    }
}
