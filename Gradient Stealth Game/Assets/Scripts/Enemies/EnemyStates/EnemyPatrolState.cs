using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(Enemy enemy) : base(enemy) {}

    public override void Enter()
    {
        Debug.Log("Entering Patrol State");
        Enemy.FOV.SetFOVData(Enemy.PatrolFOVData);
        Enemy.FOV.CreateFOV();
        Enemy.EnemyBehaviour.ResetBehaviour();
        Enemy.DetectedOnce = false;
    }
    public override void LogicUpdate()
    {
        // If player is spotted, transition to AlertState
        if (Enemy.FOV.PlayerSpotted && Enemy.Player.regionState != 3)
        {
            Enemy.alertSound.Play();
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
        // If player has attacked Enemy and Enemy is not in their region, notify the EnemyManager
        if (collision.transform.CompareTag("Player") && Enemy.regionState != 1)
        {
            Enemy.EnemyManager.PlayerAttacked(Enemy);
        }
        // Else if player has attacked Enemy and Enemy is in their region, call game over
        else if (collision.transform.CompareTag("Player") && Enemy.regionState == 1)
        {
            Enemy.StateMachine.ChangeState(Enemy.CaughtState);
        }
    }
}
