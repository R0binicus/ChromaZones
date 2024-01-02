using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera _mainCamera;

    void Awake()
    {
        _mainCamera = GetComponentInChildren<Camera>();

        switch (_mainCamera.aspect)
        {
            case (16f / 9f): 
                _mainCamera.orthographicSize = 5f;
                break;
            case (16f / 10f):
                _mainCamera.orthographicSize = 5.9f;
                break;
            case (3f / 2f):
                _mainCamera.orthographicSize = 6.2f;
                break;
            default:
                _mainCamera.orthographicSize = 5f;
                break;
        }
    }
}
