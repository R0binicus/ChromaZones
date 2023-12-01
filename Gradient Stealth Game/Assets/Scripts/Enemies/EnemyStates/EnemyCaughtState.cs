using System.Numerics;
using UnityEngine;

public class EnemyCaughtState : EnemyState
{
    public EnemyCaughtState(Enemy enemy) : base(enemy)
    {

    }

    public override void Enter()
    {
        Enemy.Agent.isStopped = true;
        Enemy.EnemyManager.PlayerCaught();
    }
}
