using System.Collections;
using System.Collections.Generic;
//using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class ColourRegion : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private ColourManager _colourManager;

    [SerializeField] private float _transitionMultiplier = 2f;
    [SerializeField] private float _localChangeMultiplier = 1f;
    [SerializeField] private int _assignmentCode = 0;

    private float _colourDiff;               // Colour from game manager
    private float _localColour;              // Colour of this region specifically
    private float _originalHue;              // Original colour value when the level started

    public int State = 0;

    private Player _player;
    private List<Enemy> _enemies = new List<Enemy>();

    void Awake()
    {
        // Set values and components
        Color.RGBToHSV(GetComponent<SpriteRenderer>().color, out var H, out var S, out var V);
        _localColour = H * 360;
        _originalHue = _localColour;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
    }

    void Update()
    {
        _colourDiff = _colourManager.colour;
        ProcessColour();
        SetColour();
    }

    // Processes the colourManager colour
    private void ProcessColour()
    {
        // set new local colour value
        TransitionZones();

        //if local colour value is over 360, change the local colour values back to being under 360 with math
        if (_localColour >= 360f)
        {
            _localColour = 0f;
        }

        SetStates();
    }

    // Sets the colour of the sprite renderer
    private void SetColour()
    {
        // used 0.95 because otherwise it hurts my eyes
        _spriteRenderer.color = Color.HSVToRGB(_localColour/360f, 0.95f, 0.95f);
    }

    private void SetStates()
    {
        int originalState = State;
        switch(_localColour) 
        {
            case float x when x < 60f:
                State = 1;
            break;
            case float x when x >= 60f && x < 180f :
                State = 2;
            break;
            case float x when x >= 180f && x < 300f :
                State = 3;
            break;
            case float x when x >= 300f && x < 360f :
                State = 1;
            break;
            default:
                State = 0;
            break;
        }

        if (originalState != State)
        {
            if (_player != null)
            {
                _player.NewState(State);
            }

            if (_enemies != null)
            {
                foreach (Enemy enemy in _enemies)
                {
                    enemy.NewState(State);
                }
            }
        }
    }

    // If local colour value is in a 'transition zone' change the colour witha multiplier,
    // otherwise simply add it to the difference value
    private void TransitionZones()
    {
        switch(_localColour) 
        {
            case float x when x >= 50f && x < 70f :
                _localColour = _localColour + (_colourDiff * _transitionMultiplier * _localChangeMultiplier);
            break;
            case float x when x >= 170f && x < 190f :
                _localColour = _localColour + (_colourDiff * _transitionMultiplier * _localChangeMultiplier);
            break;
            case float x when x >= 290f && x < 310f :
                _localColour = _localColour + (_colourDiff * _transitionMultiplier * _localChangeMultiplier);
            break;
            default:
                _localColour = _localColour + (_colourDiff * _localChangeMultiplier);
            break;
        }
    }

    // Set state variable when entering a region
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Set the object's regionState to be the current
        // state of the region. Also increment the region
        // layer by 1.
        if(collision.tag == "RegionDetector")
        {
            GameObject mainObject = collision.transform.parent.gameObject;
            if (mainObject.tag == "Player") 
            {
                _player = mainObject.GetComponent<Player>();
                _player.NewState(State);
            }
            else if (mainObject.tag == "Enemy")
            {
                var enemyObject = mainObject.GetComponent<Enemy>();
                enemyObject.NewState(State);
                _enemies.Add(enemyObject);
            }
        }
    }

    // Set state variable whenever it moves inside a region
    private void OnTriggerStay2D(Collider2D collision)
    {
        // This should only be useful when the region changes state
        // while the player or enemy is inside a region
        if(collision.tag == "RegionDetector")
        {
            if (collision.transform.parent.tag == "Player") 
            {
                var player = collision.transform.parent.GetComponent<Player>();
            }
            else if (collision.transform.parent.tag == "Enemy")
            {
                var enemy = collision.transform.parent.GetComponent<Enemy>();
            }
        }
    }

    // Reset state variable if no longer on a region
    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if(collision.tag == "RegionDetector")
        {
            GameObject mainObject = collision.transform.parent.gameObject;
            // What this does is reduce a region layer by 1
            // If region layer is 0, it should be in NO region
            // so it needs to be manually told to reset the object's
            // regionState to zero (no region)
            if (mainObject.tag == "Player") 
            {
                _player = null;
            }
            else if (mainObject.tag == "Enemy")
            {
                _enemies.Remove(mainObject.GetComponent<Enemy>());
            }
        }
    }

    private void ColourManagerHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("ColourManagerHandler is null");
        }

        _colourManager = (ColourManager)data;
    }
}