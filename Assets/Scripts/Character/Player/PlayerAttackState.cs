using UnityEngine;

public class PlayerAttackState : PlayerState
{
    protected virtual string AnimatorParam => "IsAttack";

    public PlayerAttackState(Player player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool(AnimatorParam, true);
        FaceNearestEnemy();
        OnAttack();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 moveDir = InputManager.MoveDir;
        float speed = moveDir.magnitude;
        player.animator.SetFloat("Speed", speed);

        if (speed > 0.1f)
        {
            player.playerRoot.transform.position +=
                (Vector3)moveDir * (player.moveSpeed * Time.fixedDeltaTime);
        }
    }

    public override void Exit()
    {
        player.animator.SetBool(AnimatorParam, false);
        base.Exit();
    }

    protected virtual void OnAttack() { }

    protected void FaceNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            player.transform.position,
            player.Stats.attackRange,
            LayerMask.GetMask("Enemy")
        );

        if (hits.Length == 0)
        {
            OnNoEnemy();
            return;
        }

        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(player.transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }

        if (nearest != null)
        {
            Vector2 direction = (nearest.position - player.transform.position).normalized;
            player.SetDirection(direction);
        }
    }

    protected virtual void OnNoEnemy() { }
}
