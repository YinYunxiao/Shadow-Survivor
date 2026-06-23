using UnityEngine;

public class KnightMeleeState : PlayerAttackState
{
    private const string BUFF_ID = "knight_melee";

    protected override string AnimatorParam => "IsMelee";

    public KnightMeleeState(Knight k) : base(k) { }

    public override void Enter()
    {
        player.buffSystem.AddBuff(new Buff
        {
            id = BUFF_ID,
            duration = 999f,
            moveSpeedMult = 0.5f
        });
        base.Enter();
    }

    public override void Exit()
    {
        player.buffSystem.RemoveBuff(BUFF_ID);
        base.Exit();
    }

    protected override void OnAttack()
    {
        (player as Knight)?.OnMeleeHit();
    }

    protected override void OnNoEnemy()
    {
        player.stateMachine.ChangeState(player.MoveState);
    }
}
