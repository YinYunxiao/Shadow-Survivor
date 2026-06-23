using UnityEngine;

public abstract class PassiveSkill
{
    protected Player player;
    public float cooldown;
    public float timer;

    public PassiveSkill(Player player, float cooldown)
    {
        this.player = player;
        this.cooldown = cooldown;
        this.timer = 0f;
    }

    public virtual void Update(float deltaTime)
    {
        timer += deltaTime;
        if (timer >= cooldown)
        {
            timer -= cooldown;
            Execute();
        }

        GameEvents.OnPassiveSkillProgress?.Invoke(timer, cooldown);
    }

    protected abstract void Execute();
}
