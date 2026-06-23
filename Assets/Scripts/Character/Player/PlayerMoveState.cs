using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player p) : base(p) { }

    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("IsMove", true);
    }

    public override void Update()
    {
        base.Update();

        Vector2 moveDir = InputManager.MoveDir;

        if (moveDir.magnitude < 0.1f)
        {
            player.stateMachine.ChangeState(player.IdleState);
            return;
        }

        if (DetectEnemy() && player.CanAttack())
        {
            player.AutoAttack();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Vector2 moveDir = InputManager.MoveDir;

        float standardAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        if (standardAngle < 0) standardAngle += 360;

        float angle = (90 - standardAngle + 360) % 360;
        player.animator.SetFloat("Angle", angle);
        player.animator.SetFloat("Speed", moveDir.magnitude);

        player.playerRoot.transform.position +=
        (Vector3)moveDir * (player.moveSpeed * Time.fixedDeltaTime);
    }

    public override void Exit()
    {
        player.animator.SetBool("IsMove", false);
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