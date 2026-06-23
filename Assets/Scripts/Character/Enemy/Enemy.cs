using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public static List<Enemy> ActiveEnemies = new List<Enemy>();
    public static List<Enemy> AlliedEnemies = new List<Enemy>();

    public bool isConverted;
    public virtual bool CanBeConverted => false;

    public EnemyStats Stats { get; protected set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyMoveState MoveState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemyDeadState DeadState { get; private set; }

    public bool isDead;
    public float lastAttackTime;
    private bool isKnockedBack;
    private Coroutine knockbackCoroutine;

    protected bool IsDead { get => isDead; set => isDead = value; }

    public static void ClearAllEnemies()
    {
        for (int i = ActiveEnemies.Count - 1; i >= 0; i--)
        {
            ActiveEnemies[i].Recycle();
        }
        for (int i = AlliedEnemies.Count - 1; i >= 0; i--)
        {
            AlliedEnemies[i].Recycle();
        }
    }

    public static List<Enemy> GetEnemyTargets()
    {
        return ActiveEnemies;
    }

    public static List<Enemy> GetAlliedTargets()
    {
        return AlliedEnemies;
    }

    protected override void Awake()
    {
        base.Awake();
        IdleState = new EnemyIdleState(this);
        MoveState = new EnemyMoveState(this);
        AttackState = new EnemyAttackState(this);
        DeadState = new EnemyDeadState(this);
    }

    protected virtual void OnEnable()
    {
        isKnockedBack = false;
        isConverted = false;
        ActiveEnemies.Add(this);
    }

    protected virtual void OnDisable()
    {
        ActiveEnemies.Remove(this);
        AlliedEnemies.Remove(this);
    }

    protected override void Update()
    {
        if (!isKnockedBack)
        {
            base.Update();
        }
    }

    public void Init(string enemyName)
    {
        var data = JsonManager.Load<EnemyStatsData>(
            "Data/EnemyStats",
            JsonManager.JsonType.LitJson,
            JsonManager.JsonLocation.Resources
        );
        Stats = data.enemies[enemyName];

        currentHP = Stats.maxHP;
        baseMoveSpeed = Stats.moveSpeed;
        baseArmor = 0;
        isDead = false;
        isConverted = false;
        lastAttackTime = -Stats.attackCooldown;

        stateMachine.ChangeState(IdleState);
    }

    public void Convert()
    {
        if (isConverted || !CanBeConverted) return;
        isConverted = true;
        ActiveEnemies.Remove(this);
        AlliedEnemies.Add(this);
        stateMachine.ChangeState(IdleState);
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + Stats.attackCooldown;
    }

    public override void TakeDamage(float damage)
    {
        if (isDead) return;

        base.TakeDamage(damage);

        if (currentHP > 0 && stateMachine.currentState != AttackState)
        {
            stateMachine.ChangeState(AttackState);
        }
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        base.Die();

        if (!isConverted)
        {
            GameManager.Instance.RecordKill();
            WaveManager.Instance.OnEnemyKilled();

            Transform player = GameManager.Instance.PlayerTransform;
            if (player != null)
            {
                var playerChar = player.GetComponent<Player>();
                if (playerChar != null)
                {
                    playerChar.AddXP(Stats.xpValue);
                }
            }
        }

        stateMachine.ChangeState(DeadState);
        Invoke("Recycle", 1f);
    }

    private void Recycle()
    {
        buffSystem.Clear();
        PoolManager.Instance.PushGameObject(gameObject);
    }

    public override void OnMeleeHit()
    {
        Transform target = GetAttackTarget();
        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist <= Stats.attackRange)
        {
            var targetChar = target.GetComponent<Character>();
            if (targetChar != null)
                targetChar.TakeDamage(Stats.damage);
        }
    }

    public Transform GetAttackTarget()
    {
        if (isConverted)
        {
            Enemy nearest = null;
            float minDist = float.MaxValue;
            foreach (var enemy in ActiveEnemies)
            {
                if (enemy == null || enemy.isDead) continue;
                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy;
                }
            }
            return nearest != null ? nearest.transform : null;
        }
        else
        {
            Enemy nearestAlly = null;
            float minAllyDist = float.MaxValue;
            foreach (var ally in AlliedEnemies)
            {
                if (ally == null || ally.isDead) continue;
                float dist = Vector2.Distance(transform.position, ally.transform.position);
                if (dist < minAllyDist)
                {
                    minAllyDist = dist;
                    nearestAlly = ally;
                }
            }

            if (nearestAlly != null && minAllyDist <= Stats.detectRange)
                return nearestAlly.transform;

            return GameManager.Instance.PlayerTransform;
        }
    }

    public override void AttackOver()
    {
        base.AttackOver();
        if (!isKnockedBack)
        {
            stateMachine.ChangeState(MoveState);
        }
    }

    public void Knockback(Vector2 direction)
    {
        if (isDead || Stats == null) return;

        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }
        knockbackCoroutine = StartCoroutine(KnockbackCoroutine(direction, Stats.knockbackDistance, Stats.knockbackDuration));
    }

    private IEnumerator KnockbackCoroutine(Vector2 direction, float distance, float duration)
    {
        isKnockedBack = true;

        Vector2 startPos = rb.position;
        Vector2 endPos = startPos + direction.normalized * distance;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = 1f - (1f - t) * (1f - t);
            rb.MovePosition(Vector2.Lerp(startPos, endPos, curveValue));
            yield return null;
        }

        rb.MovePosition(endPos);
        isKnockedBack = false;
        knockbackCoroutine = null;
    }
}