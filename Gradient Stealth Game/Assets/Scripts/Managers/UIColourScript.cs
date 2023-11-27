using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIColourScript : MonoBehaviour
{
    public float colour;        // Final colour value for other things to access

    private Vector2 tmpMousePosition;

    void Start()
    {
        tmpMousePosition = Mouse.current.position.ReadValue();
    }

    void Update()
    {
        if (tmpMousePosition != Mouse.current.position.ReadValue())
        {
            colour = 0.35f;
            tmpMousePosition = Mouse.current.position.ReadValue();
        }
        else
        {
            colour = 0f;
        }
    }
}
