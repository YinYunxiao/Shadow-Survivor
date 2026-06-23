public class GameSetting : SingleBase<GameSetting>
{
    public void Save()
    {
        JsonManager.Save(this, "GameSetting.json", JsonManager.JsonType.JsonUtility);
    }

    public void Load()
    {
        instance = JsonManager.Load<GameSetting>("GameSetting.json", JsonManager.JsonType.JsonUtility);
    }
}
