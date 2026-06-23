using System.Collections.Generic;
using UnityEngine;

public class WaveManager : SingleBaseMono<WaveManager>
{
    private List<WaveDef> waves;
    private int currentWaveIndex;
    private int waveKillCount;
    private float spawnTimer;
    private bool isActive;
    private bool bossSpawned;
    private int totalWeight;

    public int CurrentWave => currentWaveIndex + 1;
    public WaveDef CurrentWaveDef => currentWaveIndex < waves.Count ? waves[currentWaveIndex] : null;

    protected override void Awake()
    {
        base.Awake();
        LoadConfig();
    }

    private void LoadConfig()
    {
        var data = JsonManager.Load<WaveConfigData>(
            "Data/WaveConfig",
            JsonManager.JsonType.LitJson,
            JsonManager.JsonLocation.Resources
        );
        waves = data.waves;
    }

    public void StartWaves()
    {
        currentWaveIndex = 0;
        waveKillCount = 0;
        spawnTimer = 0f;
        isActive = true;
        bossSpawned = false;

        GameEvents.OnWaveChanged?.Invoke(CurrentWave);
        CalculateTotalWeight();
    }

    public void StopWaves()
    {
        isActive = false;
    }

    private void Update()
    {
        if (!isActive) return;
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        var wave = CurrentWaveDef;
        if (wave == null) return;

        if (!string.IsNullOrEmpty(wave.boss))
        {
            UpdateBossWave(wave);
        }
        else
        {
            UpdateNormalWave(wave);
        }
    }

    private void UpdateNormalWave(WaveDef wave)
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f && Enemy.ActiveEnemies.Count < wave.maxAlive)
        {
            SpawnWaveEnemy(wave);
            spawnTimer = wave.spawnInterval;
        }
    }

    private void UpdateBossWave(WaveDef wave)
    {
        if (!bossSpawned)
        {
            SpawnBoss(wave);
            bossSpawned = true;
        }

        if (Enemy.ActiveEnemies.Count == 0 && bossSpawned)
        {
            NextWave();
        }
    }

    private void SpawnWaveEnemy(WaveDef wave)
    {
        string enemyName = RollEnemy(wave);
        if (string.IsNullOrEmpty(enemyName)) return;

        var spawnData = EnemySpawnConfig.GetEnemyData(enemyName);
        if (spawnData == null) return;

        Transform player = GameManager.Instance.PlayerTransform;
        if (player == null) return;

        Vector2 randomPos = Random.insideUnitCircle.normalized * spawnData.spawnDistance;
        Vector2 spawnPos = (Vector2)player.position + randomPos;

        GameObject enemy = PoolManager.Instance.PopGameObject(
            spawnData.prefabPath,
            enemyName,
            spawnData.maxCount
        );
        enemy.transform.position = spawnPos;

        ApplyDifficultyScale(enemy, wave);
    }

    private void SpawnBoss(WaveDef wave)
    {
        var spawnData = EnemySpawnConfig.GetEnemyData(wave.boss);
        if (spawnData == null) return;

        Transform player = GameManager.Instance.PlayerTransform;
        if (player == null) return;

        Vector2 randomPos = Random.insideUnitCircle.normalized * spawnData.spawnDistance;
        Vector2 spawnPos = (Vector2)player.position + randomPos;

        GameObject boss = PoolManager.Instance.PopGameObject(
            spawnData.prefabPath,
            wave.boss,
            spawnData.maxCount
        );
        boss.transform.position = spawnPos;

        var dragon = boss.GetComponent<Dragon>();
        if (dragon != null)
        {
            dragon.InitDragon();
            if (wave.hpMult > 1f)
            {
                dragon.Stats.maxHP *= wave.hpMult;
                dragon.currentHP = dragon.Stats.maxHP;
            }
            if (wave.dmgMult > 1f)
            {
                dragon.Stats.damage *= wave.dmgMult;
            }
        }
    }

    private string RollEnemy(WaveDef wave)
    {
        if (wave.enemies == null || wave.enemies.Count == 0) return null;

        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var entry in wave.enemies)
        {
            cumulative += entry.weight;
            if (roll < cumulative)
                return entry.name;
        }

        return wave.enemies[wave.enemies.Count - 1].name;
    }

    private void ApplyDifficultyScale(GameObject enemyObj, WaveDef wave)
    {
        if (wave.hpMult <= 1f && wave.dmgMult <= 1f) return;

        var enemy = enemyObj.GetComponent<Enemy>();
        if (enemy == null || enemy.Stats == null) return;

        if (wave.hpMult > 1f)
        {
            enemy.Stats.maxHP *= wave.hpMult;
            enemy.currentHP = enemy.Stats.maxHP;
        }
        if (wave.dmgMult > 1f)
        {
            enemy.Stats.damage *= wave.dmgMult;
        }
    }

    private void CalculateTotalWeight()
    {
        totalWeight = 0;
        var wave = CurrentWaveDef;
        if (wave == null || wave.enemies == null) return;

        foreach (var entry in wave.enemies)
        {
            totalWeight += entry.weight;
        }
    }

    public void OnEnemyKilled()
    {
        if (!isActive) return;

        var wave = CurrentWaveDef;
        if (wave == null) return;

        if (!string.IsNullOrEmpty(wave.boss)) return;

        waveKillCount++;
        if (waveKillCount >= wave.killTarget)
        {
            NextWave();
        }
    }

    private void NextWave()
    {
        currentWaveIndex++;
        waveKillCount = 0;
        spawnTimer = 0f;
        bossSpawned = false;

        if (currentWaveIndex >= waves.Count)
        {
            currentWaveIndex = waves.Count - 1;
        }

        CalculateTotalWeight();
        GameEvents.OnWaveChanged?.Invoke(CurrentWave);
    }
}
