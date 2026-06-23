using UnityEngine;

public class DragonBreath : MonoBehaviour
{
    public float speed = 7f;
    public float lifetime = 4f;
    public float aoeRadius = 1.2f;

    private float damage;
    private Vector2 direction;
    private bool hasHit;

    public void Init(float damage, Vector2 direction)
    {
        this.damage = damage;
        this.direction = direction.normalized;
        this.hasHit = false;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        CancelInvoke();
        Invoke(nameof(ExplodeAndRecycle), lifetime);
    }

    private void Update()
    {
        if (hasHit) return;

        transform.position += (Vector3)direction * (speed * Time.deltaTime);
        CheckHitPlayer();
    }

    private void CheckHitPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            0.3f,
            LayerMask.GetMask("Player")
        );

        foreach (var hit in hits)
        {
            DealDamage(transform.position);
            Recycle();
            return;
        }
    }

    private void DealDamage(Vector2 center)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            center,
            aoeRadius,
            LayerMask.GetMask("Player")
        );

        foreach (var hit in hits)
        {
            var character = hit.GetComponent<Character>();
            if (character != null)
                character.TakeDamage(damage);
        }
    }

    private void Explode(Vector2 center)
    {
        if (hasHit) return;
        hasHit = true;

        DealDamage(center);

        CancelInvoke();
        Invoke(nameof(Recycle), 0.3f);
    }

    private void ExplodeAndRecycle()
    {
        Explode(transform.position);
    }

    private void Recycle()
    {
        hasHit = false;
        PoolManager.Instance.PushGameObject(gameObject);
    }
}
