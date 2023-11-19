using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIColourScript : MonoBehaviour
{
    public float colour;        // Final colour value for other things to access

    private Vector3 tmpMousePosition;

    void Start()
    {
        tmpMousePosition = Input.mousePosition;
    }

    void Update()
    {
        if (tmpMousePosition != Input.mousePosition)
        {
            colour = 0.35f;
            tmpMousePosition = Input.mousePosition;
        }
        else
        {
            colour = 0f;
        }
    }
}
