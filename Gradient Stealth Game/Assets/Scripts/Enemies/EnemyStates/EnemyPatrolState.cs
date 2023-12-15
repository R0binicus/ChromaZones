using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(Enemy enemy) : base(enemy) {}

    public override void Enter()
    {
        Enemy.FOVsIsActive(true, false);
        Enemy.SetFOVsData(Enemy.PatrolFOVData);
        Enemy.CreateFOVs();
        Enemy.EnemyBehaviour.ResetBehaviour();
    }
    public override void LogicUpdate()
    {
        // If player is spotted, transition to AlertState
        if (Enemy.PlayerSpotted() && Enemy.Player.RegionState != 3)
        {
            Enemy.StateMachine.ChangeState(Enemy.ChaseState);
            Enemy.EnemyAlertNearbyEnemies();
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

    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        // If player has attacked Enemy and Enemy is not in their region, notify the EnemyManager
        if (collision.transform.CompareTag("Player") && Enemy.RegionState != 1 && !Enemy.InvulnerableState)
        {
            Enemy.EnemyManager.PlayerAttacked(Enemy);
        }
        else if (collision.transform.CompareTag("Player") && Enemy.RegionState != 1 && Enemy.InvulnerableState)
        {
            Enemy.Caught();
        }
        // Else if player has attacked Enemy and Enemy is in their region, call game over
        else if (collision.transform.CompareTag("Player") && Enemy.RegionState == 1)
        {
            Enemy.Caught();
        }
    }
}
