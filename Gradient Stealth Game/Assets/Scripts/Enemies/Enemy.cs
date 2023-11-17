using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] public float DetectionTime { get; private set; }
    [field: SerializeField] public float HiddenTime { get; private set; }
    [field: SerializeField] public AnimationCurve Velocity { get; private set; }
    
    // Components
    public EnemyFieldOfView FOV { get; private set; }
    public EnemyBehaviour EnemyBehaviour { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Collider2D Collider { get; private set; }

    // States
    public StateMachine StateMachine { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyAlertState AlertState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyCaughtState CaughtState { get; private set; }

    // GameObject References
    [field: SerializeField] public Player Player { get; private set; }
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
    // Regions
    public int regionState = 0;
    public int regionLayer = 0;

    private void Awake()
    {
        // Get components
        FOV = GetComponentInChildren<EnemyFieldOfView>();
        EnemyBehaviour = GetComponent<EnemyBehaviour>();
        RB = GetComponentInChildren<Rigidbody2D>();
        Collider = GetComponentInChildren<Collider2D>();

        // Set up states
        PatrolState = new EnemyPatrolState(this);
        AlertState = new EnemyAlertState(this);
        ChaseState = new EnemyChaseState(this);
        CaughtState = new EnemyCaughtState(this);

        // Set up state machine
        StateMachine = new StateMachine(PatrolState);
    }

    private void Start()
    {
        // Send Enemy to EnemyManager to be stored in a list and kept track of for win condition
        EventManager.EventTrigger(EventType.ADD_ENEMY, this);
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
}
