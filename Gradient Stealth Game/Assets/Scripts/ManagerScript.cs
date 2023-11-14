using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    public float colour;
    public float colourMove;
    public float colourTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (colourTime > 360f)
        {
            colourTime = 0;
        }
        else if (colourTime < 0)
        {
            colourTime = 360f;
        }

        if (colourMove > 360f)
        {
            colourMove = 0;
        }
        else if (colourMove < 0)
        {
            colourMove = 360f;
        }

        colour = colourMove + colourTime;
    }
}
