public class PlayerProgressManager : SingleBase<PlayerProgressManager>
{
    public PlayerProgress Progress { get; private set; }

    private const string FILE_NAME = "PlayerProgress.json";

    private PlayerProgressManager()
    {
        Load();
    }

    public void Save()
    {
        JsonManager.Save(Progress, FILE_NAME, JsonManager.JsonType.LitJson);
    }

    public void Load()
    {
        Progress = JsonManager.Load<PlayerProgress>(FILE_NAME, JsonManager.JsonType.LitJson);
        if (Progress == null)
            Progress = new PlayerProgress();
    }

    public void AddGold(int amount)
    {
        Progress.gold += amount;
        Save();
    }

    public void SetLastCharacter(string characterName)
    {
        Progress.lastCharacter = characterName;
        Save();
    }

    public void UpdateGameResult(int kills, float time, int level)
    {
        Progress.totalKills += kills;
        if (time > Progress.bestTime) Progress.bestTime = time;
        if (level > Progress.highestLevel) Progress.highestLevel = level;
        Save();
    }
}
