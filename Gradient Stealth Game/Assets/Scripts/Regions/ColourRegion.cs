using System.Collections;
using System.Collections.Generic;
//using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class ColourRegion : MonoBehaviour
{
    
    private ColourManager _colourManager;
    
    [field: Header("Colour Region Settings")]
    [SerializeField] private float _transitionMultiplier = 2f;
    [SerializeField] private float _localChangeMultiplier = 1f;
    

    private float _colourDiff;               // Colour from game manager
    private float _localColour;              // Colour of this region specifically
    private float _originalHue;              // Original colour value when the level started

    [SerializeField] private bool _disabledColourChange = false;
    [SerializeField] private bool _reversedColourChange = false;

    public int State = 0;

    private Player _player;
    private List<Enemy> _enemies = new List<Enemy>();

    [field: Header("Assignment Code Stuff")]
    [SerializeField] private int _assignmentCode = 0;
    [SerializeField] private bool _changeEnableDisable = true;
    [SerializeField] private bool _changeColourReset = false;
    [SerializeField] private bool _changeColourReverse = false;

    [field: Header("Sprites")]
    [SerializeField] private SpriteRenderer _outlineSprite;
    private SpriteRenderer _backgroundSprite;

    void Awake()
    {
        _backgroundSprite = GetComponent<SpriteRenderer>();
        //_outlineSprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        Color.RGBToHSV(_backgroundSprite.color, out var H, out var S, out var V);
        Color.RGBToHSV(_outlineSprite.color, out var H2, out var S2, out var V2);
        _localColour = H * 360;
        _originalHue = _localColour;
        _outlineSprite.size = _backgroundSprite.size;
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
        if (_assignmentCode != 0)
        {
            EventManager.EventSubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
        }
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
        EventManager.EventUnsubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
    }

    void Update()
    {
        if (!_disabledColourChange)
        {
            if (!_reversedColourChange)
            {
                _colourDiff = _colourManager.colour;
            }
            else 
            {
                _colourDiff = _colourManager.colour * -1f;
            }

        }
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
        else if (_localColour < 0f)
        {
            _localColour = 360f;
        }

        SetStates();
    }

    // Sets the colour of the sprite renderer
    private void SetColour()
    {
        // used 0.95 because otherwise it hurts my eyes
        float darkness = 0.95f;
        if (_disabledColourChange)
        {
            darkness = 0.7f;
        }
        
        _backgroundSprite.color = Color.HSVToRGB(_localColour/360f, 0.95f, darkness);
    }

    private void SetBorderColour(float borderColour)
    {
        if (_disabledColourChange)
        {
            _outlineSprite.color = Color.HSVToRGB(borderColour/360f, 0f, 1f);
        }
        else
        {
            _outlineSprite.color = Color.HSVToRGB(borderColour/360f, 0.8f, 0.7f);
        }
    }

    private void SetStates()
    {
        int originalState = State;
        switch(_localColour) 
        {
            case float x when x < 45f: 
                State = 1;
                SetBorderColour(330f);
            break;
            case float x when x >= 45f && x < 165f :
                State = 2;
                SetBorderColour(90f);
            break;
            case float x when x >= 165f && x < 285f :
                State = 3;
                SetBorderColour(210f);
            break;
            case float x when x >= 285f && x <= 360f :
                State = 1;
                SetBorderColour(330f);
            break;
            default:
                State = 2;
                SetBorderColour(90f);
            break;
        }

        //switch(_localColour) 
        //{
        //    case float x when x < 40f:
        //        State = 1;
        //        _outlineSprite.enabled = true;
        //        SetBorderColour(330f);
        //    break;
        //    case float x when x >= 50f && x < 160f :
        //        State = 2;
        //        if (_changeEnableDisable)
        //        {
        //            _outlineSprite.enabled = true;
        //            SetBorderColour(90f);
        //        }
        //        _outlineSprite.enabled = false;
        //    break;
        //    case float x when x >= 170f && x < 280f :
        //        State = 3;
        //        _outlineSprite.enabled = true;
        //        SetBorderColour(210f);
        //    break;
        //    case float x when x >= 290f && x <= 360f :
        //        State = 1;
        //        _outlineSprite.enabled = true;
        //        SetBorderColour(330f);
        //    break;
        //    default:
        //        State = 2;
        //        if (_changeEnableDisable)
        //        {
        //            _outlineSprite.enabled = true;
        //            SetBorderColour(90f);
        //        }
        //        _outlineSprite.enabled = false;
        //    break;
        //}

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
            case float x when x >= 35f && x < 55f :
                _localColour = _localColour + (_colourDiff * _transitionMultiplier * _localChangeMultiplier);
            break;
            case float x when x >= 155f && x < 175f :
                _localColour = _localColour + (_colourDiff * _transitionMultiplier * _localChangeMultiplier);
            break;
            case float x when x >= 275f && x < 295f :
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

    private void AssignmentCodeHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("ColourRegion AssignmentCodeHandler is null");
        }

        if (_assignmentCode == (int)data)
        {
            if (_changeEnableDisable)
            {
                _disabledColourChange = !_disabledColourChange;
            }
            if (_changeColourReset)
            {
                _localColour = _originalHue;
            }
            if (_changeColourReverse)
            {
                _reversedColourChange = !_reversedColourChange;
            }
        }
    }
}