using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnData
{
    public string prefabPath;
    public int maxCount;
    public float spawnDistance;
}

public static class EnemySpawnConfig
{
    private static Dictionary<string, EnemySpawnData> _data;

    public static Dictionary<string, EnemySpawnData> Data
    {
        get
        {
            if (_data == null)
            {
                _data = JsonManager.Load<Dictionary<string, EnemySpawnData>>(
                    "Data/EnemySpawnConfig",
                    JsonManager.JsonType.LitJson,
                    JsonManager.JsonLocation.Resources
                );
            }
            return _data;
        }
    }

    public static EnemySpawnData GetEnemyData(string enemyName)
    {
        if (Data.TryGetValue(enemyName, out var data))
        {
            return data;
        }
        Debug.LogError($"Enemy config not found: {enemyName}");
        return null;
    }

    public static List<string> GetAllEnemyNames()
    {
        return new List<string>(Data.Keys);
    }

    public static bool HasEnemy(string enemyName)
    {
        return Data.ContainsKey(enemyName);
    }

    public static void Reload()
    {
        _data = null;
    }
}
