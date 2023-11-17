using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(Enemy enemy) : base(enemy) {}

    public override void Enter()
    {
        Debug.Log("Entering Patrol State");

        Enemy.FOV.SetPatrolFOVData();
        Enemy.FOV.CreateFOV();
        Enemy.EnemyBehaviour.ResetBehaviour();
    }
    public override void LogicUpdate()
    {
        // If player is spotted, transition to AlertState
        if (Enemy.FOV.PlayerSpotted && Enemy.Player.regionState != 3)
        {
            Enemy.StateMachine.ChangeState(Enemy.AlertState);
        }
        else
        {
            Enemy.EnemyBehaviour.UpdateLogicBehaviour();
        }
    }

    public override void PhysicsUpdate()
    {
        Enemy.EnemyBehaviour.UpdatePhysicsBehaviour();
    }

    public override void Exit()
    {
        Debug.Log("Leaving Patrol State");
        Enemy.RB.velocity = Vector2.zero;
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
