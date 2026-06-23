using UnityEngine;

public class Slime : NormalEnemy
{
    protected override void OnEnable()
    {
        // 无特殊逻辑
        base.OnEnable();
        Init("Slime");
    }
}
