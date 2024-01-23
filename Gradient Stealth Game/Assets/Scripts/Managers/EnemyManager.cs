using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Internal Data
    List<Enemy> _enemies; // List of Enemies in the level

    [Header("Sounds")]
    [SerializeField] private Sound _soundEnemyDeath;
    [SerializeField] private Sound _soundPlayerWin;
    [SerializeField] private Sound _soundPlayerLose;

    [Header("GameObject References")]
    [SerializeField] private Player _player;
    [SerializeField] private NavMeshPlus.Components.NavMeshSurface surfaceSingle;
    [Header("Debugging")]
    [SerializeField] private bool _killAllEnemies;

    private void Awake()
    {
        _enemies = new List<Enemy>();

        EventManager.EventInitialise(EventType.LOSE);
        EventManager.EventInitialise(EventType.ASSIGNMENT_CODE_TRIGGER);
        EventManager.EventInitialise(EventType.AREA_CHASE_TRIGGER);
        EventManager.EventInitialise(EventType.REBUILD_NAVMESH);
        EventManager.EventInitialise(EventType.DEBUG_GAME);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.ADD_ENEMY, AddEnemy);
        EventManager.EventSubscribe(EventType.AREA_CHASE_TRIGGER, AlertNearbyEnemies);
        EventManager.EventSubscribe(EventType.REBUILD_NAVMESH, RebuildNavMesh);
        EventManager.EventSubscribe(EventType.INIT_PLAYER, PlayerInitHandler);
        EventManager.EventSubscribe(EventType.LEVEL_STARTED, LevelStart);
        EventManager.EventSubscribe(EventType.LEVEL_ENDED, LevelEnd);
        EventManager.EventSubscribe(EventType.KILL_ALL_ENEMIES, KillAllEnemies);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.ADD_ENEMY, AddEnemy);
        EventManager.EventUnsubscribe(EventType.AREA_CHASE_TRIGGER, AlertNearbyEnemies);
        EventManager.EventUnsubscribe(EventType.REBUILD_NAVMESH, RebuildNavMesh);
        EventManager.EventUnsubscribe(EventType.INIT_PLAYER, PlayerInitHandler);
        EventManager.EventUnsubscribe(EventType.LEVEL_STARTED, LevelStart);
        EventManager.EventUnsubscribe(EventType.LEVEL_ENDED, LevelEnd);
        EventManager.EventUnsubscribe(EventType.KILL_ALL_ENEMIES, KillAllEnemies);
    }

    private void Start()
    {
        EventManager.EventTrigger(EventType.DEBUG_GAME, _killAllEnemies);
        
        RebuildNavMesh(null);
    }

    // Called once a level is loaded
    public void LevelStart(object data)
    {
        RebuildNavMesh(null);
    }

    // Called when the level is about to be unloaded
    public void LevelEnd(object data)
    {
        _enemies.Clear();
    }

    public void PlayerInitHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("Player has not been assigned!");
        }

        _player = (Player)data;
        AssignPlayerToEnemy();
    }

    public void RebuildNavMesh(object data)
    {
        surfaceSingle.BuildNavMesh();
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

    // To assign player to enemies that have already been added to the enemy list before the player init
    // event was received
    private void AssignPlayerToEnemy()
    {
        foreach (Enemy enemy in _enemies)
        {
            enemy.Player = _player;
        }
    }

    public void AlertNearbyEnemies(object data)
    {
        if (data == null)
        {
            Debug.Log("AlertNearbyEnemies is null");
        }

        var data2 = (AlertData)data;
        var centre = data2.Centre;
        var type = data2.Type;
        var alertOthersRadius = data2.AlertOthersRadius;

        foreach (Enemy enemy in _enemies)
        {
            float magnitude = (centre - enemy.transform.position).magnitude;
            if (magnitude < alertOthersRadius)
            {
                if (type == 0)
                {
                    enemy.CheckWalls(magnitude, centre);
                }
                else
                {
                    enemy.CheckWallsProjectile(magnitude, centre);
                }
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

        EventManager.EventTrigger(EventType.SFX, _soundPlayerLose);
        EventManager.EventTrigger(EventType.LOSE, null);
    }

    // Deactivate Enemy that was attacked then check to see how many enemies are left
    public void PlayerAttacked(Enemy enemy)
    {
        EventManager.EventTrigger(EventType.SFX, _soundEnemyDeath);
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
        EventManager.EventTrigger(EventType.SFX, _soundPlayerWin);
        EventManager.EventTrigger(EventType.WIN, null);
    }

    public void KillAllEnemies(object data)
    {
        foreach(Enemy enemy in _enemies)
        {
            enemy.gameObject.SetActive(false);
        }

        CheckEnemiesLeft();
    }
}