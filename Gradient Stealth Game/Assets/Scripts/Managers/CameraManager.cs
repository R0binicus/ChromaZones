using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{
    private Camera _mainCamera;
    [SerializeField] float _aspect15 = 6.2f;
    [SerializeField] float _aspect16 = 5.9f;
    [SerializeField] float _aspect17 = 5.5f;
    [SerializeField] float _aspect18 = 5f;

    void Awake()
    {
        _mainCamera = GetComponentInChildren<Camera>();

        switch (Math.Round(_mainCamera.aspect, 1))
        {
            case 1.5: 
                _mainCamera.orthographicSize = _aspect15;
                break;
            case 1.6:
                _mainCamera.orthographicSize = _aspect16;
                break;
            case 1.7:
                _mainCamera.orthographicSize = _aspect17;
                break;
            case 1.8:
                _mainCamera.orthographicSize = _aspect18;
                break;
            default:
                _mainCamera.orthographicSize = _aspect16;
                break;
        }
    }
}
