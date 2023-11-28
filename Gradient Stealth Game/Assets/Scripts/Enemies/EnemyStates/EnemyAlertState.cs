using Unity.VisualScripting;
using UnityEngine;

public class EnemyAlertState : EnemyState
{
    private float _timer;

    public EnemyAlertState(Enemy enemy) : base(enemy) {}

    public override void Enter()
    {
        Debug.Log("Entering Enemy Alert State");

        Enemy.FOV.IsActive(true);
        Enemy.FOV.SetFOVData(Enemy.AlertFOVData);
        Enemy.FOV.CreateFOV();
        _timer = 0;
    }

    public override void LogicUpdate()
    {
        _timer += Time.deltaTime;
        
        if (_timer > Enemy.DetectionTime)
        {
            // If Player is still within FOV and not hiding, chase
            if (Enemy.FOV.PlayerSpotted && Enemy.Player.RegionState != 3)
            {
                Enemy.StateMachine.ChangeState(Enemy.ChaseState);
            }
            else
            {
                Enemy.deAlertSound.Play();
                Enemy.StateMachine.ChangeState(Enemy.PatrolState);
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("Leaving Enemy Alert State");
    }

    // If player touches Enemy whilst in hiding
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if Player
        if (collision.transform.CompareTag("Player"))
        {
            Enemy.StateMachine.ChangeState(Enemy.CaughtState);
        }
    }
}
