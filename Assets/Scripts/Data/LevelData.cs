using System.Collections.Generic;

public class LevelDef
{
    public int level;
    public int xpRequired;
    public float hpBonusPercent;
}

public class LevelConfigData
{
    public List<LevelDef> levels;
}
