using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourManager : MonoBehaviour
{
    public float colour;                    // Final colour value for other things to access
    [Header("Debug Stuff")]
    [SerializeField] private float _colourChangeSpeed = 0.1f;  
    private bool _changing_bool = false;

    private float time = 0f;

    private float period = 0.1f;

    void Awake()
    {
        EventManager.EventInitialise(EventType.INIT_COLOUR_MANAGER);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LEVEL_STARTED, LevelStart);
        EventManager.EventSubscribe(EventType.COLOUR_CHANGE_BOOL, ColourBoolHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LEVEL_STARTED, LevelStart);
        EventManager.EventUnsubscribe(EventType.COLOUR_CHANGE_BOOL, ColourBoolHandler);
    }

    public void LevelStart(object data)
    {
        EventManager.EventTrigger(EventType.INIT_COLOUR_MANAGER, this);
    }

    void Update()
    {
        if (_changing_bool == true)
        {
            colour = 14.25f * _colourChangeSpeed * Time.deltaTime;
        }
        else
        {
            colour = 0f;
        }
    }

    private void ColourBoolHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("ColourBoolHandler is null");
        }
        
        _changing_bool = (bool)data;
    }
}
