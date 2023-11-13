using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    public float colour;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (colour > 360f)
        {
            colour = 0;
        }
        else if (colour < 0)
        {
            colour = 360f;
        }
    }
}
