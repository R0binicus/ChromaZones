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

    [Header("Return to Start Data")]
    private Vector2 _originWaypoint;
    private Quaternion _originAngle;
    private Vector2 _destinationDirection;
    private bool _rotatingToOrigin = false;

    //Components
    private Rigidbody2D rb;
    private UnityEngine.AI.NavMeshAgent Agent;

    // Internal Data
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
        _originAngle = transform.rotation;
        GetLocation(_originWaypoint);
    }

    public override void ResetBehaviour()
    {
        ResetTimer();
    }

    public override void UpdateLogicBehaviour()
    {
        // If Enemy is at their origin waypoint, start rotating
        if ((_originWaypoint - (Vector2)transform.position).magnitude < 0.05f)
        {
            // Stay still
            //rb.velocity = Vector2.zero;
            Agent.ResetPath();
            
            // If Enemy needs to rotate back to original angle
            if (_rotatingToOrigin)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _originAngle, _returnRotateSpeed * Time.deltaTime);

                if (Mathf.Abs(Quaternion.Angle(_originAngle, transform.rotation)) < 0.1f)
                {
                    _rotatingToOrigin = false;
                }
            }
            // Else, start rotation patrol
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
            _rotatingToOrigin = true;
            GetLocation(_originWaypoint);
            Quaternion fullRotatation = Quaternion.LookRotation(transform.forward, _destinationDirection);
            Quaternion lookRot = Quaternion.identity;
            lookRot.eulerAngles = new Vector3(0,0,fullRotatation.eulerAngles.z);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, _returnRotateSpeed * Time.deltaTime);
            //rb.velocity = _destinationDirection;
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

    IEnumerator Rotate()
    {
        Quaternion _currentRot = transform.rotation;

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

        Quaternion _endRot = _currentRot * Quaternion.Euler(0f, 0f, 90f * signChange);

        while (Mathf.Abs(Quaternion.Angle(_endRot, transform.rotation)) > 0.05f)
        {
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _endRot, _rotateSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = _endRot;
    }

    private void GetLocation(Vector2 point)
    {
        _destinationDirection = (point - (Vector2)transform.position).normalized;
    }
}
