using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlertState : EnemyState
{
    public EnemyAlertState(Enemy enemy) : base(enemy)
    {

    }

    public override void Enter()
    {
        Enemy.FOV.SetAlertFOVData();
        Enemy.FOV.CreateFOV();
    }
}
