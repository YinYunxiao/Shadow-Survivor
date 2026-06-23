using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player p) : base(p) { }

    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("IsIdle", true);
    }

    public override void Update()
    {
        base.Update();

        Vector2 moveDir = InputManager.MoveDir;

        if (moveDir.magnitude > 0.1f)
        {
            player.stateMachine.ChangeState(player.MoveState);
            return;
        }

        if (DetectEnemy() && player.CanAttack())
        {
            player.AutoAttack();
        }
    }

    public override void Exit()
    {
        player.animator.SetBool("IsIdle", false);
        base.Exit();
    }

    private bool DetectEnemy()
    {
        float range = player.Stats.attackRange;
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            player.transform.position,
            range,
            LayerMask.GetMask("Enemy")
        );
        for (int i = 0; i < hits.Length; i++)
        {
            float dist = Vector2.Distance(player.transform.position, hits[i].transform.position);
            if (dist <= range + hits[i].bounds.extents.x)
                return true;
        }
        return false;
    }
}