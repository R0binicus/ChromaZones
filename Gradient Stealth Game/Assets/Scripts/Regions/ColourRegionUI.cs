using System.Collections;
using System.Collections.Generic;
//using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class ColourRegionUI : MonoBehaviour
{
    private UnityEngine.UI.Image spriteRenderer;
    private UIColourScript gameManager;

    [SerializeField] private float transitionMultiplier = 2f;
    [SerializeField] private float localChangeMultiplier = 1f;

    private float colourDiff;                           // Colour from game manager
    private float localColour;         // Colour of this region specifically
    private float originalHue;                          // Original colour value when the level started

    public int state;

    void Start()
    {
        // Set values and components
        Color.RGBToHSV(GetComponent<UnityEngine.UI.Image>().color, out var H, out var S, out var V);
        localColour = H * 360;
        originalHue = localColour;
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<UIColourScript>();
        spriteRenderer = GetComponent<UnityEngine.UI.Image>();
    }

    void Update()
    {
        colourDiff = gameManager.colour;
        ProcessColour();
        SetColour();
    }

    // Processes the gameManager colour
    private void ProcessColour()
    {
        // set new local colour value
        TransitionZones();

        //if local colour value is over 360, change the local colour values back to being under 360 with math
        if (localColour >= 360f)
        {
            //localColour = localColour - ((Mathf.Floor(localColour / 360f)) * 360);
            localColour = 0f;
        }
    }

    // Sets the colour of the sprite renderer
    private void SetColour()
    {
        // used 0.95 because otherwise it hurts my eyes
        spriteRenderer.color = Color.HSVToRGB(localColour/360f, 0.95f, 0.95f);
    }

    // If local colour value is in a 'transition zone' change the colour witha multiplier,
    // otherwise simply add it to the difference value
    private void TransitionZones()
    {
        switch(localColour) 
        {
            case float x when x >= 50f && x < 70f :
                localColour = localColour + (colourDiff * transitionMultiplier * localChangeMultiplier);
            break;
            case float x when x >= 170f && x < 190f :
                localColour = localColour + (colourDiff * transitionMultiplier * localChangeMultiplier);
            break;
            case float x when x >= 290f && x < 310f :
                localColour = localColour + (colourDiff * transitionMultiplier * localChangeMultiplier);
            break;
            default:
                localColour = localColour + (colourDiff * localChangeMultiplier);
            break;
        }
    }
}