using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyChaseState : EnemyState
{
    Vector2 _moveDir;
    float _timer;
    float _accelTimer;

    public EnemyChaseState(Enemy enemy) : base(enemy) {}

    public override void Enter()
    {
        Debug.Log("Entering Enemy Chase State");
        Enemy.chaseSound.Play();
        _timer = 0;
        _accelTimer = 0;
        Enemy.FOV.IsActive(false);
    }

    public override void LogicUpdate()
    {
        _moveDir = (Enemy.Player.transform.position - Enemy.transform.position).normalized;

        Quaternion fullRotatation = Quaternion.LookRotation(Enemy.transform.forward, _moveDir);
        Quaternion lookRot = Quaternion.identity;
        lookRot.eulerAngles = new Vector3(0, 0, fullRotatation.eulerAngles.z);
        Enemy.transform.rotation = Quaternion.RotateTowards(Enemy.transform.rotation, lookRot, Enemy.ChaseRotation * Time.deltaTime);

        // If player is in their own region
        if (Enemy.Player.regionState == 3)
        {
            // If player has been hidden in their region for a certain amount of time
            // Change to AlertState
            if (_timer > Enemy.HiddenTime)
            {
                _timer = 0;
                Enemy.Agent.ResetPath();
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
        Enemy.Agent.SetDestination(Enemy.Player.transform.position);
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
