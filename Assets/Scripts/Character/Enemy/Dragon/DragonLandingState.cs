using UnityEngine;

public class DragonLandingState : EnemyState
{
    private float stateTimer;
    private const float DURATION = 1.5f;
    private const float AOE_RADIUS = 1f;
    private bool hasDealtDamage;

    public DragonLandingState(Enemy e) : base(e) { }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 0f;
        hasDealtDamage = false;
        enemy.animator.SetBool("IsLanding", true);
        enemy.transform.position = new Vector3(
            enemy.transform.position.x,
            enemy.transform.position.y,
            0f
        );

        var dragon = enemy as Dragon;
        if (dragon != null) dragon.SetInvincible(false);
    }

    public override void Update()
    {
        base.Update();
        stateTimer += Time.deltaTime;

        if (!hasDealtDamage && stateTimer >= 0.3f)
        {
            DealLandingDamage();
            hasDealtDamage = true;
        }

        if (stateTimer >= DURATION)
        {
            var dragon = enemy as Dragon;
            if (dragon != null)
            {
                dragon.ResetLiftOffTimer();
                dragon.stateMachine.ChangeState(dragon.ChaseState);
            }
        }
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsLanding", false);
        base.Exit();
    }

    private void DealLandingDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            enemy.transform.position,
            AOE_RADIUS,
            LayerMask.GetMask("Player")
        );

        foreach (var hit in hits)
        {
            var character = hit.GetComponent<Character>();
            if (character != null)
                character.TakeDamage(enemy.Stats.damage);
        }
    }
}
