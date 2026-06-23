using UnityEngine;
using System.Collections.Generic;

public class Arrow : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;

    private float damage;
    private Transform target;
    private Vector2 direction;
    private bool isPenetrating;
    private HashSet<int> hitEnemies = new HashSet<int>();
    private int penetrateCount;
    private int maxPenetrate;

    public void Init(float damage, Transform target, float speedMult = 1f, float rangeMult = 1f, int extraPenetrate = 0)
    {
        this.damage = damage;
        this.target = target;
        this.direction = Vector2.zero;
        this.isPenetrating = false;
        this.speed = 10f * speedMult;
        this.lifetime = 3f * rangeMult;
        this.penetrateCount = 0;
        this.maxPenetrate = extraPenetrate;
        hitEnemies.Clear();

        CancelInvoke();
        Invoke(nameof(Recycle), lifetime);
    }

    public void InitPenetrating(float damage, Vector2 direction, float speedMult = 1f, float rangeMult = 1f)
    {
        this.damage = damage;
        this.target = null;
        this.direction = direction.normalized;
        this.isPenetrating = true;
        this.speed = 10f * speedMult;
        this.lifetime = 3f * rangeMult;
        this.penetrateCount = 0;
        this.maxPenetrate = 999;
        hitEnemies.Clear();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        CancelInvoke();
        Invoke(nameof(Recycle), lifetime);
    }

    private void Update()
    {
        if (isPenetrating)
        {
            transform.position += (Vector3)direction * (speed * Time.deltaTime);
            CheckPenetratingHit();
            return;
        }

        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Recycle();
            return;
        }

        Vector2 dir = (target.position - transform.position).normalized;
        transform.position += (Vector3)dir * (speed * Time.deltaTime);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist < 0.2f)
        {
            var enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                var player = GetPlayer();
                if (player != null)
                    player.DealDamageToEnemy(enemy, damage);
                else
                    enemy.TakeDamage(damage);
            }

            penetrateCount++;
            if (penetrateCount > maxPenetrate)
            {
                Recycle();
            }
            else
            {
                hitEnemies.Add(target.GetInstanceID());
                target = FindNextTarget();
                if (target == null)
                    Recycle();
            }
        }
    }

    private Transform FindNextTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            5f,
            LayerMask.GetMask("Enemy")
        );

        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            int id = hit.gameObject.GetInstanceID();
            if (hitEnemies.Contains(id)) continue;

            float d = Vector2.Distance(transform.position, hit.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    private void CheckPenetratingHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            0.2f,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            int id = hit.gameObject.GetInstanceID();
            if (hitEnemies.Contains(id)) continue;

            var enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                var player = GetPlayer();
                if (player != null)
                    player.DealDamageToEnemy(enemy, damage);
                else
                    enemy.TakeDamage(damage);
                hitEnemies.Add(id);
            }
        }
    }

    private void Recycle()
    {
        target = null;
        isPenetrating = false;
        hitEnemies.Clear();
        PoolManager.Instance.PushGameObject(gameObject);
    }

    private Player GetPlayer()
    {
        Transform playerTransform = GameManager.Instance.PlayerTransform;
        if (playerTransform == null) return null;
        return playerTransform.GetComponentInChildren<Player>();
    }
}
