using FairyGUI;

public class MenuPanelController : System.IDisposable
{
    private All.MenuPanel _view;

    public MenuPanelController(All.MenuPanel view)
    {
        _view = view;
        Init();
    }

    private void Init()
    {
        SpawnLastCharacter();
        RefreshGold();
        SubscribeEvents();
        BindEvents();
        MapGenerator.Instance.StartMap();
    }

    private void SpawnLastCharacter()
    {
        string lastCharacter = PlayerProgressManager.Instance.Progress.lastCharacter;
        PlayerManager.Instance.SpawnCharacter(lastCharacter);
    }

    private void BindEvents()
    {
        _view.startGameGraph.onTouchBegin.Add(OnStartGameClick);
        _view.characterButton.onClick.Add(OnCharacterClick);
        _view.shopButton.onClick.Add(OnShopClick);
        _view.settingButton.onClick.Add(OnSettingClick);
    }

    private void SubscribeEvents()
    {
        GameEvents.OnGoldChanged += UpdateGold;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGoldChanged -= UpdateGold;
    }

    private void RefreshGold()
    {
        _view.coinDisplayLabel.title = PlayerProgressManager.Instance.Progress.gold.ToString();
    }

    private void UpdateGold(int gold)
    {
        _view.coinDisplayLabel.title = gold.ToString();
    }

    private void OnStartGameClick(EventContext context)
    {
        UIManager.Instance.HidePanel<All.MenuPanel>(false);
        UIManager.Instance.ShowPanel<All.PlayingPanel, PlayingPanelController>("All");
        GameManager.Instance.StartGame();
    }

    private void OnCharacterClick(EventContext context)
    {
        UIManager.Instance.HidePanel<All.MenuPanel>(false);
        UIManager.Instance.ShowPanel<All.CharacterPanel, CharacterPanelController>("All");
    }

    private void OnShopClick(EventContext context)
    {
        UIManager.Instance.HidePanel<All.MenuPanel>(false);
        UIManager.Instance.ShowPanel<All.ShopPanel, ShopPanelController>("All");
    }

    private void OnSettingClick(EventContext context)
    {
        UIManager.Instance.ShowPanel<All.PausePanel, PausePanelController>("All");
    }

    public void Dispose()
    {
        UnsubscribeEvents();
        _view.startGameGraph.onTouchBegin.Remove(OnStartGameClick);
        _view.characterButton.onClick.Remove(OnCharacterClick);
        _view.shopButton.onClick.Remove(OnShopClick);
        _view.settingButton.onClick.Remove(OnSettingClick);
    }
}
