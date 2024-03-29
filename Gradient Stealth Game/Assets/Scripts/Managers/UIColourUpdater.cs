using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIColourUpdater : MonoBehaviour
{
    public float Colour;        // Final colour value for other things to access

    private Vector2 _tmpMousePosition;

    void Start()
    {
        _tmpMousePosition = Mouse.current.position.ReadValue();
    }

    void Update()
    {
        if (_tmpMousePosition != Mouse.current.position.ReadValue())
        {
            Colour = 45f * Time.unscaledDeltaTime;
            _tmpMousePosition = Mouse.current.position.ReadValue();
        }
        else
        {
            Colour = 0f;
        }
    }
}
