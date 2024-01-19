using System.Collections;
using UnityEngine;

public class EnemyStaticBehaviour : EnemyBehaviour
{
    [SerializeField] float _returnRotateSpeed;

    // Returning to waypoint data
    private Vector2 _originWaypoint;
    private Vector2 _destinationDirection;
    private bool _rotatingToOrigin = false;
    private bool _rotatingToPrevAngle = false;
    private bool _reachedDestination = true;

    private Quaternion _endRot;

    // Components
    private Rigidbody2D rb;
    private UnityEngine.AI.NavMeshAgent Agent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    private void Start()
    {
        // Retrieve origin transform data so Enemy can return to it if necessary
        _originWaypoint = transform.position;
        _endRot = transform.rotation;
        GetLocation(_originWaypoint);
        //StopAllCoroutines();
        StartCoroutine(ResetPosition());
    }

    public override void ResetBehaviour()
    {
        StopAllCoroutines();
    }

    public override void UpdateLogicBehaviour()
    {
        // If Enemy is at their origin waypoint
        if ((_originWaypoint - (Vector2)transform.position).magnitude < 0.05f)
        {
            // If it has just reached its origin, stay still
            if (!_reachedDestination)
            {
                _reachedDestination = true;
                Agent.ResetPath();
                rb.velocity = Vector2.zero;
                transform.position = _originWaypoint;
            }             
            // Then once still, rotate back to original angle
            else 
            {
                if (_rotatingToOrigin)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, _endRot, _returnRotateSpeed * Time.deltaTime);

                    if (Mathf.Abs(Quaternion.Angle(_endRot, transform.rotation)) < 0.1f)
                    {
                        _rotatingToOrigin = false;
                        transform.rotation = _endRot;
                    }
                }
            }         
        } 
        // If Enemy is not at origin waypoint, move back to it
        else 
        {
            if (_reachedDestination)
            {
                _reachedDestination = false;
            }
            _rotatingToOrigin = true;
            _rotatingToPrevAngle = true;
            GetLocation(_originWaypoint);
            Quaternion fullRotatation = Quaternion.LookRotation(transform.forward, _destinationDirection);
            Quaternion lookRot = Quaternion.identity;
            lookRot.eulerAngles = new Vector3(0,0,fullRotatation.eulerAngles.z);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, _returnRotateSpeed * Time.deltaTime);
            Agent.SetDestination(_originWaypoint);
        }
    }

    public override void UpdatePhysicsBehaviour()
    {
        
    }

    private void GetLocation(Vector2 point)
    {
        _destinationDirection = (point - (Vector2)transform.position).normalized;
    }

    private IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(0.05f);
        transform.position = _originWaypoint;
    }
}
