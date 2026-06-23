public abstract class EnemyState : CharacterState<Enemy>
{
    protected Enemy enemy;

    public EnemyState(Enemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }
}
