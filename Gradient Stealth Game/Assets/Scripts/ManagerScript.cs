using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    public float colour;        // Final colour value for other things to access
    public float colourCoords;    // Colour value taken from the current coords of the player
    public float colourTime;    // Colour value taken from amount of time spent while moving

    void Start()
    {
        
    }

    void Update()
    {
        // Flip colour time values back if it goes over/under values
        if (colourTime > 360f)
        {
            colourTime = 0;
        }
        else if (colourTime < 0)
        {
            colourTime = 360f;
        }

        // Flip colour move coords values back if it goes over/under values
        if (colourCoords > 360f)
        {
            colourCoords = 0;
        }
        else if (colourCoords < 0)
        {
            colourCoords = 360f;
        }

        // Combine the colour values from movement coords and time taken to move
        colour = colourCoords + colourTime;
    }
}
