using UnityEngine;

public class PaladinMeleeState : PlayerAttackState
{
    private const string BUFF_ID = "paladin_melee";

    protected override string AnimatorParam => "IsMelee";

    public PaladinMeleeState(Paladin p) : base(p) { }

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

    protected override void OnNoEnemy()
    {
        player.stateMachine.ChangeState(player.MoveState);
    }
}
