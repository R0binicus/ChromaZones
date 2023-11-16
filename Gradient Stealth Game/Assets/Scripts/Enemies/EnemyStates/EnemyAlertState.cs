using UnityEngine;

public class EnemyAlertState : EnemyState
{
    private float _timer;

    public EnemyAlertState(Enemy enemy) : base(enemy)
    {

    }

    public override void Enter()
    {
        Debug.Log("Entering Enemy Alert State");

        Enemy.FOV.IsActive(true);
        Enemy.FOV.SetAlertFOVData();
        Enemy.FOV.CreateFOV();
        _timer = 0;
    }

    public override void LogicUpdate()
    {
        _timer += Time.deltaTime;

        if (_timer > Enemy.DetectionTime)
        {
            // If player is still within FOV, chase
            if (Enemy.FOV.PlayerSpotted)
            {
                Enemy.StateMachine.ChangeState(Enemy.ChaseState);
            }
            else
            {
                Enemy.StateMachine.ChangeState(Enemy.PatrolState);
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("Leaving Enemy Alert State");
    }
}
