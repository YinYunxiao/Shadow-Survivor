using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class CharacterPanelController : System.IDisposable
{
    private All.CharacterPanel _view;
    private Dictionary<string, PlayerStats> _characters;
    private List<string> _characterNames;
    private string _selectedCharacter;

    public CharacterPanelController(All.CharacterPanel view)
    {
        _view = view;
        Init();
    }

    private void Init()
    {
        LoadCharacterData();
        InitCharacterList();
        BindEvents();

        SelectCharacter(0);
    }

    private void LoadCharacterData()
    {
        var data = JsonManager.Load<PlayerStatsData>(
            "Data/PlayerStats",
            JsonManager.JsonType.LitJson,
            JsonManager.JsonLocation.Resources
        );
        _characters = data.characters;
        _characterNames = new List<string>(_characters.Keys);
    }

    private void InitCharacterList()
    {
        _view.characterList.itemRenderer = OnRenderCharacter;
        _view.characterList.numItems = _characterNames.Count;
    }

    private void BindEvents()
    {
        _view.characterList.onClickItem.Add(OnItemClick);
        _view.goBackButton.onClick.Add(OnGoBackClick);
        _view.confirmButton.onClick.Add(OnConfirmClick);
    }

    private void OnRenderCharacter(int index, GObject obj)
    {
        GButton item = obj as GButton;
        if (item == null) return;

        string charName = _characterNames[index];
        string url = CharacterUnlockManager.Instance.GetCharacterUrl(charName);

        // 尝试设置图标
        item.icon = url;

        // 设置解锁状态
        item.grayed = !CharacterUnlockManager.Instance.IsUnlocked(charName);
    }

    private void OnItemClick(EventContext context)
    {
        int index = _view.characterList.GetChildIndex((GObject)context.data);

        SelectCharacter(index);
    }

    private void SelectCharacter(int index)
    {
        if (index < 0 || index >= _characterNames.Count) return;

        _selectedCharacter = _characterNames[index];

        _view.characterName.text = _selectedCharacter;

        var stats = _characters[_selectedCharacter];
        _view.characterInfo.text =
            $"HP: {stats.maxHP}\n" +
            $"攻击: {stats.attackDamage}\n" +
            $"速度: {stats.moveSpeed}\n" +
            $"护甲: {stats.armor}";

        UpdateCanSelect();
    }

    private void UpdateCanSelect()
    {
        if (_selectedCharacter == null) return;

        bool isUnlocked = CharacterUnlockManager.Instance.IsUnlocked(_selectedCharacter);
        _view.canSelect.selectedIndex = isUnlocked ? 1 : 0;
    }

    private void OnGoBackClick(EventContext context)
    {
        UIManager.Instance.HidePanel<All.CharacterPanel>(true);
        UIManager.Instance.ShowPanel<All.MenuPanel, MenuPanelController>("All");
    }

    private void OnConfirmClick(EventContext context)
    {
        if (_selectedCharacter == null) return;

        if (!CharacterUnlockManager.Instance.IsUnlocked(_selectedCharacter))
            return;

        PlayerProgressManager.Instance.SetLastCharacter(_selectedCharacter);
        PlayerManager.Instance.SwitchCharacter(_selectedCharacter);
        UIManager.Instance.HidePanel<All.CharacterPanel>(true);
        UIManager.Instance.ShowPanel<All.MenuPanel, MenuPanelController>("All");
    }

    public void Dispose()
    {
        _view.characterList.onClickItem.Remove(OnItemClick);
        _view.goBackButton.onClick.Remove(OnGoBackClick);
        _view.confirmButton.onClick.Remove(OnConfirmClick);
    }
}
