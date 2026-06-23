using System.Collections.Generic;

public class EnemyStats
{
    public float maxHP;
    public float moveSpeed;
    public float damage;
    public float attackRange;
    public float attackCooldown;
    public int xpValue;
    public float detectRange;
    public float knockbackDistance;
    public float knockbackDuration;
}

public class EnemyStatsData
{
    public Dictionary<string, EnemyStats> enemies;
}
