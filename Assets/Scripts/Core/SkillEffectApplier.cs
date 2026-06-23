using UnityEngine;

public class SkillEffectApplier : MonoBehaviour
{
    private Player player;

    private float hpRegenTimer;
    private float shieldTimer;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        GameEvents.OnSkillSelected += OnSkillSelected;
    }

    private void OnDisable()
    {
        GameEvents.OnSkillSelected -= OnSkillSelected;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;
        if (player == null) return;

        ApplyHpRegen();
        ApplyShield();
    }

    private void OnSkillSelected(string skillId, int level)
    {
        ApplyStatBonuses();
    }

    private void ApplyStatBonuses()
    {
        var mgr = SkillUpgradeManager.Instance;
        var stats = player.Stats;

        float atkMult = 1f + mgr.GetTotalValue("atk_boost") + mgr.GetTotalValue("all_stats_boost");
        float spdMult = 1f + mgr.GetTotalValue("atk_speed_boost") + mgr.GetTotalValue("all_stats_boost");
        float hpMult = 1f + mgr.GetTotalValue("hp_boost") + mgr.GetTotalValue("all_stats_boost");
        float moveMult = 1f + mgr.GetTotalValue("move_speed_boost") + mgr.GetTotalValue("speed_enhance") + mgr.GetTotalValue("all_stats_boost");
        float rangeMult = 1f + mgr.GetTotalValue("atk_range_boost");

        stats.attackDamage = GetBaseStat("attackDamage") * atkMult;
        stats.attackCooldown = GetBaseStat("attackCooldown") / spdMult;
        stats.maxHP = GetBaseStat("maxHP") * hpMult;
        stats.moveSpeed = GetBaseStat("moveSpeed") * moveMult;
        stats.attackRange = GetBaseStat("attackRange") * rangeMult;
        stats.critChance = GetBaseStat("critChance") + mgr.GetTotalValue("crit_rate_boost");
        stats.critDamage = GetBaseStat("critDamage") + mgr.GetTotalValue("crit_dmg_boost");
        stats.armor = GetBaseStat("armor") + mgr.GetTotalValue("armor_boost") + mgr.GetTotalValue("armor_boost_survival");

        player.currentHP = Mathf.Min(player.currentHP, stats.maxHP);
    }

    private float GetBaseStat(string statName)
    {
        var json = JsonManager.Load<PlayerStatsData>(
            "Data/PlayerStats",
            JsonManager.JsonType.LitJson,
            JsonManager.JsonLocation.Resources
        );

        string charName = PlayerProgressManager.Instance.Progress.lastCharacter;
        if (json != null && json.characters.ContainsKey(charName))
        {
            var stats = json.characters[charName];
            switch (statName)
            {
                case "attackDamage": return stats.attackDamage;
                case "attackCooldown": return stats.attackCooldown;
                case "maxHP": return stats.maxHP;
                case "moveSpeed": return stats.moveSpeed;
                case "attackRange": return stats.attackRange;
                case "critChance": return stats.critChance;
                case "critDamage": return stats.critDamage;
                case "armor": return stats.armor;
            }
        }
        return 0f;
    }

    private void ApplyHpRegen()
    {
        float regenRate = SkillUpgradeManager.Instance.GetTotalValue("hp_regen_boost")
                        + SkillUpgradeManager.Instance.GetTotalValue("hp_regen");

        if (regenRate <= 0) return;

        hpRegenTimer += Time.deltaTime;
        if (hpRegenTimer >= 1f)
        {
            hpRegenTimer -= 1f;
            float healAmount = player.Stats.maxHP * regenRate;
            player.currentHP = Mathf.Min(player.currentHP + healAmount, player.Stats.maxHP);
            GameEvents.OnHPChanged?.Invoke(player.currentHP, player.Stats.maxHP);
        }
    }

    private void ApplyShield()
    {
        if (!SkillUpgradeManager.Instance.HasSkill("shield_periodic")) return;

        shieldTimer += Time.deltaTime;
        if (shieldTimer >= 60f)
        {
            shieldTimer -= 60f;
            float shieldAmount = player.Stats.maxHP * SkillUpgradeManager.Instance.GetTotalValue("shield_periodic");
            player.AddShield(shieldAmount);
        }
    }

    public float GetDamageMultiplier()
    {
        return 1f + SkillUpgradeManager.Instance.GetTotalValue("skill_dmg_boost");
    }

    public int GetExtraProjectiles()
    {
        return (int)(SkillUpgradeManager.Instance.GetTotalValue("proj_count_boost")
                    + SkillUpgradeManager.Instance.GetTotalValue("multi_shot"));
    }

    public float GetProjectileSpeedMult()
    {
        return 1f + SkillUpgradeManager.Instance.GetTotalValue("proj_speed_boost");
    }

    public float GetProjectileRangeMult()
    {
        return 1f + SkillUpgradeManager.Instance.GetTotalValue("proj_range_boost");
    }

    public int GetPenetrationCount()
    {
        return (int)(SkillUpgradeManager.Instance.GetTotalValue("proj_penetrate_boost")
                    + SkillUpgradeManager.Instance.GetTotalValue("penetrate_boost"));
    }

    public float GetAoeRangeMult()
    {
        return 1f + SkillUpgradeManager.Instance.GetTotalValue("aoe_range_boost")
                  + SkillUpgradeManager.Instance.GetTotalValue("fireball_aoe_boost");
    }

    public float GetLifestealRate()
    {
        return SkillUpgradeManager.Instance.GetTotalValue("lifesteal_effect");
    }

    public float GetSplashDamageRate()
    {
        return SkillUpgradeManager.Instance.GetTotalValue("splash_effect");
    }

    public float GetCritChance()
    {
        return player.Stats.critChance;
    }

    public float GetCritDamage()
    {
        return player.Stats.critDamage;
    }

    public float GetDodgeChance()
    {
        return SkillUpgradeManager.Instance.GetTotalValue("dodge_effect");
    }

    public float GetBlockChance()
    {
        return SkillUpgradeManager.Instance.GetTotalValue("block_effect");
    }

    public float GetDamageReduction()
    {
        return SkillUpgradeManager.Instance.GetTotalValue("dmg_reduction");
    }

    public float GetReflectRate()
    {
        return SkillUpgradeManager.Instance.GetTotalValue("reflect_effect");
    }

    public float GetInvincibleChance()
    {
        return SkillUpgradeManager.Instance.GetTotalValue("invincible_effect");
    }

    public float GetBurnChance()
    {
        return 0.1f;
    }

    public float GetBurnDamage()
    {
        return SkillUpgradeManager.Instance.GetTotalValue("burn_effect");
    }

    public float GetFreezeChance()
    {
        return 0.1f;
    }

    public float GetFreezeDuration()
    {
        return SkillUpgradeManager.Instance.GetTotalValue("freeze_effect");
    }

    public float GetStunChance()
    {
        return 0.1f;
    }

    public float GetStunDuration()
    {
        return SkillUpgradeManager.Instance.GetTotalValue("stun_effect");
    }

    public float GetSlowChance()
    {
        return 0.15f;
    }

    public float GetSlowAmount()
    {
        return SkillUpgradeManager.Instance.GetTotalValue("slow_effect");
    }

    public float GetShieldValueMult()
    {
        return 1f + SkillUpgradeManager.Instance.GetTotalValue("shield_value_boost");
    }

    public float GetPassiveCooldownMult()
    {
        return 1f - SkillUpgradeManager.Instance.GetTotalValue("passive_cd_boost")
                   - SkillUpgradeManager.Instance.GetTotalValue("skill_cd_boost");
    }

    public float GetXpBonusMult()
    {
        return 1f + SkillUpgradeManager.Instance.GetTotalValue("xp_boost");
    }
}
