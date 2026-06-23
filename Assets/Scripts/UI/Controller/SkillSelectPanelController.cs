using FairyGUI;
using System.Collections.Generic;

public class SkillSelectPanelController : System.IDisposable
{
    private All.SkillSelectPanel _view;
    private List<SkillUpgradeDef> currentSelection;

    public SkillSelectPanelController(All.SkillSelectPanel view)
    {
        _view = view;
        Init();
    }

    private void Init()
    {
        _view.skillList.SetVirtual();
        _view.skillList.itemRenderer = OnRenderSkillItem;
        _view.skillList.numItems = 0;
    }

    public void ShowSelection(List<SkillUpgradeDef> skills)
    {
        currentSelection = skills;
        _view.skillList.numItems = skills.Count;
    }

    private void OnRenderSkillItem(int index, GObject obj)
    {
        if (currentSelection == null || index >= currentSelection.Count) return;

        var skill = currentSelection[index];
        int currentLevel = SkillUpgradeManager.Instance.GetSkillLevel(skill.id);

        string rarityTag = GetRarityTag(skill.rarity);
        string levelText = currentLevel > 0 ? $"Lv.{currentLevel}" : "NEW";

        string desc = skill.description;
        if (currentLevel > 0)
        {
            float totalValue = SkillUpgradeManager.Instance.GetTotalValue(skill.id);
            float nextValue = skill.baseValue + skill.valuePerLevel * currentLevel;
            desc += $"\n当前+{totalValue:P0} → 下一级+{nextValue:P0}";
        }
        else
        {
            desc += $"\n效果+{skill.baseValue:P0}";
        }
        GButton button = obj.asButton;
        button.title = $"{rarityTag} {skill.name} [{levelText}]\n{desc}";

        button.data = skill.id;
        button.onClick.Clear();
        button.onClick.Add(() => OnSkillSelected(skill.id));
    }

    private string GetRarityTag(SkillRarity rarity)
    {
        switch (rarity)
        {
            case SkillRarity.Common: return "[普通]";
            case SkillRarity.Rare: return "[稀有]";
            case SkillRarity.Epic: return "[史诗]";
            case SkillRarity.Legend: return "[传说]";
            default: return "";
        }
    }

    private void OnSkillSelected(string skillId)
    {
        SkillUpgradeManager.Instance.SelectSkill(skillId);
        GameManager.Instance.ResumeGame();
        UIManager.Instance.HidePanel<All.SkillSelectPanel>(true);
    }

    public void Dispose()
    {
        _view.skillList.numItems = 0;
    }
}
