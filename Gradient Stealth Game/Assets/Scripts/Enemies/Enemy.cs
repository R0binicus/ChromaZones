using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Data
    [field: SerializeField] public float ChaseRotation { get; private set; }
    [field: SerializeField] public float DetectionTime { get; private set; }
    [field: SerializeField] public float ReDetectionTime { get; private set; } // If has just come from chase state to alert state
    public bool DetectedOnce { get; set; } // If player has been detected going from chase to alert
    [field: SerializeField] public float HiddenTime { get; private set; }
    [field: SerializeField] public AnimationCurve Velocity { get; private set; }
    [field: SerializeField] public Sprite _hidingSprite;
    [field: SerializeField] public Sprite _normalSprite;
    
    // Regions
    public int regionState = 0;
    public int regionLayer = 0;
    private bool isEnemyHiding;
    
    // Components
    public EnemyFieldOfView FOV { get; private set; }
    public EnemyBehaviour EnemyBehaviour { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Collider2D Collider { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public UnityEngine.AI.NavMeshAgent Agent { get; private set; }

    // States
    public StateMachine StateMachine { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyAlertState AlertState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyCaughtState CaughtState { get; private set; }

    [Header("Sounds")]
    [SerializeField] private string alertName = "EnemyAlert";
	public AudioSource alertSound { get; private set; }

    [SerializeField] private string deAlertName = "EnemyDeAlert";
	public AudioSource deAlertSound { get; private set; }

    [SerializeField] private string chaseName = "EnemyChase";
	public AudioSource chaseSound { get; private set; }

    // GameObject References
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
    }

    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();

        if (regionState == 1)
        {
            if (!isEnemyHiding)
            {
                HidingSprite();
                isEnemyHiding = true;
            }
        }
        else 
        {
            if (isEnemyHiding)
            {
                NormalSprite();
                isEnemyHiding = false;
            }
        }
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
}
