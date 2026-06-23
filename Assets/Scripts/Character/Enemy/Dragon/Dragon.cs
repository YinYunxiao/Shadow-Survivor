using UnityEngine;

public class Dragon : BossEnemy
{
    public DragonChaseState ChaseState { get; private set; }
    public DragonBreathState BreathState { get; private set; }
    public DragonLiftOffState LiftOffState { get; private set; }
    public DragonLandingState LandingState { get; private set; }

    private float liftOffTimer;
    private const float LIFT_OFF_COOLDOWN = 15f;
    private bool isInvincible;

    public bool IsInvincible => isInvincible;

    protected override void Awake()
    {
        base.Awake();
        ChaseState = new DragonChaseState(this);
        BreathState = new DragonBreathState(this);
        LiftOffState = new DragonLiftOffState(this);
        LandingState = new DragonLandingState(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        liftOffTimer = LIFT_OFF_COOLDOWN;
        isInvincible = false;
        GameEvents.OnBossAppear?.Invoke();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    public override void TakeDamage(float damage)
    {
        if (isInvincible || IsDead) return;

        float actualDamage = damage - armor;
        if (actualDamage > 0)
        {
            currentHP -= actualDamage;
        }

        GameEvents.OnBossHPChanged?.Invoke(currentHP, Stats.maxHP);

        if (currentHP < 0)
            Die();
    }

    public void InitDragon()
    {
        var data = JsonManager.Load<EnemyStatsData>(
            "Data/EnemyStats",
            JsonManager.JsonType.LitJson,
            JsonManager.JsonLocation.Resources
        );
        Stats = data.enemies["Dragon"];

        currentHP = Stats.maxHP;
        baseMoveSpeed = Stats.moveSpeed;
        baseArmor = 0;
        IsDead = false;
        lastAttackTime = -Stats.attackCooldown;

        stateMachine.ChangeState(ChaseState);
    }

    public override void AttackOver()
    {
        base.AttackOver();
        stateMachine.ChangeState(ChaseState);
    }

    public bool CanLiftOff()
    {
        return liftOffTimer <= 0f;
    }

    public void ResetLiftOffTimer()
    {
        liftOffTimer = LIFT_OFF_COOLDOWN;
    }

    public void SpawnBreath(Vector2 dir)
    {
        GameObject breath = PoolManager.Instance.PopGameObject(
            "Prefabs/Projectile/DragonBreath/DragonBreath",
            "DragonBreath",
            10
        );
        breath.transform.position = transform.position;

        var db = breath.GetComponent<DragonBreath>();
        if (db != null)
            db.Init(Stats.damage, dir);
    }

    protected override void Update()
    {
        liftOffTimer -= Time.deltaTime;
        base.Update();
    }

    protected override void Die()
    {
        GameEvents.OnBossDefeated?.Invoke();
        base.Die();
    }
}
