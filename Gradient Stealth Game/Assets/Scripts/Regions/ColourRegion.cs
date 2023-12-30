using System;
using System.Collections;
using System.Collections.Generic;
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

    private Player _player = null;

    private bool _playerInRegion = false;

    private ResetRegionLinkData _linkData;
    private bool _resetLink = true;
    private List<Enemy> _enemies = new List<Enemy>();

    [field: Header("Assignment Code Stuff")]
    [SerializeField] private int _assignmentCode = 0;
    [SerializeField] private bool _changeEnableDisable = true;
    [SerializeField] private bool _changeColourReset = false;
    [SerializeField] private bool _changeColourReverse = false;

    [field: Header("Sprites")]
    [SerializeField] private SpriteRenderer _outlineSprite;
    [SerializeField] private SpriteRenderer _crossSprite;
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

        _linkData = new ResetRegionLinkData(false);
        
        if (_disabledColourChange)
        {
            _crossSprite.enabled = true;
        }
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
        EventManager.EventSubscribe(EventType.RESET_REGION_GAMEOBJECT_LINKS, LinkResetHandler);
        EventManager.EventSubscribe(EventType.INIT_PLAYER_REGION, InitPlayerHandler);
        EventManager.EventSubscribe(EventType.REGION_CHECK_AGAIN, CheckAgainHandler);

        if (_assignmentCode != 0)
        {
            EventManager.EventSubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
        }
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.INIT_COLOUR_MANAGER, ColourManagerHandler);
        EventManager.EventUnsubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
        EventManager.EventUnsubscribe(EventType.RESET_REGION_GAMEOBJECT_LINKS, LinkResetHandler);
        EventManager.EventUnsubscribe(EventType.INIT_PLAYER_REGION, InitPlayerHandler);
        EventManager.EventUnsubscribe(EventType.REGION_CHECK_AGAIN, CheckAgainHandler);
    }

    void Update()
    {
        if (!_disabledColourChange && _colourManager != null)
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
        else
        {
            _colourDiff = 0f;
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
        // Legacy Code
        //if (_disabledColourChange)
        //{
        //    darkness = 0.7f;
        //}
        
        _backgroundSprite.color = Color.HSVToRGB(_localColour/360f, 0.95f, darkness);
    }

    private void SetBorderColour(float borderColour)
    {
        _outlineSprite.color = Color.HSVToRGB(borderColour/360f, 0.8f, 0.7f);
        // Legacy Code
        //if (_disabledColourChange)
        //{
        //    _outlineSprite.color = Color.HSVToRGB(borderColour/360f, 0.8f, 1f);
        //}
        //else
        //{
        //    _outlineSprite.color = Color.HSVToRGB(borderColour/360f, 0.8f, 0.7f);
        //}
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

        if (originalState != State)
        {
            if (_playerInRegion == true)
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

            case float x when x > 30f && x < 40f :
                _localColour = 50f;
            break;
            case float x when x >= 40f && x < 50f :
                _localColour = 30f;
            break;
            case float x when x > 150f && x < 160f :
                _localColour = 170f;
            break;
            case float x when x >= 160f && x < 170f :
                _localColour = 150f;
            break;
            case float x when x > 270f && x < 280f :
                _localColour = 290;
            break;
            case float x when x >= 280f && x < 290f :
                _localColour = 270;
            break;
            default:
                _localColour = _localColour + (_colourDiff * _localChangeMultiplier);
            break;

            // Legacy Code

            //case float x when x >= 35f && x < 55f :
            //    _localColour = _localColour + (_colourDiff * _transitionMultiplier * _localChangeMultiplier);
            //break;
            //case float x when x >= 155f && x < 175f :
            //    _localColour = _localColour + (_colourDiff * _transitionMultiplier * _localChangeMultiplier);
            //break;
            //case float x when x >= 275f && x < 295f :
            //    _localColour = _localColour + (_colourDiff * _transitionMultiplier * _localChangeMultiplier);
            //break;
            //default:
            //    _localColour = _localColour + (_colourDiff * _localChangeMultiplier);
            //break;
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
            if (mainObject.name == "Player") 
            {
                if (_player == null)
                {
                    _player = mainObject.GetComponent<Player>();
                    EventManager.EventTrigger(EventType.INIT_PLAYER_REGION, _player);
                }
                //
                _playerInRegion = true;

                _player.NewState(State);


                //_linkData._isPlayer = true;
                //StartCoroutine(DisableLinkReset());
                //EventManager.EventTrigger(EventType.RESET_REGION_GAMEOBJECT_LINKS, _linkData);
                //ResetLinkObject();
            }
            else if (mainObject.tag == "Enemy")
            {
                var enemyObject = mainObject.GetComponent<Enemy>();
                enemyObject.NewState(State);
                _enemies.Add(enemyObject);


                //_linkData._enemy = enemyObject;
                //StartCoroutine(DisableLinkReset());
                //EventManager.EventTrigger(EventType.RESET_REGION_GAMEOBJECT_LINKS, _linkData);
                //ResetLinkObject();
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
            if (mainObject.name == "Player") 
            {
                _linkData._isPlayer = true;
                //StartCoroutine(DisableLinkReset());
                //EventManager.EventTrigger(EventType.RESET_REGION_GAMEOBJECT_LINKS, _linkData);
                LinkResetHandler(_linkData);
                ResetLinkObject();

                _playerInRegion = false;
            }
            else if (mainObject.tag == "Enemy")
            {
                var enemyObject = mainObject.GetComponent<Enemy>();

                _linkData._enemy = enemyObject;
                //StartCoroutine(DisableLinkReset());
                //EventManager.EventTrigger(EventType.RESET_REGION_GAMEOBJECT_LINKS, _linkData);
                LinkResetHandler(_linkData);
                ResetLinkObject();

                _enemies.Remove(enemyObject);
            }
            EventManager.EventTrigger(EventType.REGION_CHECK_AGAIN, null);
        }
    }

    private void ResetLinkObject()
    {
        _linkData._isPlayer = false;
        _linkData._enemy = null;
    }

    private void ColourManagerHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("ColourManagerHandler is null");
        }

        _colourManager = (ColourManager)data;
    }

    private void LinkResetHandler(object data)
    {
        if (_resetLink)
        {
            if (data == null)
            {
                Debug.Log("LinkResetHandler is null");
            }

            ResetRegionLinkData linkObject = (ResetRegionLinkData)data;

            if (linkObject._isPlayer)
            {
                _playerInRegion = false;
            }
            else if (linkObject._enemy != null)
            {
                if (_enemies.Contains(linkObject._enemy))
                {
                    _enemies.Remove(linkObject._enemy);
                }
                else
                {
                    //Debug.Log("WTF have you done LinkResetHandler");
                }
            }
            else 
            {
                Debug.LogError("WTF have you done LinkResetHandler");
            }
        }
    }
    
    private void CheckAgainHandler(object data)
    {
        //Debug.Log("_playerInRegion" + _playerInRegion);
        if (_playerInRegion)
        {
            _player.NewState(State);
            Debug.Log(State);
        }
        foreach (var enemy in _enemies)
        {
            enemy.NewState(State);
        }
    }

    private void InitPlayerHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("InitPlayerHandler is null");
        }

        _player = (Player)data;
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
                _crossSprite.enabled = _disabledColourChange;
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

    private IEnumerator DisableLinkReset()
    {
        _resetLink = false;
        yield return new WaitForSeconds(0.5f);
        _resetLink = true;
    }
}