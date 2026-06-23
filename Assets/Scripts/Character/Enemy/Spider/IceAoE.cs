using UnityEngine;

public class IceAoE : MonoBehaviour
{
    // 减速效果
    public float slowAmount = 0.5f;
    // 持续时间
    public float duration = 2f;
    public float lifetime = 3f;

    private float damage;
    private float radius;
    private bool hasHit;

    public void Init(float damage, float radius)
    {
        this.damage = damage;
        this.radius = radius;
        this.hasHit = false;

        CancelInvoke();
        Invoke(nameof(Recycle), lifetime);
    }

    private void Update()
    {
        if (hasHit) return;

        Transform player = GameManager.Instance.PlayerTransform;
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= radius)
        {
            var playerChar = player.GetComponent<Character>();
            if (playerChar != null)
            {
                playerChar.TakeDamage(damage);
                ApplySlow(playerChar);
                hasHit = true;
                Invoke(nameof(Recycle), 0.5f);
            }
        }
    }

    private void ApplySlow(Character target)
    {
        var player = target as Player;
        if (player != null)
        {
            player.ApplySlow(slowAmount, duration);
        }
    }

    private void Recycle()
    {
        hasHit = false;
        PoolManager.Instance.PushGameObject(gameObject);
    }
}
