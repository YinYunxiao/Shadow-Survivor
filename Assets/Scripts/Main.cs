using UnityEngine;

public class Main : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.ShowPanel<All.MenuPanel, MenuPanelController>("All");
        GameManager.Instance.ChangeState(GameState.Menu);
    }

    private void SpawnEnemy(string enemyName)
    {
        if (GameManager.Instance.PlayerTransform == null) return;

        var data = EnemySpawnConfig.GetEnemyData(enemyName);
        if (data == null) return;

        Vector2 randomPos = Random.insideUnitCircle.normalized * data.spawnDistance;
        Vector2 spawnPos = (Vector2)GameManager.Instance.PlayerTransform.position + randomPos;

        GameObject enemy = PoolManager.Instance.PopGameObject(
            data.prefabPath,
            enemyName,
            data.maxCount
        );
        enemy.transform.position = spawnPos;

        var dragon = enemy.GetComponent<Dragon>();
        if (dragon != null)
            dragon.InitDragon();
    }
}
