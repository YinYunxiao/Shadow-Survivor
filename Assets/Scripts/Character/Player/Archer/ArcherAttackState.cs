public class ArcherAttackState : PlayerAttackState
{
    public ArcherAttackState(Archer a) : base(a) { }

    protected override void OnAttack()
    {
        (player as Archer)?.ShootArrows();
    }
}
