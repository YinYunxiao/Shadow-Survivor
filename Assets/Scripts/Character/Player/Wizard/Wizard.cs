using UnityEngine;

public class Wizard : Player
{
    public WizardAttackState AttackState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AttackState = new WizardAttackState(this);
    }

    protected override void InitPassiveSkills()
    {
        AddPassiveSkill(new WizardPassiveSkill(this));
    }

    public override void AutoAttack()
    {
        RecordAttack();
        stateMachine.ChangeState(AttackState);
    }

    public override void OnAoeHit()
    {
        ShootFireBall();
    }

    public void ShootFireBall()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            Stats.attackRange,
            LayerMask.GetMask("Enemy")
        );

        if (hits.Length == 0) return;

        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }

        if (nearest != null)
            SpawnFireBall(nearest);
    }

    private void SpawnFireBall(Transform target)
    {
        GameObject fireBall = PoolManager.Instance.PopGameObject(
            "Prefabs/Projectile/FireBall/FireBall",
            "FireBall",
            10
        );
        fireBall.transform.position = transform.position;

        var fb = fireBall.GetComponent<FireBall>();
        if (fb != null)
        {
            float speedMult = 1f;
            float rangeMult = 1f;
            float aoeMult = 1f;

            var applier = GetComponent<SkillEffectApplier>();
            if (applier != null)
            {
                speedMult = applier.GetProjectileSpeedMult();
                rangeMult = applier.GetProjectileRangeMult();
                aoeMult = applier.GetAoeRangeMult();
            }

            fb.Init(Stats.attackDamage, target, speedMult, rangeMult, aoeMult);
        }
    }
}
