using System.Collections;
using UnityEngine;

public enum RotateType { Clockwise, CounterClockwise, Random }

public class EnemyRotatorBehaviour : EnemyBehaviour
{
    [Header("Rotation Data")]
    [SerializeField] float _rotateSpeed;
    [SerializeField] float _timeToRotate;
    [SerializeField] RotateType _rotateType;

    // Internal Data
    private float _timer;

    public override void ResetBehaviour()
    {
        ResetTimer();
    }

    public override void ExecuteBehaviour()
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

    private void ResetTimer()
    {
        _timer = 0;
    }

    IEnumerator Rotate()
    {
        Quaternion _currentRot = transform.rotation;
        Quaternion _endRot = Quaternion.Euler(0f, 0f, 90f);

        while (Mathf.Abs(Quaternion.Angle(_currentRot * _endRot, transform.rotation)) > 0.1f)
        {
            Debug.Log("Rotating");
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _endRot, _rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
