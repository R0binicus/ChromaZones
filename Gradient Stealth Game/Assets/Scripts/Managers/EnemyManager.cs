using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // GameObject References
    private Player _player;

    // Internal Data
    List<Enemy> _enemies; // List of Enemies in the scene

    [Header("Sounds")]
    [SerializeField] private string deathName = "EnemyDeath";
	private AudioSource deathSound;

    [SerializeField] private string winName = "PlayerWin";
	private AudioSource winSound;

    [SerializeField] private string loseName = "PlayerLose";
	private AudioSource loseSound;

    [SerializeField] private NavMeshPlus.Components.NavMeshSurface surfaceSingle;

    private void Awake()
    {
        deathSound = GameObject.Find(deathName).GetComponent<AudioSource>();
        winSound = GameObject.Find(winName).GetComponent<AudioSource>();
        loseSound = GameObject.Find(loseName).GetComponent<AudioSource>();
        _enemies = new List<Enemy>();
        EventManager.EventInitialise(EventType.LOSE);
        EventManager.EventInitialise(EventType.ASSIGNMENT_CODE_TRIGGER);
        EventManager.EventInitialise(EventType.AREA_CHASE_TRIGGER);
        EventManager.EventInitialise(EventType.REBUILD_NAVMESH);
    }

    void Start()
    {
        RebuildNavMesh(null);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.ADD_ENEMY, AddEnemy);
        EventManager.EventSubscribe(EventType.AREA_CHASE_TRIGGER, AlertNearbyEnemies);
        EventManager.EventSubscribe(EventType.REBUILD_NAVMESH, RebuildNavMesh);
        EventManager.EventSubscribe(EventType.INIT_PLAYER, PlayerInitHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.ADD_ENEMY, AddEnemy);
        EventManager.EventUnsubscribe(EventType.AREA_CHASE_TRIGGER, AlertNearbyEnemies);
        EventManager.EventUnsubscribe(EventType.REBUILD_NAVMESH, RebuildNavMesh);
        EventManager.EventUnsubscribe(EventType.INIT_PLAYER, PlayerInitHandler);
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
        //Debug.Log("RebuildNavMesh");
        surfaceSingle.BuildNavMesh();
    }

    // Receives Enemies that are instantiated within the level to keep track of for win condition
    private void AddEnemy(object data)
    {
        //Debug.Log("Enemy Added");
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
                    enemy.CheckWalls(magnitude);
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
        loseSound.Play();

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
        deathSound.Play();
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
        winSound.Play();
        EventManager.EventTrigger(EventType.WIN, null);
    }
}
