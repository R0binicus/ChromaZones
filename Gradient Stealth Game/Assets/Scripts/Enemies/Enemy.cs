using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] public float DetectionTime { get; protected set; }
    [field: SerializeField] public float HiddenTime { get; protected set; }
    
    // Components
    public EnemyFieldOfView FOV { get; protected set; }
    public EnemyBehaviour EnemyBehaviour { get; protected set; }

    // States
    public StateMachine StateMachine { get; protected set; }
    public EnemyPatrolState PatrolState { get; protected set; }
    public EnemyAlertState AlertState { get; protected set; }
    public EnemyChaseState ChaseState { get; protected set; }
    public EnemyCaughtState CaughtState { get; protected set; }

    private void Awake()
    {
        // Get components
        FOV = GetComponentInChildren<EnemyFieldOfView>();
        EnemyBehaviour = GetComponent<EnemyBehaviour>();

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
}
