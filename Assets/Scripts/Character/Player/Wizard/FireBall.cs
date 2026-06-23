using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed = 8f;
    public float lifetime = 3f;
    public float aoeRadius = 1.5f;

    private float damage;
    private Transform target;

    public void Init(float damage, Transform target, float speedMult = 1f, float rangeMult = 1f, float aoeMult = 1f)
    {
        this.damage = damage;
        this.target = target;
        this.speed = 8f * speedMult;
        this.lifetime = 3f * rangeMult;
        this.aoeRadius = 1.5f * aoeMult;

        CancelInvoke();
        Invoke(nameof(Recycle), lifetime);
    }

    private void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Explode(transform.position);
            return;
        }

        Vector2 dir = (target.position - transform.position).normalized;
        transform.position += (Vector3)dir * (speed * Time.deltaTime);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist < 0.3f)
        {
            Explode(target.position);
        }
    }

    private void Explode(Vector2 center)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            center,
            aoeRadius,
            LayerMask.GetMask("Enemy")
        );

        var player = GetPlayer();

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (player != null)
                    player.DealDamageToEnemy(enemy, damage);
                else
                    enemy.TakeDamage(damage);
            }
        }

        Recycle();
    }

    private Player GetPlayer()
    {
        Transform playerTransform = GameManager.Instance.PlayerTransform;
        if (playerTransform == null) return null;
        return playerTransform.GetComponentInChildren<Player>();
    }

    private void Recycle()
    {
        target = null;
        PoolManager.Instance.PushGameObject(gameObject);
    }
}
