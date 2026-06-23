using FairyGUI;
using System.Collections.Generic;

public class PlayerInfoPanelController : System.IDisposable
{
    private All.PlayerInfoPanel _view;

    public PlayerInfoPanelController(All.PlayerInfoPanel view)
    {
        _view = view;
        Init();
    }

    private void Init()
    {
        _view.goBackButton.onClick.Add(OnCloseClick);
        _view.buffList.SetVirtual();
        _view.buffList.itemRenderer = OnRenderBuffItem;
        Refresh();
    }

    public void Refresh()
    {
        RefreshBaseInfo();
        RefreshBuffList();
    }

    private void RefreshBaseInfo()
    {
        var player = GameManager.Instance.PlayerTransform?.GetComponentInChildren<Player>();
        if (player == null) return;

        var stats = player.Stats;
        string info = $"攻击力: {stats.attackDamage:F1}\n"
                    + $"攻速: {stats.attackCooldown:F2}s\n"
                    + $"生命: {player.currentHP:F0}/{stats.maxHP:F0}\n"
                    + $"护甲: {stats.armor:F0}\n"
                    + $"移速: {stats.moveSpeed:F2}\n"
                    + $"暴击率: {stats.critChance:P0}\n"
                    + $"暴击伤害: {stats.critDamage:P0}\n"
                    + $"攻击范围: {stats.attackRange:F1}";

        _view.baseInfoText.text = info;
    }

    private List<BuffInfo> GetActiveBuffs()
    {
        var player = GameManager.Instance.PlayerTransform?.GetComponentInChildren<Player>();
        if (player == null) return new List<BuffInfo>();

        var buffs = new List<BuffInfo>();
        var buffSystem = player.buffSystem;

        if (buffSystem == null) return buffs;

        var skills = SkillUpgradeManager.Instance.GetAllOwnedSkills();
        foreach (var kvp in skills)
        {
            var def = SkillUpgradeManager.Instance.GetSkillDef(kvp.Key);
            if (def == null) continue;

            buffs.Add(new BuffInfo
            {
                name = def.name,
                level = kvp.Value,
                description = def.description,
                value = SkillUpgradeManager.Instance.GetTotalValue(kvp.Key)
            });
        }

        return buffs;
    }

    private List<BuffInfo> currentBuffs;

    private void RefreshBuffList()
    {
        currentBuffs = GetActiveBuffs();
        _view.buffList.numItems = currentBuffs.Count;
    }

    private void OnRenderBuffItem(int index, GObject obj)
    {
        if (currentBuffs == null || index >= currentBuffs.Count) return;

        var buff = currentBuffs[index];
        var item = obj as GComponent;
        if (item == null) return;

        GTextField titleField = item.GetChild("title") as GTextField;
        if (titleField != null)
        {
            titleField.text = $"{buff.name} Lv.{buff.level}\n{buff.description} +{buff.value:P0}";
        }
    }

    private void OnCloseClick(EventContext context)
    {
        UIManager.Instance.HidePanel<All.PlayerInfoPanel>(true);
    }

    public void Dispose()
    {
        _view.buffList.numItems = 0;
    }

    private class BuffInfo
    {
        public string name;
        public int level;
        public string description;
        public float value;
    }
}
