using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum TwoFOVTypes { HORIZONTAL, VERTICAL }

[ExecuteAlways]
public class Enemy : MonoBehaviour
{
    #region Exposed Data
    [field: Header("Timers")]
    [field: SerializeField] public float DetectionTime { get; private set; }

    [field: Header("Movement")]
    [field: SerializeField] public AnimationCurve Velocity { get; private set; }
    [field: SerializeField] private float _walkSpeed = 1.0f; 
    [field: SerializeField] private float _chaseSpeed = 2.8f; 
    private float _moveSpeed; 
    [field: SerializeField] public float ChaseRotation { get; private set; }

    [field: Header("FOV")]

    private AlertData _alertData;
    [field: SerializeField] public float AlertOthersRadius { get; private set; } = 3f;
    [field: SerializeField] public FOVData PatrolFOVData { get; private set; }
    
    [field: Header("Sprites")]
    [field: SerializeField] private Sprite _hidingSprite;
    [field: SerializeField] private Sprite _normalSprite;
    [field: SerializeField] private Sprite _invulnerableSprite;
    [field: SerializeField] private SpriteRenderer _fillSprite;


    #endregion
   
    #region Region Data
    [Header("Region Data")]
    public int RegionState = 0;
    public bool InvulnerableState = false;

    [field: SerializeField] private int _assignmentCode = 0;
    private bool _isEnemyHiding;
    #endregion
    
