using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    List<Enemy> _enemies; // List of Enemies in the scene

    private void Start()
    {
        _enemies = new List<Enemy>();
        EventManager.EventInitialise(EventType.LOSE);    
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.ADD_ENEMY, AddEnemy);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.ADD_ENEMY, AddEnemy);
    }

    // Receives Enemies that are instantiated within the level to keep track of for win condition
    private void AddEnemy(object data)
    {
        // Make sure enemies are being passed in as data
        Enemy enemy = data as Enemy;
        if (enemy == null) return;

        // Assign EnemyManager to enemy then add to list of enemies
        enemy.EnemyManager = this;
        _enemies.Add(enemy);
    }

    public void PlayerCaught()
    {
        EventManager.EventTrigger(EventType.LOSE, null);
    }
}
