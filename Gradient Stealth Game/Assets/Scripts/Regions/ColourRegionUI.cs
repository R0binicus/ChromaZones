using System.Collections;
using System.Collections.Generic;
//using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class ColourRegionUI : MonoBehaviour
{
    private UnityEngine.UI.Image _spriteRenderer;
    private UIColourUpdater _UIColourChanger;

    [SerializeField] private float _transitionMultiplier = 2f;
    [SerializeField] private float _localChangeMultiplier = 1f;

    private float _colourDiff;          // Colour from game manager
    private float _localColour;         // Colour of this region specifically
    public bool Enabled = true;

    private void Awake()
    {
        // Set values and components
        Color.RGBToHSV(GetComponent<UnityEngine.UI.Image>().color, out var H, out var S, out var V);
        _localColour = H * 360;
        _UIColourChanger = GameObject.FindGameObjectWithTag("Manager").GetComponent<UIColourUpdater>();
        _spriteRenderer = GetComponent<UnityEngine.UI.Image>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        _colourDiff = _UIColourChanger.Colour;
        ProcessColour();
    }

    // Processes the _UIColourChanger colour
    private void ProcessColour()
    {
        if (Enabled)
        {
            // set new local colour value
            TransitionZones();
            SetColour(_localColour);

            //if local colour value is over 360, change the local colour values back to being under 360 with math
            if (_localColour >= 360f)
            {
                _localColour = 0f;
            }
        }
        else
        {
            _spriteRenderer.color = Color.grey;
            //_spriteRenderer.color = Color.HSVToRGB(300/360f, 0.95f, 0.95f);
        }
    }

    // Sets the colour of the sprite renderer
    private void SetColour(float localColour)
    {
        // used 0.95 because otherwise it hurts my eyes
        _spriteRenderer.color = Color.HSVToRGB(localColour/360f, 0.95f, 0.95f);
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
}