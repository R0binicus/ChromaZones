using System.Collections;
using UnityEngine;

public enum RotateType { Clockwise, CounterClockwise, Random }

public class EnemyRotatorBehaviour : EnemyBehaviour
{
    [Header("Rotation Data")]
    [SerializeField] float _rotateSpeed;
    [SerializeField] float _timeToRotate;
    [SerializeField] RotateType _rotateType;

    [Header("Return to start Data")]
    private Vector2 _originWaypoint;
    private Vector2 _destinationDirection;
    private bool _startedRoation = true;

    //Components
    private Rigidbody2D rb;

    // Internal Data
    private float _timer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Random.InitState((int)System.DateTime.Now.Ticks);
        _originWaypoint = transform.position;
        GetLocation(_originWaypoint);
    }

    public override void ResetBehaviour()
    {
        ResetTimer();
    }

    public override void UpdateLogicBehaviour()
    {
        Debug.Log((_originWaypoint - (Vector2)transform.position).magnitude);
        if ((_originWaypoint - (Vector2)transform.position).magnitude < 0.05f)
        {
            rb.velocity = Vector2.zero;
            if (_startedRoation == false)
            {
                Quaternion _currentRot = transform.rotation;
                Quaternion _endRot = Quaternion.Euler(0f, 0f, 0f);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _endRot, _rotateSpeed * Time.deltaTime);
                if (Mathf.Abs(Quaternion.Angle(_endRot, transform.rotation)) < 0.1f)
                {
                    _startedRoation = true;
                }
            }
            else
            {
                if (_timer > _timeToRotate)
                {
                    ResetTimer();
                    StartCoroutine(Rotate());
                }
                else
                {
                    _timer += Time.deltaTime;
                }
            }
            
        } else 
        {
            _startedRoation = false;
            GetLocation(_originWaypoint);
            Quaternion fullRotatation = Quaternion.LookRotation(transform.forward, _destinationDirection);
            Quaternion lookRot = Quaternion.identity;
            lookRot.eulerAngles = new Vector3(0,0,fullRotatation.eulerAngles.z);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, (_rotateSpeed * 2) * Time.deltaTime);
            rb.velocity = _destinationDirection;
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

        while (Mathf.Abs(Quaternion.Angle(_endRot, transform.rotation)) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _endRot, _rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void GetLocation(Vector2 point)
    {
        _destinationDirection = (point - (Vector2)transform.position).normalized;
    }
}
