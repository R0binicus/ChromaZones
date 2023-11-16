using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(Enemy enemy) : base(enemy) {}

    public override void LogicUpdate()
    {
        // If player is spotted, transition to AlertState
        if (Enemy.FOV.PlayerSpotted)
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