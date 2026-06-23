using System.Collections.Generic;

public class WaveEnemyEntry
{
    public string name;
    public int weight;
}

public class WaveDef
{
    public int wave;
    public int killTarget;
    public List<WaveEnemyEntry> enemies;
    public string boss;
    public int maxAlive;
    public float spawnInterval;
    public float hpMult;
    public float dmgMult;
}

public class WaveConfigData
{
    public List<WaveDef> waves;
}
