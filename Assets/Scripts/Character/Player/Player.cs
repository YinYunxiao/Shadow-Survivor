using UnityEngine;
using System.Collections.Generic;

public class Player : Character
{
    public int level = 1;
    public float currentXP = 0;
    public float currentShield = 0;

    public GameObject playerRoot;
    public PlayerStats Stats { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }
    private float lastAttackTime;
    private List<PassiveSkill> passiveSkills = new List<PassiveSkill>();
    public bool HasPassiveSkill => passiveSkills.Count > 0;
    private SkillEffectApplier skillApplier;

    protected override void Awake()
    {
        base.Awake();
        playerRoot = this.gameObject.transform.parent.gameObject;
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        DeadState = new PlayerDeadState(this);
    }

    protected override void Start()
    {
        base.Start();
        skillApplier = GetComponent<SkillEffectApplier>();
        if (skillApplier == null)
            skillApplier = gameObject.AddComponent<SkillEffectApplier>();
    }

    protected virtual void InitPassiveSkills()
    {
    }

    public void AddPassiveSkill(PassiveSkill skill)
    {
        passiveSkills.Add(skill);
    }

    protected override void Update()
    {
        base.Update();

        if (GameManager.Instance.CurrentState != GameState.Playing)
            return;

        foreach (var skill in passiveSkills)
        {
            skill.Update(Time.deltaTime);
        }
    }

    public void Init(string characterName)
    {
        // 从Resources中加载角色对应信息
        var data = JsonManager.Load<PlayerStatsData>(
            "Data/PlayerStats",
            JsonManager.JsonType.LitJson,
            JsonManager.JsonLocation.Resources
        );
        Stats = data.characters[characterName];

        currentHP = Stats.maxHP;
        baseMoveSpeed = Stats.moveSpeed;
        baseArmor = Stats.armor;
        level = 1;
        currentXP = 0;
        currentShield = 0;
        lastAttackTime = -Stats.attackCooldown;

        buffSystem.Clear();
        passiveSkills.Clear();

        GameManager.Instance.PlayerTransform = transform;
        stateMachine.ChangeState(IdleState);

        InitPassiveSkills();

        GameEvents.OnHPChanged?.Invoke(currentHP, Stats.maxHP);
        GameEvents.OnXPChanged?.Invoke(currentXP, GetMaxXP());
    }

    public override void TakeDamage(float damage)
    {
        if (skillApplier != null)
        {
            float dodgeChance = skillApplier.GetDodgeChance();
            if (dodgeChance > 0 && Random.value < dodgeChance)
                return;

            float blockChance = skillApplier.GetBlockChance();
            if (blockChance > 0 && Random.value < blockChance)
                damage *= 0.5f;

            float invincibleChance = skillApplier.GetInvincibleChance();
            if (invincibleChance > 0 && Random.value < invincibleChance)
            {
                buffSystem.AddBuff(new Buff { id = "invincible", duration = 2f, armorAdd = 9999f });
            }

            float reduction = skillApplier.GetDamageReduction();
            if (reduction > 0)
                damage *= (1f - reduction);
        }

        float remainingDamage = damage;

        if (currentShield > 0)
        {
            if (currentShield >= remainingDamage)
            {
                currentShield -= remainingDamage;
                remainingDamage = 0;
            }
            else
            {
                remainingDamage -= currentShield;
                currentShield = 0;
            }
            GameEvents.OnShieldChanged?.Invoke(currentShield, Stats.maxHP);
        }

        if (remainingDamage > 0)
        {
            base.TakeDamage(remainingDamage);

            if (skillApplier != null)
            {
                float reflectRate = skillApplier.GetReflectRate();
                if (reflectRate > 0)
                {
                    // 反伤需要攻击者信息，当前架构无法获取，暂时跳过
                }
            }
        }

        GameEvents.OnHPChanged?.Invoke(currentHP, Stats.maxHP);
    }

