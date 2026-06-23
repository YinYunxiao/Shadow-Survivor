using UnityEngine;

public class Knight : Player
{
    public KnightMeleeState MeleeState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        MeleeState = new KnightMeleeState(this);
    }

    public override void AutoAttack()
    {
        RecordAttack();
        stateMachine.ChangeState(MeleeState);
    }

    public override void OnMeleeHit()
    {
        base.OnMeleeHit();
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            Stats.attackRange,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                DealDamageToEnemy(enemy, Stats.attackDamage);
                Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                enemy.Knockback(knockbackDir);
            }
        }
    }
}
