using UnityEngine;
using System.Collections.Generic;

public class WizardPassiveSkill : PassiveSkill
{
    public WizardPassiveSkill(Player player) : base(player, 20f)
    {
    }

    protected override void Execute()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            player.transform.position,
            player.Stats.attackRange,
            LayerMask.GetMask("Enemy")
        );

        if (hits.Length == 0) return;

        List<Transform> targets = new List<Transform>();
        List<Collider2D> candidates = new List<Collider2D>(hits);

        int count = Mathf.Min(3, candidates.Count);
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, candidates.Count);
            targets.Add(candidates[idx].transform);
            candidates.RemoveAt(idx);
        }

        float speedMult = 1f;
        float rangeMult = 1f;
        float aoeMult = 1f;
        var applier = player.GetComponent<SkillEffectApplier>();
        if (applier != null)
        {
            speedMult = applier.GetProjectileSpeedMult();
            rangeMult = applier.GetProjectileRangeMult();
            aoeMult = applier.GetAoeRangeMult();
        }

        foreach (var target in targets)
        {
            GameObject fireBall = PoolManager.Instance.PopGameObject(
                "Prefabs/Projectile/FireBall/FireBall",
                "FireBall",
                10
            );
            fireBall.transform.position = player.transform.position;

            var fb = fireBall.GetComponent<FireBall>();
            if (fb != null)
                fb.Init(player.Stats.attackDamage, target, speedMult, rangeMult, aoeMult);
        }
    }
}
