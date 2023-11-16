using UnityEngine;

public class EnemyChaseState : EnemyState
{
    Vector2 _moveDir;
    float _timer;
    float _accelTimer;

    public EnemyChaseState(Enemy enemy) : base(enemy)
    {

    }

    public override void Enter()
    {
        Debug.Log("Entering Enemy Chase State");
        _timer = 0;
        _accelTimer = 0;
        Enemy.FOV.IsActive(false);
    }

    public override void LogicUpdate()
    {
        _moveDir = (Enemy.Player.transform.position - Enemy.transform.position).normalized;

        // If player is in their own region
        if (Enemy.Player.regionState == 3)
        {
            // If player has been hidden in their region for a certain amount of time
            // Change to AlertState
            if (_timer > Enemy.HiddenTime)
            {
                _timer = 0;
                Enemy.RB.velocity = Vector2.zero;
                Enemy.StateMachine.ChangeState(Enemy.AlertState);
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }

        _accelTimer += Time.deltaTime;
    }

    public override void PhysicsUpdate()
    {
        Enemy.RB.velocity = _moveDir * Enemy.Velocity.Evaluate(_accelTimer);
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if Player
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            Enemy.StateMachine.ChangeState(Enemy.CaughtState);
        }
    }
}
