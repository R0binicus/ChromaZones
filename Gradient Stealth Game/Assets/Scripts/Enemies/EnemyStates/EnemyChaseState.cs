using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyChaseState : EnemyState
{
    Vector2 _moveDir;

    public EnemyChaseState(Enemy enemy) : base(enemy) {}

    public override void Enter()
    {
        Enemy.SetChaseSpeed();
        Enemy.ActivateAllFOVs(false);
        EventManager.EventTrigger(EventType.SFX, Enemy.SoundEnemyChase);
    }

    public override void Exit()
    {
        Enemy.Agent.ResetPath();
        Enemy.RB.velocity = Vector2.zero;
    }

    public override void LogicUpdate()
    {
        _moveDir = (Enemy.Player.transform.position - Enemy.transform.position).normalized;

        Quaternion fullRotatation = Quaternion.LookRotation(Enemy.transform.forward, _moveDir);
        Quaternion lookRot = Quaternion.identity;
        lookRot.eulerAngles = new Vector3(0, 0, fullRotatation.eulerAngles.z);
        Enemy.transform.rotation = Quaternion.RotateTowards(Enemy.transform.rotation, lookRot, Enemy.ChaseRotation * Time.deltaTime);

        // If player is in their own region
        if (Enemy.Player.RegionState == 3)
        {
            // If player has been hidden in their region
            // Change to AlertState
            Enemy.StateMachine.ChangeState(Enemy.AlertState);
        }
    }

    public override void PhysicsUpdate()
    {
        Enemy.Agent.SetDestination(Enemy.Player.transform.position);
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if Player
        if (collision.transform.CompareTag("Player"))
        {
            Enemy.Caught();
        }
    }
}
