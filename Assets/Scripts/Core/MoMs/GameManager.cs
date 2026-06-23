using UnityEngine;

public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver
}

public class GameManager : SingleBaseMono<GameManager>
{
    public GameState CurrentState { get; private set; }
    public float PlayTime { get; private set; }
    public Transform PlayerTransform { get; set; }
    public float DifficultyMultiplier { get; private set; }
    public int KillCount { get; private set; }

    private float goldTimer;
    private const float GOLD_INTERVAL = 3f;
    private const int GOLD_AMOUNT = 1;

    protected override void Awake()
    {
        base.Awake();
        CurrentState = GameState.Menu;
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            PlayTime += Time.deltaTime;
            goldTimer += Time.deltaTime;
            if (goldTimer >= GOLD_INTERVAL)
            {
                goldTimer -= GOLD_INTERVAL;
                PlayerProgressManager.Instance.AddGold(GOLD_AMOUNT);
                GameEvents.OnGoldChanged?.Invoke(PlayerProgressManager.Instance.Progress.gold);
            }
        }
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Menu:
                Time.timeScale = 1f;
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }

    // 开始
    public void StartGame()
    {
        PlayTime = 0f;
        goldTimer = 0f;
        KillCount = 0;
        DifficultyMultiplier = 1f;
        ChangeState(GameState.Playing);
        SkillUpgradeManager.Instance.Reset();
        WaveManager.Instance.StartWaves();
        MapGenerator.Instance.StartMap();
    }

    public void RecordKill()
    {
        KillCount++;
        GameEvents.OnKillCountChanged?.Invoke(KillCount);
    }

    // 暂停
    public void PauseGame()
    {
        ChangeState(GameState.Paused);
    }

    // 恢复暂停
    public void ResumeGame()
    {
        ChangeState(GameState.Playing);
    }

    // 结束
    public void GameOver()
    {
        ChangeState(GameState.GameOver);
        Enemy.ClearAllEnemies();

        // 移除游戏面板
        UIManager.Instance.HidePanel<All.PlayingPanel>(true);
        // 显示结算面板
        UIManager.Instance.ShowPanel<All.GameOverPanel, GameOverPanelController>("All");
    }

    // 返回主菜单
    public void GoToMenu()
    {
        PlayTime = 0f;
        goldTimer = 0f;
        KillCount = 0;
        DifficultyMultiplier = 1f;
        ChangeState(GameState.Menu);

        // 重置玩家状态
        if (PlayerTransform != null)
        {
            var player = PlayerTransform.GetComponent<Player>();
            if (player != null)
                player.Init(PlayerProgressManager.Instance.Progress.lastCharacter);
        }
    }
}