    public void DealDamageToEnemy(Enemy enemy, float baseDamage)
    {
        if (enemy == null || enemy.isDead) return;

        float finalDamage = baseDamage;

        if (skillApplier != null)
        {
            finalDamage *= skillApplier.GetDamageMultiplier();

            float critChance = skillApplier.GetCritChance();
            float critDamage = skillApplier.GetCritDamage();
            if (critChance > 0 && Random.value < critChance)
            {
                finalDamage *= (1f + critDamage);
            }
        }

        enemy.TakeDamage(finalDamage);

        if (skillApplier != null)
        {
            float lifesteal = skillApplier.GetLifestealRate();
            if (lifesteal > 0)
            {
                float heal = finalDamage * lifesteal;
                currentHP = Mathf.Min(currentHP + heal, Stats.maxHP);
                GameEvents.OnHPChanged?.Invoke(currentHP, Stats.maxHP);
            }

            float splashRate = skillApplier.GetSplashDamageRate();
            if (splashRate > 0)
            {
                float splashDamage = finalDamage * splashRate;
                Collider2D[] splashHits = Physics2D.OverlapCircleAll(
                    enemy.transform.position, 1.5f, LayerMask.GetMask("Enemy"));
                foreach (var hit in splashHits)
                {
                    if (hit.gameObject == enemy.gameObject) continue;
                    var splashEnemy = hit.GetComponent<Enemy>();
                    if (splashEnemy != null && !splashEnemy.isDead)
                        splashEnemy.TakeDamage(splashDamage);
                }
            }

            float burnChance = skillApplier.GetBurnChance();
            if (burnChance > 0 && Random.value < burnChance)
            {
                // 燃烧效果 TODO: 需要DOT系统
            }

            float freezeChance = skillApplier.GetFreezeChance();
            if (freezeChance > 0 && Random.value < freezeChance)
            {
                float freezeDuration = skillApplier.GetFreezeDuration();
                enemy.buffSystem.AddBuff(new Buff
                {
                    id = "freeze",
                    duration = freezeDuration,
                    moveSpeedMult = 0f
                });
                enemy.PlayFreezeEffect(freezeDuration);
            }

            float stunChance = skillApplier.GetStunChance();
            if (stunChance > 0 && Random.value < stunChance)
            {
                float stunDuration = skillApplier.GetStunDuration();
                enemy.buffSystem.AddBuff(new Buff
                {
                    id = "stun",
                    duration = stunDuration,
                    moveSpeedMult = 0f
                });
                enemy.PlayStunEffect(stunDuration);
            }

            float slowChance = skillApplier.GetSlowChance();
            if (slowChance > 0 && Random.value < slowChance)
            {
                float slowAmount = skillApplier.GetSlowAmount();
                enemy.buffSystem.AddBuff(new Buff
                {
                    id = "slow",
                    duration = 2f,
                    moveSpeedMult = 1f - Mathf.Clamp01(slowAmount)
                });
            }
        }
    }

    public void AddXP(float amount)
    {
        currentXP += amount;
        float maxXP = GetMaxXP();

        while (currentXP >= maxXP)
        {
            currentXP -= maxXP;
            level++;

            float hpBonus = GetLevelHpBonus(level);
            float hpGain = Stats.maxHP * hpBonus;
            Stats.maxHP += hpGain;
            currentHP += hpGain;

            GameEvents.OnLevelUp?.Invoke(level);
            GameEvents.OnHPChanged?.Invoke(currentHP, Stats.maxHP);

            GameManager.Instance.PauseGame();
            UIManager.Instance.ShowPanel<All.SkillSelectPanel, SkillSelectPanelController>("All")
                .ctrl.ShowSelection(SkillUpgradeManager.Instance.GetSelection(3));
        }

        GameEvents.OnXPChanged?.Invoke(currentXP, maxXP);
    }

    public float GetMaxXP()
    {
        var config = GetLevelConfig();
        if (config == null || config.levels.Count == 0) return 999999;

        int index = Mathf.Clamp(level - 1, 0, config.levels.Count - 1);
        return config.levels[index].xpRequired;
    }

    private float GetLevelHpBonus(int targetLevel)
    {
        var config = GetLevelConfig();
        if (config == null || config.levels.Count == 0) return 0f;

        int index = Mathf.Clamp(targetLevel - 1, 0, config.levels.Count - 1);
        return config.levels[index].hpBonusPercent;
    }

    private LevelConfigData GetLevelConfig()
    {
        return JsonManager.Load<LevelConfigData>(
            "Data/LevelConfig",
            JsonManager.JsonType.LitJson,
            JsonManager.JsonLocation.Resources
        );
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + Stats.attackCooldown;
    }

    public void RecordAttack()
    {
        lastAttackTime = Time.time;
    }

    public virtual void AutoAttack() { }

    public override void AttackOver()
    {
        base.AttackOver();

        Vector2 moveDir = InputManager.MoveDir;

        if (moveDir.magnitude > 0.1f)
            stateMachine.ChangeState(MoveState);
        else
            stateMachine.ChangeState(IdleState);
    }

    protected override void Die()
    {
        base.Die();
        stateMachine.ChangeState(DeadState);

        // 保存游戏结果
        PlayerProgressManager.Instance.UpdateGameResult(
            GameManager.Instance.KillCount,
            GameManager.Instance.PlayTime,
            level
        );

        // 清理场上敌人
        Enemy.ClearAllEnemies();
    }

    public override void DeathOver()
    {
        GameManager.Instance.GameOver();
    }

    public void ApplySlow(float slowAmount, float duration)
    {
        buffSystem.AddBuff(new Buff
        {
            id = "slow",
            duration = duration,
            moveSpeedMult = 1f - slowAmount
        });
    }

    public void AddShield(float amount)
    {
        currentShield += amount;
        GameEvents.OnShieldChanged?.Invoke(currentShield, Stats.maxHP);
    }
}