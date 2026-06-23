using UnityEngine;

public class ArcherPassiveSkill : PassiveSkill
{
    public ArcherPassiveSkill(Player player) : base(player, 20f)
    {
    }

    protected override void Execute()
    {
        int level = player.level;
        int arrowCount = 4 + level;

        float angleStep = 360f / arrowCount;

        float speedMult = 1f;
        float rangeMult = 1f;
        var applier = player.GetComponent<SkillEffectApplier>();
        if (applier != null)
        {
            speedMult = applier.GetProjectileSpeedMult();
            rangeMult = applier.GetProjectileRangeMult();
        }

        for (int i = 0; i < arrowCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            GameObject arrow = PoolManager.Instance.PopGameObject(
                "Prefabs/Projectile/Arrow/Arrow",
                "Arrow",
                30
            );
            arrow.transform.position = player.transform.position;

            var arrowScript = arrow.GetComponent<Arrow>();
            if (arrowScript != null)
                arrowScript.InitPenetrating(player.Stats.attackDamage, dir, speedMult, rangeMult);
        }
    }
}
