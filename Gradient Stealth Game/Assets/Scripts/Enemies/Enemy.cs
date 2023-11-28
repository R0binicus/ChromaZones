using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Exposed Data
    [field: Header("Timers")]
    [field: SerializeField] public float DetectionTime { get; private set; }
    [field: SerializeField] public float HiddenTime { get; private set; }

    [field: Header("Movement")]
    [field: SerializeField] public AnimationCurve Velocity { get; private set; }
    [field: SerializeField] private float _walkSpeed = 1.0f; 
    [field: SerializeField] private float _chaseSpeed = 2.8f; 
    private float _moveSpeed; 
    [field: SerializeField] public float ChaseRotation { get; private set; }

    [field: Header("FOV")]
    [field: SerializeField] public FOVData PatrolFOVData { get; private set; }
    [field: SerializeField] public FOVData AlertFOVData { get; private set; }
    
    [field: Header("Sprites")]
    [field: SerializeField] public Sprite _hidingSprite;
    [field: SerializeField] public Sprite _normalSprite;
    #endregion
   
    #region Region Data
    [Header("Region Debugging")]
    public int RegionState = 0;
    private bool _isEnemyHiding;
    #endregion
    
    #region Components
    public EnemyFieldOfView FOV { get; private set; }
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
    [SerializeField] private string alertName = "EnemyAlert";
	public AudioSource alertSound { get; private set; }

    [SerializeField] private string deAlertName = "EnemyDeAlert";
	public AudioSource deAlertSound { get; private set; }

    [SerializeField] private string chaseName = "EnemyChase";
	public AudioSource chaseSound { get; private set; }
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
        FOV = GetComponentInChildren<EnemyFieldOfView>();
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

        // Sounds
        alertSound = GameObject.Find(alertName).GetComponent<AudioSource>();
        deAlertSound = GameObject.Find(deAlertName).GetComponent<AudioSource>();
        chaseSound = GameObject.Find(chaseName).GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Send Enemy to EnemyManager to be stored in a list and kept track of for win condition
        EventManager.EventTrigger(EventType.ADD_ENEMY, this);
        SetWalkSpeed();
    }

    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StateMachine.CurrentState.OnCollisionEnter2D(collision);
    }

    // Change between visible and 'hiding'
    public void HidingSprite()
    {
        SpriteRenderer.sprite = _hidingSprite;
    }

    public void NormalSprite()
    {
        SpriteRenderer.sprite = _normalSprite;
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
        if (RegionState == 1)
        {
            if (!_isEnemyHiding)
            {
                HidingSprite();
                _isEnemyHiding = true;
            }
        }
        else 
        {
            if (_isEnemyHiding)
            {
                NormalSprite();
                _isEnemyHiding = false;
            }
        }
    }
}
