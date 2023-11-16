using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] public float DetectionTime { get; protected set; }
    [field: SerializeField] public float HiddenTime { get; protected set; }
    [field: SerializeField] public AnimationCurve Velocity { get; protected set; }
    
    // Components
    public EnemyFieldOfView FOV { get; protected set; }
    public EnemyBehaviour EnemyBehaviour { get; protected set; }
    public Rigidbody2D RB { get; protected set; }
    public Collider2D Collider { get; protected set; }

    // States
    public StateMachine StateMachine { get; protected set; }
    public EnemyPatrolState PatrolState { get; protected set; }
    public EnemyAlertState AlertState { get; protected set; }
    public EnemyChaseState ChaseState { get; protected set; }
    public EnemyCaughtState CaughtState { get; protected set; }

    // Player Ref (TESTING FOR NOW. Will give all enemies player ref when created with enemy manager or something idk)
    [field: SerializeField] public Player Player { get; protected set; }

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
