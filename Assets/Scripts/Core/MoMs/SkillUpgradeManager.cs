using System.Collections.Generic;
using UnityEngine;

public class SkillUpgradeManager : SingleBase<SkillUpgradeManager>
{
    private SkillUpgradeManager() { }

    private Dictionary<string, SkillUpgradeDef> allSkills = new Dictionary<string, SkillUpgradeDef>();
    private Dictionary<string, int> ownedSkills = new Dictionary<string, int>();
    private List<SkillUpgradeDef> skillPool = new List<SkillUpgradeDef>();
    private bool isLoaded = false;

    private void LoadConfig()
    {
        if (isLoaded) return;

        var data = JsonManager.Load<SkillUpgradeConfigData>(
            "Data/SkillUpgradeConfig",
            JsonManager.JsonType.LitJson,
            JsonManager.JsonLocation.Resources
        );

        if (data != null && data.skills != null)
        {
            foreach (var skill in data.skills)
            {
                allSkills[skill.id] = skill;
                skillPool.Add(skill);
            }
        }

        isLoaded = true;
    }

    public void Reset()
    {
        ownedSkills.Clear();
    }

    public List<SkillUpgradeDef> GetSelection(int count)
    {
        LoadConfig();

        List<SkillUpgradeDef> result = new List<SkillUpgradeDef>();
        List<SkillUpgradeDef> candidates = new List<SkillUpgradeDef>(skillPool);

        for (int i = 0; i < count && candidates.Count > 0; i++)
        {
            SkillRarity rarity = RollRarity();
            List<SkillUpgradeDef> rarityPool = candidates.FindAll(s => s.rarity == rarity);

            if (rarityPool.Count == 0)
            {
                rarityPool = candidates;
            }

            int idx = Random.Range(0, rarityPool.Count);
            result.Add(rarityPool[idx]);
            candidates.Remove(rarityPool[idx]);
        }

        return result;
    }

    public void SelectSkill(string skillId)
    {
        if (!allSkills.ContainsKey(skillId)) return;

        if (ownedSkills.ContainsKey(skillId))
        {
            ownedSkills[skillId]++;
        }
        else
        {
            ownedSkills[skillId] = 1;
        }

        GameEvents.OnSkillSelected?.Invoke(skillId, ownedSkills[skillId]);
    }

    public int GetSkillLevel(string skillId)
    {
        return ownedSkills.ContainsKey(skillId) ? ownedSkills[skillId] : 0;
    }

    public float GetTotalValue(string skillId)
    {
        if (!allSkills.ContainsKey(skillId) || !ownedSkills.ContainsKey(skillId))
            return 0;

        var def = allSkills[skillId];
        int level = ownedSkills[skillId];
        return def.baseValue + def.valuePerLevel * (level - 1);
    }

    public bool HasSkill(string skillId)
    {
        return ownedSkills.ContainsKey(skillId);
    }

    public Dictionary<string, int> GetAllOwnedSkills()
    {
        return new Dictionary<string, int>(ownedSkills);
    }

    public SkillUpgradeDef GetSkillDef(string skillId)
    {
        return allSkills.ContainsKey(skillId) ? allSkills[skillId] : null;
    }

    private SkillRarity RollRarity()
    {
        float roll = Random.Range(0f, 1f);
        if (roll < 0.05f) return SkillRarity.Legend;
        if (roll < 0.15f) return SkillRarity.Epic;
        if (roll < 0.50f) return SkillRarity.Rare;
        return SkillRarity.Common;
    }
}
