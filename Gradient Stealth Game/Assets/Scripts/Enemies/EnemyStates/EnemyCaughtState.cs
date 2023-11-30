using System.Numerics;
using UnityEngine;

public class EnemyCaughtState : EnemyState
{
    public EnemyCaughtState(Enemy enemy) : base(enemy)
    {

    }

    public override void Enter()
    {
        Debug.Log("Entered Enemy Caught State. You lose.");
        Enemy.Agent.isStopped = true;
        Enemy.EnemyManager.PlayerCaught();
    }
}
