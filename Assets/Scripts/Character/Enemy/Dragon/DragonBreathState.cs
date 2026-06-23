using UnityEngine;

public class DragonBreathState : EnemyState
{
    private float stateTimer;
    private const float DURATION = 1.5f;

    public DragonBreathState(Enemy e) : base(e) { }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 0f;
        enemy.animator.SetBool("IsAttack", true);

        Transform player = GameManager.Instance.PlayerTransform;
        if (player != null)
        {
            Vector2 dir = (player.position - enemy.transform.position).normalized;
            enemy.SetDirection(dir);
            ShootBreath(dir);
        }
    }

    public override void Update()
    {
        base.Update();
        stateTimer += Time.deltaTime;
        if (stateTimer >= DURATION)
        {
            var dragon = enemy as Dragon;
            if (dragon != null)
                dragon.stateMachine.ChangeState(dragon.ChaseState);
        }
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsAttack", false);
        base.Exit();
    }

    private void ShootBreath(Vector2 dir)
    {
        var dragon = enemy as Dragon;
        if (dragon == null) return;

        float spreadAngle = 20f;
        for (int i = -1; i <= 1; i++)
        {
            float angle = i * spreadAngle * Mathf.Deg2Rad;
            Vector2 rotatedDir = new Vector2(
                dir.x * Mathf.Cos(angle) - dir.y * Mathf.Sin(angle),
                dir.x * Mathf.Sin(angle) + dir.y * Mathf.Cos(angle)
            );

            dragon.SpawnBreath(rotatedDir);
        }
    }
}
