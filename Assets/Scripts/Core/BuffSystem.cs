using System;
using System.Collections.Generic;

[Serializable]
public class Buff
{
    public string id;
    public float duration;
    public float moveSpeedMult = 1f;
    public float damageMult = 1f;
    public float armorAdd = 0f;

    [NonSerialized] public float remainingTime;
}

public class BuffSystem
{
    private readonly Dictionary<string, Buff> activeBuffs = new Dictionary<string, Buff>();
    private Action onBuffsChanged;

    public BuffSystem(Action onBuffsChanged = null)
    {
        this.onBuffsChanged = onBuffsChanged;
    }

    public void AddBuff(Buff buff)
    {
        if (activeBuffs.ContainsKey(buff.id))
        {
            activeBuffs[buff.id] = buff;
        }
        else
        {
            activeBuffs.Add(buff.id, buff);
        }
        buff.remainingTime = buff.duration;
        onBuffsChanged?.Invoke();
    }

    public void RemoveBuff(string buffId)
    {
        if (activeBuffs.Remove(buffId))
        {
            onBuffsChanged?.Invoke();
        }
    }

    public void Update(float deltaTime)
    {
        List<string> expired = null;

        foreach (var kvp in activeBuffs)
        {
            kvp.Value.remainingTime -= deltaTime;
            if (kvp.Value.remainingTime <= 0)
            {
                if (expired == null) expired = new List<string>();
                expired.Add(kvp.Key);
            }
        }

        if (expired != null)
        {
            foreach (var id in expired)
            {
                activeBuffs.Remove(id);
            }
            onBuffsChanged?.Invoke();
        }
    }

    public float GetMoveSpeedMult()
    {
        float mult = 1f;
        foreach (var buff in activeBuffs.Values)
        {
            mult *= buff.moveSpeedMult;
        }
        return mult;
    }

    public float GetDamageMult()
    {
        float mult = 1f;
        foreach (var buff in activeBuffs.Values)
        {
            mult *= buff.damageMult;
        }
        return mult;
    }

    public float GetArmorAdd()
    {
        float add = 0f;
        foreach (var buff in activeBuffs.Values)
        {
            add += buff.armorAdd;
        }
        return add;
    }

    public void Clear()
    {
        activeBuffs.Clear();
        onBuffsChanged?.Invoke();
    }
}
