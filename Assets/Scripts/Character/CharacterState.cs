public abstract class CharacterState<T> : IState where T : Character
{
    protected T character;
    public CharacterState(T character)
    {
        this.character = character;
    }

    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void FixedUpdate() { }

    public virtual void Update() { }
}