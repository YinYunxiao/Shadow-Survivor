using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    private Character character;

    private void Awake()
    {
        character = GetComponentInParent<Character>();
    }

    public void OnMeleeHit()
    {
        character?.OnMeleeHit();
    }

    public void OnRangedHit()
    {
        character?.OnRangedHit();
    }

    public void OnAoeHit()
    {
        character?.OnAoeHit();
    }

    public void AttackOver()
    {
        character?.AttackOver();
    }

    public void DeathOver()
    {
        character?.DeathOver();
    }
}
