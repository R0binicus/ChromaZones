using System.Collections;
using UnityEngine;

public enum RotateType { Clockwise, CounterClockwise, Random }

public class EnemyRotatorBehaviour : EnemyBehaviour
{
    [Header("Rotation Data")]
    [SerializeField] float _rotateSpeed;
    [SerializeField] float _returnRotateSpeed;
    [SerializeField] float _timeToRotate;
    [SerializeField] RotateType _rotateType;

    // Returning to waypoint data
    private Vector2 _originWaypoint;
    private Vector2 _destinationDirection;
    private bool _rotatingToPrevAngle = false;
    private bool _reachedDestination = true;

    private Vector3 _endRotEuler;
    private Quaternion _endRot;

    // Components
    private Rigidbody2D rb;
    private UnityEngine.AI.NavMeshAgent Agent;

    // Timer
    private float _timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    private void Start()
    {
        // Generate random seed
        Random.InitState((int)System.DateTime.Now.Ticks);

        // Retrieve origin transform data so Enemy can return to it if necessary
        _originWaypoint = transform.position;
        _endRot = transform.rotation;
        GetLocation(_originWaypoint);
        //StopAllCoroutines();
        StartCoroutine(ResetPosition());
    }

    public override void ResetBehaviour()
    {
        ResetTimer();
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
            // On return to origin, make sure angle is last angle used
            else if (_rotatingToPrevAngle)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _endRot, _returnRotateSpeed * Time.deltaTime);

                if (Mathf.Abs(Quaternion.Angle(_endRot, transform.rotation)) < 0.1f)
                {
                    _rotatingToPrevAngle = false;
                    transform.rotation = _endRot;
                }
            }
            // Start rotation patrol
            else 
            {
                if (_timer > _timeToRotate)
                {
                    ResetTimer();
                    StopAllCoroutines();
                    StartCoroutine(Rotate());
                }
                else
                {
                    _timer += Time.deltaTime;
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

    private void ResetTimer()
    {
        _timer = 0;
    }

    // Patrolling rotate
    IEnumerator Rotate()
    {
        Quaternion currentRot = transform.rotation;

        int signChange = 1;

        // Change type of rotation
        if (_rotateType == RotateType.Clockwise)
        {
            signChange = -1;
        }
        else if (_rotateType == RotateType.CounterClockwise)
        {
            signChange = 1;
        }
        else if (_rotateType == RotateType.Random)
        {
            signChange = Random.Range(0, 2) == 0 ? -1 : 1;
        }

        // Calculate new angle to rotate to
        _endRot = currentRot * Quaternion.Euler(0f, 0f, 90f * signChange);
        _endRotEuler = _endRot.eulerAngles;
        _endRotEuler.z = Mathf.Round(_endRotEuler.z / 90) * 90;
        _endRot = Quaternion.Euler(_endRotEuler.x, _endRotEuler.y, _endRotEuler.z);

        // Keep rotating until enemy has reached new angle
        while (Mathf.Abs(Quaternion.Angle(_endRot, transform.rotation)) > 0.05f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _endRot, _rotateSpeed * Time.deltaTime);
            yield return null;
        }

        // Make sure enemy is still at its waypoint to complete turn
        if ((_originWaypoint - (Vector2)transform.position).magnitude < 0.1f)
        {
            transform.rotation = _endRot;
            transform.position = _originWaypoint;
        }
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
