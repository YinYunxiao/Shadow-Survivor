using UnityEngine;

public class DragonLiftOffState : EnemyState
{
    private Vector3 startPos;
    private Vector3 targetPos;
    private float elapsed;
    private const float DURATION = 1.0f;
    private const float FLY_HEIGHT = 3f;

    public DragonLiftOffState(Enemy e) : base(e) { }

    public override void Enter()
    {
        base.Enter();
        elapsed = 0f;
        startPos = enemy.transform.position;
        enemy.animator.SetBool("IsLiftOff", true);

        var dragon = enemy as Dragon;
        if (dragon != null) dragon.SetInvincible(true);

        Transform player = GameManager.Instance.PlayerTransform;
        if (player != null)
            targetPos = player.position;
        else
            targetPos = startPos;
    }

    public override void Update()
    {
        base.Update();
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / DURATION);

        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
        currentPos.z = -Mathf.Sin(t * Mathf.PI) * FLY_HEIGHT;
        enemy.transform.position = currentPos;

        if (t >= 1f)
        {
            var dragon = enemy as Dragon;
            if (dragon != null)
                dragon.stateMachine.ChangeState(dragon.LandingState);
        }
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsLiftOff", false);
        base.Exit();
    }
}
