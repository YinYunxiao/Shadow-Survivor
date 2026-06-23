using UnityEngine;

public class Skeleton : NormalEnemy
{
    protected override void OnEnable()
    {
        // 无特殊逻辑
        base.OnEnable();
        Init("Skeleton");
    }
}
