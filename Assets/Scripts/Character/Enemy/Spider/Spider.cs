using UnityEngine;

public class Spider : NormalEnemy
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Init("Spider");
    }

    public override void OnAoeHit()
    {
        SpawnIceAoE();
    }

    private void SpawnIceAoE()
    {
        Transform player = GameManager.Instance.PlayerTransform;
        if (player == null) return;

        Vector2 spawnPos = player.position;

        GameObject iceAoE = PoolManager.Instance.PopGameObject(
            "Prefabs/Projectile/AoE/IceAoEPrefab",
            "IceAoE",
            5
        );
        iceAoE.transform.position = spawnPos;

        var aoE = iceAoE.GetComponent<IceAoE>();
        if (aoE != null)
        {
            aoE.Init(Stats.damage, Stats.attackRange);
        }
    }
}
