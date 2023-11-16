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
            Enemy.EnemyBehaviour.ExecuteBehaviour();
        }
    }

    public override void Exit()
    {
        Debug.Log("Leaving Patrol State");
    }
}
