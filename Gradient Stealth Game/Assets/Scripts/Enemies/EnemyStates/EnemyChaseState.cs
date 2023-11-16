using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(Enemy enemy) : base(enemy)
    {

    }

    public override void Enter()
    {
        Debug.Log("Entering Enemy Chase State");

        Enemy.FOV.IsActive(false);
    }
}
