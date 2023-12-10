using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrollerBehaviour : EnemyBehaviour
{
    [Header("Patroller Data")]
    public List<Vector3> waypoints;
    private Vector2 originWaypoint;
    private Vector2 destination;
    private Vector2 destinationDirection;
    private int waypointIndex = 1;
    [SerializeField] private int roationSpeed = 100;

    private Rigidbody2D rb;
    private UnityEngine.AI.NavMeshAgent Agent;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        originWaypoint = transform.position;
        UpdateWaypoint();

        Vector3 offset = (Vector3)destination - transform.position;
        
        // Construct a rotation as in the y+ case.
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward,offset);

        // Apply a compensating rotation that twists x+ to y+ before the rotation above.
        transform.rotation = rotation * Quaternion.Euler(0, 0, 0);
    }

    private void GetLocation(Vector2 point)
    {
        destinationDirection = (point - (Vector2)transform.position).normalized;
    }

    private void UpdateWaypoint()
    {
        if (waypointIndex > waypoints.Count)
        {
            waypointIndex = 0;
            destination = originWaypoint;
        }

        if (waypointIndex != 0)
        {
            destination = waypoints[waypointIndex - 1];
        }
    }

    public override void ResetBehaviour()
    {

    }

    public override void UpdatePhysicsBehaviour()
    {
        if ((destination - (Vector2)transform.position).magnitude < 0.1f)
        {
            Agent.ResetPath();
        }
        Agent.SetDestination(destination);
    }

    public override void UpdateLogicBehaviour()
    {
        if ((destination - (Vector2)transform.position).magnitude < 0.1f)
        {
            waypointIndex++;
            UpdateWaypoint();
        }

        GetLocation(destination);
        Quaternion fullRotatation = Quaternion.LookRotation(transform.forward, destinationDirection);
        Quaternion lookRot = Quaternion.identity;
        lookRot.eulerAngles = new Vector3(0,0,fullRotatation.eulerAngles.z);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, roationSpeed * Time.deltaTime);
    }
}
