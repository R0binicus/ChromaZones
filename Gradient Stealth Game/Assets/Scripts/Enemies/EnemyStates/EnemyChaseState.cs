using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    Vector2 _moveDir;
    float _timer;

    public EnemyChaseState(Enemy enemy) : base(enemy)
    {

    }

    public override void Enter()
    {
        Debug.Log("Entering Enemy Chase State");
        _timer = 0;
        Enemy.FOV.IsActive(false);
    }

    public override void LogicUpdate()
    {
        _moveDir = (Enemy.Player.transform.position - Enemy.transform.position).normalized;
        _timer += Time.deltaTime;
    }

    public override void PhysicsUpdate()
    {
        Enemy.RB.velocity = _moveDir * Enemy.Velocity.Evaluate(_timer);
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
