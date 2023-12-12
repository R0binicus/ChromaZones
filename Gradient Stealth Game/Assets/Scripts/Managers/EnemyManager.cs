using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Internal Data
    List<Enemy> _enemies; // List of Enemies in the level

    // GameObject References
    [SerializeField] private Player _player;
    [SerializeField] private NavMeshPlus.Components.NavMeshSurface surfaceSingle;

    private void Awake()
    {
        _enemies = new List<Enemy>();

        EventManager.EventInitialise(EventType.LOSE);
        EventManager.EventInitialise(EventType.ASSIGNMENT_CODE_TRIGGER);
        EventManager.EventInitialise(EventType.AREA_CHASE_TRIGGER);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.ADD_ENEMY, AddEnemy);
        EventManager.EventSubscribe(EventType.AREA_CHASE_TRIGGER, AlertNearbyEnemies);
        EventManager.EventSubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
        EventManager.EventSubscribe(EventType.LEVEL_STARTED, LevelStart);
        EventManager.EventSubscribe(EventType.LEVEL_ENDED, LevelStart);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.ADD_ENEMY, AddEnemy);
        EventManager.EventUnsubscribe(EventType.AREA_CHASE_TRIGGER, AlertNearbyEnemies);
        EventManager.EventUnsubscribe(EventType.LEVEL_STARTED, LevelStart);
        EventManager.EventUnsubscribe(EventType.LEVEL_ENDED, LevelStart);
    }

    private void Start()
    {
        surfaceSingle.BuildNavMesh();
    }

    // Called once a level is loaded
    public void LevelStart(object data)
    {
        surfaceSingle.BuildNavMesh();
    }

    // Called when the level is about to be unloaded
    public void LevelEnd(object data)
    {
        _enemies.Clear();
    }

    private void AssignmentCodeHandler(object data)
    {
        //surfaceSingle.BuildNavMesh();
    }

    // Receives Enemies that are instantiated within the level to keep track of for win condition
    private void AddEnemy(object data)
    {
        // Make sure enemies are being passed in as data
        Enemy enemy = data as Enemy;
        if (enemy == null) return;

        // Assign EnemyManager and Player to enemy then add to list of enemies
        enemy.EnemyManager = this;
        enemy.Player = _player;
        _enemies.Add(enemy);
    }

    public void AlertNearbyEnemies(object data)
    {
        if (data == null)
        {
            Debug.Log("AlertNearbyEnemies is null");
        }

        var data2 = (AlertData)data;
        var centre = data2.Centre;
        var alertOthersRadius = data2.AlertOthersRadius;

        foreach (Enemy enemy in _enemies)
        {
            if ((centre - enemy.transform.position).magnitude < alertOthersRadius)
            {
                enemy.CheckWalls();
            }
        }
    }

    public void PlayerCaught()
    {
        // Change all enemies to caught state
        foreach (Enemy enemy in _enemies)
        {
            if (enemy.gameObject.activeInHierarchy == true && enemy.StateMachine.CurrentState != enemy.CaughtState)
            {
                enemy.StateMachine.ChangeState(enemy.CaughtState);
            }
        }

        EventManager.EventTrigger(EventType.LOSE, null);
    }

    // Deactivate Enemy that was attacked then check to see how many enemies are left
    public void PlayerAttacked(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        CheckEnemiesLeft();
    }

    // Checks to see how many enemies are left
    private void CheckEnemiesLeft()
    {
        foreach (Enemy enemy in _enemies)
        {
            // If an enemy is still active, do not end game
            if (enemy.gameObject.activeInHierarchy == true)
            {
                return;
            }
        }

        // Signal game won
        EventManager.EventTrigger(EventType.WIN, null);
    }
}
