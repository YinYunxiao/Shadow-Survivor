using FairyGUI;
using UnityEngine;
public class PlayingPanelController : System.IDisposable
{
    private All.PlayingPanel _view;
    private JoystickController _joystickCtrl;
    private float _baseHPWidth;
    private float _initialMaxHP;
    private float _currentHP;
    private float _maxHP;
    private float _currentShield;
    public JoystickController JoystickCtrl => _joystickCtrl;
    public PlayingPanelController(All.PlayingPanel view)
    {
        _view = view;
        Init();
    }
    private void Init()
    {
        InitJoystick();
        InitHPBar();
        BindEvents();
        SubscribeGameEvents();
        RefreshAll();
        _view.playStart.SetSelectedIndex(1);
        _view.hasBoss.SetSelectedIndex(0);
    }
    private void InitJoystick()
    {
        _joystickCtrl = new JoystickController(_view.joystick);
        InputManager.SetJoystick(_joystickCtrl);
    }
    private void InitHPBar()
    {
        _baseHPWidth = _view.hpBar.hp.width;

        // 获取实际的初始 maxHP
        var player = GameManager.Instance.PlayerTransform?.GetComponentInChildren<Player>();
        if (player != null)
            _initialMaxHP = player.Stats.maxHP;
        else
            _initialMaxHP = 100f;
    }
    private void BindEvents()
    {
        _view.pauseButton.onClick.Add(OnPauseClick);
        _view.playerInfoButton.onClick.Add(OnPlayerInfoClick);
    }
    private void SubscribeGameEvents()
    {
        GameEvents.OnGoldChanged += UpdateGold;
        GameEvents.OnKillCountChanged += UpdateKillCount;
        GameEvents.OnHPChanged += UpdateHPBar;
        GameEvents.OnShieldChanged += UpdateShield;
        GameEvents.OnXPChanged += UpdateXPBar;
        GameEvents.OnLevelUp += UpdateLevel;
        GameEvents.OnPassiveSkillProgress += UpdatePassiveSkillBar;
        GameEvents.OnBossHPChanged += UpdateBossHPBar;
        GameEvents.OnBossAppear += ShowBossHPBar;
        GameEvents.OnBossDefeated += HideBossHPBar;
    }
    private void UnsubscribeGameEvents()
    {
        GameEvents.OnGoldChanged -= UpdateGold;
        GameEvents.OnKillCountChanged -= UpdateKillCount;
        GameEvents.OnHPChanged -= UpdateHPBar;
        GameEvents.OnShieldChanged -= UpdateShield;
        GameEvents.OnXPChanged -= UpdateXPBar;
        GameEvents.OnLevelUp -= UpdateLevel;
        GameEvents.OnPassiveSkillProgress -= UpdatePassiveSkillBar;
        GameEvents.OnBossHPChanged -= UpdateBossHPBar;
        GameEvents.OnBossAppear -= ShowBossHPBar;
        GameEvents.OnBossDefeated -= HideBossHPBar;
    }
    private void RefreshAll()
    {
        UpdateGold(PlayerProgressManager.Instance.Progress.gold);
        UpdateKillCount(0);
        UpdateLevel(1);

        var player = GameManager.Instance.PlayerTransform?.GetComponentInChildren<Player>();
        if (player != null)
        {
            UpdateHPBar(player.currentHP, player.Stats.maxHP);
            UpdateShield(player.currentShield, player.Stats.maxHP);

            _view.hasPassiveSkill.SetSelectedIndex(player.HasPassiveSkill ? 1 : 0);
        }
        else
        {
            UpdateHPBar(1f, 1f);
            UpdateShield(0f, 1f);
        }

        UpdateXPBar(0f, 1f);
    }
    private void OnPauseClick(EventContext context)
    {
        GameManager.Instance.PauseGame();
        UIManager.Instance.ShowPanel<All.PausePanel, PausePanelController>("All");
    }
    private void OnPlayerInfoClick(EventContext context)
    {
        UIManager.Instance.ShowPanel<All.PlayerInfoPanel, PlayerInfoPanelController>("All");
    }
    private void UpdateGold(int gold)
    {
        _view.coinCountLabel.title = gold.ToString();
    }
    private void UpdateKillCount(int kills)
    {
        _view.killCountLabel.title = kills.ToString();
    }
    private void UpdateHPBar(float currentHP, float maxHP)
    {
        _currentHP = currentHP;
        _maxHP = maxHP;
        float widthRatio = _initialMaxHP > 0 ? maxHP / _initialMaxHP : 1f;
        float hpRatio = maxHP > 0 ? currentHP / maxHP : 0;
        _view.hpBar.hp.width = _baseHPWidth * widthRatio * hpRatio;
        UpdateFrame();
    }
    private void UpdateShield(float currentShield, float maxHP)
    {
        _currentShield = currentShield;
        _maxHP = maxHP;
        float widthRatio = _initialMaxHP > 0 ? maxHP / _initialMaxHP : 1f;
        float shieldRatio = maxHP > 0 ? currentShield / maxHP : 0;
        _view.hpBar.shield.width = _baseHPWidth * widthRatio * shieldRatio;
        UpdateFrame();
    }
    private void UpdateFrame()
    {
        float maxValue = Mathf.Max(_maxHP, _currentHP + _currentShield);
        float ratio = _initialMaxHP > 0 ? maxValue / _initialMaxHP : 1f;
        _view.hpBar.frame.width = _baseHPWidth * ratio;
    }
    private void UpdateXPBar(float currentXP, float maxXP)
    {
        float value = maxXP > 0 ? (currentXP / maxXP) * 100 : 0;
        _view.xpBar.value = value;
    }
    private void UpdateLevel(int level)
    {
        _view.levelText.text = $"Level: {level}";
    }
    private void UpdatePassiveSkillBar(float timer, float cooldown)
    {
        float value = cooldown > 0 ? (timer / cooldown) * 100 : 0;
        _view.passiveSkillBar.value = value;
    }
    public void Dispose()
    {
        _view.playStart.SetSelectedIndex(0);
        UnsubscribeGameEvents();
        _joystickCtrl?.Dispose();
    }
    private void ShowBossHPBar()
    {
        _view.hasBoss.SetSelectedIndex(1);
    }
    private void HideBossHPBar()
    {
        _view.hasBoss.SetSelectedIndex(0);
    }
    private void UpdateBossHPBar(float currentHP, float maxHP)
    {
        float ratio = maxHP > 0 ? Mathf.Clamp01(currentHP / maxHP) : 0;
        _view.bossHPBar.value = ratio * 100;
    }
}