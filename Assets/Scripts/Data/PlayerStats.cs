using System.Collections.Generic;

public class PlayerStats
{
    public float maxHP;
    public float moveSpeed;
    public float armor;
    public float critChance;
    public float critDamage;
    public float pickupRange;
    public float xpBonus;
    public float attackRange;
    public float attackDamage;
    public float attackCooldown;
}

public class PlayerStatsData
{
    public Dictionary<string, PlayerStats> characters;
}