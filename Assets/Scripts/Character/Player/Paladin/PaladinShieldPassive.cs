using UnityEngine;

public class PaladinPassiveSkill : PassiveSkill
{
    private const float SHIELD_RATIO = 0.1f;

    public PaladinPassiveSkill(Player player) : base(player, 60f)
    {
    }

    protected override void Execute()
    {
        float shieldAmount = player.Stats.maxHP * SHIELD_RATIO;
        player.AddShield(shieldAmount);
    }
}