    #region Components
    public EnemyFieldOfView[] FOVs { get; private set; }
    public EnemyBehaviour EnemyBehaviour { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Collider2D Collider { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public UnityEngine.AI.NavMeshAgent Agent { get; private set; }
    #endregion

    #region States
    public StateMachine StateMachine { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyAlertState AlertState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyCaughtState CaughtState { get; private set; }
    #endregion

    #region Sounds
    [Header("Sounds")]
    [SerializeField] public Sound SoundEnemyChase;
    [SerializeField] public Sound SoundEnemyDeAlert;
    #endregion

    #region Internal Data
    LayerMask _layersToRaycast;
    private int _dontStartYet = 0;
    #endregion

    #region GameObject Refs
    private Player _player;
    private EnemyManager _enemyManager;

    public EnemyManager EnemyManager
    {
        get
        {
            if (_enemyManager != null)
            {
                return _enemyManager;
            }
            else
            {
                Debug.Log("EnemyManager has not been assigned");
                return null;
            }
        }
        set
        {
            _enemyManager = value;
        }
    }
    public Player Player
    {
        get
        {
            if (_player != null)
            {
                return _player;
            }
            else
            {
                Debug.Log("Player has not been assigned");
                return null;
            }
        }
        set
        {
            _player = value;
        }
    }
    #endregion

    private void Awake()
    {
        // Get components
        FOVs = GetComponentsInChildren<EnemyFieldOfView>();
        EnemyBehaviour = GetComponent<EnemyBehaviour>();
        RB = GetComponentInChildren<Rigidbody2D>();
        Collider = GetComponentInChildren<Collider2D>();
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Set up states
        PatrolState = new EnemyPatrolState(this);
        AlertState = new EnemyAlertState(this);
        ChaseState = new EnemyChaseState(this);
        CaughtState = new EnemyCaughtState(this);

        // Set up state machine
        StateMachine = new StateMachine(PatrolState);

        // Set up nav mesh
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;

        // Get raycast layers
        _layersToRaycast = LayerMask.GetMask("Obstacle", "Enemy");
    }

    private void OnEnable()
    {
        if (_assignmentCode != 0)
        {
            EventManager.EventSubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
        }
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
    }

    private void Start()
    {
        // Send Enemy to EnemyManager to be stored in a list and kept track of for win condition
        EventManager.EventTrigger(EventType.ADD_ENEMY, this);
        
        SetWalkSpeed();
        _alertData = new AlertData(transform.position, AlertOthersRadius, 0);
    }

    // Runs when changes are made in the editor for the FOV
    private void OnValidate()
    {
        if (FOVs == null)
        {
            FOVs = GetComponentsInChildren<EnemyFieldOfView>();
        }

        foreach (EnemyFieldOfView fov in FOVs)
        {
            fov.SetFOVData(PatrolFOVData);
            fov.CreateFOV();
        }
    }

    private void Update()
    {
        // To ensure the editor does not execute update
        if (Application.IsPlaying(gameObject))
        {
            StateMachine.CurrentState.LogicUpdate();
        }
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
        if (_dontStartYet < 5)
        {
            _dontStartYet++;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StateMachine.CurrentState.OnCollisionEnter2D(collision);
    }

    public bool PlayerSpotted()
    {
        foreach (EnemyFieldOfView fov in FOVs)
        {
            if (fov.PlayerSpotted)
            {
                return true;
            }
        }

        return false;
    }

    public void ActivateAllFOVs(bool flag)
    {
        foreach (EnemyFieldOfView fov in FOVs)
        {
            fov.IsActive(flag);
        }
    }

    // Set (all?) FOV Data
    public void SetFOVsData(FOVData data)
    {
        foreach (EnemyFieldOfView fov in FOVs)
        {
            if (fov.gameObject.activeInHierarchy)
            {
                fov.SetFOVData(data);
            }
        }
    }

    public void CreateFOVs()
    {
        foreach (EnemyFieldOfView fov in FOVs)
        {
            if (fov.gameObject.activeInHierarchy)
            {
                fov.CreateFOV();
            }
        }
    }

    // Change between visible and 'hiding'
    public void HidingSprite()
    {
        if (!InvulnerableState)
        {
            SpriteRenderer.sprite = _hidingSprite;
            _fillSprite.enabled = false;
        }
    }

    public void NormalSprite()
    {
        SpriteRenderer.sprite = _normalSprite;
        _fillSprite.enabled = false;
    }

    public void InvulnerableSprite()
    {
        SpriteRenderer.sprite = _invulnerableSprite;
        _fillSprite.enabled = true;
    }

    public void SetWalkSpeed()
    {
        _moveSpeed = _walkSpeed;
        Agent.speed = _moveSpeed;
    }

    public void SetChaseSpeed()
    {
        _moveSpeed = _chaseSpeed;
        Agent.speed = _moveSpeed;
    }

    public void NewState(int input)
    {
        RegionState = input;
        StateMachine.CurrentState.Check();
    }

    public void UpdateSprite()
    {
        if (!InvulnerableState)
        {
            if (RegionState == 1)
            {
                if (!_isEnemyHiding)
                {
                    HidingSprite();
                    _isEnemyHiding = true;
                }
                else {HidingSprite();}
            }
            else 
            {
                if (_isEnemyHiding)
                {
                    NormalSprite();
                    _isEnemyHiding = false;
                }
                else {NormalSprite();}
            }
        }
        else 
        {
            InvulnerableSprite();
        }
    }

    private void AssignmentCodeHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("Enemy AssignmentCodeHandler is null");
        }

        if (_assignmentCode == (int)data)
        {
            if (RegionState != 1)
            {
                _isEnemyHiding = !_isEnemyHiding;
            }
            InvulnerableState = !InvulnerableState;
            NewState(RegionState);
        }
    }

    public void EnemyAlertNearbyEnemies()
    {
        _alertData.Centre = transform.position;
        _alertData.Type = 0;
        EnemyManager.AlertNearbyEnemies(_alertData);
    }

    public void CheckWalls(float magnitude, Vector3 callerPosition)
    {
        gameObject.layer = 1;
        Vector2 centrerDir =  callerPosition - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, centrerDir, magnitude, _layersToRaycast);
        //Debug.DrawRay(transform.position, centrerDir, Color.red, 2f, true);

        if (hit)
        {
            // If ray hits enemy first, chase. Otherwise, it has hit an obstacle - do not chase
            if (hit.collider.CompareTag("Enemy") && StateMachine.CurrentState != ChaseState && StateMachine.CurrentState != CaughtState)
            {
                EventManager.EventTrigger(EventType.SFX, SoundEnemyChase);
                StateMachine.ChangeState(ChaseState);
            }
        }
        gameObject.layer = 9;
    }

    public void CheckWallsProjectile(float magnitude, Vector3 callerPosition)
    {
        Vector2 centrerDir = transform.position - callerPosition;
        RaycastHit2D hit = Physics2D.Raycast(callerPosition, centrerDir, magnitude, _layersToRaycast);
        //Debug.DrawRay(callerPosition, centrerDir, Color.red, 5f, true);

        if (hit)
        {
            // If ray hits enemy first, chase. Otherwise, it has hit an obstacle - do not chase
            if (hit.collider.gameObject == gameObject && StateMachine.CurrentState != ChaseState && StateMachine.CurrentState != CaughtState)
            {
                EventManager.EventTrigger(EventType.SFX, SoundEnemyChase);
                StateMachine.ChangeState(ChaseState);
            }
        }
    }

    public void Caught()
    {
        StateMachine.ChangeState(CaughtState);
        EnemyManager.PlayerCaught();
    }
}