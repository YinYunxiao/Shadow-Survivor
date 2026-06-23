public abstract class PlayerState : CharacterState<Player>
{
    protected Player player;
    public PlayerState(Player player) : base(player)
    {
        this.player = player;
    }
}