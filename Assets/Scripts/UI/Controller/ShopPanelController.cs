using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class ShopPanelController : System.IDisposable
{
    private All.ShopPanel _view;
    private Dictionary<string, PlayerStats> _characters;
    private List<string> _characterNames;
    private string _selectedCharacter;

    public ShopPanelController(All.ShopPanel view)
    {
        _view = view;
        Init();
    }

    private void Init()
    {
        LoadCharacterData();
        LoadOtherPackage();
        InitCharacterList();
        BindEvents();

        SelectCharacter(0);
    }

    private void LoadOtherPackage()
    {
        UIPackage.AddPackage("UI/Other");
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
        _view.characterShopList.RemoveChildrenToPool();
        for (int i = 0; i < _characterNames.Count; i++)
        {
            GButton item = _view.characterShopList.AddItemFromPool(null) as GButton;
            OnRenderCharacter(i, item);
        }
    }

    private void BindEvents()
    {
        _view.characterShopList.onClickItem.Add(OnItemClick);
        _view.goBackButton.onClick.Add(OnGoBackClick);
        _view.confirmButton.onClick.Add(OnConfirmClick);
    }

    private void OnRenderCharacter(int index, GObject obj)
    {
        GButton item = obj.asButton;
        if (item == null) return;

        string charName = _characterNames[index];
        string url = CharacterUnlockManager.Instance.GetCharacterUrl(charName);

        item.icon = url;
        item.grayed = CharacterUnlockManager.Instance.IsUnlocked(charName);
        item.data = index;
    }

    private void OnItemClick(EventContext context)
    {
        GButton item = context.data as GButton;
        if (item == null) return;

        int index = (int)item.data;
        SelectCharacter(index);
    }

    private void SelectCharacter(int index)
    {
        if (index < 0 || index >= _characterNames.Count) return;

        _selectedCharacter = _characterNames[index];

        var stats = _characters[_selectedCharacter];
        string info = $"HP: {stats.maxHP}\n" +
                      $"攻击: {stats.attackDamage}\n" +
                      $"速度: {stats.moveSpeed}\n" +
                      $"护甲: {stats.armor}";

        string unlockInfo = GetUnlockInfo(_selectedCharacter);
        _view.characterInfo.text = info + "\n\n" + unlockInfo;

        UpdateCanBuy();
    }

    private void UpdateCanBuy()
    {
        if (_selectedCharacter == null) return;

        bool isUnlocked = CharacterUnlockManager.Instance.IsUnlocked(_selectedCharacter);
        bool canUnlock = CharacterUnlockManager.Instance.CanUnlock(_selectedCharacter);

        _view.canBuy.selectedIndex = (!isUnlocked && canUnlock) ? 1 : 0;
    }

    private string GetUnlockInfo(string charName)
    {
        if (CharacterUnlockManager.Instance.IsUnlocked(charName))
            return "已解锁";

        return "解锁条件: " + CharacterUnlockManager.Instance.GetUnlockConditionText(charName);
    }

    private void OnGoBackClick(EventContext context)
    {
        UIManager.Instance.HidePanel<All.ShopPanel>(true);
        UIManager.Instance.ShowPanel<All.MenuPanel, MenuPanelController>("All");
    }

    private void OnConfirmClick(EventContext context)
    {
        if (_selectedCharacter == null) return;

        if (CharacterUnlockManager.Instance.IsUnlocked(_selectedCharacter))
            return;

        if (CharacterUnlockManager.Instance.CanUnlock(_selectedCharacter))
        {
            CharacterUnlockManager.Instance.Unlock(_selectedCharacter);
            InitCharacterList();
            UpdateCanBuy();
        }
    }

    public void Dispose()
    {
        _view.characterShopList.onClickItem.Remove(OnItemClick);
        _view.goBackButton.onClick.Remove(OnGoBackClick);
        _view.confirmButton.onClick.Remove(OnConfirmClick);
    }
}
