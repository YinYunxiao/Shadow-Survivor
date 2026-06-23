using UnityEngine;
using System.Collections.Generic;

public class DeadKnightConvertPassive : PassiveSkill
{
    private const float CONVERT_RADIUS = 3f;

    public DeadKnightConvertPassive(Player player) : base(player, 60f)
    {
    }

    protected override void Execute()
    {
        List<Enemy> toConvert = new List<Enemy>();

        foreach (var enemy in Enemy.ActiveEnemies)
        {
            if (enemy == null || enemy.isDead || !enemy.CanBeConverted) continue;
            float dist = Vector2.Distance(player.transform.position, enemy.transform.position);
            if (dist <= CONVERT_RADIUS)
            {
                toConvert.Add(enemy);
            }
        }

        foreach (var enemy in toConvert)
        {
            enemy.Convert();
        }
    }
}
