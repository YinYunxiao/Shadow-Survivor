using UnityEngine;
using System.Collections;

public abstract class Character : MonoBehaviour
{
    public float baseMoveSpeed = 1f;
    public float currentHP;
    public float baseArmor;
    public float footOffset = 0f;

    public StateMachine stateMachine { get; private set; }
    public Animator animator { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public BuffSystem buffSystem { get; private set; }

    private Material originalMaterial;
    private Material hitMaterial;
    private Material freezeMaterial;
    private Material stunMaterial;
    private Coroutine hitEffectCoroutine;
    private Coroutine freezeEffectCoroutine;
    private Coroutine stunEffectCoroutine;

    public bool IsStunned => buffSystem.GetMoveSpeedMult() <= 0f;

    public float moveSpeed => baseMoveSpeed * buffSystem.GetMoveSpeedMult();
    public float armor => baseArmor + buffSystem.GetArmorAdd();

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        stateMachine = new StateMachine();
        buffSystem = new BuffSystem();

        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
            hitMaterial = Resources.Load<Material>("Materials/Sprites/HitFX");
            freezeMaterial = Resources.Load<Material>("Materials/Sprites/FreezeFX");
            stunMaterial = Resources.Load<Material>("Materials/Sprites/StunFX");
        }
    }

    protected virtual void Start() { }

    protected virtual void Update()
    {
        buffSystem.Update(Time.deltaTime);
        stateMachine.Update();
        UpdateSortingOrder();
    }

    protected virtual void FixedUpdate() => stateMachine.FixedUpdate();

    private void UpdateSortingOrder()
    {
        if (spriteRenderer != null)
        {
            float footY = transform.position.y + footOffset;
            spriteRenderer.sortingOrder = -Mathf.RoundToInt(footY * 100);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        float actualDamage = damage - armor;

        if (actualDamage > 0)
        {
            currentHP -= actualDamage;
            PlayHitEffect();
        }
        print(gameObject.name + currentHP);
        if (currentHP < 0)
            Die();
    }

    private void PlayHitEffect()
    {
        if (spriteRenderer == null || hitMaterial == null) return;

        if (hitEffectCoroutine != null)
            StopCoroutine(hitEffectCoroutine);

        hitEffectCoroutine = StartCoroutine(HitEffectCoroutine());
    }

    private IEnumerator HitEffectCoroutine()
    {
        spriteRenderer.material = hitMaterial;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material = originalMaterial;
        hitEffectCoroutine = null;
    }

    public void PlayFreezeEffect(float duration)
    {
        if (spriteRenderer == null || freezeMaterial == null) return;

        if (freezeEffectCoroutine != null)
            StopCoroutine(freezeEffectCoroutine);

        freezeEffectCoroutine = StartCoroutine(FreezeEffectCoroutine(duration));
    }

    private IEnumerator FreezeEffectCoroutine(float duration)
    {
        spriteRenderer.material = freezeMaterial;
        yield return new WaitForSeconds(duration);
        spriteRenderer.material = originalMaterial;
        freezeEffectCoroutine = null;
    }

    public void PlayStunEffect(float duration)
    {
        if (spriteRenderer == null || stunMaterial == null) return;

        if (stunEffectCoroutine != null)
            StopCoroutine(stunEffectCoroutine);

        stunEffectCoroutine = StartCoroutine(StunEffectCoroutine(duration));
    }

    private IEnumerator StunEffectCoroutine(float duration)
    {
        spriteRenderer.material = stunMaterial;
        yield return new WaitForSeconds(duration);
        spriteRenderer.material = originalMaterial;
        stunEffectCoroutine = null;
    }

    protected virtual void Die()
    {
        buffSystem.Clear();
    }

    public virtual void OnMeleeHit()
    {
    }

    public virtual void OnRangedHit()
    {
    }

    public virtual void OnAoeHit()
    {
    }

    public virtual void AttackOver()
    {
    }

    public virtual void DeathOver()
    {
    }

    public void SetDirection(Vector2 direction)
    {
        if (direction.magnitude < 0.1f) return;

        float standardAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (standardAngle < 0) standardAngle += 360;

        float angle = (90 - standardAngle + 360) % 360;
        animator.SetFloat("Angle", angle);
    }
}