using System.Collections.Generic;

public class UnlockCondition
{
    public string type;
    public int cost;
    public int requireLevel;
    public int requireKills;
    public string url;
}

public class CharacterUnlockConfigData
{
    public Dictionary<string, UnlockCondition> unlockConditions;
}

public class CharacterUnlockManager : SingleBase<CharacterUnlockManager>
{
    private CharacterUnlockManager()
    {
        LoadConfig();
    }

    private Dictionary<string, UnlockCondition> _conditions;

    private void LoadConfig()
    {
        var data = JsonManager.Load<CharacterUnlockConfigData>(
            "Data/CharacterUnlockConfig",
            JsonManager.JsonType.LitJson,
            JsonManager.JsonLocation.Resources
        );
        _conditions = data.unlockConditions;
    }

    public bool IsUnlocked(string characterName)
    {
        return PlayerProgressManager.Instance.Progress
            .unlockedCharacters.Contains(characterName);
    }
    public bool CanUnlock(string characterName)
    {
        var condition = _conditions[characterName];
        switch (condition.type)
        {
            case "gold":
                return PlayerProgressManager.Instance.Progress.gold >= condition.cost;
            case "level":
                return PlayerProgressManager.Instance.Progress.highestLevel >= condition.requireLevel;
            case "kill":
                return PlayerProgressManager.Instance.Progress.totalKills >= condition.requireKills;
            default:
                return true;
        }
    }
    public void Unlock(string characterName)
    {
        if (!IsUnlocked(characterName) && CanUnlock(characterName))
        {
            var progress = PlayerProgressManager.Instance.Progress;
            progress.unlockedCharacters.Add(characterName);

            if (_conditions[characterName].type == "gold")
                progress.gold -= _conditions[characterName].cost;

            PlayerProgressManager.Instance.Save();
        }
    }

    public string GetCharacterUrl(string characterName)
    {
        if (_conditions.ContainsKey(characterName))
            return _conditions[characterName].url;
        return null;
    }

    public string GetUnlockConditionText(string characterName)
    {
        if (!_conditions.ContainsKey(characterName))
            return "未知";

        var condition = _conditions[characterName];
        switch (condition.type)
        {
            case "default":
                return "默认解锁";
            case "gold":
                return $"{condition.cost} 金币";
            case "level":
                return $"最高等级达到 {condition.requireLevel}";
            case "kill":
                return $"击杀 {condition.requireKills} 个敌人";
            default:
                return "未知";
        }
    }
}

