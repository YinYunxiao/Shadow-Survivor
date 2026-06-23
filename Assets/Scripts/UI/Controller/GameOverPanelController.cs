using FairyGUI;

public class GameOverPanelController : System.IDisposable
{
    private All.GameOverPanel _view;

    public GameOverPanelController(All.GameOverPanel view)
    {
        _view = view;
        Init();
    }

    private void Init()
    {
        _view.coinCountLabel.title = PlayerProgressManager.Instance.Progress.gold.ToString();
        _view.killCountLabel.title = GameManager.Instance.KillCount.ToString();

        BindEvents();
    }

    private void BindEvents()
    {
        _view.cofirmButton.onClick.Add(OnConfirmClick);
    }

    private void OnConfirmClick(EventContext context)
    {
        UIManager.Instance.HidePanel<All.GameOverPanel>(true);
        GameManager.Instance.GoToMenu();
        UIManager.Instance.ShowPanel<All.MenuPanel, MenuPanelController>("All");
    }

    public void Dispose()
    {
        _view.cofirmButton.onClick.Remove(OnConfirmClick);
    }
}
