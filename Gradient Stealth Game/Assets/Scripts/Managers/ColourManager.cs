using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourManager : MonoBehaviour
{
    public float colour;        // Final colour value for other things to access

    void Awake()
    {
        EventManager.EventInitialise(EventType.INIT_COLOUR_MANAGER);
    }

    void Start()
    {
        EventManager.EventTrigger(EventType.INIT_COLOUR_MANAGER, this);
    }

    void Update()
    {

    }
}
