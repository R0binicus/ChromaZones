using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCaughtState : EnemyState
{
    public EnemyCaughtState(Enemy enemy) : base(enemy)
    {

    }

    public override void Enter()
    {
        Debug.Log("Entered Enemy Caught State. You lose.");
    }
}
