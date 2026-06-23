using UnityEngine;

public class DeadKnightMeleeState : PlayerAttackState
{
    private const string BUFF_ID = "deadknight_melee";

    protected override string AnimatorParam => "IsMelee";

    public DeadKnightMeleeState(DeadKnight d) : base(d) { }

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
        (player as DeadKnight)?.OnMeleeHit();
    }

    protected override void OnNoEnemy()
    {
        player.stateMachine.ChangeState(player.MoveState);
    }
}
