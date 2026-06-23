using FairyGUI;
using UnityEngine;

public class PausePanelController : System.IDisposable
{
    private All.PausePanel _view;
    private float _oldVolume;

    public PausePanelController(All.PausePanel view)
    {
        _view = view;
        Init();
    }

    private void Init()
    {
        _oldVolume = AudioListener.volume;

        _view.volueSlider.value = _oldVolume * _view.volueSlider.max;

        if (GameManager.Instance.CurrentState == GameState.Paused)
            _view.isPlaying.selectedIndex = 0;
        else
            _view.isPlaying.selectedIndex = 1;

        BindEvents();
    }

    private void BindEvents()
    {
        _view.cancelButton.onClick.Add(OnCancelClick);
        _view.cofirmButton.onClick.Add(OnConfirmClick);
        _view.volueSlider.onChanged.Add(OnVolumeChanged);
        _view.finishGameButton.onClick.Add(OnFinishGameClick);
    }

    private void OnCancelClick(EventContext context)
    {
        AudioListener.volume = _oldVolume;
        Hide();
        if (GameManager.Instance.CurrentState == GameState.Paused)
            GameManager.Instance.ResumeGame();
    }

    private void OnConfirmClick(EventContext context)
    {
        _oldVolume = AudioListener.volume;
        Hide();
        if (GameManager.Instance.CurrentState == GameState.Paused)
            GameManager.Instance.ResumeGame();
    }

    private void OnVolumeChanged(EventContext context)
    {
        float value = (float)(_view.volueSlider.value / _view.volueSlider.max);
        AudioListener.volume = value;
    }

    private void OnFinishGameClick(EventContext context)
    {
        Hide();
        GameManager.Instance.GameOver();
        UIManager.Instance.ShowPanel<All.GameOverPanel, GameOverPanelController>("All");
    }

    private void Hide()
    {
        UIManager.Instance.HidePanel<All.PausePanel>(true);
    }

    public void Dispose()
    {
        _view.cancelButton.onClick.Remove(OnCancelClick);
        _view.cofirmButton.onClick.Remove(OnConfirmClick);
        _view.volueSlider.onChanged.Remove(OnVolumeChanged);
        _view.finishGameButton.onClick.Remove(OnFinishGameClick);
    }
}
