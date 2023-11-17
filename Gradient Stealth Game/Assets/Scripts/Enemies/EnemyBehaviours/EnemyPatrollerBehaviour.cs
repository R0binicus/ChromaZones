using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrollerBehaviour : EnemyBehaviour
{
    [Header("Patroller Data")]
    [SerializeField] private List<GameObject> waypoints;
    private Vector2 originWaypoint;
    private Vector2 destination;
    private Vector2 destinationDirection;
    private int waypointIndex = 0;

    public void Start()
    {
        originWaypoint = transform.position;
    }

    public void Update()
    {
        if (destinationDirection.magnitude < 1)
        {
            waypointIndex++;
            UpdateWaypoint();
        }
    }

    private void GetLocation(Vector2 destination)
    {
        Vector2 position2d = transform.position;
        destinationDirection = destination - position2d;
    }

    private void UpdateWaypoint()
    {
        if (waypointIndex >= waypoints.Count)
        {
            waypointIndex = 0;
        }

        if (waypointIndex == 0)
        {
            destination = originWaypoint;
        }
        else 
        {
            destination = waypoints[waypointIndex + 1].transform.position;
        }
    }

    public override void ResetBehaviour()
    {

    }

    public override void ExecuteBehaviour()
    {
        
    }
}
