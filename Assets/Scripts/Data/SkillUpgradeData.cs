using System.Collections.Generic;

public enum SkillRarity
{
    Common,
    Rare,
    Epic,
    Legend
}

public enum SkillCategory
{
    Attribute,
    SkillEnhance,
    Special,
    Survival,
    Economy
}

public class SkillUpgradeDef
{
    public string id;
    public string name;
    public string description;
    public SkillRarity rarity;
    public SkillCategory category;
    public float baseValue;
    public float valuePerLevel;
}

public class SkillUpgradeConfigData
{
    public List<SkillUpgradeDef> skills;
}
