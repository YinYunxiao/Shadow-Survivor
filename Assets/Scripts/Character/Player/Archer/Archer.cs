using UnityEngine;

public class Archer : Player
{
    public ArcherAttackState AttackState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AttackState = new ArcherAttackState(this);
    }

    protected override void InitPassiveSkills()
    {
        AddPassiveSkill(new ArcherPassiveSkill(this));
    }

    public override void AutoAttack()
    {
        RecordAttack();
        stateMachine.ChangeState(AttackState);
    }

    public void ShootArrows()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            Stats.attackRange,
            LayerMask.GetMask("Enemy")
        );

        System.Array.Sort(hits, (a, b) =>
        {
            float distA = Vector2.Distance(transform.position, a.transform.position);
            float distB = Vector2.Distance(transform.position, b.transform.position);
            return distA.CompareTo(distB);
        });

        int extraProjectiles = 0;
        var applier = GetComponent<SkillEffectApplier>();
        if (applier != null)
            extraProjectiles = applier.GetExtraProjectiles();

        int count = Mathf.Min(3 + extraProjectiles, hits.Length);
        for (int i = 0; i < count; i++)
        {
            SpawnArrow(hits[i].transform);
        }
    }

    private void SpawnArrow(Transform target)
    {
        GameObject arrow = PoolManager.Instance.PopGameObject(
            "Prefabs/Projectile/Arrow/Arrow",
            "Arrow",
            20
        );
        arrow.transform.position = transform.position;

        var arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            float speedMult = 1f;
            float rangeMult = 1f;
            int penetrate = 0;

            var applier = GetComponent<SkillEffectApplier>();
            if (applier != null)
            {
                speedMult = applier.GetProjectileSpeedMult();
                rangeMult = applier.GetProjectileRangeMult();
                penetrate = applier.GetPenetrationCount();
            }

            arrowScript.Init(Stats.attackDamage, target, speedMult, rangeMult, penetrate);
        }
    }
}
