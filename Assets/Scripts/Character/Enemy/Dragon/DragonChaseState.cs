using UnityEngine;

public class DragonChaseState : EnemyState
{
    private float attackCooldown;
    private const float ATTACK_INTERVAL = 5f;

    public DragonChaseState(Enemy e) : base(e) { }

    public override void Enter()
    {
        base.Enter();
        attackCooldown = ATTACK_INTERVAL;
        enemy.animator.SetBool("IsMove", true);
    }

    public override void Update()
    {
        base.Update();

        attackCooldown -= Time.deltaTime;

        Transform player = GameManager.Instance.PlayerTransform;
        if (player == null) return;

        float dist = Vector2.Distance(enemy.transform.position, player.position);

        if (dist <= enemy.Stats.attackRange && attackCooldown <= 0f)
        {
            attackCooldown = ATTACK_INTERVAL;
            ChooseAttack();
            return;
        }

        Vector2 dir = (player.position - enemy.transform.position).normalized;
        enemy.SetDirection(dir);
        enemy.transform.position += (Vector3)dir * (enemy.moveSpeed * Time.deltaTime);
    }

    private void ChooseAttack()
    {
        var dragon = enemy as Dragon;
        if (dragon == null) return;

        if (dragon.CanLiftOff())
            dragon.stateMachine.ChangeState(dragon.LiftOffState);
        else
            dragon.stateMachine.ChangeState(dragon.BreathState);
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsMove", false);
        base.Exit();
    }
}
