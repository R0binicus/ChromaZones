using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera _mainCamera;
    [SerializeField] float _orthoSize169 = 5f;
    [SerializeField] float _orthoSize1610 = 5.9f;
    [SerializeField] float _orthoSize32 = 6.2f;

    void Awake()
    {
        _mainCamera = GetComponentInChildren<Camera>();

        switch (_mainCamera.aspect)
        {
            case (16f / 9f): 
                _mainCamera.orthographicSize = _orthoSize169;
                break;
            case (16f / 10f):
                _mainCamera.orthographicSize = _orthoSize1610;
                break;
            case (3f / 2f):
                _mainCamera.orthographicSize = _orthoSize32;
                break;
            default:
                _mainCamera.orthographicSize = _orthoSize169;
                break;
        }
    }
}
