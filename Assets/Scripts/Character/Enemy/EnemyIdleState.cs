using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(Enemy e) : base(e) { }

    public override void Enter()
    {
        base.Enter();
        enemy.animator.SetBool("IsIdle", true);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsStunned) return;

        if (DetectTarget())
        {
            enemy.stateMachine.ChangeState(enemy.MoveState);
        }
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsIdle", false);
        base.Exit();
    }

    private bool DetectTarget()
    {
        if (enemy.isConverted)
        {
            foreach (var target in Enemy.ActiveEnemies)
            {
                if (target == null || target.isDead) continue;
                float dist = Vector2.Distance(enemy.transform.position, target.transform.position);
                if (dist <= enemy.Stats.detectRange)
                    return true;
            }
            return false;
        }
        else
        {
            foreach (var ally in Enemy.AlliedEnemies)
            {
                if (ally == null || ally.isDead) continue;
                float allyDist = Vector2.Distance(enemy.transform.position, ally.transform.position);
                if (allyDist <= enemy.Stats.detectRange)
                    return true;
            }

            if (GameManager.Instance.PlayerTransform == null) return false;
            float dist = Vector2.Distance(enemy.transform.position, GameManager.Instance.PlayerTransform.position);
            return dist <= enemy.Stats.detectRange;
        }
    }
}
