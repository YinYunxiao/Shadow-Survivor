using System;

public static class GameEvents
{
    public static Action<int> OnGoldChanged;
    public static Action<int> OnKillCountChanged;
    public static Action<float, float> OnHPChanged;
    public static Action<float, float> OnShieldChanged;
    public static Action<float, float> OnXPChanged;
    public static Action<int> OnLevelUp;
    public static Action<float, float> OnPassiveSkillProgress;
    public static Action<float, float> OnBossHPChanged;
    public static Action OnBossAppear;
    public static Action OnBossDefeated;
    public static Action<string, int> OnSkillSelected;
    public static Action<int> OnWaveChanged;
}
